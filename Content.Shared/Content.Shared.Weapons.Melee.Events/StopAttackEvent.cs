using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Weapons.Melee.Events;

[Serializable]
[NetSerializable]
public sealed class StopAttackEvent : EntityEventArgs
{
	public readonly NetEntity Weapon;

	public StopAttackEvent(NetEntity weapon)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		Weapon = weapon;
	}
}
