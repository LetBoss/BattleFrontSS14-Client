// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.Connection.QueueSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._CIV14merka.ModeMenu;
using Content.Client._PUBG.Lobby;
using Content.Shared._PUBG.Connection;
using Robust.Client.State;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client._PUBG.Connection;

public sealed class QueueSystem : EntitySystem
{
  [Dependency]
  private IStateManager _stateManager;
  private bool _isInQueue;
  private int _position;
  private int _total;

  public bool IsInQueue => this._isInQueue;

  public int Position => this._position;

  public int Total => this._total;

  public event Action? OnQueueUpdated;

  public event Action? OnQueueAccepted;

  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeNetworkEvent<QueuePositionMessage>(new EntityEventHandler<QueuePositionMessage>(this.OnQueuePosition), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<QueueAcceptedMessage>(new EntityEventHandler<QueueAcceptedMessage>(this.OnAccepted), (Type[]) null, (Type[]) null);
  }

  private void OnQueuePosition(QueuePositionMessage msg)
  {
    this._isInQueue = true;
    this._position = msg.Position;
    this._total = msg.TotalInQueue;
    if (!(this._stateManager.CurrentState is QueueState))
      this._stateManager.RequestStateChange<QueueState>();
    if (this._stateManager.CurrentState is QueueState currentState)
      currentState.UpdatePosition(this._position, this._total);
    Action onQueueUpdated = this.OnQueueUpdated;
    if (onQueueUpdated == null)
      return;
    onQueueUpdated();
  }

  private void OnAccepted(QueueAcceptedMessage msg)
  {
    this._isInQueue = false;
    this._position = 0;
    this._total = 0;
    if (this._stateManager.CurrentState is QueueState)
    {
      if (msg.OpenModeMenu)
        this._stateManager.RequestStateChange<ModeSelectState>();
      else
        this._stateManager.RequestStateChange<PubgPreLobbyHubState>();
    }
    Action onQueueAccepted = this.OnQueueAccepted;
    if (onQueueAccepted == null)
      return;
    onQueueAccepted();
  }

  public void RequestPermissionsRecheck()
  {
    this.RaiseNetworkEvent((EntityEventArgs) new QueueRecheckPermissionsMessage());
  }
}
