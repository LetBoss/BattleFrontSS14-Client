// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.Inventory.PubgEventInventorySystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._PUBG.Events;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client._PUBG.Inventory;

public sealed class PubgEventInventorySystem : EntitySystem
{
  private static readonly TimeSpan RequestDebounce = TimeSpan.FromMilliseconds(500L);
  private static readonly TimeSpan RequestTimeout = TimeSpan.FromSeconds(8L);
  private DateTime _lastRequestAt = DateTime.MinValue;
  private bool _requestInFlight;

  public event Action<PubgEventInventoryStateMessage>? OnInventoryStateReceived;

  public PubgEventInventoryStateMessage? LastInventoryState { get; private set; }

  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeNetworkEvent<PubgEventInventoryStateMessage>(new EntityEventHandler<PubgEventInventoryStateMessage>(this.OnInventoryState), (Type[]) null, (Type[]) null);
  }

  private void OnInventoryState(PubgEventInventoryStateMessage msg)
  {
    this._requestInFlight = false;
    this.LastInventoryState = msg;
    Action<PubgEventInventoryStateMessage> inventoryStateReceived = this.OnInventoryStateReceived;
    if (inventoryStateReceived == null)
      return;
    inventoryStateReceived(msg);
  }

  public void RequestInventory(bool force = false)
  {
    DateTime utcNow = DateTime.UtcNow;
    if (this._requestInFlight && utcNow - this._lastRequestAt < PubgEventInventorySystem.RequestTimeout || !force && utcNow - this._lastRequestAt < PubgEventInventorySystem.RequestDebounce)
      return;
    this._requestInFlight = true;
    this._lastRequestAt = utcNow;
    this.RaisePredictiveEvent<PubgEventInventoryRequestMessage>(new PubgEventInventoryRequestMessage());
  }
}
