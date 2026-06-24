using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;

namespace Robust.Shared.GameObjects;

[ByRefEvent]
public readonly record struct TileChangedEvent
{
	public readonly Entity<MapGridComponent> Entity;

	public readonly TileChangedEntry[] Changes;

	public TileChangedEvent(Entity<MapGridComponent> entity, TileRef newTile, Tile oldTile, Vector2i chunkIndex)
		: this(entity, newTile.Tile, oldTile, chunkIndex, newTile.GridIndices)
	{
	}//IL_0009: Unknown result type (might be due to invalid IL or missing references)
	//IL_000c: Unknown result type (might be due to invalid IL or missing references)


	public TileChangedEvent(Entity<MapGridComponent> entity, Tile newTile, Tile oldTile, Vector2i chunkIndex, Vector2i gridIndices)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		Entity = entity;
		Changes = new TileChangedEntry[1]
		{
			new TileChangedEntry(newTile, oldTile, chunkIndex, gridIndices)
		};
	}

	public TileChangedEvent(Entity<MapGridComponent> entity, TileChangedEntry[] changes)
	{
		Entity = entity;
		Changes = changes;
	}
}
