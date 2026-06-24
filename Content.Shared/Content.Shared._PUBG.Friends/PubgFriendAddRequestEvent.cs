using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Friends;

[Serializable]
[NetSerializable]
public sealed class PubgFriendAddRequestEvent : EntityEventArgs
{
	public string Ckey { get; }

	public PubgFriendAddRequestEvent(string ckey)
	{
		Ckey = ckey;
	}
}
