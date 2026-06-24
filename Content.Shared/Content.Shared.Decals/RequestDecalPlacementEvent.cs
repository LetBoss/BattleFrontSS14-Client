using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization;

namespace Content.Shared.Decals;

[Serializable]
[NetSerializable]
public sealed class RequestDecalPlacementEvent : EntityEventArgs
{
	public Decal Decal;

	public NetCoordinates Coordinates;

	public RequestDecalPlacementEvent(Decal decal, NetCoordinates coordinates)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		Decal = decal;
		Coordinates = coordinates;
	}
}
