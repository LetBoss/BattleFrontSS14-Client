// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.JoinXeno.JoinXenoSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

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
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;

#nullable enable
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
    this.SubscribeLocalEvent<RoundRestartCleanupEvent>(new EntityEventHandler<RoundRestartCleanupEvent>(this.OnRoundRestartCleanup));
    this.SubscribeLocalEvent<JoinXenoComponent, MapInitEvent>(new EntityEventRefHandler<JoinXenoComponent, MapInitEvent>(this.OnJoinXenoMapInit));
    this.SubscribeLocalEvent<JoinXenoComponent, JoinXenoActionEvent>(new EntityEventRefHandler<JoinXenoComponent, JoinXenoActionEvent>(this.OnJoinXenoAction));
    this.SubscribeLocalEvent<JoinXenoComponent, JoinXenoBurrowedLarvaEvent>(new EntityEventRefHandler<JoinXenoComponent, JoinXenoBurrowedLarvaEvent>(this.OnJoinXenoBurrowedLarva));
    if (this._net.IsClient)
    {
      this.SubscribeNetworkEvent<BurrowedLarvaStatusEvent>(new EntityEventHandler<BurrowedLarvaStatusEvent>(this.OnBurrowedLarvaStatus));
    }
    else
    {
      this.SubscribeLocalEvent<RMCPlayerJoinedLobbyEvent>(new EntityEventRefHandler<RMCPlayerJoinedLobbyEvent>(this.OnPlayerJoinedLobby));
      this.SubscribeLocalEvent<BurrowedLarvaChangedEvent>(new EntityEventRefHandler<BurrowedLarvaChangedEvent>(this.OnBurrowedLarvaChanged));
      this.SubscribeNetworkEvent<JoinBurrowedLarvaRequest>(new EntitySessionEventHandler<JoinBurrowedLarvaRequest>(this.OnJoinBurrowedLarva));
      this.SubscribeNetworkEvent<BurrowedLarvaStatusRequest>(new EntitySessionEventHandler<BurrowedLarvaStatusRequest>(this.OnBurrowedLarvaStatusRequest));
    }
    this.Subs.CVar<float>(this._config, RMCCVars.RMCLateJoinsBurrowedLarvaDeathTime, (Action<float>) (v => this._burrowedLarvaDeathTime = TimeSpan.FromMinutes((double) v)), true);
    this.Subs.CVar<float>(this._config, RMCCVars.RMCLateJoinsBurrowedLarvaDeathTimeIgnoreBeforeMinutes, (Action<float>) (v => this._burrowedLarvaDeathIgnoreTime = TimeSpan.FromMinutes((double) v)), true);
  }

  private void OnRoundRestartCleanup(RoundRestartCleanupEvent ev)
  {
    this.ClientBurrowedLarva = 0;
    this.SendLarvaStatus((ICommonSession) null);
  }

  private void OnJoinXenoMapInit(Entity<JoinXenoComponent> ent, ref MapInitEvent args)
  {
    this._actions.AddAction((EntityUid) ent, ref ent.Comp.Action, (string) ent.Comp.ActionId);
  }

  private void OnJoinXenoAction(Entity<JoinXenoComponent> ent, ref JoinXenoActionEvent args)
  {
    if (this._net.IsClient || !this.CanJoinXeno(args.Performer))
      return;
    List<DialogOption> options = new List<DialogOption>();
    Robust.Shared.GameObjects.EntityQueryEnumerator<HiveComponent> entityQueryEnumerator = this.EntityQueryEnumerator<HiveComponent>();
    EntityUid uid;
    HiveComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      if (comp1.BurrowedLarva > 0)
        options.Add(new DialogOption("Burrowed Larva", (object) new JoinXenoBurrowedLarvaEvent(this.GetNetEntity(uid))));
    }
    this._dialog.OpenOptions((EntityUid) ent, "Join as Xeno", options, "Available Xenonids");
  }

  public bool CanJoinXeno(EntityUid user)
  {
    GhostComponent comp;
    if (!this.TryComp<GhostComponent>(user, out comp))
      return false;
    if (this.HasComp<JoinXenoCooldownIgnoreComponent>(user) || !(this._gameTicker.RoundDuration() > this._burrowedLarvaDeathIgnoreTime))
      return true;
    TimeSpan timeSpan = this._timing.CurTime.Subtract(comp.TimeOfDeath);
    if (!(timeSpan < this._burrowedLarvaDeathTime))
      return true;
    this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-ui-burrowed-need-time", ("seconds", (object) (this._burrowedLarvaDeathTime.TotalSeconds - (double) (int) timeSpan.TotalSeconds))), user, user, PopupType.MediumCaution);
    return false;
  }

  private void OnJoinXenoBurrowedLarva(
    Entity<JoinXenoComponent> ent,
    ref JoinXenoBurrowedLarvaEvent args)
  {
    EntityUid? entity;
    HiveComponent comp1;
    ActorComponent comp2;
    if (!this.TryGetEntity(args.Hive, out entity) || !this.TryComp<HiveComponent>(entity, out comp1) || !this.TryComp<ActorComponent>((EntityUid) ent, out comp2))
      return;
    this._hive.JoinBurrowedLarva((Entity<HiveComponent>) (entity.Value, comp1), comp2.PlayerSession);
  }

  private void OnBurrowedLarvaStatus(BurrowedLarvaStatusEvent ev)
  {
    this.ClientBurrowedLarva = ev.Larva;
    if (this._net.IsServer)
      return;
    BurrowedLarvaChangedEvent message = new BurrowedLarvaChangedEvent(ev.Larva);
    this.RaiseLocalEvent<BurrowedLarvaChangedEvent>(ref message);
  }

  private void OnPlayerJoinedLobby(ref RMCPlayerJoinedLobbyEvent ev)
  {
    this.SendLarvaStatus(ev.Player);
  }

  private void OnBurrowedLarvaChanged(ref BurrowedLarvaChangedEvent ev)
  {
    this.SendLarvaStatus((ICommonSession) null);
  }

  private void OnJoinBurrowedLarva(JoinBurrowedLarvaRequest msg, EntitySessionEventArgs args)
  {
    PlayerGameStatus playerGameStatus;
    if (!this._rmcGameTicker.PlayerGameStatuses.TryGetValue(args.SenderSession.UserId, out playerGameStatus) || playerGameStatus == PlayerGameStatus.JoinedGame)
      return;
    Robust.Shared.GameObjects.EntityQueryEnumerator<CMDistressSignalRuleComponent> entityQueryEnumerator = this.EntityQueryEnumerator<CMDistressSignalRuleComponent>();
    CMDistressSignalRuleComponent comp1;
    while (entityQueryEnumerator.MoveNext(out comp1))
    {
      HiveComponent comp;
      if (this.TryComp<HiveComponent>(comp1.Hive, out comp) && this._hive.JoinBurrowedLarva((Entity<HiveComponent>) (comp1.Hive, comp), args.SenderSession))
      {
        this._rmcGameTicker.PlayerJoinGame(args.SenderSession);
        break;
      }
    }
  }

  private void OnBurrowedLarvaStatusRequest(
    BurrowedLarvaStatusRequest msg,
    EntitySessionEventArgs args)
  {
    this.SendLarvaStatus(args.SenderSession);
  }

  private void SendLarvaStatus(ICommonSession? to)
  {
    if (this._net.IsClient)
      return;
    Robust.Shared.GameObjects.EntityQueryEnumerator<ActiveGameRuleComponent, CMDistressSignalRuleComponent, GameRuleComponent> entityQueryEnumerator = this.EntityQueryEnumerator<ActiveGameRuleComponent, CMDistressSignalRuleComponent, GameRuleComponent>();
    CMDistressSignalRuleComponent comp2;
    while (entityQueryEnumerator.MoveNext(out ActiveGameRuleComponent _, out comp2, out GameRuleComponent _))
    {
      HiveComponent comp;
      if (this.TryComp<HiveComponent>(comp2.Hive, out comp))
      {
        BurrowedLarvaStatusEvent message = new BurrowedLarvaStatusEvent(comp.BurrowedLarva);
        if (to != null)
        {
          this.RaiseNetworkEvent((EntityEventArgs) message, to);
          break;
        }
        Filter filter = Filter.Empty().AddWhere((Predicate<ICommonSession>) (s => this._rmcGameTicker.PlayerGameStatuses.GetValueOrDefault<NetUserId, PlayerGameStatus>(s.UserId) != PlayerGameStatus.JoinedGame));
        this.RaiseNetworkEvent((EntityEventArgs) message, filter);
      }
    }
  }

  public void RequestBurrowedLarvaStatus()
  {
    if (this._net.IsServer)
      return;
    this.RaiseNetworkEvent((EntityEventArgs) new BurrowedLarvaStatusRequest());
  }

  public void ClientJoinLarva()
  {
    if (this._net.IsServer)
      return;
    this.RaiseNetworkEvent((EntityEventArgs) new JoinBurrowedLarvaRequest());
  }
}
