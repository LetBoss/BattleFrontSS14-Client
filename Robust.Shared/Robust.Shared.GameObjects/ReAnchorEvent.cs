using Robust.Shared.Maths;

namespace Robust.Shared.GameObjects;

[ByRefEvent]
public readonly struct ReAnchorEvent(EntityUid uid, EntityUid oldGrid, EntityUid grid, Vector2i tilePos, TransformComponent xform)
{
	public readonly EntityUid Entity = uid;

	public readonly EntityUid OldGrid = oldGrid;

	public readonly EntityUid Grid = grid;

	public readonly TransformComponent Xform = xform;

	public readonly Vector2i TilePos = tilePos;
}
