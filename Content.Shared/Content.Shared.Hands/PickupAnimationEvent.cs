using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Hands;

[Serializable]
[NetSerializable]
public sealed class PickupAnimationEvent : EntityEventArgs
{
	public readonly NetEntity ItemUid;

	public readonly NetCoordinates InitialPosition;

	public readonly NetCoordinates FinalPosition;

	public readonly Angle InitialAngle;

	public PickupAnimationEvent(NetEntity itemUid, NetCoordinates initialPosition, NetCoordinates finalPosition, Angle initialAngle)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		ItemUid = itemUid;
		FinalPosition = finalPosition;
		InitialPosition = initialPosition;
		InitialAngle = initialAngle;
	}
}
