// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.Events.PubgEventsSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._PUBG.Events;
using Robust.Shared.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Client._PUBG.Events;

public sealed class PubgEventsSystem : EntitySystem
{
  private static readonly TimeSpan HubRequestDebounce = TimeSpan.FromMilliseconds(500L);
  private static readonly TimeSpan HubRequestTimeout = TimeSpan.FromSeconds(20L);
  private static readonly TimeSpan EventRequestDebounce = TimeSpan.FromMilliseconds(500L);
  private static readonly TimeSpan EventRequestTimeout = TimeSpan.FromSeconds(15L);
  private static readonly TimeSpan ClaimRequestDebounce = TimeSpan.FromMilliseconds(500L);
  private static readonly TimeSpan ClaimRequestTimeout = TimeSpan.FromSeconds(8L);
  private DateTime _lastHubRequestAt = DateTime.MinValue;
  private bool _hubRequestInFlight;
  private readonly Dictionary<string, DateTime> _lastEventRequestAt = new Dictionary<string, DateTime>();
  private readonly HashSet<string> _eventRequestsInFlight = new HashSet<string>();
  private DateTime _lastClaimRequestAt = DateTime.MinValue;
  private bool _claimRequestInFlight;

  public event Action<PubgEventsHubStateMessage>? OnHubStateReceived;

  public event Action<PubgEventStateMessage>? OnEventStateReceived;

  public event Action<PubgEventClaimResultMessage>? OnClaimResultReceived;

  public PubgEventsHubStateMessage? LastHubState { get; private set; }

  public Dictionary<string, PubgEventDetailInfo> LastEventStates { get; } = new Dictionary<string, PubgEventDetailInfo>();

  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeNetworkEvent<PubgEventsHubStateMessage>(new EntityEventHandler<PubgEventsHubStateMessage>(this.OnHubState), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<PubgEventStateMessage>(new EntityEventHandler<PubgEventStateMessage>(this.OnEventState), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<PubgEventClaimResultMessage>(new EntityEventHandler<PubgEventClaimResultMessage>(this.OnClaimResult), (Type[]) null, (Type[]) null);
  }

  private void OnHubState(PubgEventsHubStateMessage msg)
  {
    this._hubRequestInFlight = false;
    this.LastHubState = msg;
    Action<PubgEventsHubStateMessage> hubStateReceived = this.OnHubStateReceived;
    if (hubStateReceived == null)
      return;
    hubStateReceived(msg);
  }

  private void OnEventState(PubgEventStateMessage msg)
  {
    this._eventRequestsInFlight.Remove(msg.State.EventKey);
    this.LastEventStates[msg.State.EventKey] = msg.State;
    Action<PubgEventStateMessage> eventStateReceived = this.OnEventStateReceived;
    if (eventStateReceived == null)
      return;
    eventStateReceived(msg);
  }

  private void OnClaimResult(PubgEventClaimResultMessage msg)
  {
    this._claimRequestInFlight = false;
    Action<PubgEventClaimResultMessage> claimResultReceived = this.OnClaimResultReceived;
    if (claimResultReceived == null)
      return;
    claimResultReceived(msg);
  }

  public void RequestHub(bool force = false)
  {
    DateTime utcNow = DateTime.UtcNow;
    if (this._hubRequestInFlight && utcNow - this._lastHubRequestAt < PubgEventsSystem.HubRequestTimeout || !force && utcNow - this._lastHubRequestAt < PubgEventsSystem.HubRequestDebounce)
      return;
    this._hubRequestInFlight = true;
    this._lastHubRequestAt = utcNow;
    this.RaisePredictiveEvent<PubgEventsHubRequestMessage>(new PubgEventsHubRequestMessage());
  }

  public void RequestEventState(string eventKey, bool force = false)
  {
    if (string.IsNullOrWhiteSpace(eventKey))
      return;
    DateTime utcNow = DateTime.UtcNow;
    DateTime dateTime1;
    DateTime dateTime2;
    if (this._eventRequestsInFlight.Contains(eventKey) && this._lastEventRequestAt.TryGetValue(eventKey, out dateTime1) && utcNow - dateTime1 < PubgEventsSystem.EventRequestTimeout || !force && this._lastEventRequestAt.TryGetValue(eventKey, out dateTime2) && utcNow - dateTime2 < PubgEventsSystem.EventRequestDebounce)
      return;
    this._lastEventRequestAt[eventKey] = utcNow;
    this._eventRequestsInFlight.Add(eventKey);
    this.RaisePredictiveEvent<PubgEventStateRequestMessage>(new PubgEventStateRequestMessage(eventKey));
  }

  public bool TryClaim(string eventKey, string claimType, string claimId, bool force = false)
  {
    if (string.IsNullOrWhiteSpace(eventKey) || string.IsNullOrWhiteSpace(claimType) || string.IsNullOrWhiteSpace(claimId))
      return false;
    DateTime utcNow = DateTime.UtcNow;
    if (this._claimRequestInFlight && utcNow - this._lastClaimRequestAt < PubgEventsSystem.ClaimRequestTimeout || !force && utcNow - this._lastClaimRequestAt < PubgEventsSystem.ClaimRequestDebounce)
      return false;
    this._claimRequestInFlight = true;
    this._lastClaimRequestAt = utcNow;
    this.RaisePredictiveEvent<PubgEventClaimMessage>(new PubgEventClaimMessage(eventKey, claimType, claimId));
    return true;
  }

  public void Claim(string eventKey, string claimType, string claimId, bool force = false)
  {
    this.TryClaim(eventKey, claimType, claimId, force);
  }

  public bool HasClaimableTasksInCache()
  {
    PubgEventsHubStateMessage lastHubState = this.LastHubState;
    if ((lastHubState != null ? (lastHubState.HubHasClaimable ? 1 : 0) : 0) != 0)
      return true;
    foreach (PubgEventDetailInfo pubgEventDetailInfo in this.LastEventStates.Values)
    {
      if (pubgEventDetailInfo.HasClaimable || pubgEventDetailInfo.RedDotTasks || pubgEventDetailInfo.RedDotMilestones || pubgEventDetailInfo.MarsState != null && (pubgEventDetailInfo.MarsState.LoginTasks.Any<PubgMarsTaskInfo>((Func<PubgMarsTaskInfo, bool>) (task => task.IsClaimable && !task.IsClaimed)) || pubgEventDetailInfo.MarsState.ChallengeTasks.Any<PubgMarsTaskInfo>((Func<PubgMarsTaskInfo, bool>) (task => task.IsClaimable && !task.IsClaimed)) || pubgEventDetailInfo.MarsState.Milestones.Any<PubgMarsMilestoneInfo>((Func<PubgMarsMilestoneInfo, bool>) (milestone => milestone.IsClaimable && !milestone.IsClaimed))))
        return true;
    }
    return false;
  }
}
