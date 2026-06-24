using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Pinpointer;

[Serializable]
[NetSerializable]
public enum Distance : byte
{
	Unknown,
	Reached,
	Close,
	Medium,
	Far
}
