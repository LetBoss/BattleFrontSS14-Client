using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Decals;

[Serializable]
[NetSerializable]
public sealed class DecalGridDeltaState(Dictionary<Vector2i, DecalGridComponent.DecalChunk> modifiedChunks, HashSet<Vector2i> allChunks) : ComponentState, IComponentDeltaState<DecalGridState>, IComponentDeltaState, IComponentState
{
	public Dictionary<Vector2i, DecalGridComponent.DecalChunk> ModifiedChunks = modifiedChunks;

	public HashSet<Vector2i> AllChunks = allChunks;

	public void ApplyToFullState(DecalGridState state)
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
			state.Chunks[chunk] = new DecalGridComponent.DecalChunk(data);
		}
	}

	public DecalGridState CreateNewFullState(DecalGridState state)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		Dictionary<Vector2i, DecalGridComponent.DecalChunk> chunks = new Dictionary<Vector2i, DecalGridComponent.DecalChunk>(state.Chunks.Count);
		Vector2i key;
		DecalGridComponent.DecalChunk value;
		foreach (KeyValuePair<Vector2i, DecalGridComponent.DecalChunk> modifiedChunk in ModifiedChunks)
		{
			modifiedChunk.Deconstruct(out key, out value);
			Vector2i chunk = key;
			DecalGridComponent.DecalChunk data = value;
			chunks[chunk] = new DecalGridComponent.DecalChunk(data);
		}
		foreach (KeyValuePair<Vector2i, DecalGridComponent.DecalChunk> chunk3 in state.Chunks)
		{
			chunk3.Deconstruct(out key, out value);
			Vector2i chunk2 = key;
			DecalGridComponent.DecalChunk data2 = value;
			if (AllChunks.Contains(chunk2))
			{
				chunks.TryAdd(chunk2, new DecalGridComponent.DecalChunk(data2));
			}
		}
		return new DecalGridState(chunks);
	}
}
