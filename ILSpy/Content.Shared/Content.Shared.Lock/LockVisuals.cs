using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Lock;

[Serializable]
[NetSerializable]
public enum LockVisuals : byte
{
	Locked
}
