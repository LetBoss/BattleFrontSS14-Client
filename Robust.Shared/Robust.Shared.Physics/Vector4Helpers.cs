using System;
using System.Numerics;

namespace Robust.Shared.Physics;

internal static class Vector4Helpers
{
	public static Vector4 Inverse(Vector4 matrix)
	{
		float x = matrix.X;
		float z = matrix.Z;
		float y = matrix.Y;
		float w = matrix.W;
		float num = x * w - z * y;
		if (num != 0f)
		{
			num = 1f / num;
		}
		return new Vector4(num * w, (0f - num) * y, (0f - num) * z, num * x);
	}

	public static void Inverse(Span<Vector2> matrix)
	{
		float x = matrix[0].X;
		float x2 = matrix[1].X;
		float y = matrix[0].Y;
		float y2 = matrix[1].Y;
		float num = x * y2 - x2 * y;
		if (num != 0f)
		{
			num = 1f / num;
		}
		matrix[0].X = num * y2;
		matrix[0].Y = (0f - num) * y;
		matrix[1].X = (0f - num) * x2;
		matrix[1].Y = num * x;
	}
}
