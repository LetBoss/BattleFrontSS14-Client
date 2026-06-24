using System;
using System.Collections.Generic;
using System.Numerics;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Robust.Shared.GameObjects;

[Serializable]
[NetSerializable]
public sealed class ChunkSplitDebugMessage : EntityEventArgs
{
	public NetEntity Grid;

	public Dictionary<Vector2i, List<List<Vector2i>>> Nodes = new Dictionary<Vector2i, List<List<Vector2i>>>();

	public List<(Vector2 Start, Vector2 End)> Connections = new List<(Vector2, Vector2)>();
}
