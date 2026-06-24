using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Dropship.Weapon;

[Serializable]
[NetSerializable]
public sealed class DropshipTerminalWeaponsNightVisionMsg(bool on) : BoundUserInterfaceMessage
{
	public readonly bool On = on;
}
