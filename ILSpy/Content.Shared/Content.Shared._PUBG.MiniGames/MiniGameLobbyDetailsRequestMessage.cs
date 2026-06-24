using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.MiniGames;

[Serializable]
[NetSerializable]
public sealed class MiniGameLobbyDetailsRequestMessage : EntityEventArgs
{
	public int LobbyId { get; }

	public MiniGameLobbyDetailsRequestMessage(int lobbyId)
	{
		LobbyId = lobbyId;
	}
}
