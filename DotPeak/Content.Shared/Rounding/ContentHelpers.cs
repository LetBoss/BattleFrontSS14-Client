// Decompiled with JetBrains decompiler
// Type: Content.Shared.Rounding.ContentHelpers
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using System;

#nullable disable
namespace Content.Shared.Rounding;

public static class ContentHelpers
{
  public static int RoundToLevels(double actual, double max, int levels)
  {
    if (levels <= 0)
      throw new ArgumentException("Levels must be greater than 0.", nameof (levels));
    if (actual >= max)
      return levels - 1;
    return actual <= 0.0 ? 0 : (int) Math.Ceiling(actual / max * (double) (levels - 2));
  }

  public static int RoundToNearestLevels(double actual, double max, int levels)
  {
    if (levels <= 1)
      throw new ArgumentException("Levels must be greater than 1.", nameof (levels));
    if (actual >= max)
      return levels;
    return actual <= 0.0 ? 0 : (int) Math.Round(actual / max * (double) levels, MidpointRounding.AwayFromZero);
  }

  public static int RoundToEqualLevels(double actual, double max, int levels)
  {
    if (levels <= 1)
      throw new ArgumentException("Levels must be greater than 1.", nameof (levels));
    if (actual >= max)
      return levels - 1;
    return actual <= 0.0 ? 0 : (int) Math.Round(actual / max * (double) levels, MidpointRounding.ToZero);
  }
}
