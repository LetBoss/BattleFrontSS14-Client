using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.MiniGames;

[Serializable]
[NetSerializable]
public sealed class MiniGameLobbyListMessage : EntityEventArgs
{
	public List<MiniGameLobbyListEntry> Lobbies { get; }

	public MiniGameLobbyListMessage(List<MiniGameLobbyListEntry> lobbies)
	{
		Lobbies = lobbies;
	}
}
