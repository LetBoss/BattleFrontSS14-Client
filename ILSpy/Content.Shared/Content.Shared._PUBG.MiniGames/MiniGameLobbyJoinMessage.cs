using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.MiniGames;

[Serializable]
[NetSerializable]
public sealed class MiniGameLobbyJoinMessage : EntityEventArgs
{
	public int LobbyId { get; }

	public string Password { get; }

	public MiniGameLobbyJoinMessage(int lobbyId, string password)
	{
		LobbyId = lobbyId;
		Password = password;
	}
}
