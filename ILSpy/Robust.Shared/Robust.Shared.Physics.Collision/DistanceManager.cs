using System;
using System.Numerics;
using Robust.Shared.Maths;

namespace Robust.Shared.Physics.Collision;

internal static class DistanceManager
{
	private const byte MaxGJKIterations = 20;

	public static void ComputeDistance(out DistanceOutput output, out SimplexCache cache, in DistanceInput input)
	{
		cache = default(SimplexCache);
		DistanceProxy proxyA = input.ProxyA;
		DistanceProxy proxyB = input.ProxyB;
		Simplex simplex = default(Simplex);
		simplex.ReadCache(ref cache, proxyA, in input.TransformA, proxyB, in input.TransformB);
		Span<int> span = stackalloc int[3];
		Span<int> span2 = stackalloc int[3];
		span.Clear();
		span2.Clear();
		Span<SimplexVertex> asSpan = simplex.V.AsSpan;
		int num = 0;
		while (num < 20)
		{
			int count = simplex.Count;
			for (int i = 0; i < count; i++)
			{
				span[i] = asSpan[i].IndexA;
				span2[i] = asSpan[i].IndexB;
			}
			switch (simplex.Count)
			{
			case 2:
				simplex.Solve2();
				break;
			case 3:
				simplex.Solve3();
				break;
			default:
				throw new ArgumentOutOfRangeException();
			case 1:
				break;
			}
			if (simplex.Count == 3)
			{
				break;
			}
			Vector2 searchDirection = simplex.GetSearchDirection();
			if (searchDirection.LengthSquared() < 0f)
			{
				break;
			}
			SimplexVertex simplexVertex = asSpan[simplex.Count];
			simplexVertex.IndexA = proxyA.GetSupport(Transform.MulT(input.TransformA.Quaternion2D, -searchDirection));
			simplexVertex.WA = Transform.Mul(in input.TransformA, in proxyA.Vertices[simplexVertex.IndexA]);
			simplexVertex.IndexB = proxyB.GetSupport(Transform.MulT(input.TransformB.Quaternion2D, searchDirection));
			simplexVertex.WB = Transform.Mul(in input.TransformB, in proxyB.Vertices[simplexVertex.IndexB]);
			simplexVertex.W = simplexVertex.WB - simplexVertex.WA;
			asSpan[simplex.Count] = simplexVertex;
			num++;
			bool flag = false;
			for (int j = 0; j < count; j++)
			{
				if (simplexVertex.IndexA == span[j] && simplexVertex.IndexB == span2[j])
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				break;
			}
			simplex.Count++;
		}
		simplex.GetWitnessPoints(out output.PointA, out output.PointB);
		output.Distance = (output.PointA - output.PointB).Length();
		output.Iterations = num;
		simplex.WriteCache(ref cache);
		if (input.UseRadii)
		{
			if (output.Distance < float.Epsilon)
			{
				output.PointB = (output.PointA = (output.PointA + output.PointB) * 0.5f);
				output.Distance = 0f;
				return;
			}
			float radius = proxyA.Radius;
			float radius2 = proxyB.Radius;
			Vector2 vector = output.PointB - output.PointA;
			Vector2Helpers.Normalize(ref vector);
			output.Distance = MathF.Max(0f, output.Distance - radius - radius2);
			output.PointA += vector * radius;
			output.PointB -= vector * radius2;
		}
	}
}
