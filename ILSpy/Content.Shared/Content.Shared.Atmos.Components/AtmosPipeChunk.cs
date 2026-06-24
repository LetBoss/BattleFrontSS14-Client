using System;
using System.Collections.Generic;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Timing;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Atmos.Components;

[Serializable]
[NetSerializable]
public struct AtmosPipeChunk(Vector2i origin)
{
	[ViewVariables]
	public readonly Vector2i Origin = origin;

	[ViewVariables]
	public Dictionary<AtmosMonitoringConsoleSubnet, ulong> AtmosPipeData = new Dictionary<AtmosMonitoringConsoleSubnet, ulong>();

	[NonSerialized]
	public GameTick LastUpdate = default(GameTick);
}
