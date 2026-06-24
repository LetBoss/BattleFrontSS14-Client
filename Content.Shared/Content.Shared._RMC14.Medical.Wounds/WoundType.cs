using System;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Medical.Wounds;

[Serializable]
[NetSerializable]
public enum WoundType
{
	Brute,
	Burn,
	Surgery
}
