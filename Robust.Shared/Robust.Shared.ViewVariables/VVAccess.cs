using System;
using Robust.Shared.Serialization;

namespace Robust.Shared.ViewVariables;

[Serializable]
[NetSerializable]
public enum VVAccess : byte
{
	ReadOnly,
	ReadWrite
}
