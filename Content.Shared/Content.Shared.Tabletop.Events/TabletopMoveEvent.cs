using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization;

namespace Content.Shared.Tabletop.Events;

[Serializable]
[NetSerializable]
public sealed class TabletopMoveEvent : EntityEventArgs
{
	public NetEntity MovedEntityUid { get; }

	public MapCoordinates Coordinates { get; }

	public NetEntity TableUid { get; }

	public TabletopMoveEvent(NetEntity movedEntityUid, MapCoordinates coordinates, NetEntity tableUid)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		MovedEntityUid = movedEntityUid;
		Coordinates = coordinates;
		TableUid = tableUid;
	}
}
