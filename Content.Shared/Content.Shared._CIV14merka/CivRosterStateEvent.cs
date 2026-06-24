using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka;

[Serializable]
[NetSerializable]
public sealed class CivRosterStateEvent : EntityEventArgs
{
	public bool Enabled { get; }

	public bool RoundInProgress { get; }

	public bool LateJoinActive { get; }

	public bool CanEnterRound { get; }

	public string? EnterRoundUnavailableReason { get; }

	public bool IsJoinedRound { get; }

	public bool HasParticipatedInCurrentRound { get; }

	public bool RejoinBlockedForCurrentRound { get; }

	public int? SelectedTeamId { get; }

	public int? SelectedSquadId { get; }

	public List<CivRosterTeamEntry> Teams { get; }

	public List<CivRosterPlayerEntry> Players { get; }

	public Civ14RoundMode RoundMode { get; }

	public bool AllowAutoLeader { get; }

	public int SelfPlaytimeMinutes { get; }

	public CivRosterStateEvent(bool enabled, bool roundInProgress, bool lateJoinActive, bool canEnterRound, string? enterRoundUnavailableReason, bool isJoinedRound, bool hasParticipatedInCurrentRound, bool rejoinBlockedForCurrentRound, int? selectedTeamId, int? selectedSquadId, List<CivRosterTeamEntry> teams, List<CivRosterPlayerEntry> players, Civ14RoundMode roundMode = Civ14RoundMode.BaseCapture, bool allowAutoLeader = false, int selfPlaytimeMinutes = 0)
	{
		Enabled = enabled;
		RoundInProgress = roundInProgress;
		LateJoinActive = lateJoinActive;
		CanEnterRound = canEnterRound;
		EnterRoundUnavailableReason = enterRoundUnavailableReason;
		IsJoinedRound = isJoinedRound;
		HasParticipatedInCurrentRound = hasParticipatedInCurrentRound;
		RejoinBlockedForCurrentRound = rejoinBlockedForCurrentRound;
		SelectedTeamId = selectedTeamId;
		SelectedSquadId = selectedSquadId;
		Teams = teams;
		Players = players;
		RoundMode = roundMode;
		AllowAutoLeader = allowAutoLeader;
		SelfPlaytimeMinutes = selfPlaytimeMinutes;
	}
}
