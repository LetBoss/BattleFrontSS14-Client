using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Leaderboard;

[Serializable]
[NetSerializable]
public sealed class LeaderboardResponseMessage : EntityEventArgs
{
	public List<LeaderboardEntry> Players { get; }

	public int PlayerRank { get; }

	public int PlayerRating { get; }

	public string PlayerUsername { get; }

	public int TotalPlayers { get; }

	public LeaderboardResponseMessage(List<LeaderboardEntry> players, int playerRank, int playerRating, string playerUsername, int totalPlayers)
	{
		Players = players;
		PlayerRank = playerRank;
		PlayerRating = playerRating;
		PlayerUsername = playerUsername;
		TotalPlayers = totalPlayers;
	}
}
