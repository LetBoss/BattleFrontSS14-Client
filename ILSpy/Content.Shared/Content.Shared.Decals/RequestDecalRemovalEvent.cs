using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization;

namespace Content.Shared.Decals;

[Serializable]
[NetSerializable]
public sealed class RequestDecalRemovalEvent : EntityEventArgs
{
	public NetCoordinates Coordinates;

	public RequestDecalRemovalEvent(NetCoordinates coordinates)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		Coordinates = coordinates;
	}
}
