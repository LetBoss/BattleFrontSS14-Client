using System;

namespace Robust.Shared.GameObjects;

[Flags]
public enum LookupFlags : byte
{
	None = 0,
	Approximate = 1,
	Dynamic = 2,
	Static = 4,
	Sundries = 8,
	Contained = 0x20,
	Sensors = 0x40,
	Uncontained = 0x4E,
	StaticSundries = 0xC,
	All = 0x6E
}
