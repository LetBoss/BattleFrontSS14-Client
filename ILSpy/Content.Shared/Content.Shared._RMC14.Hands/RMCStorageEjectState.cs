using System;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Hands;

[Serializable]
[NetSerializable]
public enum RMCStorageEjectState : byte
{
	Last,
	First,
	Unequip,
	Open
}
