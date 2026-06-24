using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Weapons.Reflect;

[Serializable]
[Flags]
[NetSerializable]
public enum ReflectType : byte
{
	None = 0,
	NonEnergy = 1,
	Energy = 2
}
