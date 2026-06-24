using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Dialog;
using Content.Shared._RMC14.Marines.Roles.Ranks;
using Content.Shared._RMC14.Marines.Skills.Pamphlets;
using Content.Shared._RMC14.Marines.Squads;
using Content.Shared._RMC14.Roles;
using Content.Shared._RMC14.Vendors;
using Content.Shared.Administration.Logs;
using Content.Shared.Alert;
using Content.Shared.Database;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Roles;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Shared._RMC14.Tracker.SquadLeader;

public sealed class SquadLeaderTrackerSystem : EntitySystem
{
	[Dependency]
	private ISharedAdminLogManager _adminLog;

	[Dependency]
	private AlertsSystem _alerts;

	[Dependency]
	private IComponentFactory _factory;

	[Dependency]
	private DialogSystem _dialog;

	[Dependency]
	private InventorySystem _inventory;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private IPrototypeManager _prototypeManager;

	[Dependency]
	private SharedRankSystem _rank;

	[Dependency]
	private SquadSystem _squad;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private TrackerSystem _tracker;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private SharedUserInterfaceSystem _ui;

	private readonly Dictionary<EntityUid, MapCoordinates> _squadLeaders = new Dictionary<EntityUid, MapCoordinates>();

	private readonly Dictionary<EntityUid, MapCoordinates>?[] _fireteamLeaders = new Dictionary<EntityUid, MapCoordinates>[3];

	private EntityQuery<FireteamLeaderComponent> _fireteamLeaderQuery;

	private EntityQuery<FireteamMemberComponent> _fireteamMemberQuery;

	private EntityQuery<OriginalRoleComponent> _originalRoleQuery;

	private EntityQuery<SquadLeaderTrackerComponent> _squadLeaderTrackerQuery;

	private EntityQuery<SquadMemberComponent> _squadMemberQuery;

	private const string SquadTrackerCategory = "SquadTracker";

	private const string SquadLeaderMode = "SquadLeader";

	private const string FireteamLeader = "FireteamLeader";

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
		_fireteamLeaderQuery = ((EntitySystem)this).GetEntityQuery<FireteamLeaderComponent>();
		_fireteamMemberQuery = ((EntitySystem)this).GetEntityQuery<FireteamMemberComponent>();
		_originalRoleQuery = ((EntitySystem)this).GetEntityQuery<OriginalRoleComponent>();
		_squadLeaderTrackerQuery = ((EntitySystem)this).GetEntityQuery<SquadLeaderTrackerComponent>();
		_squadMemberQuery = ((EntitySystem)this).GetEntityQuery<SquadMemberComponent>();
		((EntitySystem)this).SubscribeLocalEvent<SquadMemberAddedEvent>((EntityEventRefHandler<SquadMemberAddedEvent>)OnSquadMemberAdded, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SquadMemberRemovedEvent>((EntityEventRefHandler<SquadMemberRemovedEvent>)OnSquadMemberRemoved, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GrantSquadLeaderTrackerComponent, GotEquippedEvent>((EntityEventRefHandler<GrantSquadLeaderTrackerComponent, GotEquippedEvent>)OnGotEquipped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GrantSquadLeaderTrackerComponent, GotUnequippedEvent>((EntityEventRefHandler<GrantSquadLeaderTrackerComponent, GotUnequippedEvent>)OnGotUnequipped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SquadLeaderTrackerComponent, MapInitEvent>((EntityEventRefHandler<SquadLeaderTrackerComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SquadLeaderTrackerComponent, ComponentRemove>((EntityEventRefHandler<SquadLeaderTrackerComponent, ComponentRemove>)OnRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SquadLeaderTrackerComponent, SquadLeaderTrackerClickedEvent>((EntityEventRefHandler<SquadLeaderTrackerComponent, SquadLeaderTrackerClickedEvent>)OnSquadLeaderTrackerClicked, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SquadLeaderTrackerComponent, SquadLeaderTrackerChangeModeEvent>((EntityEventRefHandler<SquadLeaderTrackerComponent, SquadLeaderTrackerChangeModeEvent>)OnSquadLeaderTrackerChangeMode, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SquadLeaderTrackerComponent, LeaderTrackerSelectTargetEvent>((EntityEventRefHandler<SquadLeaderTrackerComponent, LeaderTrackerSelectTargetEvent>)OnLeaderTrackerSelectTargetEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SquadLeaderTrackerComponent, GetMarineSquadNameEvent>((EntityEventRefHandler<SquadLeaderTrackerComponent, GetMarineSquadNameEvent>)OnRoleChange, (Type[])null, new Type[2]
		{
			typeof(SkillPamphletSystem),
			typeof(VendorRoleOverrideSystem)
		});
		BoundUserInterfaceRegisterExt.BuiEvents<SquadLeaderTrackerComponent>(((EntitySystem)this).Subs, (object)SquadLeaderTrackerUI.Key, (BuiEventSubscriber<SquadLeaderTrackerComponent>)delegate(Subscriber<SquadLeaderTrackerComponent> subs)
		{
			subs.Event<SquadLeaderTrackerAssignFireteamMsg>((EntityEventRefHandler<SquadLeaderTrackerComponent, SquadLeaderTrackerAssignFireteamMsg>)OnAssignFireteamMsg);
			subs.Event<SquadLeaderTrackerUnassignFireteamMsg>((EntityEventRefHandler<SquadLeaderTrackerComponent, SquadLeaderTrackerUnassignFireteamMsg>)OnUnassignFireteamMsg);
			subs.Event<SquadLeaderTrackerPromoteFireteamLeaderMsg>((EntityEventRefHandler<SquadLeaderTrackerComponent, SquadLeaderTrackerPromoteFireteamLeaderMsg>)OnPromoteFireteamLeaderMsg);
			subs.Event<SquadLeaderTrackerDemoteFireteamLeaderMsg>((EntityEventRefHandler<SquadLeaderTrackerComponent, SquadLeaderTrackerDemoteFireteamLeaderMsg>)OnDemoteFireteamLeaderMsg);
			subs.Event<SquadLeaderTrackerChangeTrackedMsg>((EntityEventRefHandler<SquadLeaderTrackerComponent, SquadLeaderTrackerChangeTrackedMsg>)OnChangeTrackedMsg);
		});
	}

	private void OnSquadMemberAdded(ref SquadMemberAddedEvent ev)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		AddFireteamMember(ev.Squad.Comp.Fireteams, ev.Member);
		if (_squad.TryGetSquadLeader(ev.Squad, out Entity<SquadLeaderComponent> leader))
		{
			ev.Squad.Comp.Fireteams.SquadLeader = ((EntitySystem)this).Name(Entity<SquadLeaderComponent>.op_Implicit(leader), (MetaDataComponent)null);
			ev.Squad.Comp.Fireteams.SquadLeaderId = ((EntitySystem)this).GetNetEntity(Entity<SquadLeaderComponent>.op_Implicit(leader), (MetaDataComponent)null);
		}
		else
		{
			ev.Squad.Comp.Fireteams.SquadLeader = null;
			ev.Squad.Comp.Fireteams.SquadLeaderId = null;
		}
		SyncMemberFireteams(Entity<SquadMemberComponent>.op_Implicit(ev.Member));
	}

	private void OnSquadMemberRemoved(ref SquadMemberRemovedEvent ev)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		NetEntity netEnt = ((EntitySystem)this).GetNetEntity(ev.Member, (MetaDataComponent)null);
		RemoveFireteamMember(ev.Squad.Comp.Fireteams, netEnt);
		SyncFireteams(ev.Squad.AsNullable());
	}

	private void OnGotEquipped(Entity<GrantSquadLeaderTrackerComponent> ent, ref GotEquippedEvent args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		if (!_timing.ApplyingState && (ent.Comp.Slots & args.SlotFlags) != SlotFlags.NONE)
		{
			SquadLeaderTrackerComponent leaderTracker = ((EntitySystem)this).EnsureComp<SquadLeaderTrackerComponent>(args.Equipee);
			leaderTracker.TrackerModes = ent.Comp.TrackerModes;
			SetMode(Entity<SquadLeaderTrackerComponent>.op_Implicit((args.Equipee, leaderTracker)), ent.Comp.DefaultMode);
			((EntitySystem)this).Dirty(args.Equipee, (IComponent)(object)leaderTracker, (MetaDataComponent)null);
		}
	}

	private void OnGotUnequipped(Entity<GrantSquadLeaderTrackerComponent> ent, ref GotUnequippedEvent args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		if (!_timing.ApplyingState && (ent.Comp.Slots & args.SlotFlags) != SlotFlags.NONE && !_inventory.TryGetInventoryEntity<GrantSquadLeaderTrackerComponent>(Entity<InventoryComponent>.op_Implicit(args.Equipee), out Entity<GrantSquadLeaderTrackerComponent> _))
		{
			((EntitySystem)this).RemCompDeferred<SquadLeaderTrackerComponent>(args.Equipee);
		}
	}

	private void OnMapInit(Entity<SquadLeaderTrackerComponent> ent, ref MapInitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		UpdateDirection(ent);
		if (_squad.TryGetMemberSquad(Entity<SquadMemberComponent>.op_Implicit(ent.Owner), out Entity<SquadTeamComponent> squad))
		{
			Entity<SquadLeaderTrackerComponent> ent2 = ent;
			string squad2 = ((EntitySystem)this).Name(Entity<SquadTeamComponent>.op_Implicit(squad), (MetaDataComponent)null);
			UpdateDirection(ent2, null, squad2);
			ent.Comp.Fireteams = squad.Comp.Fireteams;
			((EntitySystem)this).Dirty<SquadLeaderTrackerComponent>(ent, (MetaDataComponent)null);
		}
	}

	private void OnRemove(Entity<SquadLeaderTrackerComponent> ent, ref ComponentRemove args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		TrackerModePrototype trackerMode = default(TrackerModePrototype);
		_prototypeManager.TryIndex<TrackerModePrototype>(ent.Comp.Mode, ref trackerMode);
		if (trackerMode != null)
		{
			_alerts.ClearAlert(Entity<SquadLeaderTrackerComponent>.op_Implicit(ent), trackerMode.Alert);
		}
	}

	private void OnSquadLeaderTrackerClicked(Entity<SquadLeaderTrackerComponent> ent, ref SquadLeaderTrackerClickedEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		_ui.TryOpenUi(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), (Enum)SquadLeaderTrackerUI.Key, Entity<SquadLeaderTrackerComponent>.op_Implicit(ent), false);
	}

	private void OnSquadLeaderTrackerChangeMode(Entity<SquadLeaderTrackerComponent> ent, ref SquadLeaderTrackerChangeModeEvent args)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		if (!_timing.IsFirstTimePredicted || !TryFindTargets(args.Mode, out List<DialogOption> options, out List<EntityUid> trackingOptions))
		{
			return;
		}
		SquadMemberComponent squadMember = default(SquadMemberComponent);
		if (_squadMemberQuery.TryComp(Entity<SquadLeaderTrackerComponent>.op_Implicit(ent), ref squadMember))
		{
			int index = 0;
			SquadMemberComponent targetSquadMember = default(SquadMemberComponent);
			while (index < trackingOptions.Count)
			{
				if (_squadMemberQuery.TryComp(trackingOptions[index], ref targetSquadMember))
				{
					EntityUid? squad = squadMember.Squad;
					EntityUid? squad2 = targetSquadMember.Squad;
					if ((squad.HasValue != squad2.HasValue || (squad.HasValue && squad.GetValueOrDefault() != squad2.GetValueOrDefault())) && !((EntitySystem)this).HasComp<SquadLeaderComponent>(Entity<SquadLeaderTrackerComponent>.op_Implicit(ent)))
					{
						options.RemoveAt(index);
						trackingOptions.RemoveAt(index);
						continue;
					}
				}
				index++;
			}
		}
		if (options.Count <= 1)
		{
			EntityUid target = default(EntityUid);
			if (Extensions.TryGetValue<EntityUid>((IList<EntityUid>)trackingOptions, 0, ref target))
			{
				SetTarget(ent, target);
			}
			else
			{
				SetTarget(ent, null);
			}
			SetMode(ent, args.Mode);
			if (_net.IsClient)
			{
				return;
			}
			MapCoordinates? location = null;
			string squadName = "";
			if (ent.Comp.Target.HasValue)
			{
				location = _transform.GetMapCoordinates(ent.Comp.Target.Value, (TransformComponent)null);
				SquadMemberComponent squad3 = default(SquadMemberComponent);
				if (_squadMemberQuery.TryComp(ent.Comp.Target.Value, ref squad3))
				{
					EntityUid? squad2 = squad3.Squad;
					if (squad2.HasValue)
					{
						EntityUid memberSquadName = squad2.GetValueOrDefault();
						squadName = ((EntitySystem)this).Name(memberSquadName, (MetaDataComponent)null);
					}
				}
			}
			UpdateDirection(ent, location, squadName);
		}
		else
		{
			_dialog.OpenOptions(Entity<SquadLeaderTrackerComponent>.op_Implicit(ent), base.Loc.GetString("rmc-squad-info-tracking-selection"), options, base.Loc.GetString("rmc-squad-info-tracking-choose"));
		}
	}

	private void OnLeaderTrackerSelectTargetEvent(Entity<SquadLeaderTrackerComponent> ent, ref LeaderTrackerSelectTargetEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		SetTarget(ent, ((EntitySystem)this).GetEntity(args.Target));
		SetMode(ent, args.Mode);
		((EntitySystem)this).Dirty<SquadLeaderTrackerComponent>(ent, (MetaDataComponent)null);
	}

	private void OnAssignFireteamMsg(Entity<SquadLeaderTrackerComponent> ent, ref SquadLeaderTrackerAssignFireteamMsg args)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? marine = default(EntityUid?);
		if (!_net.IsClient && args.Fireteam >= 0 && args.Fireteam < ent.Comp.Fireteams.Fireteams.Length && ((EntitySystem)this).TryGetEntity(args.Marine, ref marine) && CanChangeFireteamMember(((BaseBoundUserInterfaceEvent)args).Actor, marine.Value, add: true))
		{
			RemoveFireteamMember(ent.Comp.Fireteams, args.Marine);
			FireteamMemberComponent member = ((EntitySystem)this).EnsureComp<FireteamMemberComponent>(marine.Value);
			member.Fireteam = args.Fireteam;
			((EntitySystem)this).Dirty(marine.Value, (IComponent)(object)member, (MetaDataComponent)null);
			AddFireteamMember(ent.Comp.Fireteams, marine.Value);
			ISharedAdminLogManager adminLog = _adminLog;
			LogStringHandler handler = new LogStringHandler(23, 3);
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(((BaseBoundUserInterfaceEvent)args).Actor)), "ToPrettyString(args.Actor)");
			handler.AppendLiteral(" assigned ");
			handler.AppendFormatted(((EntitySystem)this).ToPrettyString(marine, (MetaDataComponent)null), "ToPrettyString(marine)");
			handler.AppendLiteral(" to fireteam ");
			handler.AppendFormatted(args.Fireteam, "args.Fireteam");
			adminLog.Add(LogType.RMCFireteam, ref handler);
			((EntitySystem)this).Dirty<SquadLeaderTrackerComponent>(ent, (MetaDataComponent)null);
			SyncMemberFireteams(Entity<SquadMemberComponent>.op_Implicit(ent.Owner));
		}
	}

	private void OnUnassignFireteamMsg(Entity<SquadLeaderTrackerComponent> ent, ref SquadLeaderTrackerUnassignFireteamMsg args)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? marine = default(EntityUid?);
		FireteamMemberComponent member = default(FireteamMemberComponent);
		if (!_net.IsClient && ((EntitySystem)this).TryGetEntity(args.Marine, ref marine) && CanChangeFireteamMember(((BaseBoundUserInterfaceEvent)args).Actor, marine.Value, add: false) && ((EntitySystem)this).TryComp<FireteamMemberComponent>(marine.Value, ref member))
		{
			RemoveFireteamMember(ent.Comp.Fireteams, args.Marine);
			ISharedAdminLogManager adminLog = _adminLog;
			LogStringHandler handler = new LogStringHandler(27, 3);
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(((BaseBoundUserInterfaceEvent)args).Actor)), "ToPrettyString(args.Actor)");
			handler.AppendLiteral(" unassigned ");
			handler.AppendFormatted(((EntitySystem)this).ToPrettyString(marine, (MetaDataComponent)null), "ToPrettyString(marine)");
			handler.AppendLiteral(" from fireteam ");
			handler.AppendFormatted(member.Fireteam, "member.Fireteam");
			adminLog.Add(LogType.RMCFireteam, ref handler);
			((EntitySystem)this).Dirty<SquadLeaderTrackerComponent>(ent, (MetaDataComponent)null);
			SyncMemberFireteams(Entity<SquadMemberComponent>.op_Implicit(ent.Owner));
		}
	}

	private void OnPromoteFireteamLeaderMsg(Entity<SquadLeaderTrackerComponent> ent, ref SquadLeaderTrackerPromoteFireteamLeaderMsg args)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? marineId = default(EntityUid?);
		FireteamMemberComponent member = default(FireteamMemberComponent);
		if (!_net.IsClient && ((EntitySystem)this).TryGetEntity(args.Marine, ref marineId) && CanChangeFireteamMember(((BaseBoundUserInterfaceEvent)args).Actor, marineId.Value, add: true) && ((EntitySystem)this).TryComp<FireteamMemberComponent>(marineId, ref member) && member.Fireteam >= 0 && member.Fireteam < ent.Comp.Fireteams.Fireteams.Length)
		{
			NetEntity netMember = ((EntitySystem)this).GetNetEntity(marineId.Value, (MetaDataComponent)null);
			ProtoId<JobPrototype>? job = _originalRoleQuery.CompOrNull(marineId.Value)?.Job;
			Rsi iconOverride = ((EntitySystem)this).CompOrNull<RMCVendorRoleOverrideComponent>(marineId)?.GiveIcon ?? ((EntitySystem)this).CompOrNull<UsedSkillPamphletComponent>(marineId)?.Icon;
			SquadLeaderTrackerMarine marine = new SquadLeaderTrackerMarine(netMember, job, _rank.GetSpeakerRankName(marineId.Value) ?? ((EntitySystem)this).Name(marineId.Value, (MetaDataComponent)null), iconOverride);
			ref SquadLeaderTrackerFireteam fireteam = ref ent.Comp.Fireteams.Fireteams[member.Fireteam];
			if ((object)fireteam == null)
			{
				fireteam = new SquadLeaderTrackerFireteam();
			}
			DemoteFireteamLeader(fireteam, ((BaseBoundUserInterfaceEvent)args).Actor);
			fireteam.Leader = marine;
			((EntitySystem)this).EnsureComp<FireteamLeaderComponent>(marineId.Value);
			ISharedAdminLogManager adminLog = _adminLog;
			LogStringHandler handler = new LogStringHandler(29, 2);
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(((BaseBoundUserInterfaceEvent)args).Actor)), "ToPrettyString(args.Actor)");
			handler.AppendLiteral(" promoted ");
			handler.AppendFormatted(((EntitySystem)this).ToPrettyString(marineId, (MetaDataComponent)null), "ToPrettyString(marineId)");
			handler.AppendLiteral(" to fireteam leader");
			adminLog.Add(LogType.RMCFireteam, ref handler);
			((EntitySystem)this).Dirty<SquadLeaderTrackerComponent>(ent, (MetaDataComponent)null);
			SyncMemberFireteams(Entity<SquadMemberComponent>.op_Implicit(ent.Owner));
		}
	}

	private void OnDemoteFireteamLeaderMsg(Entity<SquadLeaderTrackerComponent> ent, ref SquadLeaderTrackerDemoteFireteamLeaderMsg args)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		if (!_net.IsClient && args.Fireteam >= 0 && args.Fireteam < ent.Comp.Fireteams.Fireteams.Length)
		{
			ref SquadLeaderTrackerFireteam fireteam = ref ent.Comp.Fireteams.Fireteams[args.Fireteam];
			EntityUid? marineId = default(EntityUid?);
			if (((EntitySystem)this).TryGetEntity(fireteam?.Leader?.Id, ref marineId) && CanChangeFireteamMember(((BaseBoundUserInterfaceEvent)args).Actor, marineId.Value, add: false))
			{
				DemoteFireteamLeader(fireteam, ((BaseBoundUserInterfaceEvent)args).Actor);
				((EntitySystem)this).Dirty<SquadLeaderTrackerComponent>(ent, (MetaDataComponent)null);
				SyncMemberFireteams(Entity<SquadMemberComponent>.op_Implicit(ent.Owner));
			}
		}
	}

	private void OnChangeTrackedMsg(Entity<SquadLeaderTrackerComponent> ent, ref SquadLeaderTrackerChangeTrackedMsg args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		List<DialogOption> options = new List<DialogOption>();
		foreach (ProtoId<TrackerModePrototype> mode in ent.Comp.TrackerModes)
		{
			options.Add(new DialogOption(base.Loc.GetString("rmc-squad-info-" + ProtoId<TrackerModePrototype>.op_Implicit(mode)), new SquadLeaderTrackerChangeModeEvent(mode)));
		}
		_dialog.OpenOptions(Entity<SquadLeaderTrackerComponent>.op_Implicit(ent), base.Loc.GetString("rmc-squad-info-tracking-selection"), options, base.Loc.GetString("rmc-squad-info-tracking-choose"));
	}

	private bool CanChangeFireteamMember(EntityUid user, EntityUid target, bool add)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).HasComp<SquadLeaderComponent>(user))
		{
			return false;
		}
		if (!_squad.AreInSameSquad(Entity<SquadMemberComponent>.op_Implicit(user), Entity<SquadMemberComponent>.op_Implicit(target)))
		{
			return false;
		}
		if (add && ((EntitySystem)this).HasComp<SquadLeaderComponent>(target))
		{
			return false;
		}
		return true;
	}

	private void SyncMemberFireteams(Entity<SquadMemberComponent?> member)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<SquadMemberComponent>(Entity<SquadMemberComponent>.op_Implicit(member), ref member.Comp, false) && member.Comp.Squad.HasValue)
		{
			SyncFireteams(Entity<SquadTeamComponent>.op_Implicit(member.Comp.Squad.Value));
		}
	}

	private void SyncFireteams(Entity<SquadTeamComponent?> squad)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<SquadTeamComponent>(Entity<SquadTeamComponent>.op_Implicit(squad), ref squad.Comp, false))
		{
			return;
		}
		Array.Clear(squad.Comp.Fireteams.Fireteams);
		squad.Comp.Fireteams.Unassigned.Clear();
		if (_squad.TryGetSquadLeader(Entity<SquadTeamComponent>.op_Implicit((Entity<SquadTeamComponent>.op_Implicit(squad), squad.Comp)), out Entity<SquadLeaderComponent> leader))
		{
			squad.Comp.Fireteams.SquadLeader = ((EntitySystem)this).Name(Entity<SquadLeaderComponent>.op_Implicit(leader), (MetaDataComponent)null);
			squad.Comp.Fireteams.SquadLeaderId = ((EntitySystem)this).GetNetEntity(Entity<SquadLeaderComponent>.op_Implicit(leader), (MetaDataComponent)null);
		}
		else
		{
			squad.Comp.Fireteams.SquadLeader = null;
			squad.Comp.Fireteams.SquadLeaderId = null;
		}
		foreach (EntityUid member in squad.Comp.Members)
		{
			AddFireteamMember(squad.Comp.Fireteams, member);
		}
	}

	private void AddFireteamMember(FireteamData fireteamData, EntityUid member)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		NetEntity netMember = ((EntitySystem)this).GetNetEntity(member, (MetaDataComponent)null);
		ProtoId<JobPrototype>? job = _originalRoleQuery.CompOrNull(member)?.Job;
		Rsi iconOverride = ((EntitySystem)this).CompOrNull<RMCVendorRoleOverrideComponent>(member)?.GiveIcon ?? ((EntitySystem)this).CompOrNull<UsedSkillPamphletComponent>(member)?.Icon;
		SquadLeaderTrackerMarine marine = new SquadLeaderTrackerMarine(netMember, job, _rank.GetSpeakerRankName(member) ?? ((EntitySystem)this).Name(member, (MetaDataComponent)null), iconOverride);
		FireteamMemberComponent fireteamMember = default(FireteamMemberComponent);
		if (_fireteamMemberQuery.TryComp(member, ref fireteamMember) && fireteamMember.Fireteam >= 0 && fireteamMember.Fireteam < fireteamData.Fireteams.Length)
		{
			ref SquadLeaderTrackerFireteam fireteam = ref fireteamData.Fireteams[fireteamMember.Fireteam];
			if ((object)fireteam == null)
			{
				fireteam = new SquadLeaderTrackerFireteam();
			}
			SquadLeaderTrackerFireteam squadLeaderTrackerFireteam = fireteam;
			if (squadLeaderTrackerFireteam.Members == null)
			{
				squadLeaderTrackerFireteam.Members = new Dictionary<NetEntity, SquadLeaderTrackerMarine>();
			}
			fireteam.Members[netMember] = marine;
			if (_fireteamLeaderQuery.HasComp(member))
			{
				fireteam.Leader = marine;
			}
			SquadLeaderTrackerComponent tempTracker = default(SquadLeaderTrackerComponent);
			EntityUid? fireteamLeaderUid = default(EntityUid?);
			if (_squadLeaderTrackerQuery.TryComp(member, ref tempTracker) && fireteam.Leader.HasValue && ((EntitySystem)this).TryGetEntity(fireteam?.Leader?.Id, ref fireteamLeaderUid))
			{
				EntityUid? val = fireteamLeaderUid;
				if (!val.HasValue || val.GetValueOrDefault() != member)
				{
					ProtoId<TrackerModePrototype> mode = ProtoId<TrackerModePrototype>.op_Implicit("FireteamLeader");
					SetTarget(Entity<SquadLeaderTrackerComponent>.op_Implicit((member, tempTracker)), fireteamLeaderUid);
					SetMode(Entity<SquadLeaderTrackerComponent>.op_Implicit((member, tempTracker)), mode);
				}
			}
		}
		else
		{
			fireteamData.Unassigned[netMember] = marine;
		}
		SquadLeaderTrackerComponent tracker = default(SquadLeaderTrackerComponent);
		if (_squadLeaderTrackerQuery.TryComp(member, ref tracker))
		{
			tracker.Fireteams = fireteamData;
			((EntitySystem)this).Dirty(member, (IComponent)(object)tracker, (MetaDataComponent)null);
		}
	}

	private void RemoveFireteamMember(FireteamData fireteamData, NetEntity member)
	{
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		SquadLeaderTrackerFireteam[] fireteams = fireteamData.Fireteams;
		foreach (SquadLeaderTrackerFireteam fireteam in fireteams)
		{
			if ((object)fireteam != null)
			{
				NetEntity? val = fireteam.Leader?.Id;
				if (val.HasValue && val.GetValueOrDefault() == member)
				{
					fireteam.Leader = null;
				}
			}
			fireteam?.Members?.Remove(member);
		}
		fireteamData.Unassigned.Remove(member);
		EntityUid? memberId = default(EntityUid?);
		if (((EntitySystem)this).TryGetEntity(member, ref memberId))
		{
			((EntitySystem)this).RemComp<FireteamMemberComponent>(memberId.Value);
			SquadLeaderTrackerComponent tracker = default(SquadLeaderTrackerComponent);
			if (_squadLeaderTrackerQuery.TryComp(memberId, ref tracker))
			{
				tracker.Fireteams = new FireteamData();
				((EntitySystem)this).Dirty(memberId.Value, (IComponent)(object)tracker, (MetaDataComponent)null);
			}
		}
	}

	private void DemoteFireteamLeader(SquadLeaderTrackerFireteam? fireteam, EntityUid user)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		if (!(fireteam != null))
		{
			return;
		}
		NetEntity? val = fireteam.Leader?.Id;
		if (val.HasValue)
		{
			NetEntity oldLeaderNet = val.GetValueOrDefault();
			EntityUid? oldLeader = default(EntityUid?);
			if (((EntitySystem)this).TryGetEntity(oldLeaderNet, ref oldLeader) && !((EntitySystem)this).TerminatingOrDeleted(oldLeader, (MetaDataComponent)null))
			{
				((EntitySystem)this).RemComp<FireteamLeaderComponent>(oldLeader.Value);
				fireteam.Leader = null;
				ISharedAdminLogManager adminLog = _adminLog;
				LogStringHandler handler = new LogStringHandler(30, 2);
				handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user)), "ToPrettyString(user)");
				handler.AppendLiteral(" demoted ");
				handler.AppendFormatted(((EntitySystem)this).ToPrettyString(oldLeader, (MetaDataComponent)null), "ToPrettyString(oldLeader)");
				handler.AppendLiteral(" from fireteam leader");
				adminLog.Add(LogType.RMCFireteam, ref handler);
			}
		}
	}

	private void UpdateDirection(Entity<SquadLeaderTrackerComponent> ent, MapCoordinates? coordinates = null, string squad = "")
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		_alerts.ClearAlertCategory(Entity<SquadLeaderTrackerComponent>.op_Implicit(ent), ProtoId<AlertCategoryPrototype>.op_Implicit("SquadTracker"));
		TrackerModePrototype trackerMode = default(TrackerModePrototype);
		_prototypeManager.TryIndex<TrackerModePrototype>(ent.Comp.Mode, ref trackerMode);
		if (trackerMode != null)
		{
			ProtoId<AlertPrototype> alert = trackerMode.Alert;
			short severity = TrackerSystem.CenterSeverity;
			ProtoId<TrackerModePrototype>? mode = ent.Comp.Mode;
			ProtoId<TrackerModePrototype>? val = ProtoId<TrackerModePrototype>.op_Implicit("SquadLeader");
			if (mode.HasValue == val.HasValue && (!mode.HasValue || mode.GetValueOrDefault() == val.GetValueOrDefault()))
			{
				alert = ProtoId<AlertPrototype>.op_Implicit(ProtoId<AlertPrototype>.op_Implicit(alert) + squad);
			}
			if (coordinates.HasValue)
			{
				severity = _tracker.GetAlertSeverity(ent.Owner, coordinates.Value);
			}
			_alerts.ShowAlert(ent.Owner, alert, severity);
		}
	}

	private void SetTarget(Entity<SquadLeaderTrackerComponent> ent, EntityUid? target)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.Target = target;
		((EntitySystem)this).Dirty<SquadLeaderTrackerComponent>(ent, (MetaDataComponent)null);
	}

	private void SetMode(Entity<SquadLeaderTrackerComponent> ent, ProtoId<TrackerModePrototype> mode)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.Mode = mode;
		((EntitySystem)this).Dirty<SquadLeaderTrackerComponent>(ent, (MetaDataComponent)null);
	}

	private void OnRoleChange(Entity<SquadLeaderTrackerComponent> ent, ref GetMarineSquadNameEvent _)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		SyncMemberFireteams(Entity<SquadMemberComponent>.op_Implicit(ent.Owner));
	}

	public bool TryFindTargets(ProtoId<TrackerModePrototype> mode, out List<DialogOption> options, out List<EntityUid> trackingOptions)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		options = new List<DialogOption>();
		trackingOptions = new List<EntityUid>();
		TrackerModePrototype trackerMode = default(TrackerModePrototype);
		_prototypeManager.TryIndex<TrackerModePrototype>(mode, ref trackerMode);
		if (trackerMode == null)
		{
			return false;
		}
		EntityQueryEnumerator<RMCTrackableComponent> query = ((EntitySystem)this).EntityQueryEnumerator<RMCTrackableComponent>();
		EntityUid trackableUid = default(EntityUid);
		RMCTrackableComponent rMCTrackableComponent = default(RMCTrackableComponent);
		IComponent val = default(IComponent);
		OriginalRoleComponent original = default(OriginalRoleComponent);
		SquadMemberComponent targetSquadMember = default(SquadMemberComponent);
		while (query.MoveNext(ref trackableUid, ref rMCTrackableComponent))
		{
			if (trackerMode.Component != null)
			{
				Type trackingComponent = ((object)_factory.GetComponent(trackerMode.Component, false)).GetType();
				string targetName = "";
				if (base.EntityManager.TryGetComponent(trackableUid, trackingComponent, ref val))
				{
					if (!_net.IsClient)
					{
						targetName = ((EntitySystem)this).Name(trackableUid, (MetaDataComponent)null);
					}
					RequestTrackableNameEvent nameEv = default(RequestTrackableNameEvent);
					((EntitySystem)this).RaiseLocalEvent<RequestTrackableNameEvent>(trackableUid, ref nameEv, false);
					if (nameEv.Name != null)
					{
						targetName = nameEv.Name;
					}
					options.Add(new DialogOption(targetName, new LeaderTrackerSelectTargetEvent(((EntitySystem)this).GetNetEntity(trackableUid, (MetaDataComponent)null), mode)));
					trackingOptions.Add(trackableUid);
				}
				continue;
			}
			string originalRole = "NoOriginalRole";
			string targetSquadName = "";
			ProtoId<JobPrototype>? job;
			if (_originalRoleQuery.TryComp(trackableUid, ref original))
			{
				job = original.Job;
				originalRole = (job.HasValue ? ProtoId<JobPrototype>.op_Implicit(job.GetValueOrDefault()) : null);
			}
			job = ProtoId<JobPrototype>.op_Implicit(originalRole);
			ProtoId<JobPrototype>? job2 = trackerMode.Job;
			if ((job.HasValue != job2.HasValue || (job.HasValue && job.GetValueOrDefault() != job2.GetValueOrDefault())) && (mode != ProtoId<TrackerModePrototype>.op_Implicit("SquadLeader") || !((EntitySystem)this).HasComp<SquadLeaderComponent>(trackableUid)))
			{
				continue;
			}
			if (!_net.IsClient && _squadMemberQuery.TryComp(trackableUid, ref targetSquadMember))
			{
				EntityUid? squad = targetSquadMember.Squad;
				if (squad.HasValue)
				{
					EntityUid targetSquad = squad.GetValueOrDefault();
					targetSquadName = ((EntitySystem)this).Name(targetSquad, (MetaDataComponent)null);
				}
			}
			options.Add(new DialogOption("(" + targetSquadName + ") " + _rank.GetSpeakerFullRankName(trackableUid), new LeaderTrackerSelectTargetEvent(((EntitySystem)this).GetNetEntity(trackableUid, (MetaDataComponent)null), mode)));
			trackingOptions.Add(trackableUid);
		}
		return true;
	}

	public override void Update(float frameTime)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0506: Unknown result type (might be due to invalid IL or missing references)
		//IL_050b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06de: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0562: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0573: Unknown result type (might be due to invalid IL or missing references)
		//IL_057c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0581: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_0605: Unknown result type (might be due to invalid IL or missing references)
		//IL_059f: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0465: Unknown result type (might be due to invalid IL or missing references)
		//IL_046e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0473: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_0644: Unknown result type (might be due to invalid IL or missing references)
		//IL_064b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0416: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0457: Unknown result type (might be due to invalid IL or missing references)
		//IL_045d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0462: Unknown result type (might be due to invalid IL or missing references)
		//IL_035e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0367: Unknown result type (might be due to invalid IL or missing references)
		//IL_036c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_0695: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0438: Unknown result type (might be due to invalid IL or missing references)
		//IL_043d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0440: Unknown result type (might be due to invalid IL or missing references)
		//IL_0686: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		_squadLeaders.Clear();
		EntityQueryEnumerator<SquadLeaderComponent, SquadMemberComponent, RMCTrackableComponent> squadLeaders = ((EntitySystem)this).EntityQueryEnumerator<SquadLeaderComponent, SquadMemberComponent, RMCTrackableComponent>();
		EntityUid uid = default(EntityUid);
		SquadLeaderComponent squadLeaderComponent = default(SquadLeaderComponent);
		SquadMemberComponent member = default(SquadMemberComponent);
		RMCTrackableComponent rMCTrackableComponent = default(RMCTrackableComponent);
		while (squadLeaders.MoveNext(ref uid, ref squadLeaderComponent, ref member, ref rMCTrackableComponent))
		{
			EntityUid? squad = member.Squad;
			if (squad.HasValue)
			{
				EntityUid squad2 = squad.GetValueOrDefault();
				_squadLeaders.TryAdd(squad2, _transform.GetMapCoordinates(uid, (TransformComponent)null));
			}
		}
		Array.Clear(_fireteamLeaders);
		EntityQueryEnumerator<FireteamLeaderComponent, FireteamMemberComponent, SquadMemberComponent> fireteamLeaders = ((EntitySystem)this).EntityQueryEnumerator<FireteamLeaderComponent, FireteamMemberComponent, SquadMemberComponent>();
		EntityUid uid2 = default(EntityUid);
		FireteamLeaderComponent fireteamLeaderComponent = default(FireteamLeaderComponent);
		FireteamMemberComponent fireteamMember = default(FireteamMemberComponent);
		SquadMemberComponent squadMember = default(SquadMemberComponent);
		while (fireteamLeaders.MoveNext(ref uid2, ref fireteamLeaderComponent, ref fireteamMember, ref squadMember))
		{
			EntityUid? squad = squadMember.Squad;
			if (!squad.HasValue)
			{
				continue;
			}
			EntityUid squad3 = squad.GetValueOrDefault();
			if (fireteamMember.Fireteam >= 0 && fireteamMember.Fireteam < _fireteamLeaders.Length)
			{
				ref Dictionary<EntityUid, MapCoordinates> leaders = ref _fireteamLeaders[fireteamMember.Fireteam];
				if (leaders == null)
				{
					leaders = new Dictionary<EntityUid, MapCoordinates>();
				}
				leaders[squad3] = _transform.GetMapCoordinates(uid2, (TransformComponent)null);
			}
		}
		TimeSpan time = _timing.CurTime;
		EntityQueryEnumerator<SquadLeaderTrackerComponent> query = ((EntitySystem)this).EntityQueryEnumerator<SquadLeaderTrackerComponent>();
		EntityUid uid3 = default(EntityUid);
		SquadLeaderTrackerComponent tracker = default(SquadLeaderTrackerComponent);
		SquadMemberComponent target = default(SquadMemberComponent);
		SquadTeamComponent team = default(SquadTeamComponent);
		SquadMemberComponent squadMember2 = default(SquadMemberComponent);
		FireteamMemberComponent fireteamMember2 = default(FireteamMemberComponent);
		SquadMemberComponent target2 = default(SquadMemberComponent);
		SquadMemberComponent targetSquad3 = default(SquadMemberComponent);
		EntityUid trackableUid = default(EntityUid);
		TrackerModePrototype trackerMode = default(TrackerModePrototype);
		IComponent val2 = default(IComponent);
		OriginalRoleComponent original = default(OriginalRoleComponent);
		SquadMemberComponent targetSquad4 = default(SquadMemberComponent);
		while (query.MoveNext(ref uid3, ref tracker))
		{
			if (time < tracker.UpdateAt)
			{
				continue;
			}
			tracker.UpdateAt = time + tracker.UpdateEvery;
			string targetSquadName = "";
			if (tracker.Target.HasValue)
			{
				ProtoId<TrackerModePrototype>? mode = tracker.Mode;
				ProtoId<TrackerModePrototype>? val = ProtoId<TrackerModePrototype>.op_Implicit("SquadLeader");
				if (mode.HasValue == val.HasValue && (!mode.HasValue || mode.GetValueOrDefault() == val.GetValueOrDefault()) && !((EntitySystem)this).HasComp<SquadLeaderComponent>(tracker.Target) && _squadMemberQuery.TryComp(tracker.Target.Value, ref target))
				{
					EntityUid? squad = target.Squad;
					if (squad.HasValue)
					{
						EntityUid targetSquad = squad.GetValueOrDefault();
						if (((EntitySystem)this).TryComp<SquadTeamComponent>(targetSquad, ref team) && _squad.TryGetSquadLeader(Entity<SquadTeamComponent>.op_Implicit((targetSquad, team)), out Entity<SquadLeaderComponent> leader))
						{
							SetTarget(Entity<SquadLeaderTrackerComponent>.op_Implicit((uid3, tracker)), Entity<SquadLeaderComponent>.op_Implicit(leader));
							targetSquadName = ((EntitySystem)this).Name(targetSquad, (MetaDataComponent)null);
							MapCoordinates targetCoordinates = _transform.GetMapCoordinates(tracker.Target.Value, (TransformComponent)null);
							UpdateDirection(Entity<SquadLeaderTrackerComponent>.op_Implicit((uid3, tracker)), targetCoordinates, targetSquadName);
							continue;
						}
					}
				}
			}
			if (_squadMemberQuery.TryComp(uid3, ref squadMember2))
			{
				EntityUid? squad = squadMember2.Squad;
				if (squad.HasValue)
				{
					EntityUid squad4 = squad.GetValueOrDefault();
					ProtoId<TrackerModePrototype>? mode;
					ProtoId<TrackerModePrototype>? val;
					if (_fireteamMemberQuery.TryComp(uid3, ref fireteamMember2))
					{
						val = tracker.Mode;
						mode = ProtoId<TrackerModePrototype>.op_Implicit("FireteamLeader");
						if (val.HasValue == mode.HasValue && (!val.HasValue || val.GetValueOrDefault() == mode.GetValueOrDefault()))
						{
							int fireteamIndex = fireteamMember2.Fireteam;
							if (fireteamIndex >= 0 && fireteamIndex < _fireteamLeaders.Length)
							{
								Dictionary<EntityUid, MapCoordinates> fireteam = _fireteamLeaders[fireteamIndex];
								if (fireteam != null && fireteam.TryGetValue(squad4, out var leader2))
								{
									UpdateDirection(Entity<SquadLeaderTrackerComponent>.op_Implicit((uid3, tracker)), leader2, ((EntitySystem)this).Name(squad4, (MetaDataComponent)null));
									continue;
								}
							}
							goto IL_0486;
						}
					}
					mode = tracker.Mode;
					val = ProtoId<TrackerModePrototype>.op_Implicit("SquadLeader");
					if (mode.HasValue == val.HasValue && (!mode.HasValue || mode.GetValueOrDefault() == val.GetValueOrDefault()) && _squadLeaders.TryGetValue(squad4, out var squadLeader))
					{
						targetSquadName = ((EntitySystem)this).Name(squad4, (MetaDataComponent)null);
						if (((EntitySystem)this).HasComp<SquadLeaderComponent>(uid3) && tracker.Target.HasValue)
						{
							if (_squadMemberQuery.TryComp(tracker.Target.Value, ref target2))
							{
								squad = target2.Squad;
								if (squad.HasValue)
								{
									EntityUid targetSquad2 = squad.GetValueOrDefault();
									targetSquadName = ((EntitySystem)this).Name(targetSquad2, (MetaDataComponent)null);
								}
							}
							squadLeader = _transform.GetMapCoordinates(tracker.Target.Value, (TransformComponent)null);
						}
						UpdateDirection(Entity<SquadLeaderTrackerComponent>.op_Implicit((uid3, tracker)), squadLeader, targetSquadName);
						continue;
					}
				}
			}
			goto IL_0486;
			IL_0486:
			if (tracker.Target.HasValue)
			{
				if (_squadMemberQuery.TryComp(tracker.Target, ref targetSquad3) && targetSquad3.Squad.HasValue)
				{
					targetSquadName = ((EntitySystem)this).Name(targetSquad3.Squad.Value, (MetaDataComponent)null);
				}
				UpdateDirection(Entity<SquadLeaderTrackerComponent>.op_Implicit((uid3, tracker)), _transform.GetMapCoordinates(tracker.Target.Value, (TransformComponent)null), targetSquadName);
				continue;
			}
			EntityQueryEnumerator<RMCTrackableComponent> trackableQuery = ((EntitySystem)this).EntityQueryEnumerator<RMCTrackableComponent>();
			while (trackableQuery.MoveNext(ref trackableUid, ref rMCTrackableComponent))
			{
				_prototypeManager.TryIndex<TrackerModePrototype>(tracker.Mode, ref trackerMode);
				if (trackerMode == null)
				{
					continue;
				}
				if (trackerMode.Component != null)
				{
					Type trackingComponent = ((object)_factory.GetComponent(trackerMode.Component, false)).GetType();
					if (base.EntityManager.TryGetComponent(trackableUid, trackingComponent, ref val2))
					{
						SetTarget(Entity<SquadLeaderTrackerComponent>.op_Implicit((uid3, tracker)), trackableUid);
						if (tracker.Target.HasValue)
						{
							UpdateDirection(Entity<SquadLeaderTrackerComponent>.op_Implicit((uid3, tracker)), _transform.GetMapCoordinates(tracker.Target.Value, (TransformComponent)null), targetSquadName);
						}
					}
					break;
				}
				string originalRole = "NoOriginalRole";
				ProtoId<JobPrototype>? job;
				if (_originalRoleQuery.TryComp(trackableUid, ref original))
				{
					job = original.Job;
					originalRole = (job.HasValue ? ProtoId<JobPrototype>.op_Implicit(job.GetValueOrDefault()) : null);
				}
				job = ProtoId<JobPrototype>.op_Implicit(originalRole);
				ProtoId<JobPrototype>? job2 = trackerMode.Job;
				if (job.HasValue == job2.HasValue && (!job.HasValue || !(job.GetValueOrDefault() != job2.GetValueOrDefault())))
				{
					if (_squadMemberQuery.TryComp(tracker.Target, ref targetSquad4) && targetSquad4.Squad.HasValue)
					{
						targetSquadName = ((EntitySystem)this).Name(targetSquad4.Squad.Value, (MetaDataComponent)null);
					}
					tracker.Target = trackableUid;
					UpdateDirection(Entity<SquadLeaderTrackerComponent>.op_Implicit((uid3, tracker)), _transform.GetMapCoordinates(tracker.Target.Value, (TransformComponent)null), targetSquadName);
					break;
				}
			}
			UpdateDirection(Entity<SquadLeaderTrackerComponent>.op_Implicit((uid3, tracker)));
		}
	}
}
