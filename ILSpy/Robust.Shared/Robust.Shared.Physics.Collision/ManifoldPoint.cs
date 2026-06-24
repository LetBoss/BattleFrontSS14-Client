using System;
using System.Numerics;
using Robust.Shared.Maths;

namespace Robust.Shared.Physics.Collision;

public struct ManifoldPoint : IEquatable<ManifoldPoint>, IApproxEquatable<ManifoldPoint>
{
	public ContactID Id;

	public Vector2 LocalPoint;

	public float NormalImpulse;

	public float TangentImpulse;

	public static bool operator ==(ManifoldPoint point, ManifoldPoint other)
	{
		if (point.Id == other.Id && point.LocalPoint.Equals(other.LocalPoint) && point.NormalImpulse.Equals(other.NormalImpulse))
		{
			return point.TangentImpulse.Equals(other.TangentImpulse);
		}
		return false;
	}

	public static bool operator !=(ManifoldPoint point, ManifoldPoint other)
	{
		return !(point == other);
	}

	public override bool Equals(object? obj)
	{
		if (!(obj is ManifoldPoint manifoldPoint))
		{
			return false;
		}
		return this == manifoldPoint;
	}

	public bool Equals(ManifoldPoint other)
	{
		return this == other;
	}

	public override int GetHashCode()
	{
		return (((((Id.GetHashCode() * 397) ^ LocalPoint.GetHashCode()) * 397) ^ NormalImpulse.GetHashCode()) * 397) ^ TangentImpulse.GetHashCode();
	}

	public bool EqualsApprox(ManifoldPoint other)
	{
		if (Id == other.Id && Vector2Helpers.EqualsApprox(LocalPoint, other.LocalPoint) && MathHelper.CloseToPercent(NormalImpulse, other.NormalImpulse, 1E-05))
		{
			return MathHelper.CloseToPercent(TangentImpulse, other.TangentImpulse, 1E-05);
		}
		return false;
	}

	public bool EqualsApprox(ManifoldPoint other, double tolerance)
	{
		if (Id == other.Id && Vector2Helpers.EqualsApprox(LocalPoint, other.LocalPoint, tolerance) && MathHelper.CloseToPercent(NormalImpulse, other.NormalImpulse, tolerance))
		{
			return MathHelper.CloseToPercent(TangentImpulse, other.TangentImpulse, tolerance);
		}
		return false;
	}
}
