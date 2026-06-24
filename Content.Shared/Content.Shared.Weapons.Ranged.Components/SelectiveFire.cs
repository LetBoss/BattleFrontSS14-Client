using System;

namespace Content.Shared.Weapons.Ranged.Components;

[Flags]
public enum SelectiveFire : byte
{
	Invalid = 0,
	SemiAuto = 1,
	Burst = 2,
	FullAuto = 4
}
