using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Gulag;

[Serializable]
[NetSerializable]
public sealed class GulagFightStartEvent : EntityEventArgs
{
	public string OpponentUsername { get; }

	public string OpponentRank { get; }

	public GulagFightStartEvent(string opponentUsername, string opponentRank)
	{
		OpponentUsername = opponentUsername;
		OpponentRank = opponentRank;
	}
}
