using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization;

namespace Content.Shared.Weapons.Melee.Events;

[Serializable]
[NetSerializable]
public sealed class DisarmAttackEvent : AttackEvent
{
	public NetEntity? Target;

	public DisarmAttackEvent(NetEntity? target, NetCoordinates coordinates)
		: base(coordinates)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		Target = target;
	}
}
