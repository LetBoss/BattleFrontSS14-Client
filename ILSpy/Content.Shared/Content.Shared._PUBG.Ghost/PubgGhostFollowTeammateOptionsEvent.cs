using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Ghost;

[Serializable]
[NetSerializable]
public sealed class PubgGhostFollowTeammateOptionsEvent : EntityEventArgs
{
	public List<PubgGhostFollowTeammateOptionState> Options { get; }

	public PubgGhostFollowTeammateOptionsEvent(List<PubgGhostFollowTeammateOptionState> options)
	{
		Options = options;
	}
}
