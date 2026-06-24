using System;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka;

[Serializable]
[NetSerializable]
public sealed class CivRoundEndSideInfo
{
	public string TeamName { get; }

	public bool IsWinner { get; }

	public bool HasScore { get; }

	public int Score { get; }

	public string CommanderName { get; }

	public CivRoundEndSideInfo(string teamName, bool isWinner, bool hasScore, int score, string commanderName)
	{
		TeamName = teamName;
		IsWinner = isWinner;
		HasScore = hasScore;
		Score = score;
		CommanderName = commanderName;
	}
}
