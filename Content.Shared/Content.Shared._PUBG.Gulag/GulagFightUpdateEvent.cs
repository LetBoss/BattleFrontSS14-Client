using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Gulag;

[Serializable]
[NetSerializable]
public sealed class GulagFightUpdateEvent : EntityEventArgs
{
	public string OpponentUsername { get; }

	public string OpponentRank { get; }

	public float TimeRemaining { get; }

	public GulagFightUpdateEvent(string opponentUsername, string opponentRank, float timeRemaining)
	{
		OpponentUsername = opponentUsername;
		OpponentRank = opponentRank;
		TimeRemaining = timeRemaining;
	}
}
