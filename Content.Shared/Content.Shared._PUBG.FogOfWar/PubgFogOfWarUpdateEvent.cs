using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.FogOfWar;

[Serializable]
[NetSerializable]
public sealed class PubgFogOfWarUpdateEvent : EntityEventArgs
{
	public NetEntity GridId { get; }

	public PubgFogOfWarChunk[] Chunks { get; }

	public PubgFogOfWarUpdateEvent(NetEntity gridId, PubgFogOfWarChunk[] chunks)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		GridId = gridId;
		Chunks = chunks;
	}
}
