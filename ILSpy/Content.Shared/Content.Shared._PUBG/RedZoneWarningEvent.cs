using System;
using System.Numerics;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG;

[Serializable]
[NetSerializable]
public sealed class RedZoneWarningEvent : EntityEventArgs
{
	public Vector2 Center;

	public float Radius;

	public RedZoneWarningEvent(Vector2 center, float radius)
	{
		Center = center;
		Radius = radius;
	}
}
