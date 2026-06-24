using System;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Weapons;

[Serializable]
[NetSerializable]
public enum RMCWeaponProfileFrameMode : byte
{
	Final,
	Viewport
}
