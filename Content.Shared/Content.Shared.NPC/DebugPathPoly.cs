using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.NPC;

[Serializable]
[NetSerializable]
public sealed class DebugPathPoly
{
	public NetEntity GraphUid;

	public Vector2i ChunkOrigin;

	public byte TileIndex;

	public Box2 Box;

	public PathfindingData Data;

	public List<NetCoordinates> Neighbors;
}
