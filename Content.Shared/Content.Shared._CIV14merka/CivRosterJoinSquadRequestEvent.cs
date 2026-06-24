using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka;

[Serializable]
[NetSerializable]
public sealed class CivRosterJoinSquadRequestEvent : EntityEventArgs
{
	public int TeamId { get; }

	public int SquadId { get; }

	public CivRosterJoinSquadRequestEvent(int teamId, int squadId)
	{
		TeamId = teamId;
		SquadId = squadId;
	}
}
