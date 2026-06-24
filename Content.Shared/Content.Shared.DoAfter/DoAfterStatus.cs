using System;
using Robust.Shared.Serialization;

namespace Content.Shared.DoAfter;

[Serializable]
[NetSerializable]
public enum DoAfterStatus : byte
{
	Invalid,
	Running,
	Cancelled,
	Finished
}
