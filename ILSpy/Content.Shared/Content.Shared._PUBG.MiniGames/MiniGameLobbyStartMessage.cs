using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.MiniGames;

[Serializable]
[NetSerializable]
public sealed class MiniGameLobbyStartMessage : EntityEventArgs
{
	public int LobbyId { get; }

	public MiniGameLobbyStartMessage(int lobbyId)
	{
		LobbyId = lobbyId;
	}
}
