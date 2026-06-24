using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Robust.Shared.Map.Components;

[Serializable]
[NetSerializable]
internal sealed class MapGridComponentDeltaState(ushort chunkSize, Dictionary<Vector2i, ChunkDatum>? chunkData, GameTick lastTileModifiedTick) : ComponentState, IComponentDeltaState<MapGridComponentState>, IComponentDeltaState, IComponentState
{
	public readonly ushort ChunkSize = chunkSize;

	public readonly Dictionary<Vector2i, ChunkDatum>? ChunkData = chunkData;

	public GameTick LastTileModifiedTick = lastTileModifiedTick;

	public void ApplyToFullState(MapGridComponentState state)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		state.ChunkSize = ChunkSize;
		if (ChunkData == null)
		{
			return;
		}
		foreach (var (key, value) in ChunkData)
		{
			if (value.IsDeleted())
			{
				state.FullGridData.Remove(key);
			}
			else
			{
				state.FullGridData[key] = value;
			}
		}
		state.LastTileModifiedTick = LastTileModifiedTick;
	}

	public MapGridComponentState CreateNewFullState(MapGridComponentState state)
	{
		if (ChunkData == null)
		{
			return new MapGridComponentState(ChunkSize, state.FullGridData, state.LastTileModifiedTick);
		}
		MapGridComponentState mapGridComponentState = new MapGridComponentState(ChunkSize, state.FullGridData.ShallowClone(), LastTileModifiedTick);
		ApplyToFullState(mapGridComponentState);
		return mapGridComponentState;
	}
}
