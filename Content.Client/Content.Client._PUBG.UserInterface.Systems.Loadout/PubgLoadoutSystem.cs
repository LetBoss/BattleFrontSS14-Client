using System;
using Content.Shared._PUBG.Loadout;
using Robust.Shared.GameObjects;

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

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeNetworkEvent<PubgLoadoutStateMessage>((EntityEventHandler<PubgLoadoutStateMessage>)OnStateMessage, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<PubgLoadoutActionResultMessage>((EntityEventHandler<PubgLoadoutActionResultMessage>)OnActionResultMessage, (Type[])null, (Type[])null);
	}

	public override void Update(float frameTime)
	{
		((EntitySystem)this).Update(frameTime);
		if (_pollingEnabled)
		{
			RequestState();
		}
	}

	public void SetPolling(bool enabled)
	{
		_pollingEnabled = enabled;
		if (enabled)
		{
			RequestState(force: true);
		}
	}

	public void RequestState(bool force = false)
	{
		DateTime utcNow = DateTime.UtcNow;
		if ((!_requestInFlight || !(utcNow - _lastRequestAt < RequestTimeout)) && (force || !(utcNow - _lastRequestAt < RequestInterval)))
		{
			_requestInFlight = true;
			_lastRequestAt = utcNow;
			((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new PubgLoadoutStateRequestMessage());
		}
	}

	public void RequestAction(PubgLoadoutActionType action, EntityUid item = default(EntityUid), PubgLoadoutSection targetSection = PubgLoadoutSection.Other, EntityUid weapon = default(EntityUid), string targetSlotId = "")
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new PubgLoadoutActionRequestMessage(action, (item == default(EntityUid)) ? NetEntity.Invalid : ((EntitySystem)this).GetNetEntity(item, (MetaDataComponent)null), targetSection, (weapon == default(EntityUid)) ? NetEntity.Invalid : ((EntitySystem)this).GetNetEntity(weapon, (MetaDataComponent)null), targetSlotId));
	}

	private void OnStateMessage(PubgLoadoutStateMessage msg)
	{
		_requestInFlight = false;
		this.OnStateReceived?.Invoke(msg);
	}

	private void OnActionResultMessage(PubgLoadoutActionResultMessage msg)
	{
		this.OnActionResultReceived?.Invoke(msg);
		RequestState(force: true);
	}
}
