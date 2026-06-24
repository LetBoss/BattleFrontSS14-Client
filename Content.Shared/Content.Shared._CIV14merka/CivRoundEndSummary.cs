using System;
using System.Collections.Generic;
using Content.Shared._CIV14merka.Stats;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka;

[Serializable]
[NetSerializable]
public sealed class CivRoundEndSummary
{
	public string WinnerText { get; }

	public string ModeText { get; }

	public string MapText { get; }

	public string ReasonText { get; }

	public string DurationText { get; }

	public string Team1Name { get; }

	public string Team2Name { get; }

	public int Team1AliveCount { get; }

	public int Team2AliveCount { get; }

	public bool HasScore { get; }

	public int Team1Score { get; }

	public int Team2Score { get; }

	public int LobbyReturnSeconds { get; }

	public List<CivRoundTopAward> TopAwards { get; }

	public List<CivRoundEndSideInfo> Sides { get; }

	public CivRoundEndSummary(string winnerText, string modeText, string mapText, string reasonText, string durationText, string team1Name, string team2Name, int team1AliveCount, int team2AliveCount, bool hasScore, int team1Score, int team2Score, int lobbyReturnSeconds, List<CivRoundTopAward>? topAwards = null, List<CivRoundEndSideInfo>? sides = null)
	{
		WinnerText = winnerText;
		ModeText = modeText;
		MapText = mapText;
		ReasonText = reasonText;
		DurationText = durationText;
		Team1Name = team1Name;
		Team2Name = team2Name;
		Team1AliveCount = team1AliveCount;
		Team2AliveCount = team2AliveCount;
		HasScore = hasScore;
		Team1Score = team1Score;
		Team2Score = team2Score;
		LobbyReturnSeconds = lobbyReturnSeconds;
		TopAwards = topAwards ?? new List<CivRoundTopAward>();
		Sides = sides ?? new List<CivRoundEndSideInfo>();
	}
}
