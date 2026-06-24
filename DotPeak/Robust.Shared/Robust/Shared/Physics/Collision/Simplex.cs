// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.Collision.Simplex
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Maths;
using Robust.Shared.Utility;
using System;
using System.Numerics;

#nullable disable
namespace Robust.Shared.Physics.Collision;

internal struct Simplex
{
  internal int Count;
  internal FixedArray4<SimplexVertex> V;

  internal unsafe void ReadCache(
    ref SimplexCache cache,
    DistanceProxy proxyA,
    in Transform transformA,
    DistanceProxy proxyB,
    in Transform transformB)
  {
    this.Count = (int) cache.Count;
    Span<SimplexVertex> asSpan = this.V.AsSpan;
    for (int index = 0; index < this.Count; ++index)
    {
      ref SimplexVertex local = ref asSpan[index];
      local.IndexA = (int) cache.IndexA[index];
      local.IndexB = (int) cache.IndexB[index];
      Vector2 vector1 = proxyA.Vertices[local.IndexA];
      Vector2 vector2 = proxyB.Vertices[local.IndexB];
      local.WA = Transform.Mul(in transformA, in vector1);
      local.WB = Transform.Mul(in transformB, in vector2);
      local.W = local.WB - local.WA;
      local.A = 0.0f;
    }
    if (this.Count > 1)
    {
      float metric1 = cache.Metric;
      float metric2 = this.GetMetric();
      if ((double) metric2 < 0.5 * (double) metric1 || 2.0 * (double) metric1 < (double) metric2 || (double) metric2 < 1.4012984643248171E-45)
        this.Count = 0;
    }
    if (this.Count != 0)
      return;
    ref SimplexVertex local1 = ref this.V._00;
    local1.IndexA = 0;
    local1.IndexB = 0;
    Vector2 vector3 = proxyA.Vertices[0];
    Vector2 vector4 = proxyB.Vertices[0];
    local1.WA = Transform.Mul(in transformA, in vector3);
    local1.WB = Transform.Mul(in transformB, in vector4);
    local1.W = local1.WB - local1.WA;
    local1.A = 1f;
    this.Count = 1;
  }

  internal unsafe void WriteCache(ref SimplexCache cache)
  {
    cache.Metric = this.GetMetric();
    cache.Count = (ushort) this.Count;
    Span<SimplexVertex> asSpan = this.V.AsSpan;
    for (int index = 0; index < this.Count; ++index)
    {
      cache.IndexA[index] = (byte) asSpan[index].IndexA;
      cache.IndexB[index] = (byte) asSpan[index].IndexB;
    }
  }

  internal Vector2 GetSearchDirection()
  {
    switch (this.Count)
    {
      case 1:
        return -this.V._00.W;
      case 2:
        Vector2 vector2 = this.V._01.W - this.V._00.W;
        return (double) Vector2Helpers.Cross(vector2, -this.V._00.W) > 0.0 ? new Vector2(-vector2.Y, vector2.X) : new Vector2(vector2.Y, -vector2.X);
      default:
        return Vector2.Zero;
    }
  }

  public static Vector2 Weight2(float a1, Vector2 w1, float a2, Vector2 w2)
  {
    return new Vector2((float) ((double) a1 * (double) w1.X + (double) a2 * (double) w2.X), (float) ((double) a1 * (double) w1.Y + (double) a2 * (double) w2.Y));
  }

  public static Vector2 Weight3(
    float a1,
    Vector2 w1,
    float a2,
    Vector2 w2,
    float a3,
    Vector2 w3)
  {
    return new Vector2((float) ((double) a1 * (double) w1.X + (double) a2 * (double) w2.X + (double) a3 * (double) w3.X), (float) ((double) a1 * (double) w1.Y + (double) a2 * (double) w2.Y + (double) a3 * (double) w3.Y));
  }

  internal Vector2 GetClosestPoint()
  {
    switch (this.Count)
    {
      case 0:
        return Vector2.Zero;
      case 1:
        return this.V._00.W;
      case 2:
        return this.V._00.W * this.V._00.A + this.V._01.W * this.V._01.A;
      case 3:
        return Vector2.Zero;
      default:
        return Vector2.Zero;
    }
  }

  public static Vector2 ComputeSimplexClosestPoint(Simplex s)
  {
    switch (s.Count)
    {
      case 0:
        return Vector2.Zero;
      case 1:
        return s.V._00.W;
      case 2:
        return Simplex.Weight2(s.V._00.A, s.V._00.W, s.V._01.A, s.V._01.W);
      case 3:
        return Vector2.Zero;
      default:
        return Vector2.Zero;
    }
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
        a = Simplex.Weight2(s.V._00.A, s.V._00.WA, s.V._01.A, s.V._01.WA);
        b = Simplex.Weight2(s.V._00.A, s.V._00.WB, s.V._01.A, s.V._01.WB);
        break;
      case 3:
        a = Simplex.Weight3(s.V._00.A, s.V._00.WA, s.V._01.A, s.V._01.WA, s.V._02.A, s.V._02.WA);
        b = a;
        break;
    }
  }

  public static void SolveSimplex2(ref Simplex s)
  {
    Vector2 w1 = s.V._00.W;
    Vector2 w2 = s.V._01.W;
    Vector2 vector2 = Vector2.Subtract(w2, w1);
    float num1 = -Vector2.Dot(w1, vector2);
    if ((double) num1 <= 0.0)
    {
      s.V._00.A = 1f;
      s.Count = 1;
    }
    else
    {
      float num2 = Vector2.Dot(w2, vector2);
      if ((double) num2 <= 0.0)
      {
        s.V._01.A = 1f;
        s.Count = 1;
        s.V._00 = s.V._01;
      }
      else
      {
        float num3 = (float) (1.0 / ((double) num2 + (double) num1));
        s.V._00.A = num2 * num3;
        s.V._01.A = num1 * num3;
        s.Count = 2;
      }
    }
  }

  public static void SolveSimplex3(ref Simplex s)
  {
    Vector2 w1 = s.V._00.W;
    Vector2 w2 = s.V._01.W;
    Vector2 w3 = s.V._02.W;
    Vector2 vector2_1 = Vector2.Subtract(w2, w1);
    double num1 = (double) Vector2.Dot(w1, vector2_1);
    float num2 = Vector2.Dot(w2, vector2_1);
    float num3 = (float) -num1;
    Vector2 vector2_2 = Vector2.Subtract(w3, w1);
    double num4 = (double) Vector2.Dot(w1, vector2_2);
    float num5 = Vector2.Dot(w3, vector2_2);
    float num6 = (float) -num4;
    Vector2 vector2_3 = Vector2.Subtract(w3, w2);
    double num7 = (double) Vector2.Dot(w2, vector2_3);
    float num8 = Vector2.Dot(w3, vector2_3);
    float num9 = (float) -num7;
    double num10 = (double) Vector2Helpers.Cross(vector2_1, vector2_2);
    float num11 = (float) num10 * Vector2Helpers.Cross(w2, w3);
    float num12 = (float) num10 * Vector2Helpers.Cross(w3, w1);
    float num13 = (float) num10 * Vector2Helpers.Cross(w1, w2);
    if ((double) num3 <= 0.0 && (double) num6 <= 0.0)
    {
      s.V._00.A = 1f;
      s.Count = 1;
    }
    else if ((double) num2 > 0.0 && (double) num3 > 0.0 && (double) num13 <= 0.0)
    {
      float num14 = (float) (1.0 / ((double) num2 + (double) num3));
      s.V._00.A = num2 * num14;
      s.V._01.A = num3 * num14;
      s.Count = 2;
    }
    else if ((double) num5 > 0.0 && (double) num6 > 0.0 && (double) num12 <= 0.0)
    {
      float num15 = (float) (1.0 / ((double) num5 + (double) num6));
      s.V._00.A = num5 * num15;
      s.V._02.A = num6 * num15;
      s.Count = 2;
      s.V._01 = s.V._02;
    }
    else if ((double) num2 <= 0.0 && (double) num9 <= 0.0)
    {
      s.V._01.A = 1f;
      s.Count = 1;
      s.V._00 = s.V._01;
    }
    else if ((double) num5 <= 0.0 && (double) num8 <= 0.0)
    {
      s.V._02.A = 1f;
      s.Count = 1;
      s.V._00 = s.V._02;
    }
    else if ((double) num8 > 0.0 && (double) num9 > 0.0 && (double) num11 <= 0.0)
    {
      float num16 = (float) (1.0 / ((double) num8 + (double) num9));
      s.V._01.A = num8 * num16;
      s.V._02.A = num9 * num16;
      s.Count = 2;
      s.V._00 = s.V._02;
    }
    else
    {
      float num17 = (float) (1.0 / ((double) num11 + (double) num12 + (double) num13));
      s.V._00.A = num11 * num17;
      s.V._01.A = num12 * num17;
      s.V._02.A = num13 * num17;
      s.Count = 3;
    }
  }

  internal void GetWitnessPoints(out Vector2 pA, out Vector2 pB)
  {
    switch (this.Count)
    {
      case 0:
        pA = Vector2.Zero;
        pB = Vector2.Zero;
        break;
      case 1:
        pA = this.V._00.WA;
        pB = this.V._00.WB;
        break;
      case 2:
        pA = this.V._00.WA * this.V._00.A + this.V._01.WA * this.V._01.A;
        pB = this.V._00.WB * this.V._00.A + this.V._01.WB * this.V._01.A;
        break;
      case 3:
        pA = this.V._00.WA * this.V._00.A + this.V._01.WA * this.V._01.A + this.V._02.WA * this.V._02.A;
        pB = pA;
        break;
      default:
        throw new Exception();
    }
  }

  internal float GetMetric()
  {
    switch (this.Count)
    {
      case 0:
        return 0.0f;
      case 1:
        return 0.0f;
      case 2:
        return (this.V._00.W - this.V._01.W).Length();
      case 3:
        return Vector2Helpers.Cross(this.V._01.W - this.V._00.W, this.V._02.W - this.V._00.W);
      default:
        return 0.0f;
    }
  }

  internal void Solve2()
  {
    Vector2 w1 = this.V._00.W;
    Vector2 w2 = this.V._01.W;
    Vector2 vector2 = w2 - w1;
    float num1 = -Vector2.Dot(w1, vector2);
    if ((double) num1 <= 0.0)
    {
      this.V._00 = this.V._00 with { A = 1f };
      this.Count = 1;
    }
    else
    {
      float num2 = Vector2.Dot(w2, vector2);
      if ((double) num2 <= 0.0)
      {
        this.V._01 = this.V._01 with { A = 1f };
        this.Count = 1;
        this.V._00 = this.V._01;
      }
      else
      {
        float num3 = (float) (1.0 / ((double) num2 + (double) num1));
        SimplexVertex simplexVertex1 = this.V._00;
        SimplexVertex simplexVertex2 = this.V._01;
        simplexVertex1.A = num2 * num3;
        simplexVertex2.A = num1 * num3;
        this.V._00 = simplexVertex1;
        this.V._01 = simplexVertex2;
        this.Count = 2;
      }
    }
  }

  internal void Solve3()
  {
    Vector2 w1 = this.V._00.W;
    Vector2 w2 = this.V._01.W;
    Vector2 w3 = this.V._02.W;
    Vector2 vector2_1 = w2 - w1;
    double num1 = (double) Vector2.Dot(w1, vector2_1);
    float num2 = Vector2.Dot(w2, vector2_1);
    float num3 = (float) -num1;
    Vector2 vector2_2 = w3 - w1;
    double num4 = (double) Vector2.Dot(w1, vector2_2);
    float num5 = Vector2.Dot(w3, vector2_2);
    float num6 = (float) -num4;
    Vector2 vector2_3 = w3 - w2;
    double num7 = (double) Vector2.Dot(w2, vector2_3);
    float num8 = Vector2.Dot(w3, vector2_3);
    float num9 = (float) -num7;
    double num10 = (double) Vector2Helpers.Cross(vector2_1, vector2_2);
    float num11 = (float) num10 * Vector2Helpers.Cross(w2, w3);
    float num12 = (float) num10 * Vector2Helpers.Cross(w3, w1);
    float num13 = (float) num10 * Vector2Helpers.Cross(w1, w2);
    if ((double) num3 <= 0.0 && (double) num6 <= 0.0)
    {
      this.V._00 = this.V._00 with { A = 1f };
      this.Count = 1;
    }
    else if ((double) num2 > 0.0 && (double) num3 > 0.0 && (double) num13 <= 0.0)
    {
      float num14 = (float) (1.0 / ((double) num2 + (double) num3));
      SimplexVertex simplexVertex1 = this.V._00;
      SimplexVertex simplexVertex2 = this.V._01;
      simplexVertex1.A = num2 * num14;
      simplexVertex2.A = num3 * num14;
      this.V._00 = simplexVertex1;
      this.V._01 = simplexVertex2;
      this.Count = 2;
    }
    else if ((double) num5 > 0.0 && (double) num6 > 0.0 && (double) num12 <= 0.0)
    {
      float num15 = (float) (1.0 / ((double) num5 + (double) num6));
      SimplexVertex simplexVertex3 = this.V._00;
      SimplexVertex simplexVertex4 = this.V._02;
      simplexVertex3.A = num5 * num15;
      simplexVertex4.A = num6 * num15;
      this.V._00 = simplexVertex3;
      this.V._02 = simplexVertex4;
      this.Count = 2;
      this.V._01 = this.V._02;
    }
    else if ((double) num2 <= 0.0 && (double) num9 <= 0.0)
    {
      this.V._01 = this.V._01 with { A = 1f };
      this.Count = 1;
      this.V._00 = this.V._01;
    }
    else if ((double) num5 <= 0.0 && (double) num8 <= 0.0)
    {
      this.V._02 = this.V._02 with { A = 1f };
      this.Count = 1;
      this.V._00 = this.V._02;
    }
    else if ((double) num8 > 0.0 && (double) num9 > 0.0 && (double) num11 <= 0.0)
    {
      float num16 = (float) (1.0 / ((double) num8 + (double) num9));
      SimplexVertex simplexVertex5 = this.V._01;
      SimplexVertex simplexVertex6 = this.V._02;
      simplexVertex5.A = num8 * num16;
      simplexVertex6.A = num9 * num16;
      this.V._01 = simplexVertex5;
      this.V._02 = simplexVertex6;
      this.Count = 2;
      this.V._00 = this.V._02;
    }
    else
    {
      float num17 = (float) (1.0 / ((double) num11 + (double) num12 + (double) num13));
      SimplexVertex simplexVertex7 = this.V._00;
      SimplexVertex simplexVertex8 = this.V._01;
      SimplexVertex simplexVertex9 = this.V._02;
      simplexVertex7.A = num11 * num17;
      simplexVertex8.A = num12 * num17;
      simplexVertex9.A = num13 * num17;
      this.V._00 = simplexVertex7;
      this.V._01 = simplexVertex8;
      this.V._02 = simplexVertex9;
      this.Count = 3;
    }
  }
}
