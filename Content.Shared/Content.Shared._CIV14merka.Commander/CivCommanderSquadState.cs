using System;
using System.Collections.Generic;
using System.Linq;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.Commander;

[Serializable]
[NetSerializable]
public sealed class CivCommanderSquadState
{
	public int TeamId { get; }

	public int SquadId { get; }

	public bool IsArtillery { get; }

	public CivCommanderOrderType Order { get; }

	public string LeaderName { get; }

	public List<CivCommanderPlayerState> Members { get; }

	public CivCommanderSquadState(int teamId, int squadId, bool isArtillery, CivCommanderOrderType order, string leaderName, IEnumerable<CivCommanderPlayerState> members)
	{
		TeamId = teamId;
		SquadId = squadId;
		IsArtillery = isArtillery;
		Order = order;
		LeaderName = leaderName;
		Members = members.ToList();
	}
}
