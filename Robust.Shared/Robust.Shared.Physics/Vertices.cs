using System;
using System.Numerics;

namespace Robust.Shared.Physics;

public static class Vertices
{
	public static Vector2[] ForceCounterClockwise(Span<Vector2> vertices)
	{
		if (vertices.Length < 3)
		{
			return vertices.ToArray();
		}
		if (!IsCounterClockwise(vertices))
		{
			vertices.Reverse();
		}
		return vertices.ToArray();
	}

	public static bool IsCounterClockwise(Span<Vector2> vertices)
	{
		if (vertices.Length < 3)
		{
			return false;
		}
		return GetSignedArea(vertices) > 0f;
	}

	public static float GetSignedArea(Span<Vector2> vertices)
	{
		int length = vertices.Length;
		if (length < 3)
		{
			return 0f;
		}
		float num = 0f;
		for (int i = 0; i < length; i++)
		{
			int index = (i + 1) % length;
			Vector2 vector = vertices[i];
			Vector2 vector2 = vertices[index];
			num += vector.X * vector2.Y;
			num -= vector.Y * vector2.X;
		}
		return num / 2f;
	}
}
