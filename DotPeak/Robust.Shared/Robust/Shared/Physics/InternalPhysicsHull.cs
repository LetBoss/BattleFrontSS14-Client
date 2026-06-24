// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.InternalPhysicsHull
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Maths;
using System;
using System.Numerics;

#nullable disable
namespace Robust.Shared.Physics;

internal ref struct InternalPhysicsHull
{
  public Span<Vector2> Points;
  public int Count;

  internal InternalPhysicsHull(Span<Vector2> vertices, int count)
    : this()
  {
    this.Count = count;
    this.Points = vertices.Slice(0, count);
  }

  private static unsafe InternalPhysicsHull RecurseHull(
    Vector2 p1,
    Vector2 p2,
    Span<Vector2> ps,
    int count)
  {
    InternalPhysicsHull internalPhysicsHull1 = new InternalPhysicsHull()
    {
      Count = 0
    };
    if (count == 0)
      return internalPhysicsHull1;
    Vector2 vector2_1 = p2 - p1;
    double num1 = (double) Vector2Helpers.Normalize(ref vector2_1);
    ; // Unable to render the statement
    Span<Vector2> ps1 = new Span<Vector2>((void*) pointer, 8);
    int count1 = 0;
    int index1 = 0;
    float num2 = Vector2Helpers.Cross(ps[index1] - p1, vector2_1);
    if ((double) num2 > 0.0)
      ps1[count1++] = ps[index1];
    for (int index2 = 1; index2 < count; ++index2)
    {
      float num3 = Vector2Helpers.Cross(ps[index2] - p1, vector2_1);
      if ((double) num3 > (double) num2)
      {
        index1 = index2;
        num2 = num3;
      }
      if ((double) num3 > 0.0)
        ps1[count1++] = ps[index2];
    }
    if ((double) num2 < 0.0099999997764825821)
      return internalPhysicsHull1;
    internalPhysicsHull1.Points = (Span<Vector2>) new Vector2[8];
    Vector2 vector2_2 = ps[index1];
    InternalPhysicsHull internalPhysicsHull2 = InternalPhysicsHull.RecurseHull(p1, vector2_2, ps1, count1);
    InternalPhysicsHull internalPhysicsHull3 = InternalPhysicsHull.RecurseHull(vector2_2, p2, ps1, count1);
    for (int index3 = 0; index3 < internalPhysicsHull2.Count; ++index3)
      internalPhysicsHull1.Points[internalPhysicsHull1.Count++] = internalPhysicsHull2.Points[index3];
    internalPhysicsHull1.Points[internalPhysicsHull1.Count++] = vector2_2;
    for (int index4 = 0; index4 < internalPhysicsHull3.Count; ++index4)
      internalPhysicsHull1.Points[internalPhysicsHull1.Count++] = internalPhysicsHull3.Points[index4];
    return internalPhysicsHull1;
  }

  public static unsafe InternalPhysicsHull ComputeHull(ReadOnlySpan<Vector2> points, int count)
  {
    InternalPhysicsHull hull = new InternalPhysicsHull();
    if (count < 3 || count > 8)
    {
      hull.Count = 0;
      return hull;
    }
    count = Math.Min(count, 8);
    Box2 box2;
    // ISSUE: explicit constructor call
    ((Box2) ref box2).\u002Ector(float.MaxValue, float.MaxValue, float.MinValue, float.MinValue);
    ; // Unable to render the statement
    Span<Vector2> span = new Span<Vector2>((void*) pointer1, 8);
    int num1 = 0;
    Vector2 vector2_1;
    for (int index1 = 0; index1 < count; ++index1)
    {
      box2.BottomLeft = Vector2.Min(box2.BottomLeft, points[index1]);
      box2.TopRight = Vector2.Max(box2.TopRight, points[index1]);
      Vector2 vector2_2 = points[index1];
      bool flag = true;
      for (int index2 = 0; index2 < index1; ++index2)
      {
        vector2_1 = points[index2] - vector2_2;
        if ((double) vector2_1.LengthSquared() < 0.00039999998989515007)
        {
          flag = false;
          break;
        }
      }
      if (flag)
        span[num1++] = vector2_2;
    }
    if (num1 < 3)
      return hull;
    Vector2 center = ((Box2) ref box2).Center;
    int index3 = 0;
    vector2_1 = span[index3] - center;
    float num2 = vector2_1.LengthSquared();
    for (int index4 = 1; index4 < num1; ++index4)
    {
      vector2_1 = span[index4] - center;
      float num3 = vector2_1.LengthSquared();
      if ((double) num3 > (double) num2)
      {
        index3 = index4;
        num2 = num3;
      }
    }
    Vector2 vector2_3 = span[index3];
    span[index3] = span[num1 - 1];
    int num4 = num1 - 1;
    int index5 = 0;
    vector2_1 = span[index5] - vector2_3;
    float num5 = vector2_1.LengthSquared();
    for (int index6 = 1; index6 < num4; ++index6)
    {
      vector2_1 = span[index6] - vector2_3;
      float num6 = vector2_1.LengthSquared();
      if ((double) num6 > (double) num5)
      {
        index5 = index6;
        num5 = num6;
      }
    }
    Vector2 vector2_4 = span[index5];
    span[index5] = span[num4 - 1];
    int num7 = num4 - 1;
    ; // Unable to render the statement
    Span<Vector2> ps1 = new Span<Vector2>((void*) pointer2, 6);
    int count1 = 0;
    ; // Unable to render the statement
    Span<Vector2> ps2 = new Span<Vector2>((void*) pointer3, 6);
    int count2 = 0;
    Vector2 vector2_5 = vector2_4 - vector2_3;
    double num8 = (double) Vector2Helpers.Normalize(ref vector2_5);
    for (int index7 = 0; index7 < num7; ++index7)
    {
      float num9 = Vector2Helpers.Cross(span[index7] - vector2_3, vector2_5);
      if ((double) num9 >= 0.0099999997764825821)
        ps1[count1++] = span[index7];
      else if ((double) num9 <= -0.0099999997764825821)
        ps2[count2++] = span[index7];
    }
    InternalPhysicsHull internalPhysicsHull1 = InternalPhysicsHull.RecurseHull(vector2_3, vector2_4, ps1, count1);
    InternalPhysicsHull internalPhysicsHull2 = InternalPhysicsHull.RecurseHull(vector2_4, vector2_3, ps2, count2);
    if (internalPhysicsHull1.Count == 0 && internalPhysicsHull2.Count == 0)
    {
      hull.Count = 0;
      return hull;
    }
    hull.Points = (Span<Vector2>) new Vector2[8];
    hull.Points[hull.Count++] = vector2_3;
    for (int index8 = 0; index8 < internalPhysicsHull1.Count; ++index8)
      hull.Points[hull.Count++] = internalPhysicsHull1.Points[index8];
    hull.Points[hull.Count++] = vector2_4;
    for (int index9 = 0; index9 < internalPhysicsHull2.Count; ++index9)
      hull.Points[hull.Count++] = internalPhysicsHull2.Points[index9];
    bool flag1 = true;
    while (flag1 && hull.Count > 2)
    {
      flag1 = false;
      for (int index10 = 0; index10 < hull.Count; ++index10)
      {
        int index11 = index10;
        int index12 = (index10 + 1) % hull.Count;
        int index13 = (index10 + 2) % hull.Count;
        Vector2 vector2_6 = hull.Points[index11];
        Vector2 vector2_7 = hull.Points[index12];
        vector2_5 = hull.Points[index13] - vector2_6;
        double num10 = (double) Vector2Helpers.Normalize(ref vector2_5);
        Vector2 vector2_8 = vector2_7 - vector2_6;
        if ((double) Vector2Helpers.Cross(vector2_7 - vector2_6, vector2_5) <= 0.0099999997764825821)
        {
          for (int index14 = index12; index14 < hull.Count - 1; ++index14)
            hull.Points[index14] = hull.Points[index14 + 1];
          --hull.Count;
          flag1 = true;
          break;
        }
      }
    }
    if (hull.Count < 3)
      hull.Count = 0;
    return hull;
  }

  public static bool ValidateHull(InternalPhysicsHull hull)
  {
    if (hull.Count < 3 || 8 < hull.Count)
      return false;
    for (int index1 = 0; index1 < hull.Count; ++index1)
    {
      int index2 = index1;
      int index3 = index1 < hull.Count - 1 ? index2 + 1 : 0;
      Vector2 vector2_1 = hull.Points[index2];
      Vector2 vector2_2 = hull.Points[index3] - vector2_1;
      double num = (double) Vector2Helpers.Normalize(ref vector2_2);
      for (int index4 = 0; index4 < hull.Count; ++index4)
      {
        if (index4 != index2 && index4 != index3 && (double) Vector2Helpers.Cross(hull.Points[index4] - vector2_1, vector2_2) >= 0.0)
          return false;
      }
    }
    for (int index5 = 0; index5 < hull.Count; ++index5)
    {
      int index6 = index5;
      int index7 = (index5 + 1) % hull.Count;
      int index8 = (index5 + 2) % hull.Count;
      Vector2 vector2_3 = hull.Points[index6];
      Vector2 vector2_4 = hull.Points[index7];
      Vector2 vector2_5 = hull.Points[index8] - vector2_3;
      double num = (double) Vector2Helpers.Normalize(ref vector2_5);
      Vector2 vector2_6 = vector2_4 - vector2_3;
      if ((double) Vector2Helpers.Cross(vector2_4 - vector2_3, vector2_5) <= 0.004999999888241291)
        return false;
    }
    return true;
  }
}
