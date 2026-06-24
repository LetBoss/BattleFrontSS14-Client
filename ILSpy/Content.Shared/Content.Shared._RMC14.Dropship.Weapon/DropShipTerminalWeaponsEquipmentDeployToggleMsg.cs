using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Dropship.Weapon;

[Serializable]
[NetSerializable]
public sealed class DropShipTerminalWeaponsEquipmentDeployToggleMsg(bool deploy) : BoundUserInterfaceMessage
{
	public readonly bool Deploy = deploy;
}
