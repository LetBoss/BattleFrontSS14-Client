using System;
using System.Numerics;
using Robust.Shared.Maths;
using Robust.Shared.Utility;

namespace Robust.Shared.Physics.Collision;

public struct Manifold : IEquatable<Manifold>, IApproxEquatable<Manifold>
{
	public Vector2 LocalNormal;

	public Vector2 LocalPoint;

	public int PointCount;

	internal FixedArray2<ManifoldPoint> Points;

	public ManifoldType Type;

	public bool Equals(Manifold other)
	{
		if (PointCount != other.PointCount || Type != other.Type || !LocalNormal.Equals(other.LocalNormal) || !LocalPoint.Equals(other.LocalPoint))
		{
			return false;
		}
		Span<ManifoldPoint> asSpan = Points.AsSpan;
		Span<ManifoldPoint> asSpan2 = other.Points.AsSpan;
		for (int i = 0; i < PointCount; i++)
		{
			if (!asSpan[i].Equals(asSpan2[i]))
			{
				return false;
			}
		}
		return true;
	}

	public bool EqualsApprox(Manifold other)
	{
		if (PointCount != other.PointCount || Type != other.Type || !Vector2Helpers.EqualsApprox(LocalNormal, other.LocalNormal) || !Vector2Helpers.EqualsApprox(LocalPoint, other.LocalPoint))
		{
			return false;
		}
		Span<ManifoldPoint> asSpan = Points.AsSpan;
		Span<ManifoldPoint> asSpan2 = other.Points.AsSpan;
		for (int i = 0; i < PointCount; i++)
		{
			if (!asSpan[i].EqualsApprox(asSpan2[i]))
			{
				return false;
			}
		}
		return true;
	}

	public bool EqualsApprox(Manifold other, double tolerance)
	{
		if (PointCount != other.PointCount || Type != other.Type || !Vector2Helpers.EqualsApprox(LocalNormal, other.LocalNormal, tolerance) || !Vector2Helpers.EqualsApprox(LocalPoint, other.LocalPoint, tolerance))
		{
			return false;
		}
		Span<ManifoldPoint> asSpan = Points.AsSpan;
		Span<ManifoldPoint> asSpan2 = other.Points.AsSpan;
		for (int i = 0; i < PointCount; i++)
		{
			if (!asSpan[i].EqualsApprox(asSpan2[i], tolerance))
			{
				return false;
			}
		}
		return true;
	}

	public override bool Equals(object? obj)
	{
		if (obj is Manifold other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(LocalNormal, LocalPoint, PointCount, Points, (int)Type);
	}
}
