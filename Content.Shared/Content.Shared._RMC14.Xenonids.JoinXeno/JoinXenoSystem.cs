using System;
using System.Collections.Generic;
using Content.Shared._RMC14.CCVar;
using Content.Shared._RMC14.Dialog;
using Content.Shared._RMC14.GameTicking;
using Content.Shared._RMC14.Rules;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared.Actions;
using Content.Shared.GameTicking;
using Content.Shared.GameTicking.Components;
using Content.Shared.Ghost;
using Content.Shared.Popups;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Xenonids.JoinXeno;

public sealed class JoinXenoSystem : EntitySystem
{
	[Dependency]
	private SharedActionsSystem _actions;

	[Dependency]
	private DialogSystem _dialog;

	[Dependency]
	private SharedXenoHiveSystem _hive;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedRMCGameTickerSystem _rmcGameTicker;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedGameTicker _gameTicker;

	[Dependency]
	private IConfigurationManager _config;

	[Dependency]
	private SharedPopupSystem _popup;

	private TimeSpan _burrowedLarvaDeathTime;

	private TimeSpan _burrowedLarvaDeathIgnoreTime;

	public int ClientBurrowedLarva { get; private set; }

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<RoundRestartCleanupEvent>((EntityEventHandler<RoundRestartCleanupEvent>)OnRoundRestartCleanup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<JoinXenoComponent, MapInitEvent>((EntityEventRefHandler<JoinXenoComponent, MapInitEvent>)OnJoinXenoMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<JoinXenoComponent, JoinXenoActionEvent>((EntityEventRefHandler<JoinXenoComponent, JoinXenoActionEvent>)OnJoinXenoAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<JoinXenoComponent, JoinXenoBurrowedLarvaEvent>((EntityEventRefHandler<JoinXenoComponent, JoinXenoBurrowedLarvaEvent>)OnJoinXenoBurrowedLarva, (Type[])null, (Type[])null);
		if (_net.IsClient)
		{
			((EntitySystem)this).SubscribeNetworkEvent<BurrowedLarvaStatusEvent>((EntityEventHandler<BurrowedLarvaStatusEvent>)OnBurrowedLarvaStatus, (Type[])null, (Type[])null);
		}
		else
		{
			((EntitySystem)this).SubscribeLocalEvent<RMCPlayerJoinedLobbyEvent>((EntityEventRefHandler<RMCPlayerJoinedLobbyEvent>)OnPlayerJoinedLobby, (Type[])null, (Type[])null);
			((EntitySystem)this).SubscribeLocalEvent<BurrowedLarvaChangedEvent>((EntityEventRefHandler<BurrowedLarvaChangedEvent>)OnBurrowedLarvaChanged, (Type[])null, (Type[])null);
			((EntitySystem)this).SubscribeNetworkEvent<JoinBurrowedLarvaRequest>((EntitySessionEventHandler<JoinBurrowedLarvaRequest>)OnJoinBurrowedLarva, (Type[])null, (Type[])null);
			((EntitySystem)this).SubscribeNetworkEvent<BurrowedLarvaStatusRequest>((EntitySessionEventHandler<BurrowedLarvaStatusRequest>)OnBurrowedLarvaStatusRequest, (Type[])null, (Type[])null);
		}
		EntitySystemSubscriptionExt.CVar<float>(((EntitySystem)this).Subs, _config, RMCCVars.RMCLateJoinsBurrowedLarvaDeathTime, (Action<float>)delegate(float v)
		{
			_burrowedLarvaDeathTime = TimeSpan.FromMinutes(v);
		}, true);
		EntitySystemSubscriptionExt.CVar<float>(((EntitySystem)this).Subs, _config, RMCCVars.RMCLateJoinsBurrowedLarvaDeathTimeIgnoreBeforeMinutes, (Action<float>)delegate(float v)
		{
			_burrowedLarvaDeathIgnoreTime = TimeSpan.FromMinutes(v);
		}, true);
	}

	private void OnRoundRestartCleanup(RoundRestartCleanupEvent ev)
	{
		ClientBurrowedLarva = 0;
		SendLarvaStatus(null);
	}

	private void OnJoinXenoMapInit(Entity<JoinXenoComponent> ent, ref MapInitEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		_actions.AddAction(Entity<JoinXenoComponent>.op_Implicit(ent), ref ent.Comp.Action, EntProtoId.op_Implicit(ent.Comp.ActionId));
	}

	private void OnJoinXenoAction(Entity<JoinXenoComponent> ent, ref JoinXenoActionEvent args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		EntityUid user = args.Performer;
		if (!CanJoinXeno(user))
		{
			return;
		}
		List<DialogOption> options = new List<DialogOption>();
		EntityQueryEnumerator<HiveComponent> hives = ((EntitySystem)this).EntityQueryEnumerator<HiveComponent>();
		EntityUid hiveId = default(EntityUid);
		HiveComponent hive = default(HiveComponent);
		while (hives.MoveNext(ref hiveId, ref hive))
		{
			if (hive.BurrowedLarva > 0)
			{
				options.Add(new DialogOption("Burrowed Larva", new JoinXenoBurrowedLarvaEvent(((EntitySystem)this).GetNetEntity(hiveId, (MetaDataComponent)null))));
			}
		}
		_dialog.OpenOptions(Entity<JoinXenoComponent>.op_Implicit(ent), "Join as Xeno", options, "Available Xenonids");
	}

	public bool CanJoinXeno(EntityUid user)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		GhostComponent ghostComp = default(GhostComponent);
		if (!((EntitySystem)this).TryComp<GhostComponent>(user, ref ghostComp))
		{
			return false;
		}
		if (((EntitySystem)this).HasComp<JoinXenoCooldownIgnoreComponent>(user))
		{
			return true;
		}
		if (_gameTicker.RoundDuration() > _burrowedLarvaDeathIgnoreTime)
		{
			TimeSpan timeSinceDeath = _timing.CurTime.Subtract(ghostComp.TimeOfDeath);
			if (timeSinceDeath < _burrowedLarvaDeathTime)
			{
				string msg = base.Loc.GetString("rmc-xeno-ui-burrowed-need-time", (ValueTuple<string, object>)("seconds", _burrowedLarvaDeathTime.TotalSeconds - (double)(int)timeSinceDeath.TotalSeconds));
				_popup.PopupEntity(msg, user, user, PopupType.MediumCaution);
				return false;
			}
		}
		return true;
	}

	private void OnJoinXenoBurrowedLarva(Entity<JoinXenoComponent> ent, ref JoinXenoBurrowedLarvaEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? hive = default(EntityUid?);
		HiveComponent hiveComp = default(HiveComponent);
		ActorComponent actor = default(ActorComponent);
		if (((EntitySystem)this).TryGetEntity(args.Hive, ref hive) && ((EntitySystem)this).TryComp<HiveComponent>(hive, ref hiveComp) && ((EntitySystem)this).TryComp<ActorComponent>(Entity<JoinXenoComponent>.op_Implicit(ent), ref actor))
		{
			_hive.JoinBurrowedLarva(Entity<HiveComponent>.op_Implicit((hive.Value, hiveComp)), actor.PlayerSession);
		}
	}

	private void OnBurrowedLarvaStatus(BurrowedLarvaStatusEvent ev)
	{
		ClientBurrowedLarva = ev.Larva;
		if (!_net.IsServer)
		{
			BurrowedLarvaChangedEvent changedEv = new BurrowedLarvaChangedEvent(ev.Larva);
			((EntitySystem)this).RaiseLocalEvent<BurrowedLarvaChangedEvent>(ref changedEv);
		}
	}

	private void OnPlayerJoinedLobby(ref RMCPlayerJoinedLobbyEvent ev)
	{
		SendLarvaStatus(ev.Player);
	}

	private void OnBurrowedLarvaChanged(ref BurrowedLarvaChangedEvent ev)
	{
		SendLarvaStatus(null);
	}

	private void OnJoinBurrowedLarva(JoinBurrowedLarvaRequest msg, EntitySessionEventArgs args)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		if (!_rmcGameTicker.PlayerGameStatuses.TryGetValue(((EntitySessionEventArgs)(ref args)).SenderSession.UserId, out var status) || status == PlayerGameStatus.JoinedGame)
		{
			return;
		}
		EntityQueryEnumerator<CMDistressSignalRuleComponent> query = ((EntitySystem)this).EntityQueryEnumerator<CMDistressSignalRuleComponent>();
		CMDistressSignalRuleComponent comp = default(CMDistressSignalRuleComponent);
		HiveComponent hive = default(HiveComponent);
		while (query.MoveNext(ref comp))
		{
			if (((EntitySystem)this).TryComp<HiveComponent>(comp.Hive, ref hive) && _hive.JoinBurrowedLarva(Entity<HiveComponent>.op_Implicit((comp.Hive, hive)), ((EntitySessionEventArgs)(ref args)).SenderSession))
			{
				_rmcGameTicker.PlayerJoinGame(((EntitySessionEventArgs)(ref args)).SenderSession);
				break;
			}
		}
	}

	private void OnBurrowedLarvaStatusRequest(BurrowedLarvaStatusRequest msg, EntitySessionEventArgs args)
	{
		SendLarvaStatus(((EntitySessionEventArgs)(ref args)).SenderSession);
	}

	private void SendLarvaStatus(ICommonSession? to)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		EntityQueryEnumerator<ActiveGameRuleComponent, CMDistressSignalRuleComponent, GameRuleComponent> query = ((EntitySystem)this).EntityQueryEnumerator<ActiveGameRuleComponent, CMDistressSignalRuleComponent, GameRuleComponent>();
		ActiveGameRuleComponent activeGameRuleComponent = default(ActiveGameRuleComponent);
		CMDistressSignalRuleComponent comp = default(CMDistressSignalRuleComponent);
		GameRuleComponent gameRuleComponent = default(GameRuleComponent);
		HiveComponent hive = default(HiveComponent);
		while (query.MoveNext(ref activeGameRuleComponent, ref comp, ref gameRuleComponent))
		{
			if (((EntitySystem)this).TryComp<HiveComponent>(comp.Hive, ref hive))
			{
				BurrowedLarvaStatusEvent statusEv = new BurrowedLarvaStatusEvent(hive.BurrowedLarva);
				if (to != null)
				{
					((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)statusEv, to);
					break;
				}
				Filter filter = Filter.Empty().AddWhere((Predicate<ICommonSession>)((ICommonSession s) => _rmcGameTicker.PlayerGameStatuses.GetValueOrDefault(s.UserId) != PlayerGameStatus.JoinedGame), (ISharedPlayerManager)null);
				((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)statusEv, filter, true);
			}
		}
	}

	public void RequestBurrowedLarvaStatus()
	{
		if (!_net.IsServer)
		{
			BurrowedLarvaStatusRequest ev = new BurrowedLarvaStatusRequest();
			((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)ev);
		}
	}

	public void ClientJoinLarva()
	{
		if (!_net.IsServer)
		{
			JoinBurrowedLarvaRequest ev = new JoinBurrowedLarvaRequest();
			((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)ev);
		}
	}
}
