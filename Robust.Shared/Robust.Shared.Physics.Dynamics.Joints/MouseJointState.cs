using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Robust.Shared.Physics.Dynamics.Joints;

[Serializable]
[NetSerializable]
internal sealed class MouseJointState : JointState
{
	public float MaxForce { get; internal set; }

	public float Stiffness { get; internal set; }

	public float Damping { get; internal set; }

	public override Joint GetJoint(IEntityManager entManager, EntityUid owner)
	{
		return new MouseJoint(this, entManager, owner);
	}
}
