using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka;

[Serializable]
[NetSerializable]
public sealed class CivRosterSetSquadOpenRequestEvent : EntityEventArgs
{
	public int TeamId { get; }

	public int SquadId { get; }

	public bool IsOpen { get; }

	public CivRosterSetSquadOpenRequestEvent(int teamId, int squadId, bool isOpen)
	{
		TeamId = teamId;
		SquadId = squadId;
		IsOpen = isOpen;
	}
}
