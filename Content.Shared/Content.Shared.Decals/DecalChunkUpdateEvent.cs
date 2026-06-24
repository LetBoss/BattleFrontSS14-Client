using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Decals;

[Serializable]
[NetSerializable]
public sealed class DecalChunkUpdateEvent : EntityEventArgs
{
	public Dictionary<NetEntity, Dictionary<Vector2i, DecalGridComponent.DecalChunk>> Data = new Dictionary<NetEntity, Dictionary<Vector2i, DecalGridComponent.DecalChunk>>();

	public Dictionary<NetEntity, HashSet<Vector2i>> RemovedChunks = new Dictionary<NetEntity, HashSet<Vector2i>>();
}
