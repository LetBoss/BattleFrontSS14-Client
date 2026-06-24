using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.WorldEdit;

[Serializable]
[NetSerializable]
public sealed class WorldEditPlacePreviewEvent : EntityEventArgs
{
	public NetCoordinates Coordinates { get; }

	public Angle Rotation { get; }

	public WorldEditPlacePreviewEvent(NetCoordinates coordinates, Angle rotation)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		Coordinates = coordinates;
		Rotation = rotation;
	}
}
