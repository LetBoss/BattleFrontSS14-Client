using System;
using System.Collections.Generic;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka;

[Serializable]
[NetSerializable]
public sealed class CivRosterTeamEntry
{
	public int TeamId { get; }

	public string SideId { get; }

	public string TeamName { get; }

	public string TeamRoleName { get; }

	public int PlayerCount { get; }

	public bool IsSelected { get; }

	public bool CanSelect { get; }

	public string? SelectUnavailableReason { get; }

	public bool CanCreateSquad { get; }

	public string? CreateSquadUnavailableReason { get; }

	public List<CivRosterSquadEntry> Squads { get; }

	public string? CommanderName { get; }

	public NetUserId? CommanderUserId { get; }

	public List<CivCommanderCandidateEntry> CommanderCandidates { get; }

	public bool CanNominate { get; }

	public string? NominateUnavailableReason { get; }

	public CivRosterTeamEntry(int teamId, string sideId, string teamName, string teamRoleName, int playerCount, bool isSelected, bool canSelect, string? selectUnavailableReason, bool canCreateSquad, string? createSquadUnavailableReason, List<CivRosterSquadEntry> squads, string? commanderName = null, NetUserId? commanderUserId = null, List<CivCommanderCandidateEntry>? commanderCandidates = null, bool canNominate = false, string? nominateUnavailableReason = null)
	{
		TeamId = teamId;
		SideId = sideId ?? string.Empty;
		TeamName = teamName;
		TeamRoleName = teamRoleName;
		PlayerCount = playerCount;
		IsSelected = isSelected;
		CanSelect = canSelect;
		SelectUnavailableReason = selectUnavailableReason;
		CanCreateSquad = canCreateSquad;
		CreateSquadUnavailableReason = createSquadUnavailableReason;
		Squads = squads;
		CommanderName = commanderName;
		CommanderUserId = commanderUserId;
		CommanderCandidates = commanderCandidates ?? new List<CivCommanderCandidateEntry>();
		CanNominate = canNominate;
		NominateUnavailableReason = nominateUnavailableReason;
	}
}
