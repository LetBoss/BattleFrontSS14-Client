using System;
using System.Numerics;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Robust.Shared.Physics.Components;

[Serializable]
[NetSerializable]
public sealed class PhysicsComponentState : IComponentState
{
	public bool CanCollide;

	public bool SleepingAllowed;

	public bool FixedRotation;

	public BodyStatus Status;

	public Vector2 LinearVelocity;

	public float AngularVelocity;

	public BodyType BodyType;

	public float Friction;

	public float LinearDamping;

	public float AngularDamping;

	public Vector2 Force;

	public float Torque;

	public PhysicsComponentState()
	{
	}

	public PhysicsComponentState(PhysicsComponentState existing)
	{
		CanCollide = existing.CanCollide;
		SleepingAllowed = existing.SleepingAllowed;
		FixedRotation = existing.FixedRotation;
		Status = existing.Status;
		LinearVelocity = existing.LinearVelocity;
		AngularVelocity = existing.AngularVelocity;
		BodyType = existing.BodyType;
		Friction = existing.Friction;
		LinearDamping = existing.LinearDamping;
		AngularDamping = existing.AngularDamping;
		Force = existing.Force;
		Torque = existing.Torque;
	}
}
