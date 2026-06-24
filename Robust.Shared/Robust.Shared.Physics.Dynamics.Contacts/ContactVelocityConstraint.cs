using System.Numerics;
using Robust.Shared.Utility;

namespace Robust.Shared.Physics.Dynamics.Contacts;

internal struct ContactVelocityConstraint
{
	public int ContactIndex;

	public int IndexA;

	public int IndexB;

	public FixedArray2<VelocityConstraintPoint> Points;

	public Vector2 Normal;

	public Vector4 NormalMass;

	public Vector4 K;

	public float InvMassA;

	public float InvMassB;

	public float InvIA;

	public float InvIB;

	public float Friction;

	public float Restitution;

	public float TangentSpeed;

	public int PointCount;
}
