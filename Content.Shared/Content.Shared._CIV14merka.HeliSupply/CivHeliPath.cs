using System;
using System.Collections.Generic;
using System.Numerics;

namespace Content.Shared._CIV14merka.HeliSupply;

public sealed class CivHeliPath
{
	public readonly List<CivHeliPathSegment> Segments = new List<CivHeliPathSegment>();

	public float[] PointDist = Array.Empty<float>();

	public float Total;

	public float TotalCost;

	public void SampleByCost(float cost, out Vector2 pos, out Vector2 tangent)
	{
		pos = Vector2.Zero;
		tangent = Vector2.UnitX;
		if (Segments.Count == 0)
		{
			return;
		}
		float travelled = 0f;
		for (int i = 0; i < Segments.Count; i++)
		{
			CivHeliPathSegment seg = Segments[i];
			if (cost <= travelled + seg.Cost || i == Segments.Count - 1)
			{
				float t = ((seg.Cost > 0f) ? Math.Clamp((cost - travelled) / seg.Cost, 0f, 1f) : 1f);
				pos = Vector2.Lerp(seg.A, seg.B, t);
				tangent = seg.B - seg.A;
				break;
			}
			travelled += seg.Cost;
		}
	}

	public float DistAtCost(float cost)
	{
		float cum = 0f;
		float dist = 0f;
		foreach (CivHeliPathSegment seg in Segments)
		{
			if (cost <= cum + seg.Cost)
			{
				float t = ((seg.Cost > 0f) ? Math.Clamp((cost - cum) / seg.Cost, 0f, 1f) : 1f);
				return dist + seg.Length * t;
			}
			cum += seg.Cost;
			dist += seg.Length;
		}
		return dist;
	}

	public float CostAtDist(float dist)
	{
		float cum = 0f;
		float cost = 0f;
		foreach (CivHeliPathSegment seg in Segments)
		{
			if (dist <= cum + seg.Length)
			{
				float t = ((seg.Length > 0f) ? Math.Clamp((dist - cum) / seg.Length, 0f, 1f) : 1f);
				return cost + seg.Cost * t;
			}
			cum += seg.Length;
			cost += seg.Cost;
		}
		return cost;
	}
}
