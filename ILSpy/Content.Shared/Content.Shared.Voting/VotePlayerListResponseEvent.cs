using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared.Voting;

[Serializable]
[NetSerializable]
public sealed class VotePlayerListResponseEvent : EntityEventArgs
{
	public bool Denied;

	public (NetUserId, NetEntity, string)[] Players { get; }

	public VotePlayerListResponseEvent((NetUserId, NetEntity, string)[] players, bool denied)
	{
		Players = players;
		Denied = denied;
	}
}
