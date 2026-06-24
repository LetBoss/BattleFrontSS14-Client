using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.NPC;

[Serializable]
[NetSerializable]
public sealed class PathRouteMessage : EntityEventArgs
{
	public List<DebugPathPoly> Path;

	public Dictionary<DebugPathPoly, float> Costs;

	public PathRouteMessage(List<DebugPathPoly> path, Dictionary<DebugPathPoly, float> costs)
	{
		Path = path;
		Costs = costs;
	}
}
