using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Rotation;

[Serializable]
[NetSerializable]
public enum RotationState
{
	Vertical,
	Horizontal
}
