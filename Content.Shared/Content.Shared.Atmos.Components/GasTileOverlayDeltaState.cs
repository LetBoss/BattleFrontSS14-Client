using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Components;

[Serializable]
[NetSerializable]
public sealed class GasTileOverlayDeltaState(Dictionary<Vector2i, GasOverlayChunk> modifiedChunks, HashSet<Vector2i> allChunks) : ComponentState, IComponentDeltaState<GasTileOverlayState>, IComponentDeltaState, IComponentState
{
	public readonly Dictionary<Vector2i, GasOverlayChunk> ModifiedChunks = modifiedChunks;

	public readonly HashSet<Vector2i> AllChunks = allChunks;

	public void ApplyToFullState(GasTileOverlayState state)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		foreach (Vector2i key in state.Chunks.Keys)
		{
			if (!AllChunks.Contains(key))
			{
				state.Chunks.Remove(key);
			}
		}
		foreach (var (chunk, data) in ModifiedChunks)
		{
			state.Chunks[chunk] = new GasOverlayChunk(data);
		}
	}

	public GasTileOverlayState CreateNewFullState(GasTileOverlayState state)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		Dictionary<Vector2i, GasOverlayChunk> chunks = new Dictionary<Vector2i, GasOverlayChunk>(AllChunks.Count);
		Vector2i key;
		GasOverlayChunk value;
		foreach (KeyValuePair<Vector2i, GasOverlayChunk> modifiedChunk in ModifiedChunks)
		{
			modifiedChunk.Deconstruct(out key, out value);
			Vector2i chunk = key;
			GasOverlayChunk data = value;
			chunks[chunk] = new GasOverlayChunk(data);
		}
		foreach (KeyValuePair<Vector2i, GasOverlayChunk> chunk3 in state.Chunks)
		{
			chunk3.Deconstruct(out key, out value);
			Vector2i chunk2 = key;
			GasOverlayChunk data2 = value;
			if (AllChunks.Contains(chunk2))
			{
				chunks.TryAdd(chunk2, new GasOverlayChunk(data2));
			}
		}
		return new GasTileOverlayState(chunks);
	}
}
