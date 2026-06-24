using System;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Friends;

[Serializable]
[NetSerializable]
public sealed class PubgFriendEntry
{
	public NetUserId UserId { get; }

	public string Ckey { get; }

	public PubgFriendStatus Status { get; }

	public int PartySize { get; }

	public int PartyMax { get; }

	public DateTime? LastSeenUtc { get; }

	public PubgFriendEntry(NetUserId userId, string ckey, PubgFriendStatus status, int partySize, int partyMax, DateTime? lastSeenUtc)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		UserId = userId;
		Ckey = ckey;
		Status = status;
		PartySize = partySize;
		PartyMax = partyMax;
		LastSeenUtc = lastSeenUtc;
	}
}
