using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Lobby;

[Serializable]
[NetSerializable]
public sealed class LobbyStatusEvent : EntityEventArgs
{
	public bool InLobby { get; }

	public int TotalPlayers { get; }

	public int ReadyPlayers { get; }

	public int TimeRemaining { get; }

	public LobbyStatusEvent(bool inLobby, int totalPlayers, int readyPlayers, int timeRemaining = 0)
	{
		InLobby = inLobby;
		TotalPlayers = totalPlayers;
		ReadyPlayers = readyPlayers;
		TimeRemaining = timeRemaining;
	}
}
