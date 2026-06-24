using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Dropship.Weapon;

[Serializable]
[NetSerializable]
public sealed class DropshipTerminalWeaponsMedevacSelectMsg(NetEntity target) : BoundUserInterfaceMessage
{
	public readonly NetEntity Target = target;
}
