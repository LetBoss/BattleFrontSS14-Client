using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.GameTicking;

[Serializable]
[NetSerializable]
public sealed class TickerLobbyInfoEvent : EntityEventArgs
{
	public string TextBlob { get; }

	public TickerLobbyInfoEvent(string textBlob)
	{
		TextBlob = textBlob;
	}
}
