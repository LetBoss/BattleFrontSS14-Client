using System;
using System.Collections.Generic;
using System.Numerics;

namespace Robust.Shared.Physics;

public sealed class RamerDouglasPeuckerSimplifier : IVerticesSimplifier
{
	public List<Vector2> Simplify(List<Vector2> vertices, float distanceTolerance)
	{
		if (vertices.Count <= 3)
		{
			return vertices;
		}
		Span<bool> usePoint = stackalloc bool[vertices.Count];
		for (int i = 0; i < vertices.Count; i++)
		{
			usePoint[i] = true;
		}
		SimplifySection(vertices, 0, vertices.Count - 1, usePoint, distanceTolerance);
		List<Vector2> list = new List<Vector2>(vertices.Count);
		for (int j = 0; j < vertices.Count; j++)
		{
			if (usePoint[j])
			{
				list.Add(vertices[j]);
			}
		}
		return list;
	}

	private static void SimplifySection(List<Vector2> vertices, int i, int j, Span<bool> usePoint, float distanceTolerance)
	{
		if (i + 1 == j)
		{
			return;
		}
		Vector2 start = vertices[i];
		Vector2 end = vertices[j];
		double num = -1.0;
		int num2 = i;
		for (int k = i + 1; k < j; k++)
		{
			double num3 = DistanceBetweenPointAndLineSegment(vertices[k], in start, in end);
			if (num3 > num)
			{
				num = num3;
				num2 = k;
			}
		}
		if (num <= (double)distanceTolerance)
		{
			for (int l = i + 1; l < j; l++)
			{
				usePoint[l] = false;
			}
		}
		else
		{
			SimplifySection(vertices, i, num2, usePoint, distanceTolerance);
			SimplifySection(vertices, num2, j, usePoint, distanceTolerance);
		}
	}

	public static float DistanceBetweenPointAndLineSegment(in Vector2 point, in Vector2 start, in Vector2 end)
	{
		if (start == end)
		{
			return (point - start).Length();
		}
		Vector2 vector = end - start;
		float num = Vector2.Dot(point - start, vector);
		if (num <= 0f)
		{
			return (point - start).Length();
		}
		float num2 = Vector2.Dot(vector, vector);
		if (num2 <= num)
		{
			return (point - end).Length();
		}
		float num3 = num / num2;
		Vector2 vector2 = start + vector * num3;
		return (point - vector2).Length();
	}
}
