using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.MiniGames;

[Serializable]
[NetSerializable]
public sealed class MiniGameLobbyLeaveMessage : EntityEventArgs
{
	public int LobbyId { get; }

	public MiniGameLobbyLeaveMessage(int lobbyId)
	{
		LobbyId = lobbyId;
	}
}
