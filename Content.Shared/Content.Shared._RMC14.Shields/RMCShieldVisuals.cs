using System;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Shields;

[Serializable]
[NetSerializable]
public enum RMCShieldVisuals
{
	Base,
	Current,
	Max,
	Active,
	Prefix
}
