using System;
using System.Numerics;
using Robust.Shared.Maths;
using Robust.Shared.Utility;

namespace Robust.Shared.Physics.Collision;

internal struct Simplex
{
	internal int Count;

	internal FixedArray4<SimplexVertex> V;

	internal unsafe void ReadCache(ref SimplexCache cache, DistanceProxy proxyA, in Transform transformA, DistanceProxy proxyB, in Transform transformB)
	{
		Count = cache.Count;
		Span<SimplexVertex> asSpan = V.AsSpan;
		for (int i = 0; i < Count; i++)
		{
			ref SimplexVertex reference = ref asSpan[i];
			reference.IndexA = cache.IndexA[i];
			reference.IndexB = cache.IndexB[i];
			Vector2 vector = proxyA.Vertices[reference.IndexA];
			Vector2 vector2 = proxyB.Vertices[reference.IndexB];
			reference.WA = Transform.Mul(in transformA, in vector);
			reference.WB = Transform.Mul(in transformB, in vector2);
			reference.W = reference.WB - reference.WA;
			reference.A = 0f;
		}
		if (Count > 1)
		{
			float metric = cache.Metric;
			float metric2 = GetMetric();
			if (metric2 < 0.5f * metric || 2f * metric < metric2 || metric2 < float.Epsilon)
			{
				Count = 0;
			}
		}
		if (Count == 0)
		{
			ref SimplexVertex _ = ref V._00;
			_.IndexA = 0;
			_.IndexB = 0;
			Vector2 vector3 = proxyA.Vertices[0];
			Vector2 vector4 = proxyB.Vertices[0];
			_.WA = Transform.Mul(in transformA, in vector3);
			_.WB = Transform.Mul(in transformB, in vector4);
			_.W = _.WB - _.WA;
			_.A = 1f;
			Count = 1;
		}
	}

	internal unsafe void WriteCache(ref SimplexCache cache)
	{
		cache.Metric = GetMetric();
		cache.Count = (ushort)Count;
		Span<SimplexVertex> asSpan = V.AsSpan;
		for (int i = 0; i < Count; i++)
		{
			cache.IndexA[i] = (byte)asSpan[i].IndexA;
			cache.IndexB[i] = (byte)asSpan[i].IndexB;
		}
	}

	internal Vector2 GetSearchDirection()
	{
		switch (Count)
		{
		case 1:
			return -V._00.W;
		case 2:
		{
			Vector2 vector = V._01.W - V._00.W;
			if (Vector2Helpers.Cross(vector, -V._00.W) > 0f)
			{
				return new Vector2(0f - vector.Y, vector.X);
			}
			return new Vector2(vector.Y, 0f - vector.X);
		}
		default:
			return Vector2.Zero;
		}
	}

	public static Vector2 Weight2(float a1, Vector2 w1, float a2, Vector2 w2)
	{
		return new Vector2(a1 * w1.X + a2 * w2.X, a1 * w1.Y + a2 * w2.Y);
	}

	public static Vector2 Weight3(float a1, Vector2 w1, float a2, Vector2 w2, float a3, Vector2 w3)
	{
		return new Vector2(a1 * w1.X + a2 * w2.X + a3 * w3.X, a1 * w1.Y + a2 * w2.Y + a3 * w3.Y);
	}

	internal Vector2 GetClosestPoint()
	{
		return Count switch
		{
			0 => Vector2.Zero, 
			1 => V._00.W, 
			2 => V._00.W * V._00.A + V._01.W * V._01.A, 
			3 => Vector2.Zero, 
			_ => Vector2.Zero, 
		};
	}

	public static Vector2 ComputeSimplexClosestPoint(Simplex s)
	{
		return s.Count switch
		{
			0 => Vector2.Zero, 
			1 => s.V._00.W, 
			2 => Weight2(s.V._00.A, s.V._00.W, s.V._01.A, s.V._01.W), 
			3 => Vector2.Zero, 
			_ => Vector2.Zero, 
		};
	}

	public static void ComputeSimplexWitnessPoints(ref Vector2 a, ref Vector2 b, Simplex s)
	{
		switch (s.Count)
		{
		case 1:
			a = s.V._00.WA;
			b = s.V._00.WB;
			break;
		case 2:
			a = Weight2(s.V._00.A, s.V._00.WA, s.V._01.A, s.V._01.WA);
			b = Weight2(s.V._00.A, s.V._00.WB, s.V._01.A, s.V._01.WB);
			break;
		case 3:
			a = Weight3(s.V._00.A, s.V._00.WA, s.V._01.A, s.V._01.WA, s.V._02.A, s.V._02.WA);
			b = a;
			break;
		case 0:
			break;
		}
	}

	public static void SolveSimplex2(ref Simplex s)
	{
		Vector2 w = s.V._00.W;
		Vector2 w2 = s.V._01.W;
		Vector2 value = Vector2.Subtract(w2, w);
		float num = 0f - Vector2.Dot(w, value);
		if (num <= 0f)
		{
			s.V._00.A = 1f;
			s.Count = 1;
			return;
		}
		float num2 = Vector2.Dot(w2, value);
		if (num2 <= 0f)
		{
			s.V._01.A = 1f;
			s.Count = 1;
			s.V._00 = s.V._01;
		}
		else
		{
			float num3 = 1f / (num2 + num);
			s.V._00.A = num2 * num3;
			s.V._01.A = num * num3;
			s.Count = 2;
		}
	}

	public static void SolveSimplex3(ref Simplex s)
	{
		Vector2 w = s.V._00.W;
		Vector2 w2 = s.V._01.W;
		Vector2 w3 = s.V._02.W;
		Vector2 vector = Vector2.Subtract(w2, w);
		float num = Vector2.Dot(w, vector);
		float num2 = Vector2.Dot(w2, vector);
		float num3 = 0f - num;
		Vector2 vector2 = Vector2.Subtract(w3, w);
		float num4 = Vector2.Dot(w, vector2);
		float num5 = Vector2.Dot(w3, vector2);
		float num6 = 0f - num4;
		Vector2 value = Vector2.Subtract(w3, w2);
		float num7 = Vector2.Dot(w2, value);
		float num8 = Vector2.Dot(w3, value);
		float num9 = 0f - num7;
		float num10 = Vector2Helpers.Cross(vector, vector2);
		float num11 = num10 * Vector2Helpers.Cross(w2, w3);
		float num12 = num10 * Vector2Helpers.Cross(w3, w);
		float num13 = num10 * Vector2Helpers.Cross(w, w2);
		if (num3 <= 0f && num6 <= 0f)
		{
			s.V._00.A = 1f;
			s.Count = 1;
		}
		else if (num2 > 0f && num3 > 0f && num13 <= 0f)
		{
			float num14 = 1f / (num2 + num3);
			s.V._00.A = num2 * num14;
			s.V._01.A = num3 * num14;
			s.Count = 2;
		}
		else if (num5 > 0f && num6 > 0f && num12 <= 0f)
		{
			float num15 = 1f / (num5 + num6);
			s.V._00.A = num5 * num15;
			s.V._02.A = num6 * num15;
			s.Count = 2;
			s.V._01 = s.V._02;
		}
		else if (num2 <= 0f && num9 <= 0f)
		{
			s.V._01.A = 1f;
			s.Count = 1;
			s.V._00 = s.V._01;
		}
		else if (num5 <= 0f && num8 <= 0f)
		{
			s.V._02.A = 1f;
			s.Count = 1;
			s.V._00 = s.V._02;
		}
		else if (num8 > 0f && num9 > 0f && num11 <= 0f)
		{
			float num16 = 1f / (num8 + num9);
			s.V._01.A = num8 * num16;
			s.V._02.A = num9 * num16;
			s.Count = 2;
			s.V._00 = s.V._02;
		}
		else
		{
			float num17 = 1f / (num11 + num12 + num13);
			s.V._00.A = num11 * num17;
			s.V._01.A = num12 * num17;
			s.V._02.A = num13 * num17;
			s.Count = 3;
		}
	}

	internal void GetWitnessPoints(out Vector2 pA, out Vector2 pB)
	{
		switch (Count)
		{
		case 0:
			pA = Vector2.Zero;
			pB = Vector2.Zero;
			break;
		case 1:
			pA = V._00.WA;
			pB = V._00.WB;
			break;
		case 2:
			pA = V._00.WA * V._00.A + V._01.WA * V._01.A;
			pB = V._00.WB * V._00.A + V._01.WB * V._01.A;
			break;
		case 3:
			pA = V._00.WA * V._00.A + V._01.WA * V._01.A + V._02.WA * V._02.A;
			pB = pA;
			break;
		default:
			throw new Exception();
		}
	}

	internal float GetMetric()
	{
		return Count switch
		{
			0 => 0f, 
			1 => 0f, 
			2 => (V._00.W - V._01.W).Length(), 
			3 => Vector2Helpers.Cross(V._01.W - V._00.W, V._02.W - V._00.W), 
			_ => 0f, 
		};
	}

	internal void Solve2()
	{
		Vector2 w = V._00.W;
		Vector2 w2 = V._01.W;
		Vector2 value = w2 - w;
		float num = 0f - Vector2.Dot(w, value);
		if (num <= 0f)
		{
			SimplexVertex _ = V._00;
			_.A = 1f;
			V._00 = _;
			Count = 1;
			return;
		}
		float num2 = Vector2.Dot(w2, value);
		if (num2 <= 0f)
		{
			SimplexVertex _2 = V._01;
			_2.A = 1f;
			V._01 = _2;
			Count = 1;
			V._00 = V._01;
		}
		else
		{
			float num3 = 1f / (num2 + num);
			SimplexVertex _3 = V._00;
			SimplexVertex _4 = V._01;
			_3.A = num2 * num3;
			_4.A = num * num3;
			V._00 = _3;
			V._01 = _4;
			Count = 2;
		}
	}

	internal void Solve3()
	{
		Vector2 w = V._00.W;
		Vector2 w2 = V._01.W;
		Vector2 w3 = V._02.W;
		Vector2 vector = w2 - w;
		float num = Vector2.Dot(w, vector);
		float num2 = Vector2.Dot(w2, vector);
		float num3 = 0f - num;
		Vector2 vector2 = w3 - w;
		float num4 = Vector2.Dot(w, vector2);
		float num5 = Vector2.Dot(w3, vector2);
		float num6 = 0f - num4;
		Vector2 value = w3 - w2;
		float num7 = Vector2.Dot(w2, value);
		float num8 = Vector2.Dot(w3, value);
		float num9 = 0f - num7;
		float num10 = Vector2Helpers.Cross(vector, vector2);
		float num11 = num10 * Vector2Helpers.Cross(w2, w3);
		float num12 = num10 * Vector2Helpers.Cross(w3, w);
		float num13 = num10 * Vector2Helpers.Cross(w, w2);
		if (num3 <= 0f && num6 <= 0f)
		{
			SimplexVertex _ = V._00;
			_.A = 1f;
			V._00 = _;
			Count = 1;
		}
		else if (num2 > 0f && num3 > 0f && num13 <= 0f)
		{
			float num14 = 1f / (num2 + num3);
			SimplexVertex _2 = V._00;
			SimplexVertex _3 = V._01;
			_2.A = num2 * num14;
			_3.A = num3 * num14;
			V._00 = _2;
			V._01 = _3;
			Count = 2;
		}
		else if (num5 > 0f && num6 > 0f && num12 <= 0f)
		{
			float num15 = 1f / (num5 + num6);
			SimplexVertex _4 = V._00;
			SimplexVertex _5 = V._02;
			_4.A = num5 * num15;
			_5.A = num6 * num15;
			V._00 = _4;
			V._02 = _5;
			Count = 2;
			V._01 = V._02;
		}
		else if (num2 <= 0f && num9 <= 0f)
		{
			SimplexVertex _6 = V._01;
			_6.A = 1f;
			V._01 = _6;
			Count = 1;
			V._00 = V._01;
		}
		else if (num5 <= 0f && num8 <= 0f)
		{
			SimplexVertex _7 = V._02;
			_7.A = 1f;
			V._02 = _7;
			Count = 1;
			V._00 = V._02;
		}
		else if (num8 > 0f && num9 > 0f && num11 <= 0f)
		{
			float num16 = 1f / (num8 + num9);
			SimplexVertex _8 = V._01;
			SimplexVertex _9 = V._02;
			_8.A = num8 * num16;
			_9.A = num9 * num16;
			V._01 = _8;
			V._02 = _9;
			Count = 2;
			V._00 = V._02;
		}
		else
		{
			float num17 = 1f / (num11 + num12 + num13);
			SimplexVertex _10 = V._00;
			SimplexVertex _11 = V._01;
			SimplexVertex _12 = V._02;
			_10.A = num11 * num17;
			_11.A = num12 * num17;
			_12.A = num13 * num17;
			V._00 = _10;
			V._01 = _11;
			V._02 = _12;
			Count = 3;
		}
	}
}
