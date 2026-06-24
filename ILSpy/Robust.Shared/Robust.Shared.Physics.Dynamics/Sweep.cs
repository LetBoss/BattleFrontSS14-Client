using System;
using System.Numerics;

namespace Robust.Shared.Physics.Dynamics;

internal struct Sweep
{
	public Vector2 LocalCenter;

	public float Angle;

	public float Angle0;

	public float Alpha0;

	public Vector2 Center;

	public Vector2 Center0;

	public Transform GetTransform(float beta)
	{
		Transform result = new Transform(Center0 * (1f - beta) + Center * beta, (1f - beta) * Angle0 + beta * Angle);
		result.Position -= Transform.Mul(in result.Quaternion2D, in LocalCenter);
		return result;
	}

	public void Advance(float alpha)
	{
		float num = (alpha - Alpha0) / (1f - Alpha0);
		Center0 += (Center - Center0) * num;
		Angle0 += num * (Angle - Angle0);
		Alpha0 = alpha;
	}

	public void Normalize()
	{
		float num = (float)Math.PI * 2f;
		float num2 = num * MathF.Floor(Angle0 / num);
		Angle0 -= num2;
		Angle -= num2;
	}
}
