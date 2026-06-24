using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Dropship.Weapon;

[Serializable]
[NetSerializable]
public sealed class DropshipTerminalWeaponsChooseMedevacMsg(bool first) : BoundUserInterfaceMessage
{
	public readonly bool First = first;
}
