using System;

namespace Robust.Shared.Physics.Systems;

[Flags]
public enum QueryFlags : byte
{
	None = 0,
	Dynamic = 1,
	Static = 2,
	Sensors = 4
}
