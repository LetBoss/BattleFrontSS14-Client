using Robust.Shared.GameObjects;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;

namespace Robust.Shared.Map;

internal delegate void TileModifiedDelegate(EntityUid uid, MapGridComponent grid, MapChunk mapChunk, Vector2i tileIndices, Tile newTile, Tile oldTile, bool chunkShapeChanged);
