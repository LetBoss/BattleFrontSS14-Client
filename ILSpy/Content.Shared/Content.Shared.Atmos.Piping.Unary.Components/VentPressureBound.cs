using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Piping.Unary.Components;

[Serializable]
[Flags]
[NetSerializable]
public enum VentPressureBound : sbyte
{
	NoBound = 0,
	InternalBound = 1,
	ExternalBound = 2,
	Both = 3
}
