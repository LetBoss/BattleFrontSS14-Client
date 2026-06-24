using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Party;

[Serializable]
[NetSerializable]
public sealed class PubgAdminPartyListResponseEvent : EntityEventArgs
{
	public List<PubgAdminPartyPlayerInfo> Players { get; }

	public PubgAdminPartyListResponseEvent(List<PubgAdminPartyPlayerInfo> players)
	{
		Players = players;
	}
}
