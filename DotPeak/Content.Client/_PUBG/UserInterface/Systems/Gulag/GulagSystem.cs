// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.UserInterface.Systems.Gulag.GulagSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._PUBG.Gulag;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Player;
using System;

#nullable enable
namespace Content.Client._PUBG.UserInterface.Systems.Gulag;

public sealed class GulagSystem : EntitySystem
{
  [Dependency]
  private IPlayerManager _player;
  private MapId? _gulagMapId;
  private bool _localOnGulagMap;

  public event Action<GulagStatusEvent, EntitySessionEventArgs>? OnGulagStatusReceived;

  public event Action<GulagQueueUpdateEvent, EntitySessionEventArgs>? OnQueueUpdateReceived;

  public event Action<GulagFightStartEvent, EntitySessionEventArgs>? OnFightStartReceived;

  public event Action<GulagFightUpdateEvent, EntitySessionEventArgs>? OnFightUpdateReceived;

  public event Action<GulagSpectatorUpdateEvent, EntitySessionEventArgs>? OnSpectatorUpdateReceived;

  public event Action<GulagAdminOfferEvent, EntitySessionEventArgs>? OnAdminOfferReceived;

  public event Action<GulagMapInfoEvent, EntitySessionEventArgs>? OnMapInfoReceived;

  public event Action<bool>? OnLocalGulagMapChanged;

  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeNetworkEvent<GulagStatusEvent>(new EntitySessionEventHandler<GulagStatusEvent>(this.OnGulagStatus), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<GulagQueueUpdateEvent>(new EntitySessionEventHandler<GulagQueueUpdateEvent>(this.OnQueueUpdate), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<GulagFightStartEvent>(new EntitySessionEventHandler<GulagFightStartEvent>(this.OnFightStart), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<GulagFightUpdateEvent>(new EntitySessionEventHandler<GulagFightUpdateEvent>(this.OnFightUpdate), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<GulagSpectatorUpdateEvent>(new EntitySessionEventHandler<GulagSpectatorUpdateEvent>(this.OnSpectatorUpdate), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<GulagAdminOfferEvent>(new EntitySessionEventHandler<GulagAdminOfferEvent>(this.OnAdminOffer), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<GulagMapInfoEvent>(new EntitySessionEventHandler<GulagMapInfoEvent>(this.OnMapInfo), (Type[]) null, (Type[]) null);
  }

  public virtual void Update(float frameTime)
  {
    base.Update(frameTime);
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    bool flag = false;
    if (localEntity.HasValue && this._gulagMapId.HasValue)
      flag = MapId.op_Equality(this.Transform(localEntity.Value).MapID, this._gulagMapId.Value);
    if (flag == this._localOnGulagMap)
      return;
    this._localOnGulagMap = flag;
    Action<bool> localGulagMapChanged = this.OnLocalGulagMapChanged;
    if (localGulagMapChanged == null)
      return;
    localGulagMapChanged(flag);
  }

  private void OnGulagStatus(GulagStatusEvent msg, EntitySessionEventArgs args)
  {
    Action<GulagStatusEvent, EntitySessionEventArgs> gulagStatusReceived = this.OnGulagStatusReceived;
    if (gulagStatusReceived == null)
      return;
    gulagStatusReceived(msg, args);
  }

  private void OnQueueUpdate(GulagQueueUpdateEvent msg, EntitySessionEventArgs args)
  {
    Action<GulagQueueUpdateEvent, EntitySessionEventArgs> queueUpdateReceived = this.OnQueueUpdateReceived;
    if (queueUpdateReceived == null)
      return;
    queueUpdateReceived(msg, args);
  }

  private void OnFightStart(GulagFightStartEvent msg, EntitySessionEventArgs args)
  {
    Action<GulagFightStartEvent, EntitySessionEventArgs> fightStartReceived = this.OnFightStartReceived;
    if (fightStartReceived == null)
      return;
    fightStartReceived(msg, args);
  }

  private void OnFightUpdate(GulagFightUpdateEvent msg, EntitySessionEventArgs args)
  {
    Action<GulagFightUpdateEvent, EntitySessionEventArgs> fightUpdateReceived = this.OnFightUpdateReceived;
    if (fightUpdateReceived == null)
      return;
    fightUpdateReceived(msg, args);
  }

  private void OnSpectatorUpdate(GulagSpectatorUpdateEvent msg, EntitySessionEventArgs args)
  {
    Action<GulagSpectatorUpdateEvent, EntitySessionEventArgs> spectatorUpdateReceived = this.OnSpectatorUpdateReceived;
    if (spectatorUpdateReceived == null)
      return;
    spectatorUpdateReceived(msg, args);
  }

  private void OnAdminOffer(GulagAdminOfferEvent msg, EntitySessionEventArgs args)
  {
    Action<GulagAdminOfferEvent, EntitySessionEventArgs> adminOfferReceived = this.OnAdminOfferReceived;
    if (adminOfferReceived == null)
      return;
    adminOfferReceived(msg, args);
  }

  private void OnMapInfo(GulagMapInfoEvent msg, EntitySessionEventArgs args)
  {
    this._gulagMapId = msg.GulagMapId;
    Action<GulagMapInfoEvent, EntitySessionEventArgs> onMapInfoReceived = this.OnMapInfoReceived;
    if (onMapInfoReceived == null)
      return;
    onMapInfoReceived(msg, args);
  }
}
