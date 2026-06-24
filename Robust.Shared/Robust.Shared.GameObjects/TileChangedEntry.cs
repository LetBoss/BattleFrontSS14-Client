using Robust.Shared.Map;
using Robust.Shared.Maths;

namespace Robust.Shared.GameObjects;

public readonly record struct TileChangedEntry(Tile NewTile, Tile OldTile, Vector2i ChunkIndex, Vector2i GridIndices)
{
	public bool EmptyChanged => OldTile.IsEmpty != NewTile.IsEmpty;
}
