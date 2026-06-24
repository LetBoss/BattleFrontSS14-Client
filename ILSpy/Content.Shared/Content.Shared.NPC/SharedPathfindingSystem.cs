using System;
using System.Collections.Generic;
using System.Numerics;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

namespace Content.Shared.NPC;

public abstract class SharedPathfindingSystem : EntitySystem
{
	public delegate bool Vector2iCallback(Vector2i index);

	public const byte SubStep = 4;

	public const byte ChunkSize = 8;

	public static readonly Vector2 ChunkSizeVec = new Vector2(8f, 8f);

	protected const float StepOffset = 0.125f;

	private static readonly Vector2 StepOffsetVec = new Vector2(0.125f, 0.125f);

	public Vector2 GetCoordinate(Vector2i chunk, Vector2i index)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		return new Vector2(index.X, index.Y) / 4f + Vector2i.op_Implicit(chunk) * ChunkSizeVec + StepOffsetVec;
	}

	public static float ManhattanDistance(Vector2i start, Vector2i end)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		Vector2i distance = end - start;
		return Math.Abs(distance.X) + Math.Abs(distance.Y);
	}

	public static float OctileDistance(Vector2i start, Vector2i end)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		Vector2 ab = Vector2.Abs(Vector2i.op_Implicit(start - end));
		return ab.X + ab.Y + -0.59000003f * Math.Min(ab.X, ab.Y);
	}

	public static IEnumerable<Vector2i> GetTileOutline(Vector2i center, float radius)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		Vector2 vecCircle = Vector2i.op_Implicit(center) + Vector2.One / 2f;
		for (int r = 0; (double)r <= Math.Floor(radius * MathF.Sqrt(0.5f)); r++)
		{
			float d = MathF.Floor(MathF.Sqrt(radius * radius - (float)(r * r)));
			yield return Vector2Helpers.Floored(new Vector2(vecCircle.X - d, vecCircle.Y + (float)r));
			yield return Vector2Helpers.Floored(new Vector2(vecCircle.X + d, vecCircle.Y + (float)r));
			yield return Vector2Helpers.Floored(new Vector2(vecCircle.X - d, vecCircle.Y - (float)r));
			yield return Vector2Helpers.Floored(new Vector2(vecCircle.X + d, vecCircle.Y - (float)r));
			yield return Vector2Helpers.Floored(new Vector2(vecCircle.X + (float)r, vecCircle.Y - d));
			yield return Vector2Helpers.Floored(new Vector2(vecCircle.X + (float)r, vecCircle.Y + d));
			yield return Vector2Helpers.Floored(new Vector2(vecCircle.X - (float)r, vecCircle.Y - d));
			yield return Vector2Helpers.Floored(new Vector2(vecCircle.X - (float)r, vecCircle.Y + d));
		}
	}

	public static void GridCast(Vector2i start, Vector2i end, Vector2iCallback callback)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		if (!callback(start))
		{
			return;
		}
		int xinc = ((end.X >= start.X) ? 1 : (-1));
		int yinc = ((end.Y >= start.Y) ? 1 : (-1));
		int dx = xinc * (end.X - start.X);
		int dy = yinc * (end.Y - start.Y);
		int ax = start.X;
		int ay = start.Y;
		if (dx == dy)
		{
			while (dx-- > 0)
			{
				ax += xinc;
				ay += yinc;
				if (!callback(new Vector2i(ax, ay)))
				{
					break;
				}
			}
			return;
		}
		int side = -1 * (((dx == 0) ? yinc : xinc) - 1);
		int i = dx + dy;
		int error = dx - dy;
		dx *= 2;
		dy *= 2;
		while (i-- > 0)
		{
			if (error > 0 || error == side)
			{
				ax += xinc;
				error -= dy;
			}
			else
			{
				ay += yinc;
				error += dx;
			}
			if (!callback(new Vector2i(ax, ay)))
			{
				break;
			}
		}
	}
}
