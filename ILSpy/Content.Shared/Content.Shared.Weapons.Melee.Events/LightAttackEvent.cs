using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization;

namespace Content.Shared.Weapons.Melee.Events;

[Serializable]
[NetSerializable]
public sealed class LightAttackEvent : AttackEvent
{
	public readonly NetEntity? Target;

	public readonly NetEntity Weapon;

	public LightAttackEvent(NetEntity? target, NetEntity weapon, NetCoordinates coordinates)
		: base(coordinates)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		Target = target;
		Weapon = weapon;
	}
}
