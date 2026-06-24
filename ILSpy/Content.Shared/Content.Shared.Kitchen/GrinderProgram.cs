using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Kitchen;

[Serializable]
[NetSerializable]
public enum GrinderProgram : byte
{
	Grind,
	Juice
}
