using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Party;

[Serializable]
[NetSerializable]
public sealed class PubgPartyInvitePromptEvent : EntityEventArgs
{
	public string InviterName { get; }

	public int TimeoutSeconds { get; }

	public PubgPartyInvitePromptEvent(string inviterName, int timeoutSeconds)
	{
		InviterName = inviterName;
		TimeoutSeconds = timeoutSeconds;
	}
}
