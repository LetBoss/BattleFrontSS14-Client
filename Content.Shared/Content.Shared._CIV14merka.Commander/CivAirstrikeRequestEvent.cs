using System;
using System.Numerics;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.Commander;

[Serializable]
[NetSerializable]
public sealed class CivAirstrikeRequestEvent : EntityEventArgs
{
	public Vector2 Target { get; }

	public CivAirstrikeVector Vector { get; }

	public CivAirstrikeRequestEvent(Vector2 target, CivAirstrikeVector vector)
	{
		Target = target;
		Vector = vector;
	}
}
