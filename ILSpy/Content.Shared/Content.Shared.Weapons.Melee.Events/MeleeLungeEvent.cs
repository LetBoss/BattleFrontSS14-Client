using System;
using System.Numerics;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Weapons.Melee.Events;

[Serializable]
[NetSerializable]
public sealed class MeleeLungeEvent : EntityEventArgs
{
	public NetEntity Entity;

	public NetEntity Weapon;

	public Angle Angle;

	public Vector2 LocalPos;

	public string? Animation;

	public MeleeLungeEvent(NetEntity entity, NetEntity weapon, Angle angle, Vector2 localPos, string? animation)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		Entity = entity;
		Weapon = weapon;
		Angle = angle;
		LocalPos = localPos;
		Animation = animation;
	}
}
