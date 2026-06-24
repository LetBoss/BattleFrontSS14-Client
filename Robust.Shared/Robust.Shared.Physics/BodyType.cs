using System;
using Robust.Shared.Serialization;

namespace Robust.Shared.Physics;

[Serializable]
[NetSerializable]
[Flags]
public enum BodyType : byte
{
	Kinematic = 0,
	KinematicController = 2,
	Static = 4,
	Dynamic = 8
}
