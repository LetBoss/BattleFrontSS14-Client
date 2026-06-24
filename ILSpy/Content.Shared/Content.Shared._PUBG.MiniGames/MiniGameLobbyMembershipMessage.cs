using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.MiniGames;

[Serializable]
[NetSerializable]
public sealed class MiniGameLobbyMembershipMessage : EntityEventArgs
{
	public bool IsInLobby { get; }

	public int LobbyId { get; }

	public MiniGameLobbyMembershipMessage(bool isInLobby, int lobbyId)
	{
		IsInLobby = isInLobby;
		LobbyId = lobbyId;
	}
}
