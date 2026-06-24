using System;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.FogOfWar;

[Serializable]
[NetSerializable]
public sealed class PubgFogOfWarChunk
{
	public Vector2i Index { get; }

	public byte[] TileStates { get; }

	public PubgFogOfWarChunk(Vector2i index, byte[] tileStates)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		Index = index;
		TileStates = tileStates;
	}
}
