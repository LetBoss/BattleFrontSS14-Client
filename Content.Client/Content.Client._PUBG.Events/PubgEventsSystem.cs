using System;
using System.Collections.Generic;
using System.Linq;
using Content.Shared._PUBG.Events;
using Robust.Shared.GameObjects;

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

	public PubgEventsHubStateMessage? LastHubState { get; private set; }

	public Dictionary<string, PubgEventDetailInfo> LastEventStates { get; } = new Dictionary<string, PubgEventDetailInfo>();

	public event Action<PubgEventsHubStateMessage>? OnHubStateReceived;

	public event Action<PubgEventStateMessage>? OnEventStateReceived;

	public event Action<PubgEventClaimResultMessage>? OnClaimResultReceived;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeNetworkEvent<PubgEventsHubStateMessage>((EntityEventHandler<PubgEventsHubStateMessage>)OnHubState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<PubgEventStateMessage>((EntityEventHandler<PubgEventStateMessage>)OnEventState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<PubgEventClaimResultMessage>((EntityEventHandler<PubgEventClaimResultMessage>)OnClaimResult, (Type[])null, (Type[])null);
	}

	private void OnHubState(PubgEventsHubStateMessage msg)
	{
		_hubRequestInFlight = false;
		LastHubState = msg;
		this.OnHubStateReceived?.Invoke(msg);
	}

	private void OnEventState(PubgEventStateMessage msg)
	{
		_eventRequestsInFlight.Remove(msg.State.EventKey);
		LastEventStates[msg.State.EventKey] = msg.State;
		this.OnEventStateReceived?.Invoke(msg);
	}

	private void OnClaimResult(PubgEventClaimResultMessage msg)
	{
		_claimRequestInFlight = false;
		this.OnClaimResultReceived?.Invoke(msg);
	}

	public void RequestHub(bool force = false)
	{
		DateTime utcNow = DateTime.UtcNow;
		if ((!_hubRequestInFlight || !(utcNow - _lastHubRequestAt < HubRequestTimeout)) && (force || !(utcNow - _lastHubRequestAt < HubRequestDebounce)))
		{
			_hubRequestInFlight = true;
			_lastHubRequestAt = utcNow;
			((EntitySystem)this).RaisePredictiveEvent<PubgEventsHubRequestMessage>(new PubgEventsHubRequestMessage());
		}
	}

	public void RequestEventState(string eventKey, bool force = false)
	{
		if (!string.IsNullOrWhiteSpace(eventKey))
		{
			DateTime utcNow = DateTime.UtcNow;
			if ((!_eventRequestsInFlight.Contains(eventKey) || !_lastEventRequestAt.TryGetValue(eventKey, out var value) || !(utcNow - value < EventRequestTimeout)) && (force || !_lastEventRequestAt.TryGetValue(eventKey, out var value2) || !(utcNow - value2 < EventRequestDebounce)))
			{
				_lastEventRequestAt[eventKey] = utcNow;
				_eventRequestsInFlight.Add(eventKey);
				((EntitySystem)this).RaisePredictiveEvent<PubgEventStateRequestMessage>(new PubgEventStateRequestMessage(eventKey));
			}
		}
	}

	public bool TryClaim(string eventKey, string claimType, string claimId, bool force = false)
	{
		if (string.IsNullOrWhiteSpace(eventKey) || string.IsNullOrWhiteSpace(claimType) || string.IsNullOrWhiteSpace(claimId))
		{
			return false;
		}
		DateTime utcNow = DateTime.UtcNow;
		if (_claimRequestInFlight && utcNow - _lastClaimRequestAt < ClaimRequestTimeout)
		{
			return false;
		}
		if (!force && utcNow - _lastClaimRequestAt < ClaimRequestDebounce)
		{
			return false;
		}
		_claimRequestInFlight = true;
		_lastClaimRequestAt = utcNow;
		((EntitySystem)this).RaisePredictiveEvent<PubgEventClaimMessage>(new PubgEventClaimMessage(eventKey, claimType, claimId));
		return true;
	}

	public void Claim(string eventKey, string claimType, string claimId, bool force = false)
	{
		TryClaim(eventKey, claimType, claimId, force);
	}

	public bool HasClaimableTasksInCache()
	{
		PubgEventsHubStateMessage? lastHubState = LastHubState;
		if (lastHubState != null && lastHubState.HubHasClaimable)
		{
			return true;
		}
		foreach (PubgEventDetailInfo value in LastEventStates.Values)
		{
			if (value.HasClaimable || value.RedDotTasks || value.RedDotMilestones)
			{
				return true;
			}
			if (value.MarsState != null)
			{
				if (value.MarsState.LoginTasks.Any((PubgMarsTaskInfo task) => task.IsClaimable && !task.IsClaimed))
				{
					return true;
				}
				if (value.MarsState.ChallengeTasks.Any((PubgMarsTaskInfo task) => task.IsClaimable && !task.IsClaimed))
				{
					return true;
				}
				if (value.MarsState.Milestones.Any((PubgMarsMilestoneInfo milestone) => milestone.IsClaimable && !milestone.IsClaimed))
				{
					return true;
				}
			}
		}
		return false;
	}
}
