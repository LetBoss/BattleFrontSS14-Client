using System;
using System.Numerics;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Robust.Shared.Physics.Dynamics.Joints;

[Serializable]
[NetSerializable]
public abstract class JointState
{
	public string ID { get; internal set; }

	public bool Enabled { get; internal set; }

	public bool CollideConnected { get; internal set; }

	public NetEntity UidA { get; internal set; }

	public NetEntity UidB { get; internal set; }

	public Vector2 LocalAnchorA { get; internal set; }

	public Vector2 LocalAnchorB { get; internal set; }

	public float Breakpoint { get; internal set; }

	public abstract Joint GetJoint(IEntityManager entManager, EntityUid owner);
}
