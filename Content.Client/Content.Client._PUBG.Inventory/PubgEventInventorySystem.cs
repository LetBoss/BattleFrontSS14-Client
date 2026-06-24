using System;
using Content.Shared._PUBG.Events;
using Robust.Shared.GameObjects;

namespace Content.Client._PUBG.Inventory;

public sealed class PubgEventInventorySystem : EntitySystem
{
	private static readonly TimeSpan RequestDebounce = TimeSpan.FromMilliseconds(500L);

	private static readonly TimeSpan RequestTimeout = TimeSpan.FromSeconds(8L);

	private DateTime _lastRequestAt = DateTime.MinValue;

	private bool _requestInFlight;

	public PubgEventInventoryStateMessage? LastInventoryState { get; private set; }

	public event Action<PubgEventInventoryStateMessage>? OnInventoryStateReceived;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeNetworkEvent<PubgEventInventoryStateMessage>((EntityEventHandler<PubgEventInventoryStateMessage>)OnInventoryState, (Type[])null, (Type[])null);
	}

	private void OnInventoryState(PubgEventInventoryStateMessage msg)
	{
		_requestInFlight = false;
		LastInventoryState = msg;
		this.OnInventoryStateReceived?.Invoke(msg);
	}

	public void RequestInventory(bool force = false)
	{
		DateTime utcNow = DateTime.UtcNow;
		if ((!_requestInFlight || !(utcNow - _lastRequestAt < RequestTimeout)) && (force || !(utcNow - _lastRequestAt < RequestDebounce)))
		{
			_requestInFlight = true;
			_lastRequestAt = utcNow;
			((EntitySystem)this).RaisePredictiveEvent<PubgEventInventoryRequestMessage>(new PubgEventInventoryRequestMessage());
		}
	}
}
