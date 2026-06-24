using System;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Xenonids.ClawSharpness;

[Serializable]
[Flags]
[NetSerializable]
public enum XenoClawType
{
	Normal = 0,
	Sharp = 1,
	VerySharp = 2,
	ImpossiblySharp = 3
}
