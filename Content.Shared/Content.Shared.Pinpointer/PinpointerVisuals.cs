using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Pinpointer;

[Serializable]
[NetSerializable]
public enum PinpointerVisuals : byte
{
	IsActive,
	ArrowAngle,
	TargetDistance
}
