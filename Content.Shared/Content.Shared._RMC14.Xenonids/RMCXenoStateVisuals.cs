using System;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Xenonids;

[Serializable]
[NetSerializable]
public enum RMCXenoStateVisuals
{
	Resting,
	Downed,
	Fortified,
	Ovipositor,
	Dead
}
