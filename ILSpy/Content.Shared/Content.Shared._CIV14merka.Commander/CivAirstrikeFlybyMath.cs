using System;
using System.Numerics;

namespace Content.Shared._CIV14merka.Commander;

public static class CivAirstrikeFlybyMath
{
	public static Vector2 Left(Vector2 value)
	{
		return new Vector2(0f - value.Y, value.X);
	}

	public static float ArcAngle(Vector2 from, Vector2 to, bool ccw)
	{
		float start = MathF.Atan2(from.Y, from.X);
		float end = MathF.Atan2(to.Y, to.X);
		float delta;
		for (delta = (ccw ? (end - start) : (start - end)); delta < 0f; delta += (float)Math.PI * 2f)
		{
		}
		while (delta >= (float)Math.PI * 2f)
		{
			delta -= (float)Math.PI * 2f;
		}
		return delta;
	}

	public static float ArcLen(Vector2 center, Vector2 from, Vector2 to, bool ccw)
	{
		float radius = (from - center).Length();
		if (radius <= 0.001f)
		{
			return 0f;
		}
		return ArcAngle(from - center, to - center, ccw) * radius;
	}

	public static Vector2 ArcPos(Vector2 center, Vector2 from, Vector2 to, bool ccw, float dist)
	{
		Vector2 radiusVec = from - center;
		float radius = radiusVec.Length();
		if (radius <= 0.001f)
		{
			return to;
		}
		float total = ArcAngle(radiusVec, to - center, ccw);
		if (total <= 0.001f)
		{
			return to;
		}
		float step = MathF.Min(dist / radius, total);
		float start = MathF.Atan2(radiusVec.Y, radiusVec.X);
		float angle = (ccw ? (start + step) : (start - step));
		return center + new Vector2(MathF.Cos(angle), MathF.Sin(angle)) * radius;
	}

	public static Vector2 ArcTangent(Vector2 center, Vector2 pos, bool ccw)
	{
		Vector2 radius = pos - center;
		if (radius.LengthSquared() <= 0.0001f)
		{
			return Vector2.UnitX;
		}
		Vector2 tangent = Left(Vector2.Normalize(radius));
		if (!ccw)
		{
			return -tangent;
		}
		return tangent;
	}
}
