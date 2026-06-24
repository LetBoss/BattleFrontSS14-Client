using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.Commander;

[Serializable]
[NetSerializable]
public sealed class CivCommanderSetArtilleryRequestEvent : EntityEventArgs
{
	public int SquadId { get; }

	public bool Enabled { get; }

	public CivCommanderSetArtilleryRequestEvent(int squadId, bool enabled)
	{
		SquadId = squadId;
		Enabled = enabled;
	}
}
