using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Singularity.Components;

[Serializable]
[NetSerializable]
public enum PowerLevelVisuals : byte
{
	NoPower,
	LowPower,
	MediumPower,
	HighPower
}
