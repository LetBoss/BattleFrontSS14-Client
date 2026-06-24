using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Dropship.Weapon;

[Serializable]
[NetSerializable]
public sealed class DropshipTerminalWeaponsChooseSpotlightMsg(bool first, NetEntity slot) : BoundUserInterfaceMessage
{
	public readonly bool First = first;

	public readonly NetEntity Slot = slot;
}
