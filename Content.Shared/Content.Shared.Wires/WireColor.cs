using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Wires;

[Serializable]
[NetSerializable]
public enum WireColor : byte
{
	Red,
	Blue,
	Green,
	Orange,
	Brown,
	Gold,
	Gray,
	Cyan,
	Navy,
	Purple,
	Pink,
	Fuchsia
}
