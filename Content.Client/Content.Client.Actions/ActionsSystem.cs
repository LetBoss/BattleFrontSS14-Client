using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Content.Client._RMC14.Movement;
using Content.Shared._RMC14.Actions;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Charges.Components;
using Content.Shared.Charges.Systems;
using Content.Shared.Mapping;
using Content.Shared.Maps;
using Robust.Client.Player;
using Robust.Shared.Analyzers;
using Robust.Shared.ContentPack;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Serialization.Markdown.Sequence;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Utility;
using YamlDotNet.RepresentationModel;

namespace Content.Client.Actions;

public sealed class ActionsSystem : SharedActionsSystem
{
	public delegate void OnActionReplaced(EntityUid actionId);

	public record struct SlotAssignment(byte Hotbar, byte Slot, EntityUid ActionId);

	[Dependency]
	private SharedChargesSystem _sharedCharges;

	[Dependency]
	private IPlayerManager _playerManager;

	[Dependency]
	private IPrototypeManager _proto;

	[Dependency]
	private IResourceManager _resources;

	[Dependency]
	private MetaDataSystem _metaData;

	private readonly List<EntityUid> _removed = new List<EntityUid>();

	private readonly List<Entity<ActionComponent>> _added = new List<Entity<ActionComponent>>();

	public static readonly EntProtoId MappingEntityAction = EntProtoId.op_Implicit("BaseMappingEntityAction");

	[Dependency]
	private RMCLagCompensationSystem _rmcLagCompensation;

	public event Action<EntityUid>? OnActionAdded;

	public event Action<EntityUid>? OnActionRemoved;

	public event Action? ActionsUpdated;

	public event Action<ActionsComponent>? LinkActions;

	public event Action? UnlinkActions;

	public event Action? ClearAssignments;

	public event Action<List<SlotAssignment>>? AssignSlot;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ActionsComponent, LocalPlayerAttachedEvent>((ComponentEventHandler<ActionsComponent, LocalPlayerAttachedEvent>)OnPlayerAttached, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActionsComponent, LocalPlayerDetachedEvent>((ComponentEventHandler<ActionsComponent, LocalPlayerDetachedEvent>)OnPlayerDetached, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActionsComponent, ComponentHandleState>((EntityEventRefHandler<ActionsComponent, ComponentHandleState>)OnHandleState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActionComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<ActionComponent, AfterAutoHandleStateEvent>)OnActionAutoHandleState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EntityTargetActionComponent, ActionTargetAttemptEvent>((EntityEventRefHandler<EntityTargetActionComponent, ActionTargetAttemptEvent>)OnEntityTargetAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WorldTargetActionComponent, ActionTargetAttemptEvent>((EntityEventRefHandler<WorldTargetActionComponent, ActionTargetAttemptEvent>)OnWorldTargetAttempt, (Type[])null, (Type[])null);
	}

	private void OnActionAutoHandleState(Entity<ActionComponent> ent, ref AfterAutoHandleStateEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UpdateAction(ent);
	}

	public override void UpdateAction(Entity<ActionComponent> ent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.IconColor = ((_sharedCharges.GetCurrentCharges(Entity<LimitedChargesComponent, AutoRechargeComponent>.op_Implicit(ent.Owner)) == 0) ? ent.Comp.DisabledIconColor : ent.Comp.OriginalIconColor);
		base.UpdateAction(ent);
		EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
		EntityUid? attachedEntity = ent.Comp.AttachedEntity;
		if (localEntity.HasValue == attachedEntity.HasValue && (!localEntity.HasValue || !(localEntity.GetValueOrDefault() != attachedEntity.GetValueOrDefault())))
		{
			this.ActionsUpdated?.Invoke();
		}
	}

	private void OnHandleState(Entity<ActionsComponent> ent, ref ComponentHandleState args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		if (!(((ComponentHandleState)(ref args)).Current is ActionsComponentState actionsComponentState))
		{
			return;
		}
		Entity<ActionsComponent> val = ent;
		ActionsComponent actionsComponent = default(ActionsComponent);
		EntityUid val2 = default(EntityUid);
		val.Deconstruct(ref val2, ref actionsComponent);
		EntityUid val3 = val2;
		ActionsComponent actionsComponent2 = actionsComponent;
		_added.Clear();
		_removed.Clear();
		HashSet<EntityUid> hashSet = ((EntitySystem)this).EnsureEntitySet<ActionsComponent>(actionsComponentState.Actions, val3);
		foreach (EntityUid action2 in actionsComponent2.Actions)
		{
			if (!hashSet.Contains(action2) && !((EntitySystem)this).IsClientSide(action2, (MetaDataComponent)null))
			{
				_removed.Add(action2);
			}
		}
		actionsComponent2.Actions.ExceptWith(_removed);
		foreach (EntityUid item in hashSet)
		{
			EntityUid current2 = item;
			if (((EntityUid)(ref current2)).IsValid() && actionsComponent2.Actions.Add(current2))
			{
				Entity<ActionComponent>? action = GetAction(Entity<ActionComponent>.op_Implicit(current2));
				if (action.HasValue)
				{
					Entity<ActionComponent> valueOrDefault = action.GetValueOrDefault();
					_added.Add(valueOrDefault);
				}
			}
		}
		EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
		val2 = val3;
		if (!localEntity.HasValue || localEntity.GetValueOrDefault() != val2)
		{
			return;
		}
		foreach (EntityUid item2 in _removed)
		{
			this.OnActionRemoved?.Invoke(item2);
		}
		_added.Sort(ActionComparer);
		foreach (Entity<ActionComponent> item3 in _added)
		{
			this.OnActionAdded?.Invoke(Entity<ActionComponent>.op_Implicit(item3));
		}
		this.ActionsUpdated?.Invoke();
	}

	public static int ActionComparer(Entity<ActionComponent> a, Entity<ActionComponent> b)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		int num = a.Comp?.Priority ?? 0;
		int num2 = b.Comp?.Priority ?? 0;
		if (num != num2)
		{
			return num - num2;
		}
		num = (a.Comp?.Container?.Id).GetValueOrDefault();
		num2 = (b.Comp?.Container?.Id).GetValueOrDefault();
		return num - num2;
	}

	protected override void ActionAdded(Entity<ActionsComponent> performer, Entity<ActionComponent> action)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
		EntityUid owner = performer.Owner;
		if (localEntity.HasValue && !(localEntity.GetValueOrDefault() != owner))
		{
			this.OnActionAdded?.Invoke(Entity<ActionComponent>.op_Implicit(action));
			this.ActionsUpdated?.Invoke();
		}
	}

	protected override void ActionRemoved(Entity<ActionsComponent> performer, Entity<ActionComponent> action)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
		EntityUid owner = performer.Owner;
		if (localEntity.HasValue && !(localEntity.GetValueOrDefault() != owner))
		{
			this.OnActionRemoved?.Invoke(Entity<ActionComponent>.op_Implicit(action));
			this.ActionsUpdated?.Invoke();
		}
	}

	public IEnumerable<Entity<ActionComponent>> GetClientActions()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
		if (localEntity.HasValue)
		{
			EntityUid valueOrDefault = localEntity.GetValueOrDefault();
			return GetActions(valueOrDefault);
		}
		return Enumerable.Empty<Entity<ActionComponent>>();
	}

	private void OnPlayerAttached(EntityUid uid, ActionsComponent component, LocalPlayerAttachedEvent args)
	{
		LinkAllActions(component);
	}

	private void OnPlayerDetached(EntityUid uid, ActionsComponent component, LocalPlayerDetachedEvent? args = null)
	{
		UnlinkAllActions();
	}

	public void UnlinkAllActions()
	{
		this.UnlinkActions?.Invoke();
	}

	public void LinkAllActions(ActionsComponent? actions = null)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
		if (localEntity.HasValue)
		{
			EntityUid valueOrDefault = localEntity.GetValueOrDefault();
			if (((EntitySystem)this).Resolve<ActionsComponent>(valueOrDefault, ref actions, false))
			{
				this.LinkActions?.Invoke(actions);
			}
		}
	}

	public override void Shutdown()
	{
		((EntitySystem)this).Shutdown();
		CommandBinds.Unregister<ActionsSystem>();
	}

	public void TriggerAction(Entity<ActionComponent> action)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
		if (!localEntity.HasValue)
		{
			return;
		}
		EntityUid valueOrDefault = localEntity.GetValueOrDefault();
		if (((EntitySystem)this).HasComp<InstantActionComponent>(Entity<ActionComponent>.op_Implicit(action)))
		{
			if (action.Comp.ClientExclusive)
			{
				PerformAction(Entity<ActionsComponent>.op_Implicit(valueOrDefault), action);
				return;
			}
			RequestPerformActionEvent requestPerformActionEvent = new RequestPerformActionEvent(((EntitySystem)this).GetNetEntity(Entity<ActionComponent>.op_Implicit(action), (MetaDataComponent)null), _rmcLagCompensation.GetLastRealTick(null));
			((EntitySystem)this).RaisePredictiveEvent<RequestPerformActionEvent>(requestPerformActionEvent);
		}
	}

	public void LoadActionAssignments(string path, bool userData)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Expected O, but got Unknown
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Expected O, but got Unknown
		EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
		if (!localEntity.HasValue)
		{
			return;
		}
		EntityUid valueOrDefault = localEntity.GetValueOrDefault();
		ResPath val = new ResPath(path);
		ResPath val2 = ((ResPath)(ref val)).ToRootedPath();
		TextReader textReader = (userData ? WritableDirProviderExt.OpenText(_resources.UserData, val2) : _resources.ContentFileReadText(val2));
		YamlStream val3 = new YamlStream();
		val3.Load(textReader);
		DataNode obj = YamlNodeHelpers.ToDataNode(val3.Documents[0].RootNode);
		SequenceDataNode val4 = (SequenceDataNode)(object)((obj is SequenceDataNode) ? obj : null);
		if (val4 == null)
		{
			return;
		}
		ActionsComponent item = ((EntitySystem)this).EnsureComp<ActionsComponent>(valueOrDefault);
		this.ClearAssignments?.Invoke();
		new List<SlotAssignment>();
		DataNode val6 = default(DataNode);
		ValueDataNode val7 = default(ValueDataNode);
		EntProtoId val8 = default(EntProtoId);
		ValueDataNode val9 = default(ValueDataNode);
		EntProtoId val10 = default(EntProtoId);
		ValueDataNode val12 = default(ValueDataNode);
		ProtoId<ContentTileDefinition> val13 = default(ProtoId<ContentTileDefinition>);
		foreach (DataNode item2 in val4.Sequence)
		{
			MappingDataNode val5 = (MappingDataNode)(object)((item2 is MappingDataNode) ? item2 : null);
			if (val5 == null || !val5.TryGet("assignments", ref val6))
			{
				continue;
			}
			EntityUid invalid = EntityUid.Invalid;
			if (val5.TryGet<ValueDataNode>("action", ref val7))
			{
				((EntProtoId)(ref val8))._002Ector(val7.Value);
				invalid = ((EntitySystem)this).Spawn(EntProtoId.op_Implicit(val8), (ComponentRegistry)null, true);
			}
			else if (val5.TryGet<ValueDataNode>("entity", ref val9))
			{
				((EntProtoId)(ref val10))._002Ector(val9.Value);
				EntityPrototype val11 = _proto.Index(val10);
				invalid = ((EntitySystem)this).Spawn(EntProtoId.op_Implicit(MappingEntityAction), (ComponentRegistry)null, true);
				SetIcon(Entity<ActionComponent>.op_Implicit(invalid), (SpriteSpecifier?)new EntityPrototype(EntProtoId.op_Implicit(val10)));
				SetEvent(invalid, new StartPlacementActionEvent
				{
					PlacementOption = "SnapgridCenter",
					EntityType = val10
				});
				_metaData.SetEntityName(invalid, val11.Name, (MetaDataComponent)null, true);
			}
			else
			{
				if (!val5.TryGet<ValueDataNode>("tileId", ref val12))
				{
					((EntitySystem)this).Log.Error("Mapping actions from " + path + " had unknown action data!");
					continue;
				}
				val13._002Ector(val12.Value);
				ContentTileDefinition contentTileDefinition = _proto.Index<ContentTileDefinition>(val13);
				invalid = ((EntitySystem)this).Spawn(EntProtoId.op_Implicit(MappingEntityAction), (ComponentRegistry)null, true);
				ResPath? sprite = contentTileDefinition.Sprite;
				if (sprite.HasValue)
				{
					ResPath valueOrDefault2 = sprite.GetValueOrDefault();
					SetIcon(Entity<ActionComponent>.op_Implicit(invalid), (SpriteSpecifier?)new Texture(valueOrDefault2));
				}
				SetEvent(invalid, new StartPlacementActionEvent
				{
					PlacementOption = "AlignTileAny",
					TileId = val13
				});
				_metaData.SetEntityName(invalid, ((EntitySystem)this).Loc.GetString(contentTileDefinition.Name), (MetaDataComponent)null, true);
			}
			AddActionDirect(Entity<ActionsComponent>.op_Implicit((valueOrDefault, item)), Entity<ActionComponent>.op_Implicit(invalid));
		}
	}

	private void OnWorldTargetAttempt(Entity<WorldTargetActionComponent> ent, ref ActionTargetAttemptEvent args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		if (args.Handled)
		{
			return;
		}
		args.Handled = true;
		Entity<WorldTargetActionComponent> val = ent;
		EntityUid val2 = default(EntityUid);
		WorldTargetActionComponent worldTargetActionComponent = default(WorldTargetActionComponent);
		val.Deconstruct(ref val2, ref worldTargetActionComponent);
		EntityUid val3 = val2;
		WorldTargetActionComponent worldTargetActionComponent2 = worldTargetActionComponent;
		ActionComponent action = args.Action;
		EntityCoordinates coordinates = args.Input.Coordinates;
		Entity<ActionsComponent> user = args.User;
		if (!ValidateWorldTarget(Entity<ActionsComponent>.op_Implicit(user), coordinates, ent))
		{
			return;
		}
		EntityUid? val4 = null;
		EntityUid entityUid = args.Input.EntityUid;
		EntityTargetActionComponent item = default(EntityTargetActionComponent);
		if (((EntitySystem)this).TryComp<EntityTargetActionComponent>(Entity<WorldTargetActionComponent>.op_Implicit(ent), ref item) && ((EntityUid)(ref entityUid)).Valid && ValidateEntityTarget(Entity<ActionsComponent>.op_Implicit(user), entityUid, Entity<EntityTargetActionComponent>.op_Implicit((val3, item))))
		{
			val4 = entityUid;
		}
		if (action.ClientExclusive)
		{
			WorldTargetActionEvent worldTargetActionEvent = worldTargetActionComponent2.Event;
			if (worldTargetActionEvent != null)
			{
				worldTargetActionEvent.Target = coordinates;
				worldTargetActionEvent.Entity = val4;
			}
			PerformAction(Entity<ActionsComponent>.op_Implicit((Entity<ActionsComponent>.op_Implicit(user), user.Comp)), Entity<ActionComponent>.op_Implicit((val3, action)));
		}
		else
		{
			((EntitySystem)this).RaisePredictiveEvent<RequestPerformActionEvent>(new RequestPerformActionEvent(((EntitySystem)this).GetNetEntity(val3, (MetaDataComponent)null), ((EntitySystem)this).GetNetEntity(val4, (MetaDataComponent)null), ((EntitySystem)this).GetNetCoordinates(coordinates, (MetaDataComponent)null), _rmcLagCompensation.GetLastRealTick(null)));
		}
		args.FoundTarget = true;
	}

	private void OnEntityTargetAttempt(Entity<EntityTargetActionComponent> ent, ref ActionTargetAttemptEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		if (args.Handled)
		{
			return;
		}
		EntityUid entityUid = args.Input.EntityUid;
		if (!((EntityUid)(ref entityUid)).Valid)
		{
			((EntitySystem)this).RaisePredictiveEvent<RMCMissedTargetActionEvent>(new RMCMissedTargetActionEvent(((EntitySystem)this).GetNetEntity(Entity<EntityTargetActionComponent>.op_Implicit(ent), (MetaDataComponent)null)));
			return;
		}
		Entity<EntityTargetActionComponent> val = ent;
		EntityUid val2 = default(EntityUid);
		EntityTargetActionComponent entityTargetActionComponent = default(EntityTargetActionComponent);
		val.Deconstruct(ref val2, ref entityTargetActionComponent);
		EntityUid val3 = val2;
		EntityTargetActionEvent entityTargetActionEvent = entityTargetActionComponent.Event;
		if (entityTargetActionEvent == null)
		{
			return;
		}
		args.Handled = true;
		ActionComponent action = args.Action;
		Entity<ActionsComponent> user = args.User;
		if (ValidateEntityTarget(Entity<ActionsComponent>.op_Implicit(user), entityUid, ent))
		{
			if (action.ClientExclusive)
			{
				entityTargetActionEvent.Target = entityUid;
				PerformAction(Entity<ActionsComponent>.op_Implicit((Entity<ActionsComponent>.op_Implicit(user), user.Comp)), Entity<ActionComponent>.op_Implicit((val3, action)));
			}
			else
			{
				((EntitySystem)this).RaisePredictiveEvent<RequestPerformActionEvent>(new RequestPerformActionEvent(((EntitySystem)this).GetNetEntity(val3, (MetaDataComponent)null), ((EntitySystem)this).GetNetEntity(entityUid, (MetaDataComponent)null), _rmcLagCompensation.GetLastRealTick(null)));
			}
			args.FoundTarget = true;
		}
	}

	public void SetAssignments(List<SlotAssignment> actions)
	{
		this.ClearAssignments?.Invoke();
		this.AssignSlot?.Invoke(actions);
	}
}
