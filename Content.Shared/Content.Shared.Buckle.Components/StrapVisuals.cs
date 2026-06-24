using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Buckle.Components;

[Serializable]
[NetSerializable]
public enum StrapVisuals : byte
{
	RotationAngle,
	State
}
