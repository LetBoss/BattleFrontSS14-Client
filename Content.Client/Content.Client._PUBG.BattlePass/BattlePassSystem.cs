using System;
using Content.Shared._PUBG.BattlePass;
using Robust.Shared.GameObjects;

namespace Content.Client._PUBG.BattlePass;

public sealed class BattlePassSystem : EntitySystem
{
	private static readonly TimeSpan RequestDebounce = TimeSpan.FromMilliseconds(500L);

	private static readonly TimeSpan RequestTimeout = TimeSpan.FromSeconds(8L);

	private DateTime _lastRequestAt = DateTime.MinValue;

	private bool _requestInFlight;

	public BattlePassStateMessage? LastState { get; private set; }

	public event Action<BattlePassStateMessage>? OnStateReceived;

	public event Action<BattlePassClaimResultMessage>? OnClaimResultReceived;

	public event Action<BattlePassSkipResultMessage>? OnSkipResultReceived;

	public event Action<BattlePassClaimTaskResultMessage>? OnClaimTaskResultReceived;

	public event Action<BattlePassBuyPremiumResultMessage>? OnBuyPremiumResultReceived;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeNetworkEvent<BattlePassStateMessage>((EntityEventHandler<BattlePassStateMessage>)OnState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<BattlePassClaimResultMessage>((EntityEventHandler<BattlePassClaimResultMessage>)OnClaimResult, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<BattlePassSkipResultMessage>((EntityEventHandler<BattlePassSkipResultMessage>)OnSkipResult, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<BattlePassClaimTaskResultMessage>((EntityEventHandler<BattlePassClaimTaskResultMessage>)OnClaimTaskResult, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<BattlePassBuyPremiumResultMessage>((EntityEventHandler<BattlePassBuyPremiumResultMessage>)OnBuyPremiumResult, (Type[])null, (Type[])null);
	}

	private void OnState(BattlePassStateMessage msg)
	{
		_requestInFlight = false;
		LastState = msg;
		this.OnStateReceived?.Invoke(msg);
	}

	private void OnClaimResult(BattlePassClaimResultMessage msg)
	{
		this.OnClaimResultReceived?.Invoke(msg);
	}

	private void OnSkipResult(BattlePassSkipResultMessage msg)
	{
		this.OnSkipResultReceived?.Invoke(msg);
	}

	private void OnClaimTaskResult(BattlePassClaimTaskResultMessage msg)
	{
		this.OnClaimTaskResultReceived?.Invoke(msg);
	}

	public void RequestBattlePass(bool force = false)
	{
		DateTime utcNow = DateTime.UtcNow;
		if ((!_requestInFlight || !(utcNow - _lastRequestAt < RequestTimeout)) && (force || !(utcNow - _lastRequestAt < RequestDebounce)))
		{
			_requestInFlight = true;
			_lastRequestAt = utcNow;
			((EntitySystem)this).RaisePredictiveEvent<BattlePassRequestMessage>(new BattlePassRequestMessage());
		}
	}

	public void ClaimReward(string rewardId)
	{
		((EntitySystem)this).RaisePredictiveEvent<BattlePassClaimMessage>(new BattlePassClaimMessage(rewardId));
	}

	public void SkipTask(string taskId)
	{
		((EntitySystem)this).RaisePredictiveEvent<BattlePassSkipTaskMessage>(new BattlePassSkipTaskMessage(taskId));
	}

	public void ClaimTaskXp(string taskId)
	{
		((EntitySystem)this).RaisePredictiveEvent<BattlePassClaimTaskMessage>(new BattlePassClaimTaskMessage(taskId));
	}

	private void OnBuyPremiumResult(BattlePassBuyPremiumResultMessage msg)
	{
		this.OnBuyPremiumResultReceived?.Invoke(msg);
	}

	public void BuyPremium()
	{
		((EntitySystem)this).RaisePredictiveEvent<BattlePassBuyPremiumMessage>(new BattlePassBuyPremiumMessage());
	}
}
