// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.Collision.DistanceManager
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Maths;
using System;
using System.Numerics;

#nullable disable
namespace Robust.Shared.Physics.Collision;

internal static class DistanceManager
{
  private const byte MaxGJKIterations = 20;

  public static void ComputeDistance(
    out DistanceOutput output,
    out SimplexCache cache,
    in DistanceInput input)
  {
    cache = new SimplexCache();
    DistanceProxy proxyA = input.ProxyA;
    DistanceProxy proxyB = input.ProxyB;
    Simplex simplex = new Simplex();
    simplex.ReadCache(ref cache, proxyA, in input.TransformA, proxyB, in input.TransformB);
    Span<int> span1 = stackalloc int[3];
    Span<int> span2 = stackalloc int[3];
    span1.Clear();
    span2.Clear();
    Span<SimplexVertex> asSpan = simplex.V.AsSpan;
    int num1 = 0;
    while (num1 < 20)
    {
      int count = simplex.Count;
      for (int index = 0; index < count; ++index)
      {
        span1[index] = asSpan[index].IndexA;
        span2[index] = asSpan[index].IndexB;
      }
      switch (simplex.Count)
      {
        case 1:
          if (simplex.Count != 3)
          {
            Vector2 searchDirection = simplex.GetSearchDirection();
            if ((double) searchDirection.LengthSquared() >= 0.0)
            {
              SimplexVertex simplexVertex = asSpan[simplex.Count] with
              {
                IndexA = proxyA.GetSupport(Transform.MulT(input.TransformA.Quaternion2D, -searchDirection))
              };
              simplexVertex.WA = Transform.Mul(in input.TransformA, in proxyA.Vertices[simplexVertex.IndexA]);
              simplexVertex.IndexB = proxyB.GetSupport(Transform.MulT(input.TransformB.Quaternion2D, searchDirection));
              simplexVertex.WB = Transform.Mul(in input.TransformB, in proxyB.Vertices[simplexVertex.IndexB]);
              simplexVertex.W = simplexVertex.WB - simplexVertex.WA;
              asSpan[simplex.Count] = simplexVertex;
              ++num1;
              bool flag = false;
              for (int index = 0; index < count; ++index)
              {
                if (simplexVertex.IndexA == span1[index] && simplexVertex.IndexB == span2[index])
                {
                  flag = true;
                  break;
                }
              }
              if (!flag)
              {
                ++simplex.Count;
                continue;
              }
              goto label_19;
            }
            goto label_19;
          }
          goto label_19;
        case 2:
          simplex.Solve2();
          goto case 1;
        case 3:
          simplex.Solve3();
          goto case 1;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }
label_19:
    simplex.GetWitnessPoints(out output.PointA, out output.PointB);
    output.Distance = (output.PointA - output.PointB).Length();
    output.Iterations = num1;
    simplex.WriteCache(ref cache);
    if (!input.UseRadii)
      return;
    if ((double) output.Distance < 1.4012984643248171E-45)
    {
      Vector2 vector2 = (output.PointA + output.PointB) * 0.5f;
      output.PointA = vector2;
      output.PointB = vector2;
      output.Distance = 0.0f;
    }
    else
    {
      float radius1 = proxyA.Radius;
      float radius2 = proxyB.Radius;
      Vector2 vector2 = output.PointB - output.PointA;
      double num2 = (double) Vector2Helpers.Normalize(ref vector2);
      output.Distance = MathF.Max(0.0f, output.Distance - radius1 - radius2);
      output.PointA += vector2 * radius1;
      output.PointB -= vector2 * radius2;
    }
  }
}
