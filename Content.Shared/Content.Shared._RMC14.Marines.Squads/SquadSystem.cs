using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Content.Shared._RMC14.Admin;
using Content.Shared._RMC14.Chat;
using Content.Shared._RMC14.Cryostorage;
using Content.Shared._RMC14.Inventory;
using Content.Shared._RMC14.Marines.Announce;
using Content.Shared._RMC14.Marines.Orders;
using Content.Shared._RMC14.Pointing;
using Content.Shared._RMC14.Roles;
using Content.Shared._RMC14.Tracker;
using Content.Shared.Access;
using Content.Shared.Access.Components;
using Content.Shared.Access.Systems;
using Content.Shared.Chat;
using Content.Shared.Clothing;
using Content.Shared.Clothing.Components;
using Content.Shared.Clothing.EntitySystems;
using Content.Shared.Dataset;
using Content.Shared.GameTicking;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Inventory;
using Content.Shared.Mind;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.Prototypes;
using Content.Shared.Radio;
using Content.Shared.Radio.Components;
using Content.Shared.Radio.EntitySystems;
using Content.Shared.Roles;
using Content.Shared.Roles.Jobs;
using Content.Shared.Storage;
using Content.Shared.Whitelist;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Shared._RMC14.Marines.Squads;

public sealed class SquadSystem : EntitySystem
{
	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private IComponentFactory _compFactory;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private EncryptionKeySystem _encryptionKey;

	[Dependency]
	private EntityWhitelistSystem _entityWhitelist;

	[Dependency]
	private SharedIdCardSystem _id;

	[Dependency]
	private SharedCMInventorySystem _cmInventory;

	[Dependency]
	private InventorySystem _inventory;

	[Dependency]
	private SharedJobSystem _job;

	[Dependency]
	private SharedHandsSystem _hands;

	[Dependency]
	private SharedMarineSystem _marine;

	[Dependency]
	private SharedMarineAnnounceSystem _marineAnnounce;

	[Dependency]
	private SharedMarineOrdersSystem _marineOrders;

	[Dependency]
	private SharedMindSystem _mind;

	[Dependency]
	private MobStateSystem _mobState;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private IPrototypeManager _prototypes;

	[Dependency]
	private SharedRMCBanSystem _rmcBan;

	[Dependency]
	private SharedCMChatSystem _rmcChat;

	private static readonly ProtoId<JobPrototype> SquadLeaderJob = ProtoId<JobPrototype>.op_Implicit("CMSquadLeader");

	private static readonly ProtoId<JobPrototype> IntelOfficerJob = ProtoId<JobPrototype>.op_Implicit("CMIntelOfficer");

	public static readonly EntProtoId<SquadTeamComponent> EchoSquadId = EntProtoId<SquadTeamComponent>.op_Implicit("SquadEcho");

	private readonly HashSet<EntityUid> _membersToUpdate = new HashSet<EntityUid>();

	private EntityQuery<RMCMapToSquadComponent> _mapToSquadQuery;

	private EntityQuery<OriginalRoleComponent> _originalRoleQuery;

	private EntityQuery<SquadArmorWearerComponent> _squadArmorWearerQuery;

	private EntityQuery<SquadMemberComponent> _squadMemberQuery;

	private EntityQuery<SquadTeamComponent> _squadTeamQuery;

	public ImmutableArray<EntityPrototype> SquadPrototypes { get; private set; }

	public ImmutableArray<JobPrototype> SquadRolePrototypes { get; private set; }

	public override void Initialize()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		_mapToSquadQuery = ((EntitySystem)this).GetEntityQuery<RMCMapToSquadComponent>();
		_originalRoleQuery = ((EntitySystem)this).GetEntityQuery<OriginalRoleComponent>();
		_squadArmorWearerQuery = ((EntitySystem)this).GetEntityQuery<SquadArmorWearerComponent>();
		_squadMemberQuery = ((EntitySystem)this).GetEntityQuery<SquadMemberComponent>();
		_squadTeamQuery = ((EntitySystem)this).GetEntityQuery<SquadTeamComponent>();
		((EntitySystem)this).SubscribeLocalEvent<SquadArmorComponent, GetEquipmentVisualsEvent>((EntityEventRefHandler<SquadArmorComponent, GetEquipmentVisualsEvent>)OnSquadArmorGetVisuals, (Type[])null, new Type[1] { typeof(ClothingSystem) });
		((EntitySystem)this).SubscribeLocalEvent<SquadMemberComponent, MapInitEvent>((EntityEventRefHandler<SquadMemberComponent, MapInitEvent>)OnSquadMemberMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SquadMemberComponent, ComponentRemove>((EntityEventRefHandler<SquadMemberComponent, ComponentRemove>)OnSquadMemberRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SquadMemberComponent, EntityTerminatingEvent>((EntityEventRefHandler<SquadMemberComponent, EntityTerminatingEvent>)OnSquadMemberTerminating, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SquadMemberComponent, MobStateChangedEvent>((EntityEventRefHandler<SquadMemberComponent, MobStateChangedEvent>)OnSquadMemberMobStateChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SquadMemberComponent, PlayerAttachedEvent>((EntityEventRefHandler<SquadMemberComponent, PlayerAttachedEvent>)OnSquadMemberPlayerAttached, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SquadMemberComponent, PlayerDetachedEvent>((EntityEventRefHandler<SquadMemberComponent, PlayerDetachedEvent>)OnSquadMemberPlayerDetached, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SquadMemberComponent, GetMarineIconEvent>((EntityEventRefHandler<SquadMemberComponent, GetMarineIconEvent>)OnSquadRoleGetIcon, (Type[])null, new Type[1] { typeof(SharedMarineSystem) });
		((EntitySystem)this).SubscribeLocalEvent<SquadMemberComponent, EnteredCryostorageEvent>((EntityEventRefHandler<SquadMemberComponent, EnteredCryostorageEvent>)OnSquadMemberEnteredCryo, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SquadMemberComponent, LeftCryostorageEvent>((EntityEventRefHandler<SquadMemberComponent, LeftCryostorageEvent>)OnSquadMemberLeftCryo, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SquadMemberComponent, GetMarineSquadNameEvent>((EntityEventRefHandler<SquadMemberComponent, GetMarineSquadNameEvent>)OnSquadRoleGetName, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SquadLeaderComponent, EntityTerminatingEvent>((EntityEventRefHandler<SquadLeaderComponent, EntityTerminatingEvent>)OnSquadLeaderTerminating, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SquadLeaderComponent, GetMarineIconEvent>((EntityEventRefHandler<SquadLeaderComponent, GetMarineIconEvent>)OnSquadLeaderGetMarineIcon, (Type[])null, new Type[1] { typeof(SharedMarineSystem) });
		((EntitySystem)this).SubscribeLocalEvent<SquadLeaderHeadsetComponent, EncryptionChannelsChangedEvent>((EntityEventRefHandler<SquadLeaderHeadsetComponent, EncryptionChannelsChangedEvent>)OnSquadLeaderHeadsetChannelsChanged, new Type[1] { typeof(SharedHeadsetSystem) }, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SquadLeaderHeadsetComponent, EntityTerminatingEvent>((EntityEventRefHandler<SquadLeaderHeadsetComponent, EntityTerminatingEvent>)OnSquadLeaderHeadsetTerminating, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AssignSquadComponent, PlayerSpawnCompleteEvent>((EntityEventRefHandler<AssignSquadComponent, PlayerSpawnCompleteEvent>)OnAssignSquadPlayerSpawnComplete, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PrototypesReloadedEventArgs>((EntityEventHandler<PrototypesReloadedEventArgs>)OnPrototypesReloaded, (Type[])null, (Type[])null);
		RefreshSquadPrototypes();
	}

	private void OnSquadArmorGetVisuals(Entity<SquadArmorComponent> ent, ref GetEquipmentVisualsEvent args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Expected O, but got Unknown
		SquadMemberComponent member = default(SquadMemberComponent);
		SquadArmorWearerComponent wearer = default(SquadArmorWearerComponent);
		if ((!_inventory.TryGetSlot(args.Equipee, args.Slot, out SlotDefinition slot) || (slot.SlotFlags & ent.Comp.Slot) != SlotFlags.NONE) && _squadMemberQuery.TryComp(args.Equipee, ref member) && _squadArmorWearerQuery.TryComp(args.Equipee, ref wearer) && !member.BlacklistedSquadArmor.Contains(ent.Comp.Layer))
		{
			Rsi rsi = (wearer.Leader ? ent.Comp.LeaderRsi : ent.Comp.Rsi);
			string layer = $"enum.{"SquadArmorLayers"}.{ent.Comp.Layer}";
			if (!args.Layers.Any<(string, PrototypeLayerData)>(((string, PrototypeLayerData) l) => l.Item1 == layer))
			{
				args.Layers.Add((layer, new PrototypeLayerData
				{
					RsiPath = ((object)rsi.RsiPath/*cast due to constrained. prefix*/).ToString(),
					State = rsi.RsiState,
					Color = member.BackgroundColor,
					Visible = true
				}));
			}
		}
	}

	private void OnSquadMemberMapInit(Entity<SquadMemberComponent> ent, ref MapInitEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_membersToUpdate.Add(Entity<SquadMemberComponent>.op_Implicit(ent));
	}

	private void OnSquadMemberRemove(Entity<SquadMemberComponent> ent, ref ComponentRemove args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		SquadTeamComponent team = default(SquadTeamComponent);
		if (_squadTeamQuery.TryComp(ent.Comp.Squad, ref team))
		{
			team.Members.Remove(Entity<SquadMemberComponent>.op_Implicit(ent));
		}
	}

	private void OnSquadMemberTerminating(Entity<SquadMemberComponent> ent, ref EntityTerminatingEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		SquadTeamComponent team = default(SquadTeamComponent);
		if (_squadTeamQuery.TryComp(ent.Comp.Squad, ref team))
		{
			team.Members.Remove(Entity<SquadMemberComponent>.op_Implicit(ent));
		}
	}

	private void OnSquadMemberMobStateChanged(Entity<SquadMemberComponent> ent, ref MobStateChangedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? squad = ent.Comp.Squad;
		if (squad.HasValue)
		{
			EntityUid squad2 = squad.GetValueOrDefault();
			SquadMemberUpdatedEvent ev = new SquadMemberUpdatedEvent(squad2);
			((EntitySystem)this).RaiseLocalEvent<SquadMemberUpdatedEvent>(Entity<SquadMemberComponent>.op_Implicit(ent), ref ev, false);
		}
	}

	private void OnSquadMemberPlayerAttached(Entity<SquadMemberComponent> ent, ref PlayerAttachedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? squad = ent.Comp.Squad;
		if (squad.HasValue)
		{
			EntityUid squad2 = squad.GetValueOrDefault();
			SquadMemberUpdatedEvent ev = new SquadMemberUpdatedEvent(squad2);
			((EntitySystem)this).RaiseLocalEvent<SquadMemberUpdatedEvent>(Entity<SquadMemberComponent>.op_Implicit(ent), ref ev, false);
		}
	}

	private void OnSquadMemberPlayerDetached(Entity<SquadMemberComponent> ent, ref PlayerDetachedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? squad = ent.Comp.Squad;
		if (squad.HasValue)
		{
			EntityUid squad2 = squad.GetValueOrDefault();
			SquadMemberUpdatedEvent ev = new SquadMemberUpdatedEvent(squad2);
			((EntitySystem)this).RaiseLocalEvent<SquadMemberUpdatedEvent>(Entity<SquadMemberComponent>.op_Implicit(ent), ref ev, false);
		}
	}

	private void OnSquadRoleGetIcon(Entity<SquadMemberComponent> member, ref GetMarineIconEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		args.Background = member.Comp.Background;
		args.BackgroundColor = member.Comp.BackgroundColor;
	}

	private void OnSquadRoleGetName(Entity<SquadMemberComponent> member, ref GetMarineSquadNameEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		if (TryGetMemberSquad(Entity<SquadMemberComponent>.op_Implicit(member.Owner), out Entity<SquadTeamComponent> squadTeam))
		{
			args.SquadName = ((EntitySystem)this).Name(Entity<SquadTeamComponent>.op_Implicit(squadTeam), (MetaDataComponent)null);
		}
		ProtoId<JobPrototype>? jobId = _originalRoleQuery.CompOrNull(Entity<SquadMemberComponent>.op_Implicit(member))?.Job;
		JobPrototype jobProto = default(JobPrototype);
		EntityUid mindId;
		MindComponent mind;
		string name;
		if (_prototypes.TryIndex<JobPrototype>(jobId, ref jobProto))
		{
			args.RoleName = jobProto.LocalizedName;
		}
		else if (_mind.TryGetMind(Entity<SquadMemberComponent>.op_Implicit(member), out mindId, out mind) && _job.MindTryGetJobName(mindId, out name))
		{
			args.RoleName = name;
		}
	}

	private void OnSquadMemberEnteredCryo(Entity<SquadMemberComponent> ent, ref EnteredCryostorageEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		OriginalRoleComponent role = default(OriginalRoleComponent);
		if (!_originalRoleQuery.TryComp(Entity<SquadMemberComponent>.op_Implicit(ent), ref role))
		{
			return;
		}
		ProtoId<JobPrototype>? job = role.Job;
		if (job.HasValue)
		{
			ProtoId<JobPrototype> jobId = job.GetValueOrDefault();
			SquadTeamComponent squad = default(SquadTeamComponent);
			if (_squadTeamQuery.TryComp(ent.Comp.Squad, ref squad) && squad.Roles.TryGetValue(jobId, out var roles) && roles > 0)
			{
				squad.Roles[jobId] = roles - 1;
			}
		}
	}

	private void OnSquadMemberLeftCryo(Entity<SquadMemberComponent> ent, ref LeftCryostorageEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		OriginalRoleComponent role = default(OriginalRoleComponent);
		if (!_originalRoleQuery.TryComp(Entity<SquadMemberComponent>.op_Implicit(ent), ref role))
		{
			return;
		}
		ProtoId<JobPrototype>? job = role.Job;
		if (job.HasValue)
		{
			ProtoId<JobPrototype> jobId = job.GetValueOrDefault();
			SquadTeamComponent squad = default(SquadTeamComponent);
			if (_squadTeamQuery.TryComp(ent.Comp.Squad, ref squad) && squad.Roles.TryGetValue(jobId, out var roles))
			{
				squad.Roles[jobId] = roles + 1;
			}
		}
	}

	private void OnSquadLeaderTerminating(Entity<SquadLeaderComponent> ent, ref EntityTerminatingEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? headset = ent.Comp.Headset;
		if (headset.HasValue)
		{
			EntityUid headset2 = headset.GetValueOrDefault();
			((EntitySystem)this).RemCompDeferred<SquadLeaderHeadsetComponent>(headset2);
		}
	}

	private void OnSquadLeaderGetMarineIcon(Entity<SquadLeaderComponent> ent, ref GetMarineIconEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		args.Icon = (SpriteSpecifier?)(object)ent.Comp.Icon;
	}

	private void OnSquadLeaderHeadsetChannelsChanged(Entity<SquadLeaderHeadsetComponent> ent, ref EncryptionChannelsChangedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		foreach (ProtoId<RadioChannelPrototype> channel in ent.Comp.Channels)
		{
			args.Component.Channels.Add(ProtoId<RadioChannelPrototype>.op_Implicit(channel));
		}
	}

	private void OnSquadLeaderHeadsetTerminating(Entity<SquadLeaderHeadsetComponent> ent, ref EntityTerminatingEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		SquadLeaderComponent leader = default(SquadLeaderComponent);
		if (((EntitySystem)this).TryComp<SquadLeaderComponent>(ent.Comp.Leader, ref leader))
		{
			leader.Headset = null;
			((EntitySystem)this).Dirty(ent.Comp.Leader, (IComponent)(object)leader, (MetaDataComponent)null);
		}
	}

	private void OnAssignSquadPlayerSpawnComplete(Entity<AssignSquadComponent> ent, ref PlayerSpawnCompleteEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<SquadTeamComponent> query = ((EntitySystem)this).EntityQueryEnumerator<SquadTeamComponent>();
		EntityUid uid = default(EntityUid);
		SquadTeamComponent comp = default(SquadTeamComponent);
		while (query.MoveNext(ref uid, ref comp))
		{
			if (_entityWhitelist.IsWhitelistPass(ent.Comp.Whitelist, uid))
			{
				AssignSquad(Entity<AssignSquadComponent>.op_Implicit(ent), Entity<SquadTeamComponent>.op_Implicit((uid, comp)), ProtoId<JobPrototype>.op_Implicit(args.JobId));
			}
		}
	}

	private void SearchForMappedItems(Entity<SquadMemberComponent> user, EntityUid squad)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		if (!_inventory.TryGetContainerSlotEnumerator(Entity<InventoryComponent>.op_Implicit(user.Owner), out var slots))
		{
			return;
		}
		ContainerSlot slot;
		RMCMapToSquadComponent mapToSquad = default(RMCMapToSquadComponent);
		StorageComponent storage = default(StorageComponent);
		RMCMapToSquadComponent mapToSquadStorage = default(RMCMapToSquadComponent);
		EncryptionKeyHolderComponent holder = default(EncryptionKeyHolderComponent);
		while (slots.MoveNext(out slot))
		{
			EntityUid? containedEntity = slot.ContainedEntity;
			if (!containedEntity.HasValue)
			{
				continue;
			}
			EntityUid slotEntity = containedEntity.GetValueOrDefault();
			if (_mapToSquadQuery.TryComp(slotEntity, ref mapToSquad))
			{
				MapToSquad(Entity<RMCMapToSquadComponent>.op_Implicit((slotEntity, mapToSquad)), Entity<SquadMemberComponent>.op_Implicit(user), squad);
			}
			else if (((EntitySystem)this).TryComp<StorageComponent>(slotEntity, ref storage))
			{
				foreach (EntityUid contained in ((BaseContainer)storage.Container).ContainedEntities)
				{
					if (_mapToSquadQuery.TryComp(contained, ref mapToSquadStorage))
					{
						MapToSquad(Entity<RMCMapToSquadComponent>.op_Implicit((contained, mapToSquadStorage)), Entity<SquadMemberComponent>.op_Implicit(user), squad);
					}
				}
			}
			else if (((EntitySystem)this).TryComp<EncryptionKeyHolderComponent>(slotEntity, ref holder))
			{
				_encryptionKey.UpdateChannels(slotEntity, holder);
				break;
			}
		}
	}

	private void MapToSquad(Entity<RMCMapToSquadComponent> ent, EntityUid user, EntityUid squad)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		EntProtoId? item = null;
		MetaDataComponent obj = ((EntitySystem)this).CompOrNull<MetaDataComponent>(squad);
		EntityPrototype squadPrototype = ((obj != null) ? obj.EntityPrototype : null);
		if (squadPrototype != null && ent.Comp.Map.TryGetValue(EntProtoId.op_Implicit(squadPrototype.ID), out var mapped))
		{
			item = mapped;
		}
		if (item.HasValue)
		{
			EntProtoId? val = item;
			EntityUid newItem = ((EntitySystem)this).SpawnNextToOrDrop(val.HasValue ? EntProtoId.op_Implicit(val.GetValueOrDefault()) : null, user, (TransformComponent)null, (ComponentRegistry)null);
			ClothingComponent clothing = default(ClothingComponent);
			if (((EntitySystem)this).TryComp<ClothingComponent>(newItem, ref clothing) && !_cmInventory.TryEquipClothing(user, Entity<ClothingComponent>.op_Implicit((newItem, clothing))))
			{
				_hands.TryPickupAnyHand(user, newItem);
			}
		}
		((EntitySystem)this).QueueDel((EntityUid?)Entity<RMCMapToSquadComponent>.op_Implicit(ent));
	}

	private void OnPrototypesReloaded(PrototypesReloadedEventArgs ev)
	{
		if (ev.WasModified<EntityPrototype>() || ev.WasModified<JobPrototype>())
		{
			RefreshSquadPrototypes();
		}
	}

	private void RefreshSquadPrototypes()
	{
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		ImmutableArray<EntityPrototype>.Builder entBuilder = ImmutableArray.CreateBuilder<EntityPrototype>();
		foreach (EntityPrototype entity in _prototypes.EnumeratePrototypes<EntityPrototype>())
		{
			if (entity.HasComponent<SquadTeamComponent>((IComponentFactory?)null))
			{
				entBuilder.Add(entity);
			}
		}
		entBuilder.Sort((EntityPrototype a, EntityPrototype b) => string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase));
		SquadPrototypes = entBuilder.ToImmutable();
		ImmutableArray<JobPrototype>.Builder jobBuilder = ImmutableArray.CreateBuilder<JobPrototype>();
		foreach (JobPrototype job in _prototypes.EnumeratePrototypes<JobPrototype>())
		{
			if (job.HasSquad)
			{
				jobBuilder.Add(job);
			}
		}
		JobPrototype intelJob = default(JobPrototype);
		if (_prototypes.TryIndex<JobPrototype>(IntelOfficerJob, ref intelJob))
		{
			jobBuilder.Add(intelJob);
		}
		SquadRolePrototypes = jobBuilder.ToImmutable();
	}

	public bool TryGetSquad(EntProtoId prototype, out Entity<SquadTeamComponent> squad)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<SquadTeamComponent, MetaDataComponent> squadQuery = ((EntitySystem)this).EntityQueryEnumerator<SquadTeamComponent, MetaDataComponent>();
		EntityUid uid = default(EntityUid);
		SquadTeamComponent team = default(SquadTeamComponent);
		MetaDataComponent metaData = default(MetaDataComponent);
		while (squadQuery.MoveNext(ref uid, ref team, ref metaData))
		{
			EntityPrototype entityPrototype = metaData.EntityPrototype;
			if (!(((entityPrototype != null) ? entityPrototype.ID : null) != ((EntProtoId)(ref prototype)).Id))
			{
				squad = Entity<SquadTeamComponent>.op_Implicit((uid, team));
				return true;
			}
		}
		squad = default(Entity<SquadTeamComponent>);
		return false;
	}

	public bool TryGetMemberSquad(Entity<SquadMemberComponent?> member, out Entity<SquadTeamComponent> squad)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		squad = default(Entity<SquadTeamComponent>);
		if (!((EntitySystem)this).Resolve<SquadMemberComponent>(Entity<SquadMemberComponent>.op_Implicit(member), ref member.Comp, false))
		{
			return false;
		}
		SquadTeamComponent team = default(SquadTeamComponent);
		if (!((EntitySystem)this).TryComp<SquadTeamComponent>(member.Comp.Squad, ref team))
		{
			return false;
		}
		squad = Entity<SquadTeamComponent>.op_Implicit((member.Comp.Squad.Value, team));
		return true;
	}

	public bool HasSquad(EntProtoId id)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		Entity<SquadTeamComponent> squad;
		return TryGetSquad(id, out squad);
	}

	public bool TryEnsureSquad(EntProtoId id, out Entity<SquadTeamComponent> squad)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		EntityPrototype prototype = default(EntityPrototype);
		if (!_prototypes.TryIndex(id, ref prototype) || !prototype.HasComponent<SquadTeamComponent>(_compFactory))
		{
			squad = default(Entity<SquadTeamComponent>);
			return false;
		}
		if (TryGetSquad(id, out squad))
		{
			return true;
		}
		EntityUid squadEnt = ((EntitySystem)this).Spawn(EntProtoId.op_Implicit(id), (ComponentRegistry)null, true);
		SquadTeamComponent squadComp = default(SquadTeamComponent);
		if (!((EntitySystem)this).TryComp<SquadTeamComponent>(squadEnt, ref squadComp))
		{
			((EntitySystem)this).Log.Error($"Squad entity prototype {id} had {"SquadTeamComponent"}, but none found on entity {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(squadEnt))}");
			return false;
		}
		squad = Entity<SquadTeamComponent>.op_Implicit((squadEnt, squadComp));
		return true;
	}

	public int GetSquadMembersAlive(Entity<SquadTeamComponent> team)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		int count = 0;
		EntityQueryEnumerator<SquadMemberComponent> members = ((EntitySystem)this).EntityQueryEnumerator<SquadMemberComponent>();
		EntityUid uid = default(EntityUid);
		SquadMemberComponent member = default(SquadMemberComponent);
		while (members.MoveNext(ref uid, ref member))
		{
			EntityUid? squad = member.Squad;
			EntityUid val = Entity<SquadTeamComponent>.op_Implicit(team);
			if (squad.HasValue && squad.GetValueOrDefault() == val && !_mobState.IsDead(uid))
			{
				count++;
			}
		}
		return count;
	}

	public void AssignSquad(EntityUid marine, Entity<SquadTeamComponent?> team, ProtoId<JobPrototype>? job)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<SquadTeamComponent>(Entity<SquadTeamComponent>.op_Implicit(team), ref team.Comp, true))
		{
			return;
		}
		SquadMemberComponent member = ((EntitySystem)this).EnsureComp<SquadMemberComponent>(marine);
		EntityUid? oldSquadId = member.Squad;
		ProtoId<JobPrototype>? role = job ?? _originalRoleQuery.CompOrNull(marine)?.Job;
		SquadTeamComponent oldSquad = default(SquadTeamComponent);
		if (_squadTeamQuery.TryComp(oldSquadId, ref oldSquad))
		{
			oldSquad.Members.Remove(marine);
			if (role.HasValue && oldSquad.Roles.TryGetValue(role.Value, out var oldJobs) && oldJobs > 0)
			{
				oldSquad.Roles[role.Value] = oldJobs - 1;
			}
		}
		member.Squad = Entity<SquadTeamComponent>.op_Implicit(team);
		member.Background = team.Comp.Background;
		member.BackgroundColor = team.Comp.Color;
		member.AccessibleBackgroundColor = team.Comp.AccessibleColor;
		member.BlacklistedSquadArmor = team.Comp.BlacklistedSquadArmor;
		((EntitySystem)this).Dirty(marine, (IComponent)(object)member, (MetaDataComponent)null);
		SquadGrantAccessComponent grant = ((EntitySystem)this).EnsureComp<SquadGrantAccessComponent>(marine);
		grant.AccessLevels = team.Comp.AccessLevels;
		JobPrototype jobProto = default(JobPrototype);
		EntityUid mindId;
		MindComponent mind;
		string name;
		if (_prototypes.TryIndex<JobPrototype>(job, ref jobProto))
		{
			grant.RoleName = ((EntitySystem)this).Name(Entity<SquadTeamComponent>.op_Implicit(team), (MetaDataComponent)null) + " " + jobProto.LocalizedName;
		}
		else if (_mind.TryGetMind(marine, out mindId, out mind) && _job.MindTryGetJobName(mindId, out name))
		{
			MarineSetTitle(marine, ((EntitySystem)this).Name(Entity<SquadTeamComponent>.op_Implicit(team), (MetaDataComponent)null) + " " + name);
		}
		((EntitySystem)this).Dirty(marine, (IComponent)(object)grant, (MetaDataComponent)null);
		team.Comp.Members.Add(marine);
		if (role.HasValue)
		{
			team.Comp.Roles.TryGetValue(role.Value, out var roles);
			team.Comp.Roles[role.Value] = roles + 1;
		}
		SquadMemberUpdatedEvent ev = new SquadMemberUpdatedEvent(Entity<SquadTeamComponent>.op_Implicit(team));
		((EntitySystem)this).RaiseLocalEvent<SquadMemberUpdatedEvent>(marine, ref ev, false);
		if (oldSquadId.HasValue && oldSquad != null)
		{
			SquadMemberRemovedEvent removeEv = new SquadMemberRemovedEvent(Entity<SquadTeamComponent>.op_Implicit((oldSquadId.Value, oldSquad)), marine);
			((EntitySystem)this).RaiseLocalEvent<SquadMemberRemovedEvent>(marine, ref removeEv, true);
		}
		SquadMemberAddedEvent addEv = new SquadMemberAddedEvent(Entity<SquadTeamComponent>.op_Implicit((Entity<SquadTeamComponent>.op_Implicit(team), team.Comp)), marine);
		((EntitySystem)this).RaiseLocalEvent<SquadMemberAddedEvent>(marine, ref addEv, true);
		EntityPrototype obj = ((EntitySystem)this).Prototype(Entity<SquadTeamComponent>.op_Implicit(team), (MetaDataComponent)null);
		string squadProto = ((obj != null) ? obj.ID : null);
		if (squadProto != null)
		{
			_appearance.SetData(marine, (Enum)SquadVisuals.Squad, (object)squadProto, (AppearanceComponent)null);
		}
		UpdateSquadTitle(marine);
		SearchForMappedItems(Entity<SquadMemberComponent>.op_Implicit((marine, member)), member.Squad.Value);
	}

	public void RemoveSquad(EntityUid marine, ProtoId<JobPrototype>? job)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).RemComp<SquadLeaderComponent>(marine);
		SquadMemberComponent member = default(SquadMemberComponent);
		if (!((EntitySystem)this).TryComp<SquadMemberComponent>(marine, ref member))
		{
			return;
		}
		EntityUid? oldSquadId = member.Squad;
		ProtoId<JobPrototype>? role = job ?? _originalRoleQuery.CompOrNull(marine)?.Job;
		SquadTeamComponent oldSquad = default(SquadTeamComponent);
		if (_squadTeamQuery.TryComp(oldSquadId, ref oldSquad))
		{
			oldSquad.Members.Remove(marine);
			if (role.HasValue && oldSquad.Roles.TryGetValue(role.Value, out var oldJobs) && oldJobs > 0)
			{
				oldSquad.Roles[role.Value] = oldJobs - 1;
			}
		}
		((EntitySystem)this).RemComp<SquadMemberComponent>(marine);
		if (oldSquadId.HasValue && oldSquad != null)
		{
			SquadMemberRemovedEvent removeEv = new SquadMemberRemovedEvent(Entity<SquadTeamComponent>.op_Implicit((oldSquadId.Value, oldSquad)), marine);
			((EntitySystem)this).RaiseLocalEvent<SquadMemberRemovedEvent>(marine, ref removeEv, true);
		}
	}

	public void UpdateSquadTitle(EntityUid marine)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		SquadNameOverrideComponent overrideComp = default(SquadNameOverrideComponent);
		if (((EntitySystem)this).TryComp<SquadNameOverrideComponent>(marine, ref overrideComp))
		{
			MarineSetTitle(marine, base.Loc.GetString(LocId.op_Implicit(overrideComp.Name)));
			return;
		}
		GetMarineSquadNameEvent ev = default(GetMarineSquadNameEvent);
		((EntitySystem)this).RaiseLocalEvent<GetMarineSquadNameEvent>(marine, ref ev, false);
		MarineSetTitle(marine, ev.SquadName + " " + ev.RoleName);
	}

	public void MarineSetTitle(EntityUid marine, string title)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		IdCardComponent idCard = default(IdCardComponent);
		foreach (EntityUid item in _inventory.GetHandOrInventoryEntities(Entity<HandsComponent, InventoryComponent>.op_Implicit(marine)))
		{
			if (((EntitySystem)this).TryComp<IdCardComponent>(item, ref idCard))
			{
				_id.TryChangeJobTitle(item, title, idCard);
			}
		}
	}

	public void RefreshSquad(Entity<SquadTeamComponent?> squad)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		if (!_squadTeamQuery.Resolve(Entity<SquadTeamComponent>.op_Implicit(squad), ref squad.Comp, false))
		{
			return;
		}
		List<EntityUid> toRemove = new List<EntityUid>();
		SquadMemberComponent memberComp = default(SquadMemberComponent);
		foreach (EntityUid member in squad.Comp.Members)
		{
			if (!((EntitySystem)this).TerminatingOrDeleted(member, (MetaDataComponent)null) && _squadMemberQuery.TryComp(member, ref memberComp))
			{
				EntityUid? squad2 = memberComp.Squad;
				EntityUid val = Entity<SquadTeamComponent>.op_Implicit(squad);
				if (squad2.HasValue && !(squad2.GetValueOrDefault() != val))
				{
					continue;
				}
			}
			toRemove.Add(member);
		}
		squad.Comp.Members.ExceptWith(toRemove);
		((EntitySystem)this).Dirty<SquadTeamComponent>(squad, (MetaDataComponent)null);
		foreach (EntityUid member2 in toRemove)
		{
			SquadMemberUpdatedEvent ev = new SquadMemberUpdatedEvent(Entity<SquadTeamComponent>.op_Implicit(squad));
			((EntitySystem)this).RaiseLocalEvent<SquadMemberUpdatedEvent>(member2, ref ev, false);
			SquadMemberRemovedEvent squadEv = new SquadMemberRemovedEvent(Entity<SquadTeamComponent>.op_Implicit((Entity<SquadTeamComponent>.op_Implicit(squad), squad.Comp)), member2);
			((EntitySystem)this).RaiseLocalEvent<SquadMemberRemovedEvent>(member2, ref squadEv, true);
		}
	}

	public bool IsInSquad(Entity<SquadMemberComponent?> member, EntProtoId<SquadTeamComponent> squad)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<SquadMemberComponent>(Entity<SquadMemberComponent>.op_Implicit(member), ref member.Comp, false))
		{
			return false;
		}
		EntityUid? squad2 = member.Comp.Squad;
		if (squad2.HasValue)
		{
			EntityUid memberSquad = squad2.GetValueOrDefault();
			EntityPrototype obj = ((EntitySystem)this).Prototype(memberSquad, (MetaDataComponent)null);
			return ((obj != null) ? obj.ID : null) == squad.Id;
		}
		return false;
	}

	public bool IsInSquad(Entity<SquadMemberComponent?> member, EntityUid squad)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<SquadMemberComponent>(Entity<SquadMemberComponent>.op_Implicit(member), ref member.Comp, false))
		{
			return false;
		}
		EntityUid? squad2 = member.Comp.Squad;
		if (squad2.HasValue)
		{
			EntityUid memberSquad = squad2.GetValueOrDefault();
			return memberSquad == squad;
		}
		return false;
	}

	public void PromoteSquadLeader(Entity<SquadMemberComponent?> toPromote, EntityUid user, Rsi icon)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0378: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0395: Unknown result type (might be due to invalid IL or missing references)
		//IL_0396: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0323: Unknown result type (might be due to invalid IL or missing references)
		//IL_0329: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<SquadLeaderComponent>(Entity<SquadMemberComponent>.op_Implicit(toPromote)))
		{
			return;
		}
		if (_rmcBan.IsJobBanned(Entity<ActorComponent>.op_Implicit(toPromote.Owner), SquadLeaderJob))
		{
			_popup.PopupCursor(((EntitySystem)this).Name(Entity<SquadMemberComponent>.op_Implicit(toPromote), (MetaDataComponent)null) + " is unfit to lead!", user, PopupType.MediumCaution);
			return;
		}
		if (_mobState.IsDead(Entity<SquadMemberComponent>.op_Implicit(toPromote)))
		{
			_popup.PopupCursor(((EntitySystem)this).Name(Entity<SquadMemberComponent>.op_Implicit(toPromote), (MetaDataComponent)null) + " is KIA!", user, PopupType.MediumCaution);
			return;
		}
		if (((EntitySystem)this).Resolve<SquadMemberComponent>(Entity<SquadMemberComponent>.op_Implicit(toPromote), ref toPromote.Comp, false))
		{
			EntityQueryEnumerator<SquadLeaderComponent, SquadMemberComponent> leaders = ((EntitySystem)this).EntityQueryEnumerator<SquadLeaderComponent, SquadMemberComponent>();
			EntityUid uid = default(EntityUid);
			SquadLeaderComponent leader = default(SquadLeaderComponent);
			SquadMemberComponent otherMember = default(SquadMemberComponent);
			EncryptionKeyHolderComponent holder = default(EncryptionKeyHolderComponent);
			MarineComponent otherMarine = default(MarineComponent);
			MarineOrdersComponent otherOrders = default(MarineOrdersComponent);
			while (leaders.MoveNext(ref uid, ref leader, ref otherMember))
			{
				EntityUid? squad = otherMember.Squad;
				EntityUid? squad2 = toPromote.Comp.Squad;
				if (squad.HasValue != squad2.HasValue || (squad.HasValue && squad.GetValueOrDefault() != squad2.GetValueOrDefault()))
				{
					continue;
				}
				squad2 = leader.Headset;
				if (squad2.HasValue)
				{
					EntityUid headset = squad2.GetValueOrDefault();
					((EntitySystem)this).RemComp<SquadLeaderHeadsetComponent>(headset);
					if (((EntitySystem)this).TryComp<EncryptionKeyHolderComponent>(headset, ref holder))
					{
						_encryptionKey.UpdateChannels(headset, holder);
					}
				}
				if (((EntitySystem)this).TryComp<MarineComponent>(uid, ref otherMarine) && object.Equals(otherMarine.Icon, leader.Icon))
				{
					_marine.ClearIcon(Entity<MarineComponent>.op_Implicit((uid, otherMarine)));
				}
				if (((EntitySystem)this).TryComp<MarineOrdersComponent>(uid, ref otherOrders) && !otherOrders.Intrinsic)
				{
					((EntitySystem)this).RemCompDeferred<MarineOrdersComponent>(uid);
				}
				((EntitySystem)this).RemComp<SquadLeaderComponent>(uid);
				((EntitySystem)this).RemComp<RMCTrackableComponent>(uid);
				((EntitySystem)this).RemCompDeferred<RMCPointingComponent>(uid);
			}
		}
		SquadLeaderComponent newLeader = ((EntitySystem)this).EnsureComp<SquadLeaderComponent>(Entity<SquadMemberComponent>.op_Implicit(toPromote));
		newLeader.Icon = icon;
		MarineOrdersComponent orders = default(MarineOrdersComponent);
		if (!((EntitySystem)this).EnsureComp<MarineOrdersComponent>(Entity<SquadMemberComponent>.op_Implicit(toPromote), ref orders))
		{
			orders.Intrinsic = false;
			((EntitySystem)this).Dirty(Entity<SquadMemberComponent>.op_Implicit(toPromote), (IComponent)(object)orders, (MetaDataComponent)null);
			_marineOrders.StartActionUseDelay(Entity<MarineOrdersComponent>.op_Implicit((Entity<SquadMemberComponent>.op_Implicit(toPromote), orders)));
		}
		((EntitySystem)this).EnsureComp<RMCTrackableComponent>(Entity<SquadMemberComponent>.op_Implicit(toPromote));
		((EntitySystem)this).EnsureComp<RMCPointingComponent>(Entity<SquadMemberComponent>.op_Implicit(toPromote));
		InventorySystem.InventorySlotEnumerator slots = _inventory.GetSlotEnumerator(Entity<InventoryComponent>.op_Implicit(toPromote.Owner), SlotFlags.EARS);
		ContainerSlot slot;
		EncryptionKeyHolderComponent holder2 = default(EncryptionKeyHolderComponent);
		while (slots.MoveNext(out slot))
		{
			EntityUid? squad2 = slot.ContainedEntity;
			if (squad2.HasValue)
			{
				EntityUid contained = squad2.GetValueOrDefault();
				if (((EntitySystem)this).TryComp<EncryptionKeyHolderComponent>(contained, ref holder2))
				{
					newLeader.Headset = contained;
					((EntitySystem)this).Dirty(Entity<SquadMemberComponent>.op_Implicit(toPromote), (IComponent)(object)newLeader, (MetaDataComponent)null);
					((EntitySystem)this).EnsureComp<SquadLeaderHeadsetComponent>(contained);
					_encryptionKey.UpdateChannels(contained, holder2);
					break;
				}
			}
		}
		EntityUid? squad3 = toPromote.Comp?.Squad;
		ActorComponent actor = default(ActorComponent);
		if (((EntitySystem)this).TryComp<ActorComponent>(Entity<SquadMemberComponent>.op_Implicit(toPromote), ref actor))
		{
			string squadStr = (((EntitySystem)this).Exists(squad3) ? (" for " + ((EntitySystem)this).Name(squad3.Value, (MetaDataComponent)null)) : string.Empty);
			string message = "Overwatch: You've been promoted to 'ACTING SQUAD LEADER'" + squadStr + ". Your headset has access to the command channel (:v).";
			_rmcChat.ChatMessageToOne(ChatChannel.Local, message, message, default(EntityUid), hideChat: false, actor.PlayerSession.Channel, Color.FromHex((ReadOnlySpan<char>)"#0084FF", (Color?)null), recordReplay: true);
		}
		if (((EntitySystem)this).Exists(squad3))
		{
			EntityPrototype squadProto = ((EntitySystem)this).Prototype(squad3.Value, (MetaDataComponent)null);
			if (squadProto != null)
			{
				_marineAnnounce.AnnounceSquad("Attention: A new Squad Leader has been set: " + ((EntitySystem)this).Name(Entity<SquadMemberComponent>.op_Implicit(toPromote), (MetaDataComponent)null), EntProtoId<SquadTeamComponent>.op_Implicit(squadProto.ID));
				_popup.PopupCursor(((EntitySystem)this).Name(Entity<SquadMemberComponent>.op_Implicit(toPromote), (MetaDataComponent)null) + " is " + ((EntitySystem)this).Name(squad3.Value, (MetaDataComponent)null) + "'s new leader!", user, PopupType.Medium);
			}
		}
	}

	public bool AreInSameSquad(Entity<SquadMemberComponent?> one, Entity<SquadMemberComponent?> two)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<SquadMemberComponent>(Entity<SquadMemberComponent>.op_Implicit(one), ref one.Comp, false) || !((EntitySystem)this).Resolve<SquadMemberComponent>(Entity<SquadMemberComponent>.op_Implicit(two), ref two.Comp, false))
		{
			return false;
		}
		if (!one.Comp.Squad.HasValue)
		{
			return false;
		}
		EntityUid? squad = one.Comp.Squad;
		EntityUid? squad2 = two.Comp.Squad;
		if (squad.HasValue != squad2.HasValue)
		{
			return false;
		}
		if (!squad.HasValue)
		{
			return true;
		}
		return squad.GetValueOrDefault() == squad2.GetValueOrDefault();
	}

	public bool TryGetSquadLeader(Entity<SquadTeamComponent> squad, out Entity<SquadLeaderComponent> leader)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<SquadLeaderComponent, SquadMemberComponent> leaders = ((EntitySystem)this).EntityQueryEnumerator<SquadLeaderComponent, SquadMemberComponent>();
		EntityUid uid = default(EntityUid);
		SquadLeaderComponent leaderComp = default(SquadLeaderComponent);
		SquadMemberComponent member = default(SquadMemberComponent);
		while (leaders.MoveNext(ref uid, ref leaderComp, ref member))
		{
			EntityUid? squad2 = member.Squad;
			EntityUid val = Entity<SquadTeamComponent>.op_Implicit(squad);
			if (squad2.HasValue && !(squad2.GetValueOrDefault() != val))
			{
				leader = Entity<SquadLeaderComponent>.op_Implicit((uid, leaderComp));
				return true;
			}
		}
		leader = default(Entity<SquadLeaderComponent>);
		return false;
	}

	public bool IsSquadLeader(ProtoId<JobPrototype> job)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return job == SquadLeaderJob;
	}

	public bool HasSpaceForRole(Entity<SquadTeamComponent> squad, ProtoId<JobPrototype> job)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		if (!squad.Comp.MaxRoles.TryGetValue(job, out var maxRoles))
		{
			return true;
		}
		squad.Comp.Roles.TryGetValue(job, out var currentRoles);
		return currentRoles < maxRoles;
	}

	public List<EntityUid>? GetEntitiesWithHighestSquad(List<EntityUid> entities, ProtoId<DatasetPrototype> squadHierarchyId)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		new List<EntityUid>();
		DatasetPrototype squadHierarchy = default(DatasetPrototype);
		if (!_prototypes.TryIndex<DatasetPrototype>(squadHierarchyId, ref squadHierarchy))
		{
			return null;
		}
		List<string> squadOrder = squadHierarchy.Values.ToList();
		if (squadOrder.Count == 0)
		{
			((EntitySystem)this).Log.Error($"The squad hierarchy dataset '{squadHierarchyId}' has an invalid value: empty. The highest squad cannot be determined.");
			return null;
		}
		Dictionary<EntityUid, int> squadScores = new Dictionary<EntityUid, int>();
		int highestSquadIndex = -1;
		SquadMemberComponent squadComp = default(SquadMemberComponent);
		foreach (EntityUid candidate in entities)
		{
			if (!((EntitySystem)this).TryComp<SquadMemberComponent>(candidate, ref squadComp) || !squadComp.Squad.HasValue)
			{
				continue;
			}
			int squadIndex = ((squadComp.Squad.HasValue && squadComp.Squad.ToString() != null) ? squadOrder.IndexOf(squadComp.Squad.ToString()) : (-1));
			if (squadIndex != -1)
			{
				squadScores[candidate] = squadIndex;
				if (squadIndex > highestSquadIndex)
				{
					highestSquadIndex = squadIndex;
				}
			}
		}
		if (highestSquadIndex == -1)
		{
			return null;
		}
		return (from pair in squadScores
			where pair.Value == highestSquadIndex
			select pair.Key).ToList();
	}

	public bool TryGetSquadMemberColor(EntityUid entity, out Color color, bool accessible = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		color = default(Color);
		SquadMemberComponent comp = default(SquadMemberComponent);
		if (!((EntitySystem)this).TryComp<SquadMemberComponent>(entity, ref comp))
		{
			return false;
		}
		color = ((accessible && comp.AccessibleBackgroundColor.HasValue) ? comp.AccessibleBackgroundColor.Value : comp.BackgroundColor);
		return true;
	}

	public void SetSquadMaxRole(Entity<SquadTeamComponent?> squad, ProtoId<JobPrototype> job, int amount)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<SquadTeamComponent>(Entity<SquadTeamComponent>.op_Implicit(squad), ref squad.Comp, false))
		{
			squad.Comp.MaxRoles[job] = amount;
		}
	}

	public override void Update(float frameTime)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<SquadGrantAccessComponent> query = ((EntitySystem)this).EntityQueryEnumerator<SquadGrantAccessComponent>();
		EntityUid uid = default(EntityUid);
		SquadGrantAccessComponent grant = default(SquadGrantAccessComponent);
		AccessComponent access = default(AccessComponent);
		IdCardOwnerComponent owner = default(IdCardOwnerComponent);
		while (query.MoveNext(ref uid, ref grant))
		{
			if (grant.RoleName != null)
			{
				UpdateSquadTitle(uid);
			}
			foreach (EntityUid item in _inventory.GetHandOrInventoryEntities(Entity<HandsComponent, InventoryComponent>.op_Implicit(uid)))
			{
				if (grant.AccessLevels.Length != 0 && ((EntitySystem)this).TryComp<AccessComponent>(item, ref access))
				{
					ProtoId<AccessLevelPrototype>[] accessLevels = grant.AccessLevels;
					foreach (ProtoId<AccessLevelPrototype> level in accessLevels)
					{
						access.Tags.Add(level);
					}
					((EntitySystem)this).Dirty(item, (IComponent)(object)access, (MetaDataComponent)null);
				}
				if (((EntitySystem)this).HasComp<IdCardComponent>(item) && !((EntitySystem)this).EnsureComp<IdCardOwnerComponent>(item, ref owner))
				{
					owner.Id = uid;
				}
			}
			((EntitySystem)this).RemCompDeferred<SquadGrantAccessComponent>(uid);
		}
		SquadMemberComponent member = default(SquadMemberComponent);
		SquadTeamComponent squad = default(SquadTeamComponent);
		foreach (EntityUid toUpdate in _membersToUpdate)
		{
			if (!((EntitySystem)this).TerminatingOrDeleted(toUpdate, (MetaDataComponent)null) && _squadMemberQuery.TryComp(toUpdate, ref member) && _squadTeamQuery.TryComp(member.Squad, ref squad))
			{
				squad.Members.Add(toUpdate);
			}
		}
		_membersToUpdate.Clear();
	}
}
