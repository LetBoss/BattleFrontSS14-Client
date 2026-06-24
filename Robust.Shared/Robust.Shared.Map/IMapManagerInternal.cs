using Robust.Shared.GameObjects;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;

namespace Robust.Shared.Map;

internal interface IMapManagerInternal : IMapManager
{
	void RaiseOnTileChanged(Entity<MapGridComponent> entity, TileRef tileRef, Tile oldTile, Vector2i chunk);
}
