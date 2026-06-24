using System;
using System.Collections.Generic;
using System.Linq;
using Content.Shared._CIV14merka.Capture;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka;

[Serializable]
[NetSerializable]
public sealed class CivHudStatusEvent : EntityEventArgs
{
	public bool Enabled { get; }

	public int ViewerTeamId { get; }

	public CivHudPhase Phase { get; }

	public bool ShowBriefingPanel { get; }

	public float PhaseTimeLeftSeconds { get; }

	public int Team1AliveCount { get; }

	public int Team2AliveCount { get; }

	public int Team1Score { get; }

	public int Team2Score { get; }

	public string ModeName { get; }

	public string ObjectiveText { get; }

	public string GuidanceText { get; }

	public bool IsSquadLeader { get; }

	public List<CivPointCapturePointState> PointStates { get; }

	public float WaveRespawnSecondsLeft { get; }

	public bool WaveConfirmActive { get; }

	public CivHudStatusEvent(bool enabled, int viewerTeamId, CivHudPhase phase, bool showBriefingPanel, float phaseTimeLeftSeconds, int team1AliveCount, int team2AliveCount, int team1Score, int team2Score, string modeName, string objectiveText, string guidanceText, bool isSquadLeader, IEnumerable<CivPointCapturePointState> pointStates, float waveRespawnSecondsLeft, bool waveConfirmActive)
	{
		Enabled = enabled;
		ViewerTeamId = viewerTeamId;
		Phase = phase;
		ShowBriefingPanel = showBriefingPanel;
		PhaseTimeLeftSeconds = phaseTimeLeftSeconds;
		Team1AliveCount = team1AliveCount;
		Team2AliveCount = team2AliveCount;
		Team1Score = team1Score;
		Team2Score = team2Score;
		ModeName = modeName;
		ObjectiveText = objectiveText;
		GuidanceText = guidanceText;
		IsSquadLeader = isSquadLeader;
		PointStates = pointStates.ToList();
		WaveRespawnSecondsLeft = waveRespawnSecondsLeft;
		WaveConfirmActive = waveConfirmActive;
	}
}
