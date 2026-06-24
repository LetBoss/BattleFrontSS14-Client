using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Solar;

[Serializable]
[NetSerializable]
public sealed class SolarControlConsoleAdjustMessage : BoundUserInterfaceMessage
{
	public Angle Rotation;

	public Angle AngularVelocity;
}
