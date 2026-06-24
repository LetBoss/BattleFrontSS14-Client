using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Friends;

[Serializable]
[NetSerializable]
public sealed class PubgFriendsStateEvent : EntityEventArgs
{
	public List<PubgFriendEntry> Friends { get; }

	public List<PubgFriendRequestEntry> Requests { get; }

	public List<PubgFriendOutgoingEntry> Outgoing { get; }

	public PubgFriendsStateEvent(List<PubgFriendEntry> friends, List<PubgFriendRequestEntry> requests, List<PubgFriendOutgoingEntry> outgoing)
	{
		Friends = friends;
		Requests = requests;
		Outgoing = outgoing;
	}
}
