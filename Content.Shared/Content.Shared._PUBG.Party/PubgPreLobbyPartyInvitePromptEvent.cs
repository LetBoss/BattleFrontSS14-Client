using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Party;

[Serializable]
[NetSerializable]
public sealed class PubgPreLobbyPartyInvitePromptEvent : EntityEventArgs
{
	public string InviterName { get; }

	public int TimeoutSeconds { get; }

	public PubgPreLobbyPartyInvitePromptEvent(string inviterName, int timeoutSeconds)
	{
		InviterName = inviterName;
		TimeoutSeconds = timeoutSeconds;
	}
}
