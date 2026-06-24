using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Robust.Shared.Physics.Dynamics.Joints;

[Serializable]
[NetSerializable]
public sealed class FrictionJointState : JointState
{
	public float MaxForce { get; }

	public float MaxTorque { get; }

	public override Joint GetJoint(IEntityManager entManager, EntityUid owner)
	{
		return new FrictionJoint(this, entManager, owner);
	}
}
