using System;
using Content.Shared.FixedPoint;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Fluids;

[Serializable]
[NetSerializable]
public readonly struct PuddleDebugOverlayData(Vector2i pos, FixedPoint2 currentVolume)
{
	public readonly Vector2i Pos = pos;

	public readonly FixedPoint2 CurrentVolume = currentVolume;
}
