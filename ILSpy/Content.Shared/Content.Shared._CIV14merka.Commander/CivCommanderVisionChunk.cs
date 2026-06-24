using System;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.Commander;

[Serializable]
[NetSerializable]
public sealed class CivCommanderVisionChunk
{
	public Vector2i Index { get; }

	public byte[] TileStates { get; }

	public CivCommanderVisionChunk(Vector2i index, byte[] tileStates)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		Index = index;
		TileStates = tileStates;
	}
}
