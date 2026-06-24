using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Leaderboard;

[Serializable]
[NetSerializable]
public sealed class LeaderboardRequestMessage : EntityEventArgs
{
	public string SortBy { get; }

	public LeaderboardRequestMessage(string sortBy = "rating")
	{
		SortBy = sortBy;
	}
}
