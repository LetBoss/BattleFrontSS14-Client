// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Maths.RMCMathExtensions
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using System;

#nullable disable
namespace Content.Shared._RMC14.Maths;

public static class RMCMathExtensions
{
  public static float CircleAreaFromSquareSide(float squareSide)
  {
    return squareSide / (float) Math.Sqrt(Math.PI);
  }

  public static float CircleAreaFromSquareAbilityRange(float squareRadius)
  {
    return (float) (((double) squareRadius * 2.0 + 1.0) / Math.Sqrt(Math.PI));
  }
}
