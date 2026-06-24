using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Content.Shared._RMC14.Areas;
using Content.Shared._RMC14.CCVar;
using Content.Shared._RMC14.Chat;
using Content.Shared._RMC14.Dialog;
using Content.Shared._RMC14.IdentityManagement;
using Content.Shared._RMC14.Marines.Announce;
using Content.Shared._RMC14.Marines.Roles.Ranks;
using Content.Shared._RMC14.Marines.Skills.Pamphlets;
using Content.Shared._RMC14.Marines.Squads;
using Content.Shared._RMC14.OrbitalCannon;
using Content.Shared._RMC14.Roles;
using Content.Shared._RMC14.Rules;
using Content.Shared._RMC14.SupplyDrop;
using Content.Shared._RMC14.TacticalMap;
using Content.Shared._RMC14.Vendors;
using Content.Shared._RMC14.Xenonids;
using Content.Shared.Administration.Logs;
using Content.Shared.Chat;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.Database;
using Content.Shared.FixedPoint;
using Content.Shared.IdentityManagement;
using Content.Shared.Inventory;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Movement.Events;
using Content.Shared.Popups;
using Content.Shared.Roles;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Shared._RMC14.Overwatch;

public abstract class SharedOverwatchConsoleSystem : EntitySystem
{
	[Dependency]
	private ISharedAdminLogManager _adminLog;

	[Dependency]
	private AreaSystem _area;

	[Dependency]
	private IConfigurationManager _config;

	[Dependency]
	private DialogSystem _dialog;

	[Dependency]
	private SharedEyeSystem _eye;

	[Dependency]
	private InventorySystem _inventory;

	[Dependency]
	private SharedMapSystem _map;

	[Dependency]
	private SharedMarineAnnounceSystem _marineAnnounce;

	[Dependency]
	private MobStateSystem _mobState;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private OrbitalCannonSystem _orbitalCannon;

	[Dependency]
	private ISharedPlayerManager _player;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private IPrototypeManager _prototypes;

	[Dependency]
	private SharedCMChatSystem _rmcChat;

	[Dependency]
	private SquadSystem _squad;

	[Dependency]
	private SharedSupplyDropSystem _supplyDrop;

	[Dependency]
	private SharedTacticalMapSystem _tacticalMap;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private SharedUserInterfaceSystem _ui;

	private EntityQuery<ActorComponent> _actor;

	private EntityQuery<MobStateComponent> _mobStateQuery;

	private EntityQuery<OriginalRoleComponent> _originalRoleQuery;

	private EntityQuery<RankComponent> _rankQuery;

	private EntityQuery<OverwatchDataComponent> _overwatchDataQuery;

	private EntityQuery<RMCPlanetComponent> _planetQuery;

	private readonly ProtoId<DamageGroupPrototype> _bruteGroup = ProtoId<DamageGroupPrototype>.op_Implicit("Brute");

	private readonly ProtoId<DamageGroupPrototype> _burnGroup = ProtoId<DamageGroupPrototype>.op_Implicit("Burn");

	private readonly ProtoId<DamageGroupPrototype> _toxinGroup = ProtoId<DamageGroupPrototype>.op_Implicit("Toxin");

	private TimeSpan _maxProcessTime;

	private TimeSpan _nextUpdateTime;

	private TimeSpan _updateEvery;

	private readonly Dictionary<Entity<SquadTeamComponent>, Queue<EntityUid>> _toProcess = new Dictionary<Entity<SquadTeamComponent>, Queue<EntityUid>>();

	private readonly HashSet<Entity<SquadTeamComponent>> _toRemove = new HashSet<Entity<SquadTeamComponent>>();

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
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		_actor = ((EntitySystem)this).GetEntityQuery<ActorComponent>();
		_mobStateQuery = ((EntitySystem)this).GetEntityQuery<MobStateComponent>();
		_originalRoleQuery = ((EntitySystem)this).GetEntityQuery<OriginalRoleComponent>();
		_rankQuery = ((EntitySystem)this).GetEntityQuery<RankComponent>();
		_overwatchDataQuery = ((EntitySystem)this).GetEntityQuery<OverwatchDataComponent>();
		_planetQuery = ((EntitySystem)this).GetEntityQuery<RMCPlanetComponent>();
		((EntitySystem)this).SubscribeLocalEvent<OrbitalCannonChangedEvent>((EntityEventRefHandler<OrbitalCannonChangedEvent>)OnOrbitalCannonChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<OrbitalCannonLaunchEvent>((EntityEventRefHandler<OrbitalCannonLaunchEvent>)OnOrbitalCannonLaunch, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<OverwatchConsoleComponent, BoundUIOpenedEvent>((EntityEventRefHandler<OverwatchConsoleComponent, BoundUIOpenedEvent>)OnBUIOpened, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<OverwatchConsoleComponent, OverwatchTransferMarineSelectedEvent>((EntityEventRefHandler<OverwatchConsoleComponent, OverwatchTransferMarineSelectedEvent>)OnTransferMarineSelected, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<OverwatchConsoleComponent, OverwatchTransferMarineSquadEvent>((EntityEventRefHandler<OverwatchConsoleComponent, OverwatchTransferMarineSquadEvent>)OnTransferMarineSquad, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<OverwatchWatchingComponent, MoveInputEvent>((EntityEventRefHandler<OverwatchWatchingComponent, MoveInputEvent>)OnWatchingMoveInput, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<OverwatchWatchingComponent, DamageChangedEvent>((EntityEventRefHandler<OverwatchWatchingComponent, DamageChangedEvent>)OnWatchingDamageChanged, (Type[])null, (Type[])null);
		BoundUserInterfaceRegisterExt.BuiEvents<OverwatchConsoleComponent>(((EntitySystem)this).Subs, (object)OverwatchConsoleUI.Key, (BuiEventSubscriber<OverwatchConsoleComponent>)delegate(Subscriber<OverwatchConsoleComponent> subs)
		{
			subs.Event<OverwatchConsoleSelectSquadBuiMsg>((EntityEventRefHandler<OverwatchConsoleComponent, OverwatchConsoleSelectSquadBuiMsg>)OnOverwatchSelectSquadBui);
			subs.Event<OverwatchViewTacticalMapBuiMsg>((EntityEventRefHandler<OverwatchConsoleComponent, OverwatchViewTacticalMapBuiMsg>)OnOverwatchViewTacticalMapBui);
			subs.Event<OverwatchConsoleTakeOperatorBuiMsg>((EntityEventRefHandler<OverwatchConsoleComponent, OverwatchConsoleTakeOperatorBuiMsg>)OnOverwatchTakeOperatorBui);
			subs.Event<OverwatchConsoleStopOverwatchBuiMsg>((EntityEventRefHandler<OverwatchConsoleComponent, OverwatchConsoleStopOverwatchBuiMsg>)OnOverwatchStopBui);
			subs.Event<OverwatchConsoleSetLocationBuiMsg>((EntityEventRefHandler<OverwatchConsoleComponent, OverwatchConsoleSetLocationBuiMsg>)OnOverwatchSetLocationBui);
			subs.Event<OverwatchConsoleShowDeadBuiMsg>((EntityEventRefHandler<OverwatchConsoleComponent, OverwatchConsoleShowDeadBuiMsg>)OnOverwatchShowDeadBui);
			subs.Event<OverwatchConsoleShowHiddenBuiMsg>((EntityEventRefHandler<OverwatchConsoleComponent, OverwatchConsoleShowHiddenBuiMsg>)OnOverwatchShowHiddenBui);
			subs.Event<OverwatchConsoleTransferMarineBuiMsg>((EntityEventRefHandler<OverwatchConsoleComponent, OverwatchConsoleTransferMarineBuiMsg>)OnOverwatchTransferMarineBui);
			subs.Event<OverwatchConsoleWatchBuiMsg>((EntityEventRefHandler<OverwatchConsoleComponent, OverwatchConsoleWatchBuiMsg>)OnOverwatchWatchBui);
			subs.Event<OverwatchConsoleHideBuiMsg>((EntityEventRefHandler<OverwatchConsoleComponent, OverwatchConsoleHideBuiMsg>)OnOverwatchHideBui);
			subs.Event<OverwatchConsolePromoteLeaderBuiMsg>((EntityEventRefHandler<OverwatchConsoleComponent, OverwatchConsolePromoteLeaderBuiMsg>)OnOverwatchPromoteLeaderBui);
			subs.Event<OverwatchConsoleSupplyDropLongitudeBuiMsg>((EntityEventRefHandler<OverwatchConsoleComponent, OverwatchConsoleSupplyDropLongitudeBuiMsg>)OnOverwatchSupplyDropLongitudeBui);
			subs.Event<OverwatchConsoleSupplyDropLatitudeBuiMsg>((EntityEventRefHandler<OverwatchConsoleComponent, OverwatchConsoleSupplyDropLatitudeBuiMsg>)OnOverwatchSupplyDropLatitudeBui);
			subs.Event<OverwatchConsoleSupplyDropLaunchBuiMsg>((EntityEventRefHandler<OverwatchConsoleComponent, OverwatchConsoleSupplyDropLaunchBuiMsg>)OnOverwatchSupplyDropLaunchBui);
			subs.Event<OverwatchConsoleSupplyDropSaveBuiMsg>((EntityEventRefHandler<OverwatchConsoleComponent, OverwatchConsoleSupplyDropSaveBuiMsg>)OnOverwatchSupplyDropSaveBui);
			subs.Event<OverwatchConsoleLocationCommentBuiMsg>((EntityEventRefHandler<OverwatchConsoleComponent, OverwatchConsoleLocationCommentBuiMsg>)OnOverwatchSupplyDropCommentBui);
			subs.Event<OverwatchConsoleOrbitalLongitudeBuiMsg>((EntityEventRefHandler<OverwatchConsoleComponent, OverwatchConsoleOrbitalLongitudeBuiMsg>)OnOverwatchOrbitalCoordinatesBui);
			subs.Event<OverwatchConsoleOrbitalLatitudeBuiMsg>((EntityEventRefHandler<OverwatchConsoleComponent, OverwatchConsoleOrbitalLatitudeBuiMsg>)OnOverwatchOrbitalCoordinatesBui);
			subs.Event<OverwatchConsoleOrbitalLaunchBuiMsg>((EntityEventRefHandler<OverwatchConsoleComponent, OverwatchConsoleOrbitalLaunchBuiMsg>)OnOverwatchOrbitalLaunchBui);
			subs.Event<OverwatchConsoleSendMessageBuiMsg>((EntityEventRefHandler<OverwatchConsoleComponent, OverwatchConsoleSendMessageBuiMsg>)OnOverwatchSendMessageBui);
		});
		EntitySystemSubscriptionExt.CVar<float>(((EntitySystem)this).Subs, _config, RMCCVars.RMCOverwatchMaxProcessTimeMilliseconds, (Action<float>)delegate(float v)
		{
			_maxProcessTime = TimeSpan.FromMilliseconds(v);
		}, true);
		EntitySystemSubscriptionExt.CVar<float>(((EntitySystem)this).Subs, _config, RMCCVars.RMCOverwatchConsoleUpdateEverySeconds, (Action<float>)delegate(float v)
		{
			_updateEvery = TimeSpan.FromSeconds(v);
		}, true);
	}

	private void OnOrbitalCannonChanged(ref OrbitalCannonChangedEvent ev)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		bool hasOrbital = ev.Cannon.Comp.Status == OrbitalCannonStatus.Chambered;
		EntityQueryEnumerator<OverwatchConsoleComponent> consoles = ((EntitySystem)this).EntityQueryEnumerator<OverwatchConsoleComponent>();
		EntityUid uid = default(EntityUid);
		OverwatchConsoleComponent console = default(OverwatchConsoleComponent);
		while (consoles.MoveNext(ref uid, ref console))
		{
			console.HasOrbital = hasOrbital;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)console, (MetaDataComponent)null);
		}
	}

	private void OnOrbitalCannonLaunch(ref OrbitalCannonLaunchEvent ev)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<OverwatchConsoleComponent> consoles = ((EntitySystem)this).EntityQueryEnumerator<OverwatchConsoleComponent>();
		EntityUid uid = default(EntityUid);
		OverwatchConsoleComponent console = default(OverwatchConsoleComponent);
		while (consoles.MoveNext(ref uid, ref console))
		{
			console.NextOrbitalLaunch = _timing.CurTime + ev.Cooldown;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)console, (MetaDataComponent)null);
		}
	}

	private void OnBUIOpened(Entity<OverwatchConsoleComponent> ent, ref BoundUIOpenedEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		if (!_net.IsClient)
		{
			OverwatchConsoleBuiState state = GetOverwatchBuiState(ent);
			_ui.SetUiState(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), (Enum)OverwatchConsoleUI.Key, (BoundUserInterfaceState)(object)state);
		}
	}

	private void OnTransferMarineSelected(Entity<OverwatchConsoleComponent> ent, ref OverwatchTransferMarineSelectedEvent args)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? actor = default(EntityUid?);
		if (_net.IsClient || !((EntitySystem)this).TryGetEntity(args.Actor, ref actor))
		{
			return;
		}
		EntityUid? currentSquad = null;
		EntityUid? marine = default(EntityUid?);
		if (((EntitySystem)this).TryGetEntity(args.Marine, ref marine) && _squad.TryGetMemberSquad(Entity<SquadMemberComponent>.op_Implicit(marine.Value), out Entity<SquadTeamComponent> marineSquad))
		{
			currentSquad = Entity<SquadTeamComponent>.op_Implicit(marineSquad);
		}
		OverwatchConsoleBuiState overwatchBuiState = GetOverwatchBuiState(ent);
		List<DialogOption> options = new List<DialogOption>();
		foreach (OverwatchSquad squad in overwatchBuiState.Squads)
		{
			EntityUid? val = currentSquad;
			EntityUid entity = ((EntitySystem)this).GetEntity(squad.Id);
			if (!val.HasValue || !(val.GetValueOrDefault() == entity))
			{
				options.Add(new DialogOption(squad.Name, new OverwatchTransferMarineSquadEvent(args.Actor, args.Marine, squad.Id)));
			}
		}
		_dialog.OpenOptions(Entity<OverwatchConsoleComponent>.op_Implicit(ent), actor.Value, "Squad Selection", options, "Choose the marine's new squad");
	}

	private void OnTransferMarineSquad(Entity<OverwatchConsoleComponent> ent, ref OverwatchTransferMarineSquadEvent args)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0303: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_0362: Unknown result type (might be due to invalid IL or missing references)
		//IL_0371: Unknown result type (might be due to invalid IL or missing references)
		//IL_0392: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		EntityUid actor = ((EntitySystem)this).GetEntity(args.Actor);
		if (!((EntityUid)(ref actor)).Valid)
		{
			return;
		}
		NetEntity squadId = args.Squad;
		OverwatchSquad? squad = default(OverwatchSquad?);
		if (!Extensions.TryFirstOrNull<OverwatchSquad>((IEnumerable<OverwatchSquad>)GetOverwatchBuiState(ent).Squads, (Func<OverwatchSquad, bool>)((OverwatchSquad s) => s.Id == squadId), ref squad))
		{
			_popup.PopupCursor("You can't transfer marines to that squad!", actor, PopupType.LargeCaution);
			return;
		}
		EntityUid? marineId = default(EntityUid?);
		if (!((EntitySystem)this).TryGetEntity(args.Marine, ref marineId))
		{
			_popup.PopupCursor("That marine is KIA.", actor, PopupType.LargeCaution);
			return;
		}
		if (_mobState.IsDead(marineId.Value))
		{
			_popup.PopupCursor(((EntitySystem)this).Name(marineId.Value, (MetaDataComponent)null) + " is KIA.", actor, PopupType.LargeCaution);
			return;
		}
		if (squad.Value.Leader.HasValue && ((EntitySystem)this).HasComp<SquadLeaderComponent>(marineId))
		{
			_popup.PopupCursor("Transfer aborted. " + squad.Value.Name + " can't have another Squad Leader.", actor, PopupType.LargeCaution);
			return;
		}
		EntityUid? newSquadEnt = default(EntityUid?);
		if (!((EntitySystem)this).TryGetEntity(squad.Value.Id, ref newSquadEnt))
		{
			_popup.PopupCursor("You can't transfer marines to that squad!", actor, PopupType.LargeCaution);
			return;
		}
		if (_squad.TryGetMemberSquad(Entity<SquadMemberComponent>.op_Implicit(marineId.Value), out Entity<SquadTeamComponent> currentSquad) && currentSquad.Owner == ((EntitySystem)this).GetEntity(args.Squad))
		{
			_popup.PopupCursor(((EntitySystem)this).Name(marineId.Value, (MetaDataComponent)null) + " is already in " + ((EntitySystem)this).Name(newSquadEnt.Value, (MetaDataComponent)null) + "!", actor, PopupType.LargeCaution);
			return;
		}
		SquadTeamComponent newSquadComp = default(SquadTeamComponent);
		OriginalRoleComponent role = default(OriginalRoleComponent);
		if (((EntitySystem)this).TryComp<SquadTeamComponent>(newSquadEnt, ref newSquadComp) && _originalRoleQuery.TryComp(marineId, ref role))
		{
			ProtoId<JobPrototype>? job = role.Job;
			if (job.HasValue)
			{
				ProtoId<JobPrototype> job2 = job.GetValueOrDefault();
				if (!_squad.HasSpaceForRole(Entity<SquadTeamComponent>.op_Implicit((newSquadEnt.Value, newSquadComp)), job2))
				{
					string jobName = job2.Id;
					JobPrototype jobProto = default(JobPrototype);
					if (_prototypes.TryIndex<JobPrototype>(job2, ref jobProto))
					{
						jobName = base.Loc.GetString(jobProto.Name);
					}
					_popup.PopupCursor($"Transfer aborted. {((EntitySystem)this).Name(newSquadEnt.Value, (MetaDataComponent)null)} can't have another {jobName}.", actor, PopupType.LargeCaution);
					return;
				}
			}
		}
		_squad.AssignSquad(marineId.Value, Entity<SquadTeamComponent>.op_Implicit(newSquadEnt.Value), null);
		string selfMsg = $"{((EntitySystem)this).Name(marineId.Value, (MetaDataComponent)null)} has been transfered from squad '{((EntitySystem)this).Name(Entity<SquadTeamComponent>.op_Implicit(currentSquad), (MetaDataComponent)null)}' to squad '{((EntitySystem)this).Name(newSquadEnt.Value, (MetaDataComponent)null)}'. Logging to enlistment file.";
		_marineAnnounce.AnnounceSingle(selfMsg, actor);
		_popup.PopupCursor(selfMsg, actor, PopupType.Large);
		string targetMsg = "You've been transfered to " + ((EntitySystem)this).Name(newSquadEnt.Value, (MetaDataComponent)null) + "!";
		_marineAnnounce.AnnounceSingle(targetMsg, marineId.Value);
		_popup.PopupEntity(targetMsg, marineId.Value, marineId.Value, PopupType.Large);
	}

	private void OnWatchingMoveInput(Entity<OverwatchWatchingComponent> ent, ref MoveInputEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		if (args.HasDirectionalMovement)
		{
			TryLocalUnwatch(ent);
		}
	}

	private void OnWatchingDamageChanged(Entity<OverwatchWatchingComponent> ent, ref DamageChangedEvent args)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		if (!args.DamageIncreased)
		{
			return;
		}
		DamageSpecifier delta = args.DamageDelta;
		if (delta == null)
		{
			return;
		}
		Dictionary<string, FixedPoint2> damagePerGroup = delta.GetDamagePerGroup(_prototypes);
		FixedPoint2 bruteDamage = damagePerGroup.GetValueOrDefault(ProtoId<DamageGroupPrototype>.op_Implicit(_bruteGroup));
		FixedPoint2 burnDamage = damagePerGroup.GetValueOrDefault(ProtoId<DamageGroupPrototype>.op_Implicit(_burnGroup));
		FixedPoint2 toxinDamage = damagePerGroup.GetValueOrDefault(ProtoId<DamageGroupPrototype>.op_Implicit(_toxinGroup));
		if (bruteDamage + burnDamage <= FixedPoint2.Zero && toxinDamage <= 10)
		{
			return;
		}
		TryLocalUnwatch(ent);
		(EntityUid, Enum)[] array = _ui.GetActorUis(Entity<UserInterfaceUserComponent>.op_Implicit(ent.Owner)).ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			var (uiEnt, uiKey) = array[i];
			if (uiKey is OverwatchConsoleUI && (OverwatchConsoleUI)(object)uiKey == OverwatchConsoleUI.Key)
			{
				_ui.CloseUi(Entity<UserInterfaceComponent>.op_Implicit(uiEnt), uiKey, (EntityUid?)Entity<OverwatchWatchingComponent>.op_Implicit(ent), false);
			}
		}
		if (_net.IsServer)
		{
			_popup.PopupEntity("The pain kicked you out of the console!", Entity<OverwatchWatchingComponent>.op_Implicit(ent), Entity<OverwatchWatchingComponent>.op_Implicit(ent), PopupType.MediumCaution);
		}
	}

	private void OnOverwatchSelectSquadBui(Entity<OverwatchConsoleComponent> ent, ref OverwatchConsoleSelectSquadBuiMsg args)
	{
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsServer)
		{
			EntityUid? squad = default(EntityUid?);
			if (!((EntitySystem)this).TryGetEntity(args.Squad, ref squad) || !((EntitySystem)this).HasComp<SquadTeamComponent>(squad))
			{
				((EntitySystem)this).Log.Warning($"{((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(((BaseBoundUserInterfaceEvent)args).Actor))} tried to select invalid squad id {((EntitySystem)this).ToPrettyString(squad, (MetaDataComponent)null)}");
				return;
			}
			SupplyDropComputerComponent supplyComputer = default(SupplyDropComputerComponent);
			if (((EntitySystem)this).TryComp<SupplyDropComputerComponent>(Entity<OverwatchConsoleComponent>.op_Implicit(ent), ref supplyComputer))
			{
				SharedSupplyDropSystem supplyDrop = _supplyDrop;
				Entity<SupplyDropComputerComponent> computer = Entity<SupplyDropComputerComponent>.op_Implicit((Entity<OverwatchConsoleComponent>.op_Implicit(ent), supplyComputer));
				EntityPrototype obj = ((EntitySystem)this).Prototype(squad.Value, (MetaDataComponent)null);
				supplyDrop.SetSquad(computer, EntProtoId<SquadTeamComponent>.op_Implicit((obj != null) ? obj.ID : null));
			}
		}
		ent.Comp.Squad = args.Squad;
		ent.Comp.Operator = Identity.Name(((BaseBoundUserInterfaceEvent)args).Actor, (IEntityManager)(object)base.EntityManager);
		((EntitySystem)this).Dirty<OverwatchConsoleComponent>(ent, (MetaDataComponent)null);
	}

	private void OnOverwatchViewTacticalMapBui(Entity<OverwatchConsoleComponent> ent, ref OverwatchViewTacticalMapBuiMsg args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		_tacticalMap.OpenComputerMap(Entity<TacticalMapComputerComponent>.op_Implicit(ent.Owner), ((BaseBoundUserInterfaceEvent)args).Actor);
	}

	private void OnOverwatchTakeOperatorBui(Entity<OverwatchConsoleComponent> ent, ref OverwatchConsoleTakeOperatorBuiMsg args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.Operator = Identity.Name(((BaseBoundUserInterfaceEvent)args).Actor, (IEntityManager)(object)base.EntityManager);
		((EntitySystem)this).Dirty<OverwatchConsoleComponent>(ent, (MetaDataComponent)null);
	}

	private void OnOverwatchStopBui(Entity<OverwatchConsoleComponent> ent, ref OverwatchConsoleStopOverwatchBuiMsg args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.Squad = null;
		ent.Comp.Operator = null;
		((EntitySystem)this).Dirty<OverwatchConsoleComponent>(ent, (MetaDataComponent)null);
	}

	private void OnOverwatchSetLocationBui(Entity<OverwatchConsoleComponent> ent, ref OverwatchConsoleSetLocationBuiMsg args)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		if (!(args.Location < OverwatchLocation.Min) && !(args.Location > OverwatchLocation.Ship))
		{
			ent.Comp.Location = args.Location;
			((EntitySystem)this).Dirty<OverwatchConsoleComponent>(ent, (MetaDataComponent)null);
		}
	}

	private void OnOverwatchShowDeadBui(Entity<OverwatchConsoleComponent> ent, ref OverwatchConsoleShowDeadBuiMsg args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.ShowDead = args.Show;
		((EntitySystem)this).Dirty<OverwatchConsoleComponent>(ent, (MetaDataComponent)null);
	}

	private void OnOverwatchShowHiddenBui(Entity<OverwatchConsoleComponent> ent, ref OverwatchConsoleShowHiddenBuiMsg args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.ShowHidden = args.Show;
		((EntitySystem)this).Dirty<OverwatchConsoleComponent>(ent, (MetaDataComponent)null);
	}

	private void OnOverwatchTransferMarineBui(Entity<OverwatchConsoleComponent> ent, ref OverwatchConsoleTransferMarineBuiMsg args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		NetEntity? squad = ent.Comp.Squad;
		if (!squad.HasValue)
		{
			return;
		}
		NetEntity selectedSquad = squad.GetValueOrDefault();
		OverwatchConsoleBuiState overwatchBuiState = GetOverwatchBuiState(ent);
		List<DialogOption> options = new List<DialogOption>();
		if (overwatchBuiState.Marines.TryGetValue(selectedSquad, out List<OverwatchMarine> marines))
		{
			foreach (OverwatchMarine marine in marines)
			{
				DialogOption option = new DialogOption
				{
					Text = (marine.Name ?? ""),
					Event = new OverwatchTransferMarineSelectedEvent(((EntitySystem)this).GetNetEntity(((BaseBoundUserInterfaceEvent)args).Actor, (MetaDataComponent)null), marine.Id)
				};
				options.Add(option);
			}
		}
		_dialog.OpenOptions(Entity<OverwatchConsoleComponent>.op_Implicit(ent), ((BaseBoundUserInterfaceEvent)args).Actor, "Transfer Marine", options, "Choose marine to transfer");
	}

	private void OnOverwatchWatchBui(Entity<OverwatchConsoleComponent> ent, ref OverwatchConsoleWatchBuiMsg args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? target = default(EntityUid?);
		if (!(args.Target == default(NetEntity)) && ((EntitySystem)this).TryGetEntity(args.Target, ref target) && _inventory.TryGetInventoryEntity<OverwatchCameraComponent>(Entity<InventoryComponent>.op_Implicit(target.Value), out Entity<OverwatchCameraComponent> camera))
		{
			Watch(Entity<ActorComponent, EyeComponent>.op_Implicit(((BaseBoundUserInterfaceEvent)args).Actor), camera);
		}
	}

	private void OnOverwatchHideBui(Entity<OverwatchConsoleComponent> ent, ref OverwatchConsoleHideBuiMsg args)
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? target = default(EntityUid?);
		if (_net.IsClient)
		{
			if (args.Hide)
			{
				ent.Comp.Hidden.Add(args.Target);
			}
			else
			{
				ent.Comp.Hidden.Remove(args.Target);
			}
			((EntitySystem)this).Dirty<OverwatchConsoleComponent>(ent, (MetaDataComponent)null);
		}
		else if (!(args.Target == default(NetEntity)) && ((EntitySystem)this).TryGetEntity(args.Target, ref target) && ((EntitySystem)this).HasComp<SquadMemberComponent>(target))
		{
			if (args.Hide)
			{
				ent.Comp.Hidden.Add(args.Target);
			}
			else
			{
				ent.Comp.Hidden.Remove(args.Target);
			}
			((EntitySystem)this).Dirty<OverwatchConsoleComponent>(ent, (MetaDataComponent)null);
			OverwatchConsoleBuiState state = GetOverwatchBuiState(ent);
			_ui.SetUiState(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), (Enum)OverwatchConsoleUI.Key, (BoundUserInterfaceState)(object)state);
		}
	}

	private void OnOverwatchPromoteLeaderBui(Entity<OverwatchConsoleComponent> ent, ref OverwatchConsolePromoteLeaderBuiMsg args)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? target = default(EntityUid?);
		SquadMemberComponent member = default(SquadMemberComponent);
		if (!_net.IsClient && ((EntitySystem)this).TryGetEntity(args.Target, ref target) && ((EntitySystem)this).TryComp<SquadMemberComponent>(target, ref member))
		{
			_squad.PromoteSquadLeader(Entity<SquadMemberComponent>.op_Implicit((target.Value, member)), ((BaseBoundUserInterfaceEvent)args).Actor, args.Icon);
			OverwatchConsoleBuiState state = GetOverwatchBuiState(ent);
			_ui.SetUiState(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), (Enum)OverwatchConsoleUI.Key, (BoundUserInterfaceState)(object)state);
		}
	}

	private void OnOverwatchSupplyDropLongitudeBui(Entity<OverwatchConsoleComponent> ent, ref OverwatchConsoleSupplyDropLongitudeBuiMsg args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		_supplyDrop.SetLongitude(Entity<SupplyDropComputerComponent>.op_Implicit(ent.Owner), args.Longitude);
	}

	private void OnOverwatchSupplyDropLatitudeBui(Entity<OverwatchConsoleComponent> ent, ref OverwatchConsoleSupplyDropLatitudeBuiMsg args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		_supplyDrop.SetLatitude(Entity<SupplyDropComputerComponent>.op_Implicit(ent.Owner), args.Latitude);
	}

	private void OnOverwatchSupplyDropLaunchBui(Entity<OverwatchConsoleComponent> ent, ref OverwatchConsoleSupplyDropLaunchBuiMsg args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		SupplyDropComputerComponent computer = default(SupplyDropComputerComponent);
		if (!_net.IsClient && ((EntitySystem)this).TryComp<SupplyDropComputerComponent>(Entity<OverwatchConsoleComponent>.op_Implicit(ent), ref computer))
		{
			_supplyDrop.TryLaunchSupplyDropPopup(Entity<SupplyDropComputerComponent>.op_Implicit((Entity<OverwatchConsoleComponent>.op_Implicit(ent), computer)), ((BaseBoundUserInterfaceEvent)args).Actor);
			OverwatchConsoleBuiState state = GetOverwatchBuiState(ent);
			_ui.SetUiState(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), (Enum)OverwatchConsoleUI.Key, (BoundUserInterfaceState)(object)state);
			((EntitySystem)this).Dirty<OverwatchConsoleComponent>(ent, (MetaDataComponent)null);
		}
	}

	private void OnOverwatchSupplyDropSaveBui(Entity<OverwatchConsoleComponent> ent, ref OverwatchConsoleSupplyDropSaveBuiMsg args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		OverwatchSavedLocation?[] locations = ent.Comp.SavedLocations;
		if (locations.Length != 0)
		{
			ref int last = ref ent.Comp.LastLocation;
			if (last >= locations.Length)
			{
				last = 0;
			}
			locations[last] = new OverwatchSavedLocation(args.Longitude, args.Latitude, string.Empty);
			last++;
			((EntitySystem)this).Dirty<OverwatchConsoleComponent>(ent, (MetaDataComponent)null);
		}
	}

	private void OnOverwatchSupplyDropCommentBui(Entity<OverwatchConsoleComponent> ent, ref OverwatchConsoleLocationCommentBuiMsg args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		OverwatchSavedLocation?[] locations = ent.Comp.SavedLocations;
		if (args.Index < 0 || args.Index >= locations.Length)
		{
			return;
		}
		OverwatchSavedLocation? overwatchSavedLocation = locations[args.Index];
		if (overwatchSavedLocation.HasValue)
		{
			OverwatchSavedLocation location = overwatchSavedLocation.GetValueOrDefault();
			string comment = args.Comment;
			if (comment.Length > 50)
			{
				comment = comment.Substring(0, 50);
			}
			locations[args.Index] = location with
			{
				Comment = comment
			};
		}
	}

	private void OnOverwatchOrbitalCoordinatesBui(Entity<OverwatchConsoleComponent> ent, ref OverwatchConsoleOrbitalLongitudeBuiMsg args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.OrbitalCoordinates = new Vector2i(args.Longitude, ent.Comp.OrbitalCoordinates.Y);
	}

	private void OnOverwatchOrbitalCoordinatesBui(Entity<OverwatchConsoleComponent> ent, ref OverwatchConsoleOrbitalLatitudeBuiMsg args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.OrbitalCoordinates = new Vector2i(ent.Comp.OrbitalCoordinates.X, args.Latitude);
	}

	private void OnOverwatchOrbitalLaunchBui(Entity<OverwatchConsoleComponent> ent, ref OverwatchConsoleOrbitalLaunchBuiMsg args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.CanOrbitalBombardment && _orbitalCannon.TryGetClosestCannon(Entity<OverwatchConsoleComponent>.op_Implicit(ent), out Entity<OrbitalCannonComponent> cannon))
		{
			EntityUid squad = default(EntityUid);
			EntityUid? squadNullable = default(EntityUid?);
			if (((EntitySystem)this).TryGetEntity(ent.Comp.Squad, ref squadNullable))
			{
				squad = squadNullable.Value;
			}
			_orbitalCannon.Fire(cannon, ent.Comp.OrbitalCoordinates, ((BaseBoundUserInterfaceEvent)args).Actor, squad);
		}
	}

	private void OnOverwatchSendMessageBui(Entity<OverwatchConsoleComponent> ent, ref OverwatchConsoleSendMessageBuiMsg args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		if (!ent.Comp.CanMessageSquad)
		{
			return;
		}
		TimeSpan time = _timing.CurTime;
		if (time < ent.Comp.LastMessage + ent.Comp.MessageCooldown)
		{
			return;
		}
		string message = args.Message;
		if (message.Length > 200)
		{
			message = message.Substring(0, 200);
		}
		EntityUid? squad = default(EntityUid?);
		if (!string.IsNullOrWhiteSpace(message) && ((EntitySystem)this).TryGetEntity(ent.Comp.Squad, ref squad))
		{
			EntityPrototype squadProto = ((EntitySystem)this).Prototype(squad.Value, (MetaDataComponent)null);
			if (squadProto != null)
			{
				ent.Comp.LastMessage = time;
				((EntitySystem)this).Dirty<OverwatchConsoleComponent>(ent, (MetaDataComponent)null);
				ISharedAdminLogManager adminLog = _adminLog;
				LogStringHandler handler = new LogStringHandler(22, 3);
				handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(((BaseBoundUserInterfaceEvent)args).Actor)), "ToPrettyString(args.Actor)");
				handler.AppendLiteral(" sent ");
				handler.AppendFormatted(squadProto.Name);
				handler.AppendLiteral(" squad message: ");
				handler.AppendFormatted(args.Message);
				adminLog.Add(LogType.RMCMarineAnnounce, ref handler);
				_marineAnnounce.AnnounceSquad($"[color=#3C70FF][bold]Overwatch:[/bold] {((EntitySystem)this).Name(((BaseBoundUserInterfaceEvent)args).Actor, (MetaDataComponent)null)} transmits: [font size=16][bold]{message}[/bold][/font][/color]", EntProtoId<SquadTeamComponent>.op_Implicit(squadProto.ID));
				MapCoordinates coordinates = _transform.GetMapCoordinates(Entity<OverwatchConsoleComponent>.op_Implicit(ent), (TransformComponent)null);
				Filter players = Filter.Empty().AddInRange(coordinates, 12f, _player, (IEntityManager)(object)base.EntityManager);
				players.RemoveWhereAttachedEntity((Predicate<EntityUid>)base.HasComp<XenoComponent>);
				string userMsg = $"[bold][color=#6685F5]'{((EntitySystem)this).Name(squad.Value, (MetaDataComponent)null)}' squad message sent: '{message}'.[/color][/bold]";
				ActorComponent obj = ((EntitySystem)this).CompOrNull<ActorComponent>(((BaseBoundUserInterfaceEvent)args).Actor);
				NetUserId? author = ((obj != null) ? new NetUserId?(obj.PlayerSession.UserId) : ((NetUserId?)null));
				SharedCMChatSystem rmcChat = _rmcChat;
				NetUserId? author2 = author;
				rmcChat.ChatMessageToMany(userMsg, userMsg, players, ChatChannel.Local, default(EntityUid), hideChat: false, null, recordReplay: false, null, 0f, author2);
			}
		}
	}

	protected virtual void Watch(Entity<ActorComponent?, EyeComponent?> watcher, Entity<OverwatchCameraComponent?> toWatch)
	{
	}

	protected virtual void Unwatch(Entity<EyeComponent?> watcher, ICommonSession player)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<EyeComponent>(Entity<EyeComponent>.op_Implicit(watcher), ref watcher.Comp, true))
		{
			_eye.SetTarget(Entity<EyeComponent>.op_Implicit(watcher), (EntityUid?)null, (EyeComponent)null);
		}
	}

	private OverwatchConsoleBuiState GetOverwatchBuiState(Entity<OverwatchConsoleComponent> console)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return GetOverwatchBuiState(console.Comp);
	}

	private OverwatchConsoleBuiState GetOverwatchBuiState(OverwatchConsoleComponent console)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		List<OverwatchSquad> squads = new List<OverwatchSquad>();
		Dictionary<NetEntity, List<OverwatchMarine>> marines = new Dictionary<NetEntity, List<OverwatchMarine>>();
		EntityQueryEnumerator<SquadTeamComponent> query = ((EntitySystem)this).EntityQueryEnumerator<SquadTeamComponent>();
		EntityUid uid = default(EntityUid);
		SquadTeamComponent team = default(SquadTeamComponent);
		while (query.MoveNext(ref uid, ref team))
		{
			if (console.Group != "ADMINISTRATOR" && team.Group != console.Group)
			{
				continue;
			}
			NetEntity netUid = ((EntitySystem)this).GetNetEntity(uid, (MetaDataComponent)null);
			OverwatchSquad squad = new OverwatchSquad(netUid, ((EntitySystem)this).Name(uid, (MetaDataComponent)null), team.Color, null, team.CanSupplyDrop, team.LeaderIcon);
			List<OverwatchMarine> members = Extensions.GetOrNew<NetEntity, List<OverwatchMarine>>(marines, netUid);
			foreach (EntityUid member in team.Members)
			{
				OverwatchMarine? overwatchMarine = _overwatchDataQuery.CompOrNull(member)?.Marine;
				if (overwatchMarine.HasValue)
				{
					OverwatchMarine data = overwatchMarine.GetValueOrDefault();
					members.Add(data);
				}
			}
			squads.Add(squad);
		}
		return new OverwatchConsoleBuiState(squads, marines);
	}

	public bool IsHidden(Entity<OverwatchConsoleComponent> console, NetEntity marine)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		return console.Comp.Hidden.Contains(marine);
	}

	private void TryLocalUnwatch(Entity<OverwatchWatchingComponent> ent)
	{
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			EntityUid? localEntity = _player.LocalEntity;
			EntityUid owner = ent.Owner;
			if (localEntity.HasValue && localEntity.GetValueOrDefault() == owner && _player.LocalSession != null)
			{
				Unwatch(Entity<EyeComponent>.op_Implicit(ent.Owner), _player.LocalSession);
				return;
			}
		}
		ActorComponent actor = default(ActorComponent);
		if (((EntitySystem)this).TryComp<ActorComponent>(Entity<OverwatchWatchingComponent>.op_Implicit(ent), ref actor))
		{
			Unwatch(Entity<EyeComponent>.op_Implicit(ent.Owner), actor.PlayerSession);
		}
	}

	private void ProcessData()
	{
		//IL_0385: Unknown result type (might be due to invalid IL or missing references)
		//IL_038a: Unknown result type (might be due to invalid IL or missing references)
		//IL_039e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0354: Unknown result type (might be due to invalid IL or missing references)
		//IL_0359: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			_toProcess.Clear();
			return;
		}
		try
		{
			TimeSpan time = _timing.CurTime;
			if (_toProcess.Count > 0)
			{
				EntityUid? mapId = default(EntityUid?);
				foreach (var (squadId, membersQueue) in _toProcess)
				{
					if (((EntitySystem)this).TerminatingOrDeleted(Entity<SquadTeamComponent>.op_Implicit(squadId), (MetaDataComponent)null))
					{
						_toRemove.Add(squadId);
						continue;
					}
					MapCoordinates? leaderCoords = null;
					if (_squad.TryGetSquadLeader(squadId, out Entity<SquadLeaderComponent> leader))
					{
						leaderCoords = _transform.GetMapCoordinates(Entity<SquadLeaderComponent>.op_Implicit(leader), (TransformComponent)null);
					}
					EntityUid member;
					while (membersQueue.TryDequeue(out member) && !(_timing.CurTime > time + _maxProcessTime))
					{
						if (((EntitySystem)this).TerminatingOrDeleted(member, (MetaDataComponent)null))
						{
							continue;
						}
						TransformComponent xform = ((EntitySystem)this).Transform(member);
						if (_map.TryGetMap((MapId?)xform.MapID, ref mapId) && !_map.IsPaused(Entity<MapComponent>.op_Implicit(mapId.Value)))
						{
							MapCoordinates coords = _transform.GetMapCoordinates(member, (TransformComponent)null);
							IdentityEntity name = Identity.Name(member, (IEntityManager)(object)base.EntityManager);
							MobState mobState = _mobStateQuery.CompOrNull(member)?.CurrentState ?? MobState.Alive;
							bool ssd = !_actor.HasComp(member);
							ProtoId<JobPrototype>? role = _originalRoleQuery.CompOrNull(member)?.Job;
							ProtoId<RankPrototype>? rank = _rankQuery.CompOrNull(member)?.Rank;
							OverwatchLocation location = ((!_planetQuery.HasComp(mapId)) ? OverwatchLocation.Ship : OverwatchLocation.Min);
							Entity<AreaComponent>? area;
							EntityPrototype areaProto;
							string areaName = (_area.TryGetArea(coords, out area, out areaProto) ? areaProto.Name : string.Empty);
							NetEntity netMember = ((EntitySystem)this).GetNetEntity(member, (MetaDataComponent)null);
							LocId? roleOverride = ((EntitySystem)this).CompOrNull<RMCVendorRoleOverrideComponent>(member)?.GiveSquadRoleName ?? ((EntitySystem)this).CompOrNull<UsedSkillPamphletComponent>(member)?.JobTitle;
							Vector2? leaderDistance = null;
							if (member != leader.Owner && leaderCoords.HasValue && leaderCoords.Value.MapId == coords.MapId)
							{
								leaderDistance = leaderCoords.Value.Position - coords.Position;
							}
							_inventory.TryGetInventoryEntity<OverwatchCameraComponent>(Entity<InventoryComponent>.op_Implicit(member), out Entity<OverwatchCameraComponent> camera);
							((EntitySystem)this).EnsureComp<OverwatchDataComponent>(member).Marine = new OverwatchMarine(netMember, ((EntitySystem)this).GetNetEntity(Entity<OverwatchCameraComponent>.op_Implicit(camera), (MetaDataComponent)null), name, mobState, ssd, role, location == OverwatchLocation.Min, location, areaName, leaderDistance, rank, roleOverride);
						}
					}
					if (membersQueue.Count == 0)
					{
						_toRemove.Add(squadId);
					}
				}
				{
					foreach (Entity<SquadTeamComponent> squad in _toRemove)
					{
						_toProcess.Remove(squad);
					}
					return;
				}
			}
			EntityQueryEnumerator<SquadTeamComponent> query = ((EntitySystem)this).EntityQueryEnumerator<SquadTeamComponent>();
			EntityUid squadId2 = default(EntityUid);
			SquadTeamComponent squadComp = default(SquadTeamComponent);
			while (query.MoveNext(ref squadId2, ref squadComp))
			{
				Queue<EntityUid> queue2 = Extensions.GetOrNew<Entity<SquadTeamComponent>, Queue<EntityUid>>(_toProcess, Entity<SquadTeamComponent>.op_Implicit((squadId2, squadComp)));
				foreach (EntityUid member2 in squadComp.Members)
				{
					queue2.Enqueue(member2);
				}
			}
		}
		catch
		{
			_toProcess.Clear();
			throw;
		}
	}

	private void UpdateConsoles()
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		TimeSpan time = _timing.CurTime;
		if (time < _nextUpdateTime)
		{
			return;
		}
		_nextUpdateTime = time + _updateEvery;
		OverwatchConsoleBuiState state = null;
		EntityQueryEnumerator<OverwatchConsoleComponent> query = ((EntitySystem)this).EntityQueryEnumerator<OverwatchConsoleComponent>();
		EntityUid uid = default(EntityUid);
		OverwatchConsoleComponent console = default(OverwatchConsoleComponent);
		while (query.MoveNext(ref uid, ref console))
		{
			if (_ui.IsUiOpen(Entity<UserInterfaceComponent>.op_Implicit(uid), (Enum)OverwatchConsoleUI.Key))
			{
				if (state == null)
				{
					state = GetOverwatchBuiState(console);
				}
				_ui.SetUiState(Entity<UserInterfaceComponent>.op_Implicit(uid), (Enum)OverwatchConsoleUI.Key, (BoundUserInterfaceState)(object)state);
			}
		}
	}

	public override void Update(float frameTime)
	{
		ProcessData();
		UpdateConsoles();
	}
}
