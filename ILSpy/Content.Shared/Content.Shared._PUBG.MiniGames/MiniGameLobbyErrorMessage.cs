using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.MiniGames;

[Serializable]
[NetSerializable]
public sealed class MiniGameLobbyErrorMessage : EntityEventArgs
{
	public int LobbyId { get; }

	public string ErrorKey { get; }

	public MiniGameLobbyErrorMessage(int lobbyId, string errorKey)
	{
		LobbyId = lobbyId;
		ErrorKey = errorKey;
	}
}
