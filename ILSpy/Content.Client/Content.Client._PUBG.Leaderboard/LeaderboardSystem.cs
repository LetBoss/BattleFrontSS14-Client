using System;
using Content.Shared._PUBG.Leaderboard;
using Robust.Shared.GameObjects;

namespace Content.Client._PUBG.Leaderboard;

public sealed class LeaderboardSystem : EntitySystem
{
	public event Action<LeaderboardResponseMessage>? OnLeaderboardReceived;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeNetworkEvent<LeaderboardResponseMessage>((EntityEventHandler<LeaderboardResponseMessage>)OnLeaderboardResponse, (Type[])null, (Type[])null);
	}

	private void OnLeaderboardResponse(LeaderboardResponseMessage msg)
	{
		this.OnLeaderboardReceived?.Invoke(msg);
	}

	public void RequestLeaderboard(string sortBy = "rating")
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new LeaderboardRequestMessage(sortBy));
	}
}
