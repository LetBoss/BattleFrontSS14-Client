using System;
using System.Collections.Generic;
using System.Numerics;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Explosion.Components;

[Serializable]
[NetSerializable]
public sealed class ExplosionVisualsState : ComponentState
{
	public MapCoordinates Epicenter;

	public Dictionary<int, List<Vector2i>>? SpaceTiles;

	public Dictionary<NetEntity, Dictionary<int, List<Vector2i>>> Tiles;

	public List<float> Intensity;

	public string ExplosionType = string.Empty;

	public Matrix3x2 SpaceMatrix;

	public ushort SpaceTileSize;

	public ExplosionVisualsState(MapCoordinates epicenter, string typeID, List<float> intensity, Dictionary<int, List<Vector2i>>? spaceTiles, Dictionary<NetEntity, Dictionary<int, List<Vector2i>>> tiles, Matrix3x2 spaceMatrix, ushort spaceTileSize)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		Epicenter = epicenter;
		SpaceTiles = spaceTiles;
		Tiles = tiles;
		Intensity = intensity;
		ExplosionType = typeID;
		SpaceMatrix = spaceMatrix;
		SpaceTileSize = spaceTileSize;
	}
}
