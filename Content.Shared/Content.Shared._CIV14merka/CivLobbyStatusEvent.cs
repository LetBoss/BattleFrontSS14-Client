using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka;

[Serializable]
[NetSerializable]
public sealed class CivLobbyStatusEvent : EntityEventArgs
{
	public TimeSpan StartTime { get; }

	public bool CountdownActive { get; }

	public int ReadyCount { get; }

	public string RoundModeName { get; }

	public string MapName { get; }

	public bool RandomMapSelection { get; }

	public CivLobbyStatusEvent(TimeSpan startTime, bool countdownActive, int readyCount, string roundModeName, string mapName, bool randomMapSelection)
	{
		StartTime = startTime;
		CountdownActive = countdownActive;
		ReadyCount = readyCount;
		RoundModeName = roundModeName;
		MapName = mapName;
		RandomMapSelection = randomMapSelection;
	}
}
