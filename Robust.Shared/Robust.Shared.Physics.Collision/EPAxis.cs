using System.Numerics;

namespace Robust.Shared.Physics.Collision;

public struct EPAxis
{
	public int Index;

	public float Separation;

	public EPAxisType Type;

	public Vector2 Normal;
}
