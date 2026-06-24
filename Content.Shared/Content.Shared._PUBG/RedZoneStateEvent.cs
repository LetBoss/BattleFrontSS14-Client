using System;
using System.Numerics;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG;

[Serializable]
[NetSerializable]
public sealed class RedZoneStateEvent : EntityEventArgs
{
	public bool ZoneActive;

	public Vector2 ZoneCenter;

	public float ZoneRadius;

	public bool HasActiveBomb;

	public Vector2 BombCenter;

	public float BombRadius;

	public RedZoneStateEvent(bool zoneActive, Vector2 zoneCenter, float zoneRadius, bool hasActiveBomb, Vector2 bombCenter, float bombRadius)
	{
		ZoneActive = zoneActive;
		ZoneCenter = zoneCenter;
		ZoneRadius = zoneRadius;
		HasActiveBomb = hasActiveBomb;
		BombCenter = bombCenter;
		BombRadius = bombRadius;
	}
}
