using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.WorldEdit;

[Serializable]
[NetSerializable]
public sealed class WorldEditPreviewDataEvent : EntityEventArgs
{
	public List<WorldEditPreviewEntityData> Entities { get; }

	public List<WorldEditPreviewTileData> Tiles { get; }

	public int Width { get; }

	public int Height { get; }

	public int Degrees { get; }

	public WorldEditPreviewDataEvent(List<WorldEditPreviewEntityData> entities, List<WorldEditPreviewTileData> tiles, int width, int height, int degrees)
	{
		Entities = entities;
		Tiles = tiles;
		Width = width;
		Height = height;
		Degrees = degrees;
	}
}
