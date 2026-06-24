// Decompiled with JetBrains decompiler
// Type: Content.Shared.FixedPoint.FixedPoint4EnumerableExt
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using System.Collections.Generic;

#nullable enable
namespace Content.Shared.FixedPoint;

public static class FixedPoint4EnumerableExt
{
  public static FixedPoint4 Sum(this IEnumerable<FixedPoint4> source)
  {
    FixedPoint4 zero = FixedPoint4.Zero;
    foreach (FixedPoint4 fixedPoint4 in source)
      zero += fixedPoint4;
    return zero;
  }
}
