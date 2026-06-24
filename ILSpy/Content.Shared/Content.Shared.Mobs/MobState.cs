using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Mobs;

[Serializable]
[NetSerializable]
public enum MobState : byte
{
	Invalid,
	Alive,
	Critical,
	Dead
}
