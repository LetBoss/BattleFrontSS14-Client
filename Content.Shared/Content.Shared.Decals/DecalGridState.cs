using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Decals;

[Serializable]
[NetSerializable]
public sealed class DecalGridState(Dictionary<Vector2i, DecalGridComponent.DecalChunk> chunks) : ComponentState
{
	public Dictionary<Vector2i, DecalGridComponent.DecalChunk> Chunks = chunks;
}
