using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Dropship.Weapon;

[Serializable]
[NetSerializable]
public sealed class DropshipTerminalWeaponsChangeScreenMsg(bool first, DropshipTerminalWeaponsScreen screen) : BoundUserInterfaceMessage
{
	public readonly bool First = first;

	public readonly DropshipTerminalWeaponsScreen Screen = screen;
}
