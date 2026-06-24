using System;
using System.Numerics;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Robust.Shared.Physics.Dynamics.Joints;

[Serializable]
[NetSerializable]
internal sealed class PrismaticJointState : JointState
{
	public Vector2 LocalAxisA;

	public float ReferenceAngle;

	public bool EnableLimit;

	public float LowerTranslation;

	public float UpperTranslation;

	public bool EnableMotor;

	public float MaxMotorForce;

	public float MotorSpeed;

	public override Joint GetJoint(IEntityManager entManager, EntityUid owner)
	{
		return new PrismaticJoint(this, entManager, owner);
	}
}
