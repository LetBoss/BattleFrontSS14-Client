using System;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Timing;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Pinpointer;

[Serializable]
[NetSerializable]
public sealed class NavMapChunk(Vector2i origin)
{
	[ViewVariables]
	public readonly Vector2i Origin = origin;

	[ViewVariables]
	public int[] TileData = new int[64];

	[NonSerialized]
	public GameTick LastUpdate;
}
