using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Party;

[Serializable]
[NetSerializable]
public sealed class PubgPreLobbyPartyInviteResponseEvent : EntityEventArgs
{
	public bool Accepted { get; }

	public PubgPreLobbyPartyInviteResponseEvent(bool accepted)
	{
		Accepted = accepted;
	}
}
