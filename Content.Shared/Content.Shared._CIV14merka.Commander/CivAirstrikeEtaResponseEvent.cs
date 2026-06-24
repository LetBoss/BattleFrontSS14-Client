using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.Commander;

[Serializable]
[NetSerializable]
public sealed class CivAirstrikeEtaResponseEvent : EntityEventArgs
{
	public float Seconds { get; }

	public CivAirstrikeEtaResponseEvent(float seconds)
	{
		Seconds = seconds;
	}
}
