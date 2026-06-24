using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.Commander;

[Serializable]
[NetSerializable]
public sealed class CivCommanderVisionUpdateEvent : EntityEventArgs
{
	public NetEntity GridId { get; }

	public CivCommanderVisionChunk[] Chunks { get; }

	public CivCommanderVisionUpdateEvent(NetEntity gridId, CivCommanderVisionChunk[] chunks)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		GridId = gridId;
		Chunks = chunks;
	}
}
