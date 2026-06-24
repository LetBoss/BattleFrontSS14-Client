using System;
using System.Numerics;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.Commander;

[Serializable]
[NetSerializable]
public sealed class CivArtilleryRequestEvent : EntityEventArgs
{
	public Vector2 Target { get; }

	public CivArtilleryRequestEvent(Vector2 target)
	{
		Target = target;
	}
}
