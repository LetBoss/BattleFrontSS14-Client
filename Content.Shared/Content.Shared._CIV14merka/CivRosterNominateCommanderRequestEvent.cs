using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka;

[Serializable]
[NetSerializable]
public sealed class CivRosterNominateCommanderRequestEvent : EntityEventArgs
{
	public int TeamId { get; }

	public CivRosterNominateCommanderRequestEvent(int teamId)
	{
		TeamId = teamId;
	}
}
