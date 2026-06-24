using System;
using Robust.Shared.Map;
using Robust.Shared.Serialization;

namespace Content.Shared.NPC;

[Serializable]
[NetSerializable]
public sealed class DebugPathPolyNeighbor
{
	public NetCoordinates Coordinates;
}
