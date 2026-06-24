using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.GameTicking;

[Serializable]
[NetSerializable]
public sealed class TickerLobbyStatusEvent : EntityEventArgs
{
	public bool IsRoundStarted { get; }

	public string? LobbyBackground { get; }

	public bool YouAreReady { get; }

	public TimeSpan StartTime { get; }

	public TimeSpan RoundStartTimeSpan { get; }

	public bool Paused { get; }

	public TickerLobbyStatusEvent(bool isRoundStarted, string? lobbyBackground, bool youAreReady, TimeSpan startTime, TimeSpan preloadTime, TimeSpan roundStartTimeSpan, bool paused)
	{
		IsRoundStarted = isRoundStarted;
		LobbyBackground = lobbyBackground;
		YouAreReady = youAreReady;
		StartTime = startTime;
		RoundStartTimeSpan = roundStartTimeSpan;
		Paused = paused;
	}
}
