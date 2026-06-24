using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Robust.Shared.Physics.Dynamics.Joints;

[Serializable]
[NetSerializable]
internal sealed class RevoluteJointState : JointState
{
	public bool EnableLimit { get; internal set; }

	public bool EnableMotor { get; internal set; }

	public float ReferenceAngle { get; internal set; }

	public float LowerAngle { get; internal set; }

	public float UpperAngle { get; internal set; }

	public float MotorSpeed { get; internal set; }

	public float MaxMotorTorque { get; internal set; }

	public override Joint GetJoint(IEntityManager entManager, EntityUid owner)
	{
		return new RevoluteJoint(this, entManager, owner);
	}
}
