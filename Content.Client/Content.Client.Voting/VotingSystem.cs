using System;
using Content.Shared.Voting;
using Robust.Shared.GameObjects;

namespace Content.Client.Voting;

public sealed class VotingSystem : EntitySystem
{
	public event Action<VotePlayerListResponseEvent>? VotePlayerListResponse;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeNetworkEvent<VotePlayerListResponseEvent>((EntityEventHandler<VotePlayerListResponseEvent>)OnVotePlayerListResponseEvent, (Type[])null, (Type[])null);
	}

	private void OnVotePlayerListResponseEvent(VotePlayerListResponseEvent msg)
	{
		this.VotePlayerListResponse?.Invoke(msg);
	}

	public void RequestVotePlayerList()
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new VotePlayerListRequestEvent());
	}
}
