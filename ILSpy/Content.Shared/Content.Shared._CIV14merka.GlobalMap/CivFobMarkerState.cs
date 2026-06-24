using System;
using System.Numerics;
using Robust.Shared.Map;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.GlobalMap;

[Serializable]
[NetSerializable]
public sealed class CivFobMarkerState
{
	public MapId MapId { get; }

	public Vector2 Position { get; }

	public CivFobMarkerState(MapId mapId, Vector2 position)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		MapId = mapId;
		Position = position;
	}
}
