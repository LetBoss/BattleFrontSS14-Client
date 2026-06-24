using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos;

[Serializable]
[Flags]
[NetSerializable]
public enum PipeDirection
{
	None = 0,
	North = 1,
	South = 2,
	West = 4,
	East = 8,
	Longitudinal = 3,
	Lateral = 0xC,
	NWBend = 5,
	NEBend = 9,
	SWBend = 6,
	SEBend = 0xA,
	TNorth = 0xD,
	TSouth = 0xE,
	TWest = 7,
	TEast = 0xB,
	Fourway = 0xF,
	All = -1
}
