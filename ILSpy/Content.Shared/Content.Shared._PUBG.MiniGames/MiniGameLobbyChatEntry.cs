using System;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.MiniGames;

[Serializable]
[NetSerializable]
public sealed class MiniGameLobbyChatEntry
{
	public string Sender { get; }

	public string Text { get; }

	public bool IsSystem { get; }

	public MiniGameLobbyChatEntry(string sender, string text, bool isSystem)
	{
		Sender = sender;
		Text = text;
		IsSystem = isSystem;
	}
}
