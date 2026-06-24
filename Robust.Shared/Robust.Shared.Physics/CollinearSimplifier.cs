using System.Collections.Generic;
using System.Numerics;

namespace Robust.Shared.Physics;

public sealed class CollinearSimplifier : IVerticesSimplifier
{
	public List<Vector2> Simplify(List<Vector2> vertices, float tolerance = 0f)
	{
		if (vertices.Count <= 3)
		{
			return vertices;
		}
		List<Vector2> list = new List<Vector2>(vertices.Count);
		for (int i = 0; i < vertices.Count; i++)
		{
			Vector2 prev = vertices[(i == 0) ? (vertices.Count - 1) : (i - 1)];
			Vector2 current = vertices[i];
			if (!IsCollinear(in prev, in current, vertices[(i + 1) % vertices.Count], tolerance))
			{
				list.Add(current);
			}
		}
		if (list.Count == 0)
		{
			list.Add(vertices[0]);
			list.Add(vertices[vertices.Count - 1]);
		}
		return list;
	}

	public static bool IsCollinear(in Vector2 prev, in Vector2 current, in Vector2 next, float tolerance)
	{
		return FloatInRange(Area(in prev, in current, in next), 0f - tolerance, tolerance);
	}

	private static float Area(in Vector2 a, in Vector2 b, in Vector2 c)
	{
		return a.X * (b.Y - c.Y) + b.X * (c.Y - a.Y) + c.X * (a.Y - b.Y);
	}

	private static bool FloatInRange(float value, float min, float max)
	{
		if (value >= min)
		{
			return value <= max;
		}
		return false;
	}
}
