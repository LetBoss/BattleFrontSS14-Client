using System;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Skin;

[Serializable]
[NetSerializable]
public sealed class MatchHistoryInfo
{
	public int Placement { get; set; }

	public int Kills { get; set; }

	public int Deaths { get; set; }

	public int Damage { get; set; }

	public int SurvivalTime { get; set; }

	public int RatingChange { get; set; }

	public bool IsWin { get; set; }

	public string Date { get; set; } = string.Empty;
}
