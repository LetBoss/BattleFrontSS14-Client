using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Friends;

[Serializable]
[NetSerializable]
public sealed class PubgFriendCancelRequestEvent : EntityEventArgs
{
	public NetUserId TargetId { get; }

	public PubgFriendCancelRequestEvent(NetUserId targetId)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		TargetId = targetId;
	}
}
