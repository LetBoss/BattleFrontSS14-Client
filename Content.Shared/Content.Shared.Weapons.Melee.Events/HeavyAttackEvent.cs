using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization;

namespace Content.Shared.Weapons.Melee.Events;

[Serializable]
[NetSerializable]
public sealed class HeavyAttackEvent : AttackEvent
{
	public readonly NetEntity Weapon;

	public List<NetEntity> Entities;

	public HeavyAttackEvent(NetEntity weapon, List<NetEntity> entities, NetCoordinates coordinates)
		: base(coordinates)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		Weapon = weapon;
		Entities = entities;
	}
}
