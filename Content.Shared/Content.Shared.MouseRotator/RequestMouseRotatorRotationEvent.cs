using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.MouseRotator;

[Serializable]
[NetSerializable]
public sealed class RequestMouseRotatorRotationEvent : EntityEventArgs
{
	public Angle Rotation;
}
