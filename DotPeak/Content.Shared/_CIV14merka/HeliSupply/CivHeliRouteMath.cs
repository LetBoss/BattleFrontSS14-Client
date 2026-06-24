// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.HeliSupply.CivHeliRouteMath
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Shared._CIV14merka.HeliSupply;

public static class CivHeliRouteMath
{
  private const float MinSeg = 0.05f;
  private const float MaxSegLen = 4f;
  private const float SpeedRampPerTile = 0.08f;
  private const float MinSpeedFactor = 0.3f;

  public static CivHeliPath Build(
    IReadOnlyList<Vector2> points,
    float smoothing,
    int passes,
    float turnSlowdown,
    int fixedIndex = -1,
    float dropSlowZone = 0.0f,
    float dropSlowFactor = 1f)
  {
    CivHeliPath path = new CivHeliPath()
    {
      PointDist = new float[points.Count]
    };
    if (points.Count < 2)
      return path;
    float dropDist = -1f;
    List<Vector2> poly1;
    if (fixedIndex > 0 && fixedIndex < points.Count - 1)
    {
      List<Vector2> poly2 = CivHeliRouteMath.Smooth(CivHeliRouteMath.SubList(points, 0, fixedIndex + 1), smoothing, passes);
      List<Vector2> vector2List = CivHeliRouteMath.Smooth(CivHeliRouteMath.SubList(points, fixedIndex, points.Count - fixedIndex), smoothing, passes);
      dropDist = CivHeliRouteMath.PolyLength(poly2);
      poly1 = poly2;
      for (int index = 1; index < vector2List.Count; ++index)
        poly1.Add(vector2List[index]);
    }
    else
      poly1 = CivHeliRouteMath.Smooth(new List<Vector2>((IEnumerable<Vector2>) points), smoothing, passes);
    CivHeliRouteMath.RemoveDuplicates(poly1);
    CivHeliRouteMath.Subdivide(poly1);
    float[] speedFactors = CivHeliRouteMath.ComputeSpeedFactors(poly1, turnSlowdown, dropDist, dropSlowZone, dropSlowFactor);
    float num1 = 0.0f;
    float num2 = 0.0f;
    for (int index = 0; index < poly1.Count - 1; ++index)
    {
      CivHeliPathSegment civHeliPathSegment = new CivHeliPathSegment()
      {
        A = poly1[index],
        B = poly1[index + 1],
        Length = (poly1[index + 1] - poly1[index]).Length(),
        SpeedFactor = speedFactors[index]
      };
      path.Segments.Add(civHeliPathSegment);
      num1 += civHeliPathSegment.Length;
      num2 += civHeliPathSegment.Cost;
    }
    path.Total = num1;
    path.TotalCost = num2;
    for (int index = 0; index < points.Count; ++index)
      path.PointDist[index] = CivHeliRouteMath.NearestDistAlong(path, points[index]);
    path.PointDist[0] = 0.0f;
    float[] pointDist = path.PointDist;
    pointDist[pointDist.Length - 1] = num1;
    if ((double) dropDist >= 0.0)
      path.PointDist[fixedIndex] = dropDist;
    return path;
  }

  private static List<Vector2> Smooth(List<Vector2> pts, float maxCut, int passes)
  {
    if ((double) maxCut <= 0.0099999997764825821 || passes <= 0)
      return pts;
    for (int index1 = 0; index1 < passes && pts.Count >= 3; ++index1)
    {
      List<Vector2> vector2List1 = new List<Vector2>(pts.Count * 2)
      {
        pts[0]
      };
      for (int index2 = 0; index2 < pts.Count - 1; ++index2)
      {
        Vector2 pt1 = pts[index2];
        Vector2 pt2 = pts[index2 + 1];
        Vector2 vector2_1 = pt2 - pt1;
        float num1 = vector2_1.Length();
        if ((double) num1 >= 0.05000000074505806)
        {
          Vector2 vector2_2 = vector2_1 / num1;
          float num2 = MathF.Min(num1 * 0.25f, maxCut);
          if (index2 > 0)
            vector2List1.Add(pt1 + vector2_2 * num2);
          if (index2 < pts.Count - 2)
            vector2List1.Add(pt2 - vector2_2 * num2);
        }
      }
      List<Vector2> vector2List2 = vector2List1;
      List<Vector2> vector2List3 = pts;
      Vector2 vector2 = vector2List3[vector2List3.Count - 1];
      vector2List2.Add(vector2);
      pts = vector2List1;
    }
    return pts;
  }

  private static float[] ComputeSpeedFactors(
    List<Vector2> poly,
    float turnSlowdown,
    float dropDist,
    float dropSlowZone,
    float dropSlowFactor)
  {
    int length = Math.Max(0, poly.Count - 1);
    float[] speedFactors = new float[length];
    for (int index = 0; index < length; ++index)
      speedFactors[index] = 1f;
    if (length < 2)
      return speedFactors;
    float[] numArray = new float[length];
    for (int index = 0; index < length; ++index)
      numArray[index] = (poly[index + 1] - poly[index]).Length();
    if ((double) turnSlowdown > 0.0)
    {
      for (int index = 1; index < poly.Count - 1; ++index)
      {
        Vector2 vector2_1 = poly[index] - poly[index - 1];
        Vector2 vector2_2 = poly[index + 1] - poly[index];
        float num1 = vector2_1.Length();
        float num2 = vector2_2.Length();
        if ((double) num1 >= 0.05000000074505806 && (double) num2 >= 0.05000000074505806)
        {
          float num3 = MathF.Acos(Math.Clamp(Vector2.Dot(vector2_1 / num1, vector2_2 / num2), -1f, 1f)) / MathF.Max((float) (0.5 * ((double) num1 + (double) num2)), 0.1f);
          float y = Math.Clamp((float) (1.0 / (1.0 + (double) turnSlowdown * (double) num3)), 0.3f, 1f);
          speedFactors[index - 1] = MathF.Min(speedFactors[index - 1], y);
          speedFactors[index] = MathF.Min(speedFactors[index], y);
        }
      }
    }
    if ((double) dropDist >= 0.0 && (double) dropSlowZone > 0.0099999997764825821 && (double) dropSlowFactor < 1.0)
    {
      float num4 = 0.0f;
      for (int index = 0; index < length; ++index)
      {
        float num5 = num4 + numArray[index] * 0.5f;
        num4 += numArray[index];
        float num6 = dropDist - num5;
        if ((double) num6 >= 0.0 && (double) num6 <= (double) dropSlowZone)
          speedFactors[index] = MathF.Min(speedFactors[index], dropSlowFactor);
      }
    }
    for (int index = length - 2; index >= 0; --index)
      speedFactors[index] = MathF.Min(speedFactors[index], speedFactors[index + 1] + 0.08f * numArray[index]);
    for (int index = 1; index < length; ++index)
      speedFactors[index] = MathF.Min(speedFactors[index], speedFactors[index - 1] + 0.08f * numArray[index - 1]);
    return speedFactors;
  }

  private static void Subdivide(List<Vector2> poly)
  {
    for (int index1 = poly.Count - 2; index1 >= 0; --index1)
    {
      Vector2 vector2_1 = poly[index1];
      Vector2 vector2_2 = poly[index1 + 1];
      float num1 = (vector2_2 - vector2_1).Length();
      if ((double) num1 > 4.0)
      {
        int num2 = (int) MathF.Ceiling(num1 / 4f);
        for (int index2 = num2 - 1; index2 >= 1; --index2)
          poly.Insert(index1 + 1, Vector2.Lerp(vector2_1, vector2_2, (float) index2 / (float) num2));
      }
    }
  }

  private static List<Vector2> SubList(IReadOnlyList<Vector2> points, int start, int count)
  {
    List<Vector2> vector2List = new List<Vector2>(count);
    for (int index = 0; index < count; ++index)
      vector2List.Add(points[start + index]);
    return vector2List;
  }

  private static float PolyLength(List<Vector2> poly)
  {
    float num = 0.0f;
    for (int index = 0; index < poly.Count - 1; ++index)
      num += (poly[index + 1] - poly[index]).Length();
    return num;
  }

  private static void RemoveDuplicates(List<Vector2> poly)
  {
    for (int index = poly.Count - 1; index > 0; --index)
    {
      if ((double) (poly[index] - poly[index - 1]).Length() < 0.05000000074505806)
        poly.RemoveAt(index);
    }
  }

  private static float NearestDistAlong(CivHeliPath path, Vector2 point)
  {
    float num1 = float.MaxValue;
    float num2 = 0.0f;
    float num3 = 0.0f;
    foreach (CivHeliPathSegment segment in path.Segments)
    {
      Vector2 vector2_1 = segment.B - segment.A;
      float num4 = vector2_1.LengthSquared();
      float num5 = (double) num4 > 9.9999997473787516E-05 ? Math.Clamp(Vector2.Dot(point - segment.A, vector2_1) / num4, 0.0f, 1f) : 0.0f;
      Vector2 vector2_2 = segment.A + vector2_1 * num5;
      float num6 = (point - vector2_2).LengthSquared();
      if ((double) num6 < (double) num1)
      {
        num1 = num6;
        num2 = num3 + segment.Length * num5;
      }
      num3 += segment.Length;
    }
    return num2;
  }
}
