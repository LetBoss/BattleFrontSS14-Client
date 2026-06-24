using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Components;

[Serializable]
[NetSerializable]
public sealed class GasTileOverlayState(Dictionary<Vector2i, GasOverlayChunk> chunks) : ComponentState
{
	public readonly Dictionary<Vector2i, GasOverlayChunk> Chunks = chunks;
}
