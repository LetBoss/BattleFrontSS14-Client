using System;
using System.Collections.Generic;
using System.Numerics;

namespace Content.Shared._CIV14merka.HeliSupply;

public static class CivHeliRouteMath
{
	private const float MinSeg = 0.05f;

	private const float MaxSegLen = 4f;

	private const float SpeedRampPerTile = 0.08f;

	private const float MinSpeedFactor = 0.3f;

	public static CivHeliPath Build(IReadOnlyList<Vector2> points, float smoothing, int passes, float turnSlowdown, int fixedIndex = -1, float dropSlowZone = 0f, float dropSlowFactor = 1f)
	{
		CivHeliPath path = new CivHeliPath
		{
			PointDist = new float[points.Count]
		};
		if (points.Count < 2)
		{
			return path;
		}
		float firstHalfLen = -1f;
		List<Vector2> poly;
		if (fixedIndex > 0 && fixedIndex < points.Count - 1)
		{
			List<Vector2> list = Smooth(SubList(points, 0, fixedIndex + 1), smoothing, passes);
			List<Vector2> second = Smooth(SubList(points, fixedIndex, points.Count - fixedIndex), smoothing, passes);
			firstHalfLen = PolyLength(list);
			poly = list;
			for (int i = 1; i < second.Count; i++)
			{
				poly.Add(second[i]);
			}
		}
		else
		{
			poly = Smooth(new List<Vector2>(points), smoothing, passes);
		}
		RemoveDuplicates(poly);
		Subdivide(poly);
		float[] factors = ComputeSpeedFactors(poly, turnSlowdown, firstHalfLen, dropSlowZone, dropSlowFactor);
		float total = 0f;
		float totalCost = 0f;
		for (int j = 0; j < poly.Count - 1; j++)
		{
			CivHeliPathSegment seg = new CivHeliPathSegment
			{
				A = poly[j],
				B = poly[j + 1],
				Length = (poly[j + 1] - poly[j]).Length(),
				SpeedFactor = factors[j]
			};
			path.Segments.Add(seg);
			total += seg.Length;
			totalCost += seg.Cost;
		}
		path.Total = total;
		path.TotalCost = totalCost;
		for (int k = 0; k < points.Count; k++)
		{
			path.PointDist[k] = NearestDistAlong(path, points[k]);
		}
		path.PointDist[0] = 0f;
		path.PointDist[^1] = total;
		if (firstHalfLen >= 0f)
		{
			path.PointDist[fixedIndex] = firstHalfLen;
		}
		return path;
	}

	private static List<Vector2> Smooth(List<Vector2> pts, float maxCut, int passes)
	{
		if (maxCut <= 0.01f || passes <= 0)
		{
			return pts;
		}
		for (int pass = 0; pass < passes; pass++)
		{
			if (pts.Count < 3)
			{
				return pts;
			}
			List<Vector2> result = new List<Vector2>(pts.Count * 2) { pts[0] };
			for (int i = 0; i < pts.Count - 1; i++)
			{
				Vector2 a = pts[i];
				Vector2 b = pts[i + 1];
				Vector2 delta = b - a;
				float len = delta.Length();
				if (!(len < 0.05f))
				{
					Vector2 dir = delta / len;
					float cut = MathF.Min(len * 0.25f, maxCut);
					if (i > 0)
					{
						result.Add(a + dir * cut);
					}
					if (i < pts.Count - 2)
					{
						result.Add(b - dir * cut);
					}
				}
			}
			List<Vector2> list = pts;
			result.Add(list[list.Count - 1]);
			pts = result;
		}
		return pts;
	}

	private static float[] ComputeSpeedFactors(List<Vector2> poly, float turnSlowdown, float dropDist, float dropSlowZone, float dropSlowFactor)
	{
		int segCount = Math.Max(0, poly.Count - 1);
		float[] factors = new float[segCount];
		for (int i = 0; i < segCount; i++)
		{
			factors[i] = 1f;
		}
		if (segCount < 2)
		{
			return factors;
		}
		float[] lens = new float[segCount];
		for (int j = 0; j < segCount; j++)
		{
			lens[j] = (poly[j + 1] - poly[j]).Length();
		}
		if (turnSlowdown > 0f)
		{
			for (int v = 1; v < poly.Count - 1; v++)
			{
				Vector2 inDir = poly[v] - poly[v - 1];
				Vector2 outDir = poly[v + 1] - poly[v];
				float inLen = inDir.Length();
				float outLen = outDir.Length();
				if (!(inLen < 0.05f) && !(outLen < 0.05f))
				{
					float curvature = MathF.Acos(Math.Clamp(Vector2.Dot(inDir / inLen, outDir / outLen), -1f, 1f)) / MathF.Max(0.5f * (inLen + outLen), 0.1f);
					float factor = Math.Clamp(1f / (1f + turnSlowdown * curvature), 0.3f, 1f);
					factors[v - 1] = MathF.Min(factors[v - 1], factor);
					factors[v] = MathF.Min(factors[v], factor);
				}
			}
		}
		if (dropDist >= 0f && dropSlowZone > 0.01f && dropSlowFactor < 1f)
		{
			float cum = 0f;
			for (int k = 0; k < segCount; k++)
			{
				float mid = cum + lens[k] * 0.5f;
				cum += lens[k];
				float toDrop = dropDist - mid;
				if (!(toDrop < 0f) && !(toDrop > dropSlowZone))
				{
					factors[k] = MathF.Min(factors[k], dropSlowFactor);
				}
			}
		}
		for (int i2 = segCount - 2; i2 >= 0; i2--)
		{
			factors[i2] = MathF.Min(factors[i2], factors[i2 + 1] + 0.08f * lens[i2]);
		}
		for (int l = 1; l < segCount; l++)
		{
			factors[l] = MathF.Min(factors[l], factors[l - 1] + 0.08f * lens[l - 1]);
		}
		return factors;
	}

	private static void Subdivide(List<Vector2> poly)
	{
		for (int i = poly.Count - 2; i >= 0; i--)
		{
			Vector2 a = poly[i];
			Vector2 b = poly[i + 1];
			float len = (b - a).Length();
			if (!(len <= 4f))
			{
				int parts = (int)MathF.Ceiling(len / 4f);
				for (int p = parts - 1; p >= 1; p--)
				{
					poly.Insert(i + 1, Vector2.Lerp(a, b, (float)p / (float)parts));
				}
			}
		}
	}

	private static List<Vector2> SubList(IReadOnlyList<Vector2> points, int start, int count)
	{
		List<Vector2> result = new List<Vector2>(count);
		for (int i = 0; i < count; i++)
		{
			result.Add(points[start + i]);
		}
		return result;
	}

	private static float PolyLength(List<Vector2> poly)
	{
		float total = 0f;
		for (int i = 0; i < poly.Count - 1; i++)
		{
			total += (poly[i + 1] - poly[i]).Length();
		}
		return total;
	}

	private static void RemoveDuplicates(List<Vector2> poly)
	{
		for (int i = poly.Count - 1; i > 0; i--)
		{
			if ((poly[i] - poly[i - 1]).Length() < 0.05f)
			{
				poly.RemoveAt(i);
			}
		}
	}

	private static float NearestDistAlong(CivHeliPath path, Vector2 point)
	{
		float bestSq = float.MaxValue;
		float bestDist = 0f;
		float cum = 0f;
		foreach (CivHeliPathSegment seg in path.Segments)
		{
			Vector2 ab = seg.B - seg.A;
			float lenSq = ab.LengthSquared();
			float t = ((lenSq > 0.0001f) ? Math.Clamp(Vector2.Dot(point - seg.A, ab) / lenSq, 0f, 1f) : 0f);
			Vector2 proj = seg.A + ab * t;
			float distSq = (point - proj).LengthSquared();
			if (distSq < bestSq)
			{
				bestSq = distSq;
				bestDist = cum + seg.Length * t;
			}
			cum += seg.Length;
		}
		return bestDist;
	}
}
