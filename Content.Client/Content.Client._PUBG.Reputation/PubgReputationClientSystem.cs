using System;
using Content.Shared._PUBG.Reputation;
using Robust.Shared.GameObjects;

namespace Content.Client._PUBG.Reputation;

public sealed class PubgReputationClientSystem : EntitySystem
{
	public int Reputation { get; private set; } = 100;

	public event Action<int>? OnReputationUpdated;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeNetworkEvent<PubgReputationStateMessage>((EntityEventHandler<PubgReputationStateMessage>)OnReputationState, (Type[])null, (Type[])null);
	}

	public void RequestState()
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new PubgReputationRequestMessage());
	}

	public void ClearState()
	{
		Reputation = 100;
		this.OnReputationUpdated?.Invoke(Reputation);
	}

	private void OnReputationState(PubgReputationStateMessage msg)
	{
		Reputation = msg.Reputation;
		this.OnReputationUpdated?.Invoke(Reputation);
	}
}
