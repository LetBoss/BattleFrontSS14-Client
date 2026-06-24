using System;
using Content.Shared.Atmos.EntitySystems;
using Robust.Shared.Analyzers;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Timing;

namespace Content.Shared.Atmos;

[Serializable]
[NetSerializable]
[Access(new Type[] { typeof(SharedGasTileOverlaySystem) })]
public sealed class GasOverlayChunk
{
	public readonly Vector2i Index;

	public readonly Vector2i Origin;

	public SharedGasTileOverlaySystem.GasOverlayData[] TileData = new SharedGasTileOverlaySystem.GasOverlayData[64];

	[NonSerialized]
	public GameTick LastUpdate;

	public GasOverlayChunk(Vector2i index)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		Index = index;
		Origin = Index * 8;
	}

	public GasOverlayChunk(GasOverlayChunk data)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		Index = data.Index;
		Origin = data.Origin;
		Array.Copy(data.TileData, TileData, data.TileData.Length);
	}

	public int GetDataIndex(Vector2i gridIndices)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		return gridIndices.X - Origin.X + (gridIndices.Y - Origin.Y) * 8;
	}

	private bool InBounds(Vector2i gridIndices)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if (gridIndices.X >= Origin.X && gridIndices.Y >= Origin.Y && gridIndices.X < Origin.X + 8)
		{
			return gridIndices.Y < Origin.Y + 8;
		}
		return false;
	}
}
