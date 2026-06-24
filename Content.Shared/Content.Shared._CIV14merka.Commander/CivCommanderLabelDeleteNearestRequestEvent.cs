using System;
using System.Numerics;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.Commander;

[Serializable]
[NetSerializable]
public sealed class CivCommanderLabelDeleteNearestRequestEvent : EntityEventArgs
{
	public MapId MapId { get; }

	public Vector2 Position { get; }

	public float Radius { get; }

	public CivCommanderLabelDeleteNearestRequestEvent(MapId mapId, Vector2 position, float radius)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		MapId = mapId;
		Position = position;
		Radius = radius;
	}
}
