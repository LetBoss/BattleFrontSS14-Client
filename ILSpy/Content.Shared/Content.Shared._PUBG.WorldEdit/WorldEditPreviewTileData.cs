using System;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.WorldEdit;

[Serializable]
[NetSerializable]
public sealed class WorldEditPreviewTileData
{
	public Vector2i RelativePosition { get; }

	public ushort TileId { get; }

	public WorldEditPreviewTileData(Vector2i relativePosition, ushort tileId)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		RelativePosition = relativePosition;
		TileId = tileId;
	}
}
