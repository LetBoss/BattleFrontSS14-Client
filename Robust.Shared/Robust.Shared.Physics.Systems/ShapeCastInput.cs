using System.Numerics;

namespace Robust.Shared.Physics.Systems;

internal record struct ShapeCastInput
{
	public Transform Origin;

	public Vector2[] Points;

	public int Count;

	public float Radius;

	public Vector2 Translation;

	public float MaxFraction;
}
