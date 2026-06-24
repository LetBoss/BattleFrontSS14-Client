using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Piping.Portable.Components;

[Serializable]
[NetSerializable]
public enum SpaceHeaterMode : byte
{
	Auto,
	Heat,
	Cool
}
