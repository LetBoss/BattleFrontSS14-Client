// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.RamerDouglasPeuckerSimplifier
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Robust.Shared.Physics;

public sealed class RamerDouglasPeuckerSimplifier : IVerticesSimplifier
{
  public List<Vector2> Simplify(List<Vector2> vertices, float distanceTolerance)
  {
    if (vertices.Count <= 3)
      return vertices;
    Span<bool> usePoint = stackalloc bool[vertices.Count];
    for (int index = 0; index < vertices.Count; ++index)
      usePoint[index] = true;
    RamerDouglasPeuckerSimplifier.SimplifySection(vertices, 0, vertices.Count - 1, usePoint, distanceTolerance);
    List<Vector2> vector2List = new List<Vector2>(vertices.Count);
    for (int index = 0; index < vertices.Count; ++index)
    {
      if (usePoint[index])
        vector2List.Add(vertices[index]);
    }
    return vector2List;
  }

  private static void SimplifySection(
    List<Vector2> vertices,
    int i,
    int j,
    Span<bool> usePoint,
    float distanceTolerance)
  {
    if (i + 1 == j)
      return;
    Vector2 start = vertices[i];
    Vector2 end = vertices[j];
    double num1 = -1.0;
    int num2 = i;
    for (int index = i + 1; index < j; ++index)
    {
      double num3 = (double) RamerDouglasPeuckerSimplifier.DistanceBetweenPointAndLineSegment(vertices[index], in start, in end);
      if (num3 > num1)
      {
        num1 = num3;
        num2 = index;
      }
    }
    if (num1 <= (double) distanceTolerance)
    {
      for (int index = i + 1; index < j; ++index)
        usePoint[index] = false;
    }
    else
    {
      RamerDouglasPeuckerSimplifier.SimplifySection(vertices, i, num2, usePoint, distanceTolerance);
      RamerDouglasPeuckerSimplifier.SimplifySection(vertices, num2, j, usePoint, distanceTolerance);
    }
  }

  public static float DistanceBetweenPointAndLineSegment(
    in Vector2 point,
    in Vector2 start,
    in Vector2 end)
  {
    if (start == end)
      return (point - start).Length();
    Vector2 vector2_1 = end - start;
    float num1 = Vector2.Dot(point - start, vector2_1);
    if ((double) num1 <= 0.0)
      return (point - start).Length();
    float num2 = Vector2.Dot(vector2_1, vector2_1);
    if ((double) num2 <= (double) num1)
      return (point - end).Length();
    float num3 = num1 / num2;
    Vector2 vector2_2 = start + vector2_1 * num3;
    return (point - vector2_2).Length();
  }
}
