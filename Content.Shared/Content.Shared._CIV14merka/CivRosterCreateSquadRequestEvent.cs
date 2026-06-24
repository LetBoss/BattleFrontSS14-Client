using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka;

[Serializable]
[NetSerializable]
public sealed class CivRosterCreateSquadRequestEvent : EntityEventArgs
{
	public int TeamId { get; }

	public CivRosterCreateSquadRequestEvent(int teamId)
	{
		TeamId = teamId;
	}
}
