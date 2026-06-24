using System;
using System.Numerics;
using Robust.Shared.Map;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.MineTable;

[Serializable]
[NetSerializable]
public struct CivMineRevealEntry(MapId mapId, Vector2 position)
{
	public MapId MapId = mapId;

	public Vector2 Position = position;
}
