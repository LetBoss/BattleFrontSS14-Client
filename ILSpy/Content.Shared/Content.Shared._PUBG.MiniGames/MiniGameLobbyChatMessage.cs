using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.MiniGames;

[Serializable]
[NetSerializable]
public sealed class MiniGameLobbyChatMessage : EntityEventArgs
{
	public int LobbyId { get; }

	public MiniGameLobbyChatEntry Entry { get; }

	public MiniGameLobbyChatMessage(int lobbyId, MiniGameLobbyChatEntry entry)
	{
		LobbyId = lobbyId;
		Entry = entry;
	}
}
