using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Content.Shared.Access.Components;
using Content.Shared.DeviceLinking.Events;
using Content.Shared.Emag.Systems;
using Content.Shared.GameTicking;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.IdentityManagement;
using Content.Shared.Inventory;
using Content.Shared.NameIdentifier;
using Content.Shared.PDA;
using Content.Shared.StationRecords;
using Content.Shared.Tag;
using Robust.Shared.Collections;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared.Access.Systems;

public sealed class AccessReaderSystem : EntitySystem
{
	[Dependency]
	private IPrototypeManager _prototype;

	[Dependency]
	private InventorySystem _inventorySystem;

	[Dependency]
	private IGameTiming _gameTiming;

	[Dependency]
	private EmagSystem _emag;

	[Dependency]
	private TagSystem _tag;

	[Dependency]
	private SharedGameTicker _gameTicker;

	[Dependency]
	private SharedHandsSystem _handsSystem;

	[Dependency]
	private SharedContainerSystem _containerSystem;

	[Dependency]
	private SharedStationRecordsSystem _recordsSystem;

	private static readonly ProtoId<TagPrototype> PreventAccessLoggingTag = ProtoId<TagPrototype>.op_Implicit("PreventAccessLogging");

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<AccessReaderComponent, GotEmaggedEvent>((ComponentEventRefHandler<AccessReaderComponent, GotEmaggedEvent>)OnEmagged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AccessReaderComponent, LinkAttemptEvent>((ComponentEventHandler<AccessReaderComponent, LinkAttemptEvent>)OnLinkAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AccessReaderComponent, ComponentGetState>((ComponentEventRefHandler<AccessReaderComponent, ComponentGetState>)OnGetState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AccessReaderComponent, ComponentHandleState>((ComponentEventRefHandler<AccessReaderComponent, ComponentHandleState>)OnHandleState, (Type[])null, (Type[])null);
	}

	private void OnGetState(EntityUid uid, AccessReaderComponent component, ref ComponentGetState args)
	{
		((ComponentGetState)(ref args)).State = (IComponentState)(object)new AccessReaderComponentState(component.Enabled, component.DenyTags, component.AccessLists, _recordsSystem.Convert(component.AccessKeys), component.AccessLog, component.AccessLogLimit);
	}

	private void OnHandleState(EntityUid uid, AccessReaderComponent component, ref ComponentHandleState args)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		if (!(((ComponentHandleState)(ref args)).Current is AccessReaderComponentState state))
		{
			return;
		}
		component.Enabled = state.Enabled;
		component.AccessKeys.Clear();
		foreach (var key in state.AccessKeys)
		{
			EntityUid id = ((EntitySystem)this).EnsureEntity<AccessReaderComponent>(key.Item1, uid);
			if (((EntityUid)(ref id)).IsValid())
			{
				component.AccessKeys.Add(new StationRecordKey(key.Item2, id));
			}
		}
		component.AccessLists = new List<HashSet<ProtoId<AccessLevelPrototype>>>(state.AccessLists);
		component.DenyTags = new HashSet<ProtoId<AccessLevelPrototype>>(state.DenyTags);
		component.AccessLog = new Queue<AccessRecord>(state.AccessLog);
		component.AccessLogLimit = state.AccessLogLimit;
	}

	private void OnLinkAttempt(EntityUid uid, AccessReaderComponent component, LinkAttemptEvent args)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		if (args.User.HasValue && !IsAllowed(args.User.Value, uid, component))
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private void OnEmagged(EntityUid uid, AccessReaderComponent reader, ref GotEmaggedEvent args)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		if (_emag.CompareFlag(args.Type, EmagType.Access) && reader.BreakOnAccessBreaker && GetMainAccessReader(uid, out Entity<AccessReaderComponent>? accessReader) && accessReader.Value.Comp.AccessLists.Count >= 1)
		{
			args.Repeatable = true;
			args.Handled = true;
			accessReader.Value.Comp.AccessLists.Clear();
			accessReader.Value.Comp.AccessLog.Clear();
			((EntitySystem)this).Dirty(uid, (IComponent)(object)reader, (MetaDataComponent)null);
		}
	}

	public bool IsAllowed(EntityUid user, EntityUid target, AccessReaderComponent? reader = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<AccessReaderComponent>(target, ref reader, false))
		{
			return true;
		}
		if (!reader.Enabled)
		{
			return true;
		}
		HashSet<EntityUid> accessSources = FindPotentialAccessItems(user);
		ICollection<ProtoId<AccessLevelPrototype>> access = FindAccessTags(user, accessSources);
		FindStationRecordKeys(user, out ICollection<StationRecordKey> stationKeys, accessSources);
		if (!IsAllowed(access, stationKeys, target, reader))
		{
			return false;
		}
		if (!_tag.HasTag(user, PreventAccessLoggingTag))
		{
			LogAccess(Entity<AccessReaderComponent>.op_Implicit((target, reader)), user);
		}
		return true;
	}

	public bool GetMainAccessReader(EntityUid uid, [NotNullWhen(true)] out Entity<AccessReaderComponent>? ent)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		ent = null;
		AccessReaderComponent accessReader = default(AccessReaderComponent);
		if (!((EntitySystem)this).TryComp<AccessReaderComponent>(uid, ref accessReader))
		{
			return false;
		}
		ent = Entity<AccessReaderComponent>.op_Implicit((uid, accessReader));
		if (ent.Value.Comp.ContainerAccessProvider == null)
		{
			return true;
		}
		BaseContainer container = default(BaseContainer);
		if (!_containerSystem.TryGetContainer(uid, ent.Value.Comp.ContainerAccessProvider, ref container, (ContainerManagerComponent)null))
		{
			return true;
		}
		AccessReaderComponent containedReader = default(AccessReaderComponent);
		foreach (EntityUid entity in container.ContainedEntities)
		{
			if (((EntitySystem)this).TryComp<AccessReaderComponent>(entity, ref containedReader))
			{
				ent = Entity<AccessReaderComponent>.op_Implicit((entity, containedReader));
				return true;
			}
		}
		return true;
	}

	public bool IsAllowed(ICollection<ProtoId<AccessLevelPrototype>> access, ICollection<StationRecordKey> stationKeys, EntityUid target, AccessReaderComponent reader)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		if (!reader.Enabled)
		{
			return true;
		}
		if (reader.ContainerAccessProvider == null)
		{
			return IsAllowedInternal(access, stationKeys, reader);
		}
		BaseContainer container = default(BaseContainer);
		if (!_containerSystem.TryGetContainer(target, reader.ContainerAccessProvider, ref container, (ContainerManagerComponent)null))
		{
			return false;
		}
		if (((EntitySystem)this).Paused(target, (MetaDataComponent)null))
		{
			return true;
		}
		AccessReaderComponent containedReader = default(AccessReaderComponent);
		foreach (EntityUid entity in container.ContainedEntities)
		{
			if (((EntitySystem)this).TryComp<AccessReaderComponent>(entity, ref containedReader) && IsAllowed(access, stationKeys, entity, containedReader))
			{
				return true;
			}
		}
		return false;
	}

	private bool IsAllowedInternal(ICollection<ProtoId<AccessLevelPrototype>> access, ICollection<StationRecordKey> stationKeys, AccessReaderComponent reader)
	{
		if (reader.Enabled && !AreAccessTagsAllowed(access, reader))
		{
			return AreStationRecordKeysAllowed(stationKeys, reader);
		}
		return true;
	}

	public bool AreAccessTagsAllowed(ICollection<ProtoId<AccessLevelPrototype>> accessTags, AccessReaderComponent reader)
	{
		if (reader.DenyTags.Overlaps(accessTags))
		{
			return false;
		}
		if (reader.AccessLists.Count == 0)
		{
			return true;
		}
		foreach (HashSet<ProtoId<AccessLevelPrototype>> accessList in reader.AccessLists)
		{
			if (accessList.IsSubsetOf(accessTags))
			{
				return true;
			}
		}
		return false;
	}

	public bool AreStationRecordKeysAllowed(ICollection<StationRecordKey> keys, AccessReaderComponent reader)
	{
		foreach (StationRecordKey key in reader.AccessKeys)
		{
			if (keys.Contains(key))
			{
				return true;
			}
		}
		return false;
	}

	public HashSet<EntityUid> FindPotentialAccessItems(EntityUid uid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		FindAccessItemsInventory(uid, out HashSet<EntityUid> items);
		GetAdditionalAccessEvent getAdditionalAccessEvent = new GetAdditionalAccessEvent();
		getAdditionalAccessEvent.Entities = items;
		GetAdditionalAccessEvent ev = getAdditionalAccessEvent;
		((EntitySystem)this).RaiseLocalEvent<GetAdditionalAccessEvent>(uid, ref ev, false);
		Enumerator<EntityUid> enumerator = new ValueList<EntityUid>((IReadOnlyCollection<EntityUid>)items).GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				EntityUid item = enumerator.Current;
				items.UnionWith(FindPotentialAccessItems(item));
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
		}
		items.Add(uid);
		return items;
	}

	public ICollection<ProtoId<AccessLevelPrototype>> FindAccessTags(EntityUid uid, HashSet<EntityUid>? items = null)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		HashSet<ProtoId<AccessLevelPrototype>> tags = null;
		bool owned = false;
		if (items == null)
		{
			items = FindPotentialAccessItems(uid);
		}
		foreach (EntityUid ent in items)
		{
			FindAccessTagsItem(ent, ref tags, ref owned);
		}
		ICollection<ProtoId<AccessLevelPrototype>> collection = tags;
		return collection ?? Array.Empty<ProtoId<AccessLevelPrototype>>();
	}

	public bool FindStationRecordKeys(EntityUid uid, out ICollection<StationRecordKey> recordKeys, HashSet<EntityUid>? items = null)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		recordKeys = new HashSet<StationRecordKey>();
		if (items == null)
		{
			items = FindPotentialAccessItems(uid);
		}
		foreach (EntityUid ent in items)
		{
			if (FindStationRecordKeyItem(ent, out var key))
			{
				recordKeys.Add(key.Value);
			}
		}
		return recordKeys.Any();
	}

	private void FindAccessTagsItem(EntityUid uid, ref HashSet<ProtoId<AccessLevelPrototype>>? tags, ref bool owned)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (!FindAccessTagsItem(uid, out HashSet<ProtoId<AccessLevelPrototype>> targetTags))
		{
			return;
		}
		if (tags != null)
		{
			if (!owned)
			{
				tags = new HashSet<ProtoId<AccessLevelPrototype>>(tags);
				owned = true;
			}
			tags.UnionWith(targetTags);
		}
		else
		{
			tags = targetTags;
			owned = false;
		}
	}

	public void ClearAccesses(Entity<AccessReaderComponent> ent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.AccessLists.Clear();
		((EntitySystem)this).Dirty<AccessReaderComponent>(ent, (MetaDataComponent)null);
		((EntitySystem)this).RaiseLocalEvent<AccessReaderConfigurationChangedEvent>(Entity<AccessReaderComponent>.op_Implicit(ent), new AccessReaderConfigurationChangedEvent(), false);
	}

	public void SetAccesses(Entity<AccessReaderComponent> ent, List<HashSet<ProtoId<AccessLevelPrototype>>> accesses)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.AccessLists.Clear();
		AddAccesses(ent, accesses);
	}

	public void SetAccesses(Entity<AccessReaderComponent> ent, List<ProtoId<AccessLevelPrototype>> accesses)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.AccessLists.Clear();
		AddAccesses(ent, accesses);
	}

	public void AddAccesses(Entity<AccessReaderComponent> ent, List<HashSet<ProtoId<AccessLevelPrototype>>> accesses)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		foreach (HashSet<ProtoId<AccessLevelPrototype>> access in accesses)
		{
			AddAccess(ent, access, dirty: false);
		}
		((EntitySystem)this).Dirty<AccessReaderComponent>(ent, (MetaDataComponent)null);
		((EntitySystem)this).RaiseLocalEvent<AccessReaderConfigurationChangedEvent>(Entity<AccessReaderComponent>.op_Implicit(ent), new AccessReaderConfigurationChangedEvent(), false);
	}

	public void AddAccesses(Entity<AccessReaderComponent> ent, List<ProtoId<AccessLevelPrototype>> accesses)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		foreach (ProtoId<AccessLevelPrototype> access in accesses)
		{
			AddAccess(ent, access, dirty: false);
		}
		((EntitySystem)this).Dirty<AccessReaderComponent>(ent, (MetaDataComponent)null);
		((EntitySystem)this).RaiseLocalEvent<AccessReaderConfigurationChangedEvent>(Entity<AccessReaderComponent>.op_Implicit(ent), new AccessReaderConfigurationChangedEvent(), false);
	}

	public void AddAccess(Entity<AccessReaderComponent> ent, HashSet<ProtoId<AccessLevelPrototype>> access, bool dirty = true)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.AccessLists.Add(access);
		if (dirty)
		{
			((EntitySystem)this).Dirty<AccessReaderComponent>(ent, (MetaDataComponent)null);
			((EntitySystem)this).RaiseLocalEvent<AccessReaderConfigurationChangedEvent>(Entity<AccessReaderComponent>.op_Implicit(ent), new AccessReaderConfigurationChangedEvent(), false);
		}
	}

	public void AddAccess(Entity<AccessReaderComponent> ent, ProtoId<AccessLevelPrototype> access, bool dirty = true)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		AddAccess(ent, new HashSet<ProtoId<AccessLevelPrototype>> { access }, dirty);
	}

	public void RemoveAccesses(Entity<AccessReaderComponent> ent, List<HashSet<ProtoId<AccessLevelPrototype>>> accesses)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		foreach (HashSet<ProtoId<AccessLevelPrototype>> access in accesses)
		{
			RemoveAccess(ent, access, dirty: false);
		}
		((EntitySystem)this).Dirty<AccessReaderComponent>(ent, (MetaDataComponent)null);
		((EntitySystem)this).RaiseLocalEvent<AccessReaderConfigurationChangedEvent>(Entity<AccessReaderComponent>.op_Implicit(ent), new AccessReaderConfigurationChangedEvent(), false);
	}

	public void RemoveAccesses(Entity<AccessReaderComponent> ent, List<ProtoId<AccessLevelPrototype>> accesses)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		foreach (ProtoId<AccessLevelPrototype> access in accesses)
		{
			RemoveAccess(ent, access, dirty: false);
		}
		((EntitySystem)this).Dirty<AccessReaderComponent>(ent, (MetaDataComponent)null);
		((EntitySystem)this).RaiseLocalEvent<AccessReaderConfigurationChangedEvent>(Entity<AccessReaderComponent>.op_Implicit(ent), new AccessReaderConfigurationChangedEvent(), false);
	}

	public void RemoveAccess(Entity<AccessReaderComponent> ent, HashSet<ProtoId<AccessLevelPrototype>> access, bool dirty = true)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		for (int i = ent.Comp.AccessLists.Count - 1; i >= 0; i--)
		{
			if (ent.Comp.AccessLists[i].SetEquals(access))
			{
				ent.Comp.AccessLists.RemoveAt(i);
			}
		}
		if (dirty)
		{
			((EntitySystem)this).Dirty<AccessReaderComponent>(ent, (MetaDataComponent)null);
			((EntitySystem)this).RaiseLocalEvent<AccessReaderConfigurationChangedEvent>(Entity<AccessReaderComponent>.op_Implicit(ent), new AccessReaderConfigurationChangedEvent(), false);
		}
	}

	public void RemoveAccess(Entity<AccessReaderComponent> ent, ProtoId<AccessLevelPrototype> access, bool dirty = true)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		RemoveAccess(ent, new HashSet<ProtoId<AccessLevelPrototype>> { access }, dirty);
	}

	public void ClearAccessKeys(Entity<AccessReaderComponent> ent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.AccessKeys.Clear();
		((EntitySystem)this).Dirty<AccessReaderComponent>(ent, (MetaDataComponent)null);
	}

	public void SetAccessKeys(Entity<AccessReaderComponent> ent, HashSet<StationRecordKey> keys)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.AccessKeys.Clear();
		foreach (StationRecordKey key in keys)
		{
			ent.Comp.AccessKeys.Add(key);
		}
		((EntitySystem)this).Dirty<AccessReaderComponent>(ent, (MetaDataComponent)null);
	}

	public void AddAccessKey(Entity<AccessReaderComponent> ent, StationRecordKey key)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.AccessKeys.Add(key);
		((EntitySystem)this).Dirty<AccessReaderComponent>(ent, (MetaDataComponent)null);
	}

	public void RemoveAccessKey(Entity<AccessReaderComponent> ent, StationRecordKey key)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.AccessKeys.Remove(key);
		((EntitySystem)this).Dirty<AccessReaderComponent>(ent, (MetaDataComponent)null);
	}

	public void ClearDenyTags(Entity<AccessReaderComponent> ent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.DenyTags.Clear();
		((EntitySystem)this).Dirty<AccessReaderComponent>(ent, (MetaDataComponent)null);
	}

	public void SetDenyTags(Entity<AccessReaderComponent> ent, HashSet<ProtoId<AccessLevelPrototype>> tags)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.DenyTags.Clear();
		foreach (ProtoId<AccessLevelPrototype> tag in tags)
		{
			ent.Comp.DenyTags.Add(tag);
		}
		((EntitySystem)this).Dirty<AccessReaderComponent>(ent, (MetaDataComponent)null);
	}

	public void AddDenyTag(Entity<AccessReaderComponent> ent, ProtoId<AccessLevelPrototype> tag)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.DenyTags.Add(tag);
		((EntitySystem)this).Dirty<AccessReaderComponent>(ent, (MetaDataComponent)null);
	}

	public void RemoveDenyTag(Entity<AccessReaderComponent> ent, ProtoId<AccessLevelPrototype> tag)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.DenyTags.Remove(tag);
		((EntitySystem)this).Dirty<AccessReaderComponent>(ent, (MetaDataComponent)null);
	}

	public void SetActive(Entity<AccessReaderComponent> ent, bool enabled)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.Enabled = enabled;
		((EntitySystem)this).Dirty<AccessReaderComponent>(ent, (MetaDataComponent)null);
	}

	public void SetLoggingActive(Entity<AccessReaderComponent> ent, bool enabled)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.LoggingDisabled = !enabled;
		((EntitySystem)this).Dirty<AccessReaderComponent>(ent, (MetaDataComponent)null);
	}

	public bool FindAccessItemsInventory(EntityUid uid, out HashSet<EntityUid> items)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		items = new HashSet<EntityUid>(_handsSystem.EnumerateHeld(Entity<HandsComponent>.op_Implicit(uid)));
		if (_inventorySystem.TryGetSlotEntity(uid, "id", out var idUid))
		{
			items.Add(idUid.Value);
		}
		return items.Any();
	}

	private bool FindAccessTagsItem(EntityUid uid, out HashSet<ProtoId<AccessLevelPrototype>> tags)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		tags = new HashSet<ProtoId<AccessLevelPrototype>>();
		GetAccessTagsEvent ev = new GetAccessTagsEvent(tags, _prototype);
		((EntitySystem)this).RaiseLocalEvent<GetAccessTagsEvent>(uid, ref ev, false);
		return tags.Count != 0;
	}

	private bool FindStationRecordKeyItem(EntityUid uid, [NotNullWhen(true)] out StationRecordKey? key)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		StationRecordKeyStorageComponent storage = default(StationRecordKeyStorageComponent);
		if (((EntitySystem)this).TryComp<StationRecordKeyStorageComponent>(uid, ref storage) && storage.Key.HasValue)
		{
			key = storage.Key;
			return true;
		}
		PdaComponent pda = default(PdaComponent);
		if (((EntitySystem)this).TryComp<PdaComponent>(uid, ref pda))
		{
			EntityUid? containedId = pda.ContainedId;
			if (containedId.HasValue)
			{
				EntityUid id = containedId.GetValueOrDefault();
				StationRecordKeyStorageComponent pdastorage = default(StationRecordKeyStorageComponent);
				if (((EntityUid)(ref id)).Valid && ((EntitySystem)this).TryComp<StationRecordKeyStorageComponent>(id, ref pdastorage) && pdastorage.Key.HasValue)
				{
					key = pdastorage.Key;
					return true;
				}
			}
		}
		key = null;
		return false;
	}

	public void LogAccess(Entity<AccessReaderComponent> ent, EntityUid accessor)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).IsPaused((EntityUid?)Entity<AccessReaderComponent>.op_Implicit(ent), (MetaDataComponent)null) && !ent.Comp.LoggingDisabled)
		{
			string name = null;
			NameIdentifierComponent nameIdentifier = default(NameIdentifierComponent);
			if (((EntitySystem)this).TryComp<NameIdentifierComponent>(accessor, ref nameIdentifier))
			{
				name = nameIdentifier.FullIdentifier;
			}
			TryGetIdentityShortInfoEvent getIdentityShortInfoEvent = new TryGetIdentityShortInfoEvent(Entity<AccessReaderComponent>.op_Implicit(ent), accessor, forLogging: true);
			((EntitySystem)this).RaiseLocalEvent<TryGetIdentityShortInfoEvent>(getIdentityShortInfoEvent);
			if (getIdentityShortInfoEvent.Title != null)
			{
				name = getIdentityShortInfoEvent.Title;
			}
			LogAccess(ent, name ?? base.Loc.GetString("access-reader-unknown-id"));
		}
	}

	public void LogAccess(Entity<AccessReaderComponent> ent, string name, TimeSpan? accessTime = null, bool force = false)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		if (!force)
		{
			if (((EntitySystem)this).IsPaused((EntityUid?)Entity<AccessReaderComponent>.op_Implicit(ent), (MetaDataComponent)null) || ent.Comp.LoggingDisabled)
			{
				return;
			}
			if (ent.Comp.AccessLog.Count >= ent.Comp.AccessLogLimit)
			{
				ent.Comp.AccessLog.Dequeue();
			}
		}
		TimeSpan stationTime = accessTime ?? _gameTiming.CurTime.Subtract(_gameTicker.RoundStartTimeSpan);
		ent.Comp.AccessLog.Enqueue(new AccessRecord(stationTime, name));
		((EntitySystem)this).Dirty<AccessReaderComponent>(ent, (MetaDataComponent)null);
	}
}
