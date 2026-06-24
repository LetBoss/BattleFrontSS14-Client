using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Party;

[Serializable]
[NetSerializable]
public sealed class PubgPartyInviteResponseEvent : EntityEventArgs
{
	public bool Accepted { get; }

	public PubgPartyInviteResponseEvent(bool accepted)
	{
		Accepted = accepted;
	}
}
