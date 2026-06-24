using System;

namespace Robust.Shared.Serialization;

[Flags]
public enum SerializerFloatFlags
{
	None = 0,
	RemoveReadNan = 1
}
