using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Ghost;

[Serializable]
[NetSerializable]
public sealed class GhostWarpsResponseEvent : EntityEventArgs
{
	public List<GhostWarp> Warps { get; }

	public GhostWarpsResponseEvent(List<GhostWarp> warps)
	{
		Warps = warps;
	}
}
