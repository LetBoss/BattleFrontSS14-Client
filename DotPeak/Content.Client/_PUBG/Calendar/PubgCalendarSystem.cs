// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.Calendar.PubgCalendarSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._PUBG.Calendar;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client._PUBG.Calendar;

public sealed class PubgCalendarSystem : EntitySystem
{
  private static readonly TimeSpan StateRequestDebounce = TimeSpan.FromMilliseconds(500L);
  private static readonly TimeSpan StateRequestTimeout = TimeSpan.FromSeconds(8L);
  private static readonly TimeSpan ClaimRequestDebounce = TimeSpan.FromMilliseconds(500L);
  private static readonly TimeSpan ClaimRequestTimeout = TimeSpan.FromSeconds(8L);
  private DateTime _lastStateRequestAt = DateTime.MinValue;
  private bool _stateRequestInFlight;
  private DateTime _lastClaimRequestAt = DateTime.MinValue;
  private bool _claimRequestInFlight;

  public event Action<PubgCalendarStateMessage>? OnStateReceived;

  public event Action<PubgCalendarClaimResultMessage>? OnClaimResultReceived;

  public PubgCalendarStateMessage? LastState { get; private set; }

  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeNetworkEvent<PubgCalendarStateMessage>(new EntityEventHandler<PubgCalendarStateMessage>(this.OnState), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<PubgCalendarClaimResultMessage>(new EntityEventHandler<PubgCalendarClaimResultMessage>(this.OnClaimResult), (Type[]) null, (Type[]) null);
  }

  private void OnState(PubgCalendarStateMessage msg)
  {
    this._stateRequestInFlight = false;
    this.LastState = msg;
    Action<PubgCalendarStateMessage> onStateReceived = this.OnStateReceived;
    if (onStateReceived == null)
      return;
    onStateReceived(msg);
  }

  private void OnClaimResult(PubgCalendarClaimResultMessage msg)
  {
    this._claimRequestInFlight = false;
    Action<PubgCalendarClaimResultMessage> claimResultReceived = this.OnClaimResultReceived;
    if (claimResultReceived == null)
      return;
    claimResultReceived(msg);
  }

  public void RequestCalendarState(bool force = false)
  {
    DateTime utcNow = DateTime.UtcNow;
    if (this._stateRequestInFlight && utcNow - this._lastStateRequestAt < PubgCalendarSystem.StateRequestTimeout || !force && utcNow - this._lastStateRequestAt < PubgCalendarSystem.StateRequestDebounce)
      return;
    this._stateRequestInFlight = true;
    this._lastStateRequestAt = utcNow;
    this.RaisePredictiveEvent<PubgCalendarRequestMessage>(new PubgCalendarRequestMessage());
  }

  public void ClaimNextDay(bool force = false)
  {
    DateTime utcNow = DateTime.UtcNow;
    if (this._claimRequestInFlight && utcNow - this._lastClaimRequestAt < PubgCalendarSystem.ClaimRequestTimeout || !force && utcNow - this._lastClaimRequestAt < PubgCalendarSystem.ClaimRequestDebounce)
      return;
    this._claimRequestInFlight = true;
    this._lastClaimRequestAt = utcNow;
    this.RaisePredictiveEvent<PubgCalendarClaimMessage>(new PubgCalendarClaimMessage());
  }
}
