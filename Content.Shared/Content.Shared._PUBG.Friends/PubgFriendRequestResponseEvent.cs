using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Friends;

[Serializable]
[NetSerializable]
public sealed class PubgFriendRequestResponseEvent : EntityEventArgs
{
	public NetUserId RequesterId { get; }

	public bool Accepted { get; }

	public PubgFriendRequestResponseEvent(NetUserId requesterId, bool accepted)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		RequesterId = requesterId;
		Accepted = accepted;
	}
}
