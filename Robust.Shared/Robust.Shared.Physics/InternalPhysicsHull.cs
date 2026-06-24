using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using Robust.Shared.Maths;

namespace Robust.Shared.Physics;

internal ref struct InternalPhysicsHull
{
	public Span<Vector2> Points;

	public int Count;

	internal InternalPhysicsHull(Span<Vector2> vertices, int count)
	{
		this = default(InternalPhysicsHull);
		Count = count;
		Points = vertices.Slice(0, count);
	}

	private static InternalPhysicsHull RecurseHull(Vector2 p1, Vector2 p2, Span<Vector2> ps, int count)
	{
		InternalPhysicsHull result = new InternalPhysicsHull
		{
			Count = 0
		};
		if (count == 0)
		{
			return result;
		}
		Vector2 vector = p2 - p1;
		Vector2Helpers.Normalize(ref vector);
		Span<Vector2> ps2 = stackalloc Vector2[8];
		int count2 = 0;
		int index = 0;
		float num = Vector2Helpers.Cross(ps[index] - p1, vector);
		if (num > 0f)
		{
			ps2[count2++] = ps[index];
		}
		for (int i = 1; i < count; i++)
		{
			float num2 = Vector2Helpers.Cross(ps[i] - p1, vector);
			if (num2 > num)
			{
				index = i;
				num = num2;
			}
			if (num2 > 0f)
			{
				ps2[count2++] = ps[i];
			}
		}
		if (num < 0.01f)
		{
			return result;
		}
		result.Points = new Vector2[8];
		Vector2 vector2 = ps[index];
		InternalPhysicsHull internalPhysicsHull = RecurseHull(p1, vector2, ps2, count2);
		InternalPhysicsHull internalPhysicsHull2 = RecurseHull(vector2, p2, ps2, count2);
		for (int j = 0; j < internalPhysicsHull.Count; j++)
		{
			result.Points[result.Count++] = internalPhysicsHull.Points[j];
		}
		result.Points[result.Count++] = vector2;
		for (int k = 0; k < internalPhysicsHull2.Count; k++)
		{
			result.Points[result.Count++] = internalPhysicsHull2.Points[k];
		}
		return result;
	}

	public static InternalPhysicsHull ComputeHull(ReadOnlySpan<Vector2> points, int count)
	{
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		InternalPhysicsHull result = default(InternalPhysicsHull);
		if ((count < 3 || count > 8) ? true : false)
		{
			result.Count = 0;
			return result;
		}
		count = Math.Min(count, 8);
		Unsafe.SkipInit(out Box2 val);
		((Box2)(ref val))._002Ector(float.MaxValue, float.MaxValue, float.MinValue, float.MinValue);
		Span<Vector2> span = stackalloc Vector2[8];
		int num = 0;
		for (int i = 0; i < count; i++)
		{
			val.BottomLeft = Vector2.Min(val.BottomLeft, points[i]);
			val.TopRight = Vector2.Max(val.TopRight, points[i]);
			Vector2 vector = points[i];
			bool flag = true;
			for (int j = 0; j < i; j++)
			{
				if ((points[j] - vector).LengthSquared() < 0.0004f)
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				span[num++] = vector;
			}
		}
		if (num < 3)
		{
			return result;
		}
		Vector2 center = ((Box2)(ref val)).Center;
		int index = 0;
		float num2 = (span[index] - center).LengthSquared();
		for (int k = 1; k < num; k++)
		{
			float num3 = (span[k] - center).LengthSquared();
			if (num3 > num2)
			{
				index = k;
				num2 = num3;
			}
		}
		Vector2 vector2 = span[index];
		span[index] = span[num - 1];
		num--;
		int index2 = 0;
		float num4 = (span[index2] - vector2).LengthSquared();
		for (int l = 1; l < num; l++)
		{
			float num5 = (span[l] - vector2).LengthSquared();
			if (num5 > num4)
			{
				index2 = l;
				num4 = num5;
			}
		}
		Vector2 vector3 = span[index2];
		span[index2] = span[num - 1];
		num--;
		Span<Vector2> ps = stackalloc Vector2[6];
		int count2 = 0;
		Span<Vector2> ps2 = stackalloc Vector2[6];
		int count3 = 0;
		Vector2 vector4 = vector3 - vector2;
		Vector2Helpers.Normalize(ref vector4);
		for (int m = 0; m < num; m++)
		{
			float num6 = Vector2Helpers.Cross(span[m] - vector2, vector4);
			if (num6 >= 0.01f)
			{
				ps[count2++] = span[m];
			}
			else if (num6 <= -0.01f)
			{
				ps2[count3++] = span[m];
			}
		}
		InternalPhysicsHull internalPhysicsHull = RecurseHull(vector2, vector3, ps, count2);
		InternalPhysicsHull internalPhysicsHull2 = RecurseHull(vector3, vector2, ps2, count3);
		if (internalPhysicsHull.Count == 0 && internalPhysicsHull2.Count == 0)
		{
			result.Count = 0;
			return result;
		}
		result.Points = new Vector2[8];
		result.Points[result.Count++] = vector2;
		for (int n = 0; n < internalPhysicsHull.Count; n++)
		{
			result.Points[result.Count++] = internalPhysicsHull.Points[n];
		}
		result.Points[result.Count++] = vector3;
		for (int num7 = 0; num7 < internalPhysicsHull2.Count; num7++)
		{
			result.Points[result.Count++] = internalPhysicsHull2.Points[num7];
		}
		bool flag2 = true;
		while (flag2 && result.Count > 2)
		{
			flag2 = false;
			for (int num8 = 0; num8 < result.Count; num8++)
			{
				index = num8;
				index2 = (num8 + 1) % result.Count;
				int index3 = (num8 + 2) % result.Count;
				vector2 = result.Points[index];
				vector3 = result.Points[index2];
				vector4 = result.Points[index3] - vector2;
				Vector2Helpers.Normalize(ref vector4);
				_ = vector3 - vector2;
				if (Vector2Helpers.Cross(vector3 - vector2, vector4) <= 0.01f)
				{
					for (int num9 = index2; num9 < result.Count - 1; num9++)
					{
						result.Points[num9] = result.Points[num9 + 1];
					}
					result.Count--;
					flag2 = true;
					break;
				}
			}
		}
		if (result.Count < 3)
		{
			result.Count = 0;
		}
		return result;
	}

	public static bool ValidateHull(InternalPhysicsHull hull)
	{
		if (hull.Count < 3 || 8 < hull.Count)
		{
			return false;
		}
		for (int i = 0; i < hull.Count; i++)
		{
			int num = i;
			int num2 = ((i < hull.Count - 1) ? (num + 1) : 0);
			Vector2 vector = hull.Points[num];
			Vector2 vector2 = hull.Points[num2] - vector;
			Vector2Helpers.Normalize(ref vector2);
			for (int j = 0; j < hull.Count; j++)
			{
				if (j != num && j != num2 && Vector2Helpers.Cross(hull.Points[j] - vector, vector2) >= 0f)
				{
					return false;
				}
			}
		}
		for (int k = 0; k < hull.Count; k++)
		{
			int index = k;
			int index2 = (k + 1) % hull.Count;
			int index3 = (k + 2) % hull.Count;
			Vector2 vector3 = hull.Points[index];
			Vector2 vector4 = hull.Points[index2];
			Vector2 vector5 = hull.Points[index3] - vector3;
			Vector2Helpers.Normalize(ref vector5);
			_ = vector4 - vector3;
			if (Vector2Helpers.Cross(vector4 - vector3, vector5) <= 0.005f)
			{
				return false;
			}
		}
		return true;
	}
}
