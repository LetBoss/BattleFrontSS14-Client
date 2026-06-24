// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Extensions.EnumExtensions
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Extensions;

public static class EnumExtensions
{
  public static T NextWrap<T>(this T en) where T : struct, Enum
  {
    Span<T> span = EnumExtensions.EnumInformation<T>.Values.AsSpan<T>();
    if (span.Length == 0)
      return default (T);
    for (int index = 0; index < span.Length; ++index)
    {
      T y = span[index];
      if (EqualityComparer<T>.Default.Equals(en, y))
        return span.Length <= index + 1 ? span[0] : span[index + 1];
    }
    return span[0];
  }

  private static class EnumInformation<T> where T : struct, Enum
  {
    internal static readonly T[] Values = Enum.GetValues<T>();
  }
}
