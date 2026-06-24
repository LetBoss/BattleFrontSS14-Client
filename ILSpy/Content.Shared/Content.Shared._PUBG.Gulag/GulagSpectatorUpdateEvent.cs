using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Gulag;

[Serializable]
[NetSerializable]
public sealed class GulagSpectatorUpdateEvent : EntityEventArgs
{
	public string Fighter1Username { get; }

	public string Fighter1Rank { get; }

	public string Fighter2Username { get; }

	public string Fighter2Rank { get; }

	public float TimeRemaining { get; }

	public int QueueSize { get; }

	public GulagSpectatorUpdateEvent(string fighter1Username, string fighter1Rank, string fighter2Username, string fighter2Rank, float timeRemaining, int queueSize)
	{
		Fighter1Username = fighter1Username;
		Fighter1Rank = fighter1Rank;
		Fighter2Username = fighter2Username;
		Fighter2Rank = fighter2Rank;
		TimeRemaining = timeRemaining;
		QueueSize = queueSize;
	}
}
