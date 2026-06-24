using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.GameTicking;

[Serializable]
[NetSerializable]
public sealed class TickerLobbyCountdownEvent : EntityEventArgs
{
	public TimeSpan StartTime { get; }

	public bool Paused { get; }

	public TickerLobbyCountdownEvent(TimeSpan startTime, bool paused)
	{
		StartTime = startTime;
		Paused = paused;
	}
}
