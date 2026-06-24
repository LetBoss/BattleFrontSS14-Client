using System;
using System.Collections.Generic;
using System.Numerics;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Radiation.Systems;

[Serializable]
[NetSerializable]
public readonly record struct DebugRadiationRay(MapId MapId, NetEntity SourceUid, Vector2 Source, NetEntity DestinationUid, Vector2 Destination, float Rads, Dictionary<NetEntity, List<(Vector2i, float)>> Blockers)
{
	public bool ReachedDestination => Rads > 0f;
}
