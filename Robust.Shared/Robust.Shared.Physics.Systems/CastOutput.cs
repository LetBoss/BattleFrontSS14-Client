using System.Numerics;

namespace Robust.Shared.Physics.Systems;

internal ref struct CastOutput
{
	public Vector2 Normal;

	public Vector2 Point;

	public float Fraction;

	public int Iterations;

	public bool Hit;
}
