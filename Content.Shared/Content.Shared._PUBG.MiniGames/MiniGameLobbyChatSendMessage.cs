using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.MiniGames;

[Serializable]
[NetSerializable]
public sealed class MiniGameLobbyChatSendMessage : EntityEventArgs
{
	public int LobbyId { get; }

	public string Text { get; }

	public MiniGameLobbyChatSendMessage(int lobbyId, string text)
	{
		LobbyId = lobbyId;
		Text = text;
	}
}
