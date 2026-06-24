using System;
using System.Collections.Generic;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka;

[Serializable]
[NetSerializable]
public sealed class CivRoundEndTeamEntry
{
	public string TeamName { get; }

	public string RoleName { get; }

	public int AliveCount { get; }

	public int TotalCount { get; }

	public List<CivRoundEndPlayerEntry> Players { get; }

	public CivRoundEndTeamEntry(string teamName, string roleName, int aliveCount, int totalCount, List<CivRoundEndPlayerEntry>? players = null)
	{
		TeamName = teamName;
		RoleName = roleName;
		AliveCount = aliveCount;
		TotalCount = totalCount;
		Players = players ?? new List<CivRoundEndPlayerEntry>();
	}
}
