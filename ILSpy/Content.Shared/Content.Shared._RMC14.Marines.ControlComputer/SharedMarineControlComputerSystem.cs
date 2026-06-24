using System;
using System.Collections.Generic;
using Content.Shared._RMC14.AlertLevel;
using Content.Shared._RMC14.Commendations;
using Content.Shared._RMC14.Dialog;
using Content.Shared._RMC14.Dropship;
using Content.Shared._RMC14.Evacuation;
using Content.Shared._RMC14.Marines.Announce;
using Content.Shared._RMC14.Survivor;
using Content.Shared.CCVar;
using Content.Shared.Database;
using Content.Shared.Dataset;
using Content.Shared.Ghost;
using Content.Shared.Popups;
using Content.Shared.UserInterface;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Marines.ControlComputer;

public abstract class SharedMarineControlComputerSystem : EntitySystem
{
	[Dependency]
	private RMCAlertLevelSystem _alertLevel;

	[Dependency]
	private SharedCommendationSystem _commendation;

	[Dependency]
	private IConfigurationManager _config;

	[Dependency]
	private DialogSystem _dialog;

	[Dependency]
	private SharedEvacuationSystem _evacuation;

	[Dependency]
	private SharedMarineAnnounceSystem _marineAnnounce;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private IPrototypeManager _prototype;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private SharedUserInterfaceSystem _ui;

	[Dependency]
	private WarshipSystem _warship;

	private static readonly ProtoId<LocalizedDatasetPrototype> MedalsDatasetId = ProtoId<LocalizedDatasetPrototype>.op_Implicit("RMCMarineMedals");

	private LocalizedDatasetPrototype _medalsDataset;

	private int _characterLimit = 1000;

	public override void Initialize()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Initialize();
		_medalsDataset = _prototype.Index<LocalizedDatasetPrototype>(MedalsDatasetId);
		((EntitySystem)this).SubscribeLocalEvent<EvacuationEnabledEvent>((EntityEventRefHandler<EvacuationEnabledEvent>)OnRefreshComputers, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EvacuationDisabledEvent>((EntityEventRefHandler<EvacuationDisabledEvent>)OnRefreshComputers, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EvacuationProgressEvent>((EntityEventRefHandler<EvacuationProgressEvent>)OnRefreshComputers, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DropshipHijackStartEvent>((EntityEventRefHandler<DropshipHijackStartEvent>)OnRefreshComputers, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCAlertLevelChangedEvent>((EntityEventRefHandler<RMCAlertLevelChangedEvent>)OnRefreshComputers, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MarineControlComputerComponent, BeforeActivatableUIOpenEvent>((EntityEventRefHandler<MarineControlComputerComponent, BeforeActivatableUIOpenEvent>)OnComputerBeforeUIOpen, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MarineControlComputerComponent, MarineControlComputerMedalMarineEvent>((EntityEventRefHandler<MarineControlComputerComponent, MarineControlComputerMedalMarineEvent>)OnComputerMedalMarine, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MarineControlComputerComponent, MarineControlComputerMedalNameEvent>((EntityEventRefHandler<MarineControlComputerComponent, MarineControlComputerMedalNameEvent>)OnComputerMedalName, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MarineControlComputerComponent, MarineControlComputerMedalMessageEvent>((EntityEventRefHandler<MarineControlComputerComponent, MarineControlComputerMedalMessageEvent>)OnComputerMedalMessage, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MarineControlComputerComponent, MarineControlComputerAlertEvent>((EntityEventRefHandler<MarineControlComputerComponent, MarineControlComputerAlertEvent>)OnComputerAlert, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MarineControlComputerComponent, MarineControlComputerShipAnnouncementDialogEvent>((EntityEventRefHandler<MarineControlComputerComponent, MarineControlComputerShipAnnouncementDialogEvent>)OnShipAnnouncementDialog, (Type[])null, (Type[])null);
		BoundUserInterfaceRegisterExt.BuiEvents<MarineControlComputerComponent>(((EntitySystem)this).Subs, (object)MarineControlComputerUi.Key, (BuiEventSubscriber<MarineControlComputerComponent>)delegate(Subscriber<MarineControlComputerComponent> subs)
		{
			subs.Event<MarineControlComputerAlertLevelMsg>((EntityEventRefHandler<MarineControlComputerComponent, MarineControlComputerAlertLevelMsg>)OnAlertLevel);
			subs.Event<MarineControlComputerShipAnnouncementMsg>((EntityEventRefHandler<MarineControlComputerComponent, MarineControlComputerShipAnnouncementMsg>)OnShipAnnouncement);
			subs.Event<MarineControlComputerMedalMsg>((EntityEventRefHandler<MarineControlComputerComponent, MarineControlComputerMedalMsg>)OnMedal);
			subs.Event<MarineControlComputerToggleEvacuationMsg>((EntityEventRefHandler<MarineControlComputerComponent, MarineControlComputerToggleEvacuationMsg>)OnToggleEvacuationMsg);
		});
		BoundUserInterfaceRegisterExt.BuiEvents<MarineCommunicationsComputerComponent>(((EntitySystem)this).Subs, (object)MarineCommunicationsComputerUI.Key, (BuiEventSubscriber<MarineCommunicationsComputerComponent>)delegate(Subscriber<MarineCommunicationsComputerComponent> subs)
		{
			subs.Event<MarineControlComputerToggleEvacuationMsg>((EntityEventRefHandler<MarineCommunicationsComputerComponent, MarineControlComputerToggleEvacuationMsg>)OnMarineCommunicationsToggleEvacuation);
		});
		EntitySystemSubscriptionExt.CVar<int>(((EntitySystem)this).Subs, _config, CCVars.ChatMaxMessageLength, (Action<int>)delegate(int limit)
		{
			_characterLimit = limit;
		}, true);
	}

	private void OnRefreshComputers<T>(ref T ev)
	{
		RefreshComputers();
	}

	private void OnComputerBeforeUIOpen(Entity<MarineControlComputerComponent> ent, ref BeforeActivatableUIOpenEvent args)
	{
		RefreshComputers();
	}

	private void OnComputerMedalMarine(Entity<MarineControlComputerComponent> ent, ref MarineControlComputerMedalMarineEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? actor = default(EntityUid?);
		if (!((EntitySystem)this).TryGetEntity(args.Actor, ref actor))
		{
			return;
		}
		if (args.Marine.HasValue)
		{
			EntityUid? marine = default(EntityUid?);
			if (!((EntitySystem)this).TryGetEntity(args.Marine, ref marine))
			{
				return;
			}
			EntityUid? val = marine;
			EntityUid? val2 = actor;
			if (val.HasValue == val2.HasValue && (!val.HasValue || val.GetValueOrDefault() == val2.GetValueOrDefault()))
			{
				_popup.PopupClient(base.Loc.GetString("rmc-medal-error-self-award"), actor, PopupType.MediumCaution);
				return;
			}
		}
		else if (args.LastPlayerId == null)
		{
			return;
		}
		if (_net.IsClient)
		{
			return;
		}
		List<DialogOption> options = new List<DialogOption>();
		foreach (string name in _medalsDataset.Values)
		{
			options.Add(new DialogOption(base.Loc.GetString(name), new MarineControlComputerMedalNameEvent(args.Actor, args.Marine, base.Loc.GetString(name), args.LastPlayerId)));
		}
		_dialog.OpenOptions(Entity<MarineControlComputerComponent>.op_Implicit(ent), actor.Value, base.Loc.GetString("rmc-medal-type"), options, base.Loc.GetString("rmc-medal-type-prompt"));
	}

	private void OnComputerMedalName(Entity<MarineControlComputerComponent> ent, ref MarineControlComputerMedalNameEvent args)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? actor = default(EntityUid?);
		if (!_net.IsClient && ((EntitySystem)this).TryGetEntity(args.Actor, ref actor))
		{
			MarineControlComputerMedalMessageEvent ev = new MarineControlComputerMedalMessageEvent(args.Actor, args.Marine, args.Name, "", args.LastPlayerId);
			_dialog.OpenInput(Entity<MarineControlComputerComponent>.op_Implicit(ent), actor.Value, base.Loc.GetString("rmc-medal-citation-prompt"), ev, largeInput: true, _commendation.CharacterLimit);
		}
	}

	private void OnComputerMedalMessage(Entity<MarineControlComputerComponent> ent, ref MarineControlComputerMedalMessageEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? actor = default(EntityUid?);
		if (!((EntitySystem)this).TryGetEntity(args.Actor, ref actor) || !((EntitySystem)this).HasComp<CommendationGiverComponent>(actor) || string.IsNullOrWhiteSpace(args.Message.Trim()))
		{
			return;
		}
		if (args.Marine.HasValue)
		{
			EntityUid? marine = default(EntityUid?);
			CommendationReceiverComponent receiver = default(CommendationReceiverComponent);
			if (!((EntitySystem)this).TryGetEntity(args.Marine, ref marine) || !((EntitySystem)this).TryComp<CommendationReceiverComponent>(marine, ref receiver) || receiver.LastPlayerId == null)
			{
				return;
			}
			_commendation.GiveCommendation(Entity<CommendationGiverComponent, ActorComponent>.op_Implicit(actor.Value), Entity<CommendationReceiverComponent>.op_Implicit(marine.Value), args.Name, args.Message, CommendationType.Medal);
		}
		else
		{
			if (args.LastPlayerId == null)
			{
				return;
			}
			MarineControlComputerComponent computer = default(MarineControlComputerComponent);
			if (((EntitySystem)this).TryComp<MarineControlComputerComponent>(Entity<MarineControlComputerComponent>.op_Implicit(ent), ref computer) && computer.GibbedMarines.TryGetValue(args.LastPlayerId, out GibbedMarineInfo info))
			{
				_commendation.GiveCommendationByLastPlayerId(Entity<CommendationGiverComponent, ActorComponent>.op_Implicit(actor.Value), args.LastPlayerId, info.Name, args.Name, args.Message, CommendationType.Medal);
			}
		}
		if (!_net.IsClient)
		{
			_popup.PopupCursor(base.Loc.GetString("rmc-medal-awarded"), actor.Value, PopupType.Large);
		}
	}

	private void OnComputerAlert(Entity<MarineControlComputerComponent> ent, ref MarineControlComputerAlertEvent args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		_alertLevel.Set(args.Level, ((EntitySystem)this).GetEntity(args.User));
	}

	private void OnAlertLevel(Entity<MarineControlComputerComponent> ent, ref MarineControlComputerAlertLevelMsg args)
	{
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		RMCAlertLevels? current = _alertLevel.Get();
		List<DialogOption> options = new List<DialogOption>();
		RMCAlertLevels[] values = Enum.GetValues<RMCAlertLevels>();
		for (int i = 0; i < values.Length; i++)
		{
			RMCAlertLevels level = values[i];
			if (level != current && level < RMCAlertLevels.Red)
			{
				string text = base.Loc.GetString("rmc-alert-" + level.ToString().ToLowerInvariant());
				options.Add(new DialogOption(text, new MarineControlComputerAlertEvent(((EntitySystem)this).GetNetEntity(((BaseBoundUserInterfaceEvent)args).Actor, (MetaDataComponent)null), level)));
			}
		}
		_dialog.OpenOptions(Entity<MarineControlComputerComponent>.op_Implicit(ent), ((BaseBoundUserInterfaceEvent)args).Actor, base.Loc.GetString("rmc-alert-level"), options, base.Loc.GetString("rmc-alert-level-which"));
	}

	private void OnShipAnnouncement(Entity<MarineControlComputerComponent> ent, ref MarineControlComputerShipAnnouncementMsg args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		if (CanUseShipAnnouncementPopup(ent, ((BaseBoundUserInterfaceEvent)args).Actor))
		{
			MarineControlComputerShipAnnouncementDialogEvent ev = new MarineControlComputerShipAnnouncementDialogEvent(((EntitySystem)this).GetNetEntity(((BaseBoundUserInterfaceEvent)args).Actor, (MetaDataComponent)null));
			_dialog.OpenInput(Entity<MarineControlComputerComponent>.op_Implicit(ent), ((BaseBoundUserInterfaceEvent)args).Actor, base.Loc.GetString("rmc-announcement-shipside-header"), ev, largeInput: true, _characterLimit);
		}
	}

	private void OnShipAnnouncementDialog(Entity<MarineControlComputerComponent> ent, ref MarineControlComputerShipAnnouncementDialogEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		EntityUid user = ((EntitySystem)this).GetEntity(args.User);
		if (((EntityUid)(ref user)).Valid && CanUseShipAnnouncementPopup(ent, user))
		{
			ent.Comp.LastShipAnnouncement = _timing.CurTime;
			MapId warshipMap;
			MapId map = (_warship.TryGetWarshipMap(Entity<MarineControlComputerComponent>.op_Implicit(ent), out warshipMap) ? warshipMap : _transform.GetMapId(Entity<TransformComponent>.op_Implicit(ent.Owner)));
			_marineAnnounce.AnnounceSigned(user, args.Message, base.Loc.GetString("rmc-announcement-author-shipside"), null, SharedMarineAnnounceSystem.AresAnnouncementSound, Filter.BroadcastMap(map).RemoveWhereAttachedEntity((Predicate<EntityUid>)((EntityUid e) => !((EntitySystem)this).HasComp<MarineComponent>(e) && !((EntitySystem)this).HasComp<GhostComponent>(e))), excludeSurvivors: false);
		}
	}

	private void OnMedal(Entity<MarineControlComputerComponent> ent, ref MarineControlComputerMedalMsg args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		GiveMedal(Entity<MarineControlComputerComponent>.op_Implicit(ent), ((BaseBoundUserInterfaceEvent)args).Actor);
	}

	public void GiveMedal(EntityUid computer, EntityUid actor)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		ActorComponent actorComp = default(ActorComponent);
		if (!((EntitySystem)this).TryComp<ActorComponent>(actor, ref actorComp))
		{
			return;
		}
		if (!((EntitySystem)this).HasComp<CommendationGiverComponent>(actor))
		{
			_popup.PopupClient(base.Loc.GetString("rmc-medal-error-officer-only"), actor, PopupType.MediumCaution);
		}
		else
		{
			if (_net.IsClient)
			{
				return;
			}
			NetEntity netActor = ((EntitySystem)this).GetNetEntity(actor, (MetaDataComponent)null);
			List<DialogOption> options = new List<DialogOption>();
			EntityQueryEnumerator<CommendationReceiverComponent, MarineComponent> receivers = ((EntitySystem)this).EntityQueryEnumerator<CommendationReceiverComponent, MarineComponent>();
			EntityUid uid = default(EntityUid);
			CommendationReceiverComponent receiver = default(CommendationReceiverComponent);
			MarineComponent marineComponent = default(MarineComponent);
			while (receivers.MoveNext(ref uid, ref receiver, ref marineComponent))
			{
				if (receiver.LastPlayerId != null && !(Guid.Parse(receiver.LastPlayerId) == NetUserId.op_Implicit(actorComp.PlayerSession.UserId)) && !((EntitySystem)this).HasComp<RMCSurvivorComponent>(uid) && !(uid == actor))
				{
					options.Add(new DialogOption(((EntitySystem)this).Name(uid, (MetaDataComponent)null), new MarineControlComputerMedalMarineEvent(netActor, ((EntitySystem)this).GetNetEntity(uid, (MetaDataComponent)null))));
				}
			}
			Dictionary<string, GibbedMarineInfo> allGibbed = new Dictionary<string, GibbedMarineInfo>();
			EntityQueryEnumerator<MarineControlComputerComponent> computers = ((EntitySystem)this).EntityQueryEnumerator<MarineControlComputerComponent>();
			EntityUid val = default(EntityUid);
			MarineControlComputerComponent comp = default(MarineControlComputerComponent);
			string key;
			GibbedMarineInfo value;
			while (computers.MoveNext(ref val, ref comp))
			{
				foreach (KeyValuePair<string, GibbedMarineInfo> gibbedMarine in comp.GibbedMarines)
				{
					gibbedMarine.Deconstruct(out key, out value);
					string playerId = key;
					GibbedMarineInfo info = value;
					if (info.LastPlayerId != null)
					{
						allGibbed[playerId] = info;
					}
				}
			}
			foreach (KeyValuePair<string, GibbedMarineInfo> item in allGibbed)
			{
				item.Deconstruct(out key, out value);
				string playerId2 = key;
				GibbedMarineInfo info2 = value;
				options.Add(new DialogOption(info2.Name, new MarineControlComputerMedalMarineEvent(netActor, null, playerId2)));
			}
			_dialog.OpenOptions(computer, actor, base.Loc.GetString("rmc-medal-recipient"), options, base.Loc.GetString("rmc-medal-recipient-prompt"));
		}
	}

	private void OnToggleEvacuationMsg(Entity<MarineControlComputerComponent> ent, ref MarineControlComputerToggleEvacuationMsg args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		if (_ui.HasUi(ent.Owner, (Enum)MarineControlComputerUi.Key, (UserInterfaceComponent)null))
		{
			_ui.CloseUi(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), (Enum)MarineControlComputerUi.Key, (EntityUid?)((BaseBoundUserInterfaceEvent)args).Actor, false);
		}
		if (_ui.HasUi(ent.Owner, (Enum)MarineCommunicationsComputerUI.Key, (UserInterfaceComponent)null))
		{
			_ui.CloseUi(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), (Enum)MarineCommunicationsComputerUI.Key, (EntityUid?)((BaseBoundUserInterfaceEvent)args).Actor, false);
		}
		if (ent.Comp.CanEvacuate)
		{
			TimeSpan time = _timing.CurTime;
			if (!(time < ent.Comp.LastToggle + ent.Comp.ToggleCooldown))
			{
				ent.Comp.LastToggle = time;
				_evacuation.ToggleEvacuation(null, ent.Comp.EvacuationCancelledSound, _transform.GetMap(Entity<TransformComponent>.op_Implicit(ent.Owner)));
				RefreshComputers();
			}
		}
	}

	private void OnMarineCommunicationsToggleEvacuation(Entity<MarineCommunicationsComputerComponent> ent, ref MarineControlComputerToggleEvacuationMsg args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		MarineControlComputerComponent controlComp = default(MarineControlComputerComponent);
		if (((EntitySystem)this).TryComp<MarineControlComputerComponent>(ent.Owner, ref controlComp))
		{
			OnToggleEvacuationMsg(new Entity<MarineControlComputerComponent>(ent.Owner, controlComp), ref args);
		}
	}

	private void RefreshComputers()
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		if (!_net.IsClient)
		{
			bool canEvacuate = _alertLevel.IsRedOrDeltaAlert() || _evacuation.IsEvacuationEnabled();
			bool evacuationEnabled = _evacuation.IsEvacuationEnabled();
			EntityQueryEnumerator<MarineControlComputerComponent> computers = ((EntitySystem)this).EntityQueryEnumerator<MarineControlComputerComponent>();
			EntityUid uid = default(EntityUid);
			MarineControlComputerComponent computer = default(MarineControlComputerComponent);
			while (computers.MoveNext(ref uid, ref computer))
			{
				computer.Evacuating = evacuationEnabled;
				computer.CanEvacuate = canEvacuate;
				((EntitySystem)this).Dirty(uid, (IComponent)(object)computer, (MetaDataComponent)null);
			}
		}
	}

	private bool CanUseShipAnnouncementPopup(Entity<MarineControlComputerComponent> ent, EntityUid user)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		TimeSpan cooldown = ent.Comp.ShipAnnouncementCooldown;
		if (ent.Comp.LastShipAnnouncement.HasValue)
		{
			TimeSpan curTime = _timing.CurTime;
			TimeSpan? timeSpan = ent.Comp.LastShipAnnouncement + cooldown;
			if (curTime < timeSpan)
			{
				string msg = base.Loc.GetString("rmc-announcement-cooldown", (ValueTuple<string, object>)("seconds", (int)cooldown.TotalSeconds));
				_popup.PopupClient(msg, user);
				return false;
			}
		}
		return true;
	}
}
