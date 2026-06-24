using System;
using System.Numerics;
using Robust.Shared.Maths;

namespace Robust.Shared.Physics;

public struct Quaternion2D
{
	public float C;

	public float S;

	public float Angle => MathF.Atan2(S, C);

	public Quaternion2D(float cos, float sin)
	{
		C = cos;
		S = sin;
	}

	public Quaternion2D(float angle)
	{
		C = MathF.Cos(angle);
		S = MathF.Sin(angle);
	}

	public Quaternion2D(Angle angle)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		double theta = angle.Theta;
		C = (float)Math.Cos(theta);
		S = (float)Math.Sin(theta);
	}

	public Quaternion2D Set(float angle)
	{
		if (angle == 0f)
		{
			return new Quaternion2D(1f, 0f);
		}
		return new Quaternion2D(MathF.Cos(angle), MathF.Sin(angle));
	}

	public static Vector2 RotateVector(Quaternion2D q, Vector2 v)
	{
		return new Vector2(q.C * v.X - q.S * v.Y, q.S * v.X + q.C * v.Y);
	}

	public static Vector2 InvRotateVector(Quaternion2D q, Vector2 v)
	{
		return new Vector2(q.C * v.X + q.S * v.Y, (0f - q.S) * v.X + q.C * v.Y);
	}

	public bool IsValid()
	{
		if (float.IsNaN(S) || float.IsNaN(C))
		{
			return false;
		}
		if (float.IsInfinity(S) || float.IsInfinity(C))
		{
			return false;
		}
		return IsNormalized();
	}

	public bool IsNormalized()
	{
		float num = S * S + C * C;
		if (0.9994f < num)
		{
			return num < 1.0006f;
		}
		return false;
	}

	public static Quaternion2D InvMulRot(Quaternion2D q, Quaternion2D r)
	{
		return new Quaternion2D(q.C * r.C + q.S * r.S, q.C * r.S - q.S * r.C);
	}
}
