using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Party;

[Serializable]
[NetSerializable]
public sealed class PubgPartyStateEvent : EntityEventArgs
{
	public List<PubgPartyMemberState> Members { get; }

	public string? TeamTag { get; }

	public PubgPartyStateEvent(List<PubgPartyMemberState> members, string? teamTag = null)
	{
		Members = members;
		TeamTag = teamTag;
	}
}
