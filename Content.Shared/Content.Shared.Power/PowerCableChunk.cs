using System;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Power;

[Serializable]
[NetSerializable]
public struct PowerCableChunk(Vector2i origin)
{
	public readonly Vector2i Origin = origin;

	public int[] PowerCableData = new int[3];
}
