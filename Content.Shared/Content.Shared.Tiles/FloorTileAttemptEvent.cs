using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

namespace Content.Shared.Tiles;

[ByRefEvent]
public record struct FloorTileAttemptEvent(Vector2i GridIndices, bool Cancelled = false);
