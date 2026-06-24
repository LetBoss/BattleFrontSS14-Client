using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.MiniGames;

[Serializable]
[NetSerializable]
public sealed class MiniGameLobbySetRoundsMessage : EntityEventArgs
{
	public int LobbyId { get; }

	public int Rounds { get; }

	public MiniGameLobbySetRoundsMessage(int lobbyId, int rounds)
	{
		LobbyId = lobbyId;
		Rounds = rounds;
	}
}
