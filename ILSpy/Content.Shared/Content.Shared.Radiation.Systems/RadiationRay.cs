using System.Collections.Generic;
using System.Numerics;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Maths;

namespace Content.Shared.Radiation.Systems;

public struct RadiationRay(MapId mapId, EntityUid sourceUid, Vector2 source, EntityUid destinationUid, Vector2 destination, float rads)
{
	public MapId MapId = mapId;

	public EntityUid SourceUid = sourceUid;

	public Vector2 Source = source;

	public EntityUid DestinationUid = destinationUid;

	public Vector2 Destination = destination;

	public float Rads = rads;

	public Dictionary<NetEntity, List<(Vector2i, float)>>? Blockers = null;

	public bool ReachedDestination => Rads > 0f;
}
