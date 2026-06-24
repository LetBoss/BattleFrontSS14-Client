// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.Commander.CivAirstrikeFlybyMath
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using System;
using System.Numerics;

#nullable disable
namespace Content.Shared._CIV14merka.Commander;

public static class CivAirstrikeFlybyMath
{
  public static Vector2 Left(Vector2 value) => new Vector2(-value.Y, value.X);

  public static float ArcAngle(Vector2 from, Vector2 to, bool ccw)
  {
    float num1 = MathF.Atan2(from.Y, from.X);
    float num2 = MathF.Atan2(to.Y, to.X);
    float num3 = ccw ? num2 - num1 : num1 - num2;
    while ((double) num3 < 0.0)
      num3 += 6.28318548f;
    while ((double) num3 >= 6.2831854820251465)
      num3 -= 6.28318548f;
    return num3;
  }

  public static float ArcLen(Vector2 center, Vector2 from, Vector2 to, bool ccw)
  {
    float num = (from - center).Length();
    return (double) num <= 1.0 / 1000.0 ? 0.0f : CivAirstrikeFlybyMath.ArcAngle(from - center, to - center, ccw) * num;
  }

  public static Vector2 ArcPos(Vector2 center, Vector2 from, Vector2 to, bool ccw, float dist)
  {
    Vector2 from1 = from - center;
    float num1 = from1.Length();
    if ((double) num1 <= 1.0 / 1000.0)
      return to;
    float y = CivAirstrikeFlybyMath.ArcAngle(from1, to - center, ccw);
    if ((double) y <= 1.0 / 1000.0)
      return to;
    float num2 = MathF.Min(dist / num1, y);
    float num3 = MathF.Atan2(from1.Y, from1.X);
    float x = ccw ? num3 + num2 : num3 - num2;
    return center + new Vector2(MathF.Cos(x), MathF.Sin(x)) * num1;
  }

  public static Vector2 ArcTangent(Vector2 center, Vector2 pos, bool ccw)
  {
    Vector2 vector2_1 = pos - center;
    if ((double) vector2_1.LengthSquared() <= 9.9999997473787516E-05)
      return Vector2.UnitX;
    Vector2 vector2_2 = CivAirstrikeFlybyMath.Left(Vector2.Normalize(vector2_1));
    return !ccw ? -vector2_2 : vector2_2;
  }
}
