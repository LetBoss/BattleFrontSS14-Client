using System.Numerics;

namespace Robust.Shared.Physics.Collision;

public struct DistanceOutput
{
	public float Distance;

	public int Iterations;

	public Vector2 PointA;

	public Vector2 PointB;
}
