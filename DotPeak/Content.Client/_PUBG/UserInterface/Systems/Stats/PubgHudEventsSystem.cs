// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.UserInterface.Systems.Stats.PubgHudEventsSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._PUBG;
using Content.Shared._PUBG.Lobby;
using Robust.Shared.GameObjects;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client._PUBG.UserInterface.Systems.Stats;

public sealed class PubgHudEventsSystem : EntitySystem
{
  public event Action<PubgModeStatusEvent>? OnPubgModeStatusReceived;

  public event Action<PubgKillsChangedEvent>? OnKillsChangedReceived;

  public event Action<PubgWarmupStatusEvent>? OnWarmupStatusReceived;

  public event Action<LobbyStatusEvent>? OnLobbyStatusReceived;

  public event Action<PubgZoneStateEvent>? OnZoneStateReceived;

  public event Action<PubgGameEndEvent>? OnGameEndReceived;

  public PubgModeStatusEvent? LastPubgModeStatus { get; private set; }

  public PubgKillsChangedEvent? LastKillsChanged { get; private set; }

  public PubgWarmupStatusEvent? LastWarmupStatus { get; private set; }

  public LobbyStatusEvent? LastLobbyStatus { get; private set; }

  public PubgZoneStateEvent? LastZoneState { get; private set; }

  public PubgGameEndEvent? LastGameEnd { get; private set; }

  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeNetworkEvent<PubgModeStatusEvent>(new EntitySessionEventHandler<PubgModeStatusEvent>(this.OnPubgModeStatus), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<PubgKillsChangedEvent>(new EntitySessionEventHandler<PubgKillsChangedEvent>(this.OnKillsChanged), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<PubgWarmupStatusEvent>(new EntitySessionEventHandler<PubgWarmupStatusEvent>(this.OnWarmupStatus), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<LobbyStatusEvent>(new EntitySessionEventHandler<LobbyStatusEvent>(this.OnLobbyStatus), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<PubgZoneStateEvent>(new EntitySessionEventHandler<PubgZoneStateEvent>(this.OnZoneState), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<PubgGameEndEvent>(new EntitySessionEventHandler<PubgGameEndEvent>(this.OnGameEnd), (Type[]) null, (Type[]) null);
  }

  private void OnPubgModeStatus(PubgModeStatusEvent msg, EntitySessionEventArgs args)
  {
    this.LastPubgModeStatus = msg;
    if (!msg.Enabled)
    {
      this.LastKillsChanged = new PubgKillsChangedEvent(0);
      this.LastWarmupStatus = new PubgWarmupStatusEvent(false, 0, 0);
      this.LastLobbyStatus = new LobbyStatusEvent(false, 0, 0);
      this.LastZoneState = new PubgZoneStateEvent(Vector2.Zero, 0.0f, Vector2.Zero, 0.0f, 0, ZoneState.Waiting, 0.0f, false, false, NetEntity.Invalid);
      this.LastGameEnd = (PubgGameEndEvent) null;
    }
    Action<PubgModeStatusEvent> modeStatusReceived = this.OnPubgModeStatusReceived;
    if (modeStatusReceived == null)
      return;
    modeStatusReceived(msg);
  }

  private void OnKillsChanged(PubgKillsChangedEvent msg, EntitySessionEventArgs args)
  {
    this.LastKillsChanged = msg;
    Action<PubgKillsChangedEvent> killsChangedReceived = this.OnKillsChangedReceived;
    if (killsChangedReceived == null)
      return;
    killsChangedReceived(msg);
  }

  private void OnWarmupStatus(PubgWarmupStatusEvent msg, EntitySessionEventArgs args)
  {
    this.LastWarmupStatus = msg;
    Action<PubgWarmupStatusEvent> warmupStatusReceived = this.OnWarmupStatusReceived;
    if (warmupStatusReceived == null)
      return;
    warmupStatusReceived(msg);
  }

  private void OnLobbyStatus(LobbyStatusEvent msg, EntitySessionEventArgs args)
  {
    this.LastLobbyStatus = msg;
    Action<LobbyStatusEvent> lobbyStatusReceived = this.OnLobbyStatusReceived;
    if (lobbyStatusReceived == null)
      return;
    lobbyStatusReceived(msg);
  }

  private void OnZoneState(PubgZoneStateEvent msg, EntitySessionEventArgs args)
  {
    this.LastZoneState = msg;
    Action<PubgZoneStateEvent> zoneStateReceived = this.OnZoneStateReceived;
    if (zoneStateReceived == null)
      return;
    zoneStateReceived(msg);
  }

  private void OnGameEnd(PubgGameEndEvent msg, EntitySessionEventArgs args)
  {
    this.LastGameEnd = msg;
    Action<PubgGameEndEvent> onGameEndReceived = this.OnGameEndReceived;
    if (onGameEndReceived == null)
      return;
    onGameEndReceived(msg);
  }
}
