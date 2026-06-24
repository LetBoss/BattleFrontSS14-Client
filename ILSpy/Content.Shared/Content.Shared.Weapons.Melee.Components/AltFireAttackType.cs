using System;

namespace Content.Shared.Weapons.Melee.Components;

[Flags]
public enum AltFireAttackType : byte
{
	Light = 0,
	Heavy = 1,
	Disarm = 2
}
