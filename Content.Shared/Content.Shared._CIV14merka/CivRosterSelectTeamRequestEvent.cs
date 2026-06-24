using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka;

[Serializable]
[NetSerializable]
public sealed class CivRosterSelectTeamRequestEvent : EntityEventArgs
{
	public int TeamId { get; }

	public CivRosterSelectTeamRequestEvent(int teamId)
	{
		TeamId = teamId;
	}
}
