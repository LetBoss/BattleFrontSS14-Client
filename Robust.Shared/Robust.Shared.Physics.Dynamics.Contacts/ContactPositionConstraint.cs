using System.Numerics;
using Robust.Shared.Physics.Collision;
using Robust.Shared.Utility;

namespace Robust.Shared.Physics.Dynamics.Contacts;

internal struct ContactPositionConstraint
{
	internal FixedArray2<Vector2> LocalPoints;

	public Vector2 LocalNormal;

	public Vector2 LocalPoint;

	public float InvMassA;

	public float InvMassB;

	public Vector2 LocalCenterA;

	public Vector2 LocalCenterB;

	public float InvIA;

	public float InvIB;

	public ManifoldType Type;

	public float RadiusA;

	public float RadiusB;

	public int PointCount;

	public int IndexA { get; set; }

	public int IndexB { get; set; }
}
