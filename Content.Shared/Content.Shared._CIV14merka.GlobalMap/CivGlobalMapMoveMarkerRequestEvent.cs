using System;
using System.Numerics;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.GlobalMap;

[Serializable]
[NetSerializable]
public sealed class CivGlobalMapMoveMarkerRequestEvent : EntityEventArgs
{
	public int MarkerId { get; }

	public MapId MapId { get; }

	public Vector2 Position { get; }

	public CivGlobalMapMoveMarkerRequestEvent(int markerId, MapId mapId, Vector2 position)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		MarkerId = markerId;
		MapId = mapId;
		Position = position;
	}
}
