using System;
using System.Numerics;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Weapons.Ranged.Events;

[Serializable]
[NetSerializable]
public sealed class MuzzleFlashEvent : EntityEventArgs
{
	public NetEntity Uid;

	public string Prototype;

	public Vector2 Offset;

	public Vector2 OriginOffset;

	public Angle Angle;

	public MuzzleFlashEvent(NetEntity uid, string prototype, Angle angle, Vector2 offset = default(Vector2), Vector2 originOffset = default(Vector2))
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		Uid = uid;
		Prototype = prototype;
		Angle = angle;
		Offset = offset;
		OriginOffset = originOffset;
	}
}
