using System;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Friends;

[Serializable]
[NetSerializable]
public sealed class PubgFriendRequestEntry
{
	public NetUserId UserId { get; }

	public string Ckey { get; }

	public PubgFriendRequestEntry(NetUserId userId, string ckey)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		UserId = userId;
		Ckey = ckey;
	}
}
