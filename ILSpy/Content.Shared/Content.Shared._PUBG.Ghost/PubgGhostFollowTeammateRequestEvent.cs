using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Ghost;

[Serializable]
[NetSerializable]
public sealed class PubgGhostFollowTeammateRequestEvent : EntityEventArgs
{
	public NetEntity? Target { get; }

	public PubgGhostFollowTeammateRequestEvent(NetEntity? target = null)
	{
		Target = target;
	}
}
