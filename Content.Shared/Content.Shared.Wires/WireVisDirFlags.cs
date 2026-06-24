using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Wires;

[Serializable]
[Flags]
[NetSerializable]
public enum WireVisDirFlags : byte
{
	None = 0,
	North = 1,
	South = 2,
	East = 4,
	West = 8
}
