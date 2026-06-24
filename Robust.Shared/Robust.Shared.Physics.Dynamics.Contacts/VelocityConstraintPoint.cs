using System.Numerics;

namespace Robust.Shared.Physics.Dynamics.Contacts;

internal struct VelocityConstraintPoint
{
	public Vector2 RelativeVelocityA;

	public Vector2 RelativeVelocityB;

	public float NormalImpulse;

	public float TangentImpulse;

	public float NormalMass;

	public float TangentMass;

	public float VelocityBias;
}
