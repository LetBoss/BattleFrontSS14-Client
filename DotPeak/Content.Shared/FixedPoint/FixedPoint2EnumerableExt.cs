// Decompiled with JetBrains decompiler
// Type: Content.Shared.FixedPoint.FixedPoint2EnumerableExt
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using System.Collections.Generic;

#nullable enable
namespace Content.Shared.FixedPoint;

public static class FixedPoint2EnumerableExt
{
  public static FixedPoint2 Sum(this IEnumerable<FixedPoint2> source)
  {
    FixedPoint2 zero = FixedPoint2.Zero;
    foreach (FixedPoint2 fixedPoint2 in source)
      zero += fixedPoint2;
    return zero;
  }
}
