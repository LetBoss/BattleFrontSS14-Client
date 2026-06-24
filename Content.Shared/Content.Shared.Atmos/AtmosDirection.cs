using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos;

[Serializable]
[Flags]
[FlagsFor(typeof(AtmosDirectionFlags))]
public enum AtmosDirection
{
	Invalid = 0,
	North = 1,
	South = 2,
	East = 4,
	West = 8,
	NorthEast = 5,
	SouthEast = 6,
	NorthWest = 9,
	SouthWest = 0xA,
	All = 0xF
}
