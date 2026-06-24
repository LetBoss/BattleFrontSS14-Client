using System;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Skin;

[Serializable]
[NetSerializable]
public sealed class LeaderboardEntryInfo
{
	public int Rank { get; set; }

	public string Username { get; set; } = string.Empty;

	public int Rating { get; set; }

	public int Games { get; set; }

	public int Wins { get; set; }

	public int Kills { get; set; }

	public int DamageDealt { get; set; }

	public int SurvivalTime { get; set; }
}
