using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Timing;

namespace Robust.Shared.Map.Components;

[Serializable]
[NetSerializable]
internal sealed class MapGridComponentState(ushort chunkSize, Dictionary<Vector2i, ChunkDatum> fullGridData, GameTick lastTileModifiedTick) : ComponentState
{
	public ushort ChunkSize = chunkSize;

	public Dictionary<Vector2i, ChunkDatum> FullGridData = fullGridData;

	public GameTick LastTileModifiedTick = lastTileModifiedTick;
}
