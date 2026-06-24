using System;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Targeting;

[Serializable]
[NetSerializable]
public enum TargetedVisuals : byte
{
	Targeted,
	TargetedDirection,
	TargetedDirectionIntense
}
