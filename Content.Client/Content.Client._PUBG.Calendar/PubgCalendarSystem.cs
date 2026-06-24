using System;
using Content.Shared._PUBG.Calendar;
using Robust.Shared.GameObjects;

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

	public PubgCalendarStateMessage? LastState { get; private set; }

	public event Action<PubgCalendarStateMessage>? OnStateReceived;

	public event Action<PubgCalendarClaimResultMessage>? OnClaimResultReceived;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeNetworkEvent<PubgCalendarStateMessage>((EntityEventHandler<PubgCalendarStateMessage>)OnState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<PubgCalendarClaimResultMessage>((EntityEventHandler<PubgCalendarClaimResultMessage>)OnClaimResult, (Type[])null, (Type[])null);
	}

	private void OnState(PubgCalendarStateMessage msg)
	{
		_stateRequestInFlight = false;
		LastState = msg;
		this.OnStateReceived?.Invoke(msg);
	}

	private void OnClaimResult(PubgCalendarClaimResultMessage msg)
	{
		_claimRequestInFlight = false;
		this.OnClaimResultReceived?.Invoke(msg);
	}

	public void RequestCalendarState(bool force = false)
	{
		DateTime utcNow = DateTime.UtcNow;
		if ((!_stateRequestInFlight || !(utcNow - _lastStateRequestAt < StateRequestTimeout)) && (force || !(utcNow - _lastStateRequestAt < StateRequestDebounce)))
		{
			_stateRequestInFlight = true;
			_lastStateRequestAt = utcNow;
			((EntitySystem)this).RaisePredictiveEvent<PubgCalendarRequestMessage>(new PubgCalendarRequestMessage());
		}
	}

	public void ClaimNextDay(bool force = false)
	{
		DateTime utcNow = DateTime.UtcNow;
		if ((!_claimRequestInFlight || !(utcNow - _lastClaimRequestAt < ClaimRequestTimeout)) && (force || !(utcNow - _lastClaimRequestAt < ClaimRequestDebounce)))
		{
			_claimRequestInFlight = true;
			_lastClaimRequestAt = utcNow;
			((EntitySystem)this).RaisePredictiveEvent<PubgCalendarClaimMessage>(new PubgCalendarClaimMessage());
		}
	}
}
