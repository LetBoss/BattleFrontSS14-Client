using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Robust.Shared.Physics;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct PhysicsHull
{
	public static Span<Vector2> ComputePoints(ReadOnlySpan<Vector2> points, int count)
	{
		return InternalPhysicsHull.ComputeHull(points, count).Points;
	}
}
