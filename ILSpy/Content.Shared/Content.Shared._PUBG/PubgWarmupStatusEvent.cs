using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG;

[Serializable]
[NetSerializable]
public sealed class PubgWarmupStatusEvent : EntityEventArgs
{
	public bool InWarmup { get; }

	public int SecondsRemaining { get; }

	public int AlivePlayers { get; }

	public int? TeamAAlive { get; }

	public int? TeamBAlive { get; }

	public PubgWarmupStatusEvent(bool inWarmup, int secondsRemaining, int alivePlayers, int? teamAAlive = null, int? teamBAlive = null)
	{
		InWarmup = inWarmup;
		SecondsRemaining = secondsRemaining;
		AlivePlayers = alivePlayers;
		TeamAAlive = teamAAlive;
		TeamBAlive = teamBAlive;
	}
}
