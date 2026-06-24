using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Skin;

[Serializable]
[NetSerializable]
public sealed class SkinProfileStateMessage : EntityEventArgs
{
	public List<LeaderboardEntryInfo> Leaderboard { get; }

	public int PlayerRank { get; }

	public int PlayerRating { get; }

	public List<MatchHistoryInfo> MatchHistory { get; }

	public int TotalDeaths { get; }

	public SkinProfileStateMessage(List<LeaderboardEntryInfo> leaderboard, int playerRank, int playerRating, List<MatchHistoryInfo> matchHistory, int totalDeaths)
	{
		Leaderboard = leaderboard;
		PlayerRank = playerRank;
		PlayerRating = playerRating;
		MatchHistory = matchHistory;
		TotalDeaths = totalDeaths;
	}
}
