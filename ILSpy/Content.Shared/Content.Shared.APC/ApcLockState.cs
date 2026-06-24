using System;
using Robust.Shared.Serialization;

namespace Content.Shared.APC;

[Serializable]
[NetSerializable]
public enum ApcLockState : sbyte
{
	None = 0,
	Lock = 1,
	Unlocked = 0,
	Locked = 1,
	All = 1,
	LogWidth = 0
}
