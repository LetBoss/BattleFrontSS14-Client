using System;

namespace Content.Shared._RMC14.Damage;

[Flags]
public enum DamageMultiplierFlag : byte
{
	None = 0,
	Turf = 1,
	Breaching = 2,
	Xeno = 4
}
