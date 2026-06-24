// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.UserInterface.Systems.Loadout.PubgLoadoutSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._PUBG.Loadout;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client._PUBG.UserInterface.Systems.Loadout;

public sealed class PubgLoadoutSystem : EntitySystem
{
  private static readonly TimeSpan RequestInterval = TimeSpan.FromMilliseconds(250L);
  private static readonly TimeSpan RequestTimeout = TimeSpan.FromSeconds(2L);
  private DateTime _lastRequestAt = DateTime.MinValue;
  private bool _requestInFlight;
  private bool _pollingEnabled;

  public event Action<PubgLoadoutStateMessage>? OnStateReceived;

  public event Action<PubgLoadoutActionResultMessage>? OnActionResultReceived;

  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeNetworkEvent<PubgLoadoutStateMessage>(new EntityEventHandler<PubgLoadoutStateMessage>(this.OnStateMessage), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<PubgLoadoutActionResultMessage>(new EntityEventHandler<PubgLoadoutActionResultMessage>(this.OnActionResultMessage), (Type[]) null, (Type[]) null);
  }

  public virtual void Update(float frameTime)
  {
    base.Update(frameTime);
    if (!this._pollingEnabled)
      return;
    this.RequestState();
  }

  public void SetPolling(bool enabled)
  {
    this._pollingEnabled = enabled;
    if (!enabled)
      return;
    this.RequestState(true);
  }

  public void RequestState(bool force = false)
  {
    DateTime utcNow = DateTime.UtcNow;
    if (this._requestInFlight && utcNow - this._lastRequestAt < PubgLoadoutSystem.RequestTimeout || !force && utcNow - this._lastRequestAt < PubgLoadoutSystem.RequestInterval)
      return;
    this._requestInFlight = true;
    this._lastRequestAt = utcNow;
    this.RaiseNetworkEvent((EntityEventArgs) new PubgLoadoutStateRequestMessage());
  }

  public void RequestAction(
    PubgLoadoutActionType action,
    EntityUid item = default (EntityUid),
    PubgLoadoutSection targetSection = PubgLoadoutSection.Other,
    EntityUid weapon = default (EntityUid),
    string targetSlotId = "")
  {
    this.RaiseNetworkEvent((EntityEventArgs) new PubgLoadoutActionRequestMessage(action, EntityUid.op_Equality(item, new EntityUid()) ? NetEntity.Invalid : this.GetNetEntity(item, (MetaDataComponent) null), targetSection, EntityUid.op_Equality(weapon, new EntityUid()) ? NetEntity.Invalid : this.GetNetEntity(weapon, (MetaDataComponent) null), targetSlotId));
  }

  private void OnStateMessage(PubgLoadoutStateMessage msg)
  {
    this._requestInFlight = false;
    Action<PubgLoadoutStateMessage> onStateReceived = this.OnStateReceived;
    if (onStateReceived == null)
      return;
    onStateReceived(msg);
  }

  private void OnActionResultMessage(PubgLoadoutActionResultMessage msg)
  {
    Action<PubgLoadoutActionResultMessage> actionResultReceived = this.OnActionResultReceived;
    if (actionResultReceived != null)
      actionResultReceived(msg);
    this.RequestState(true);
  }
}
