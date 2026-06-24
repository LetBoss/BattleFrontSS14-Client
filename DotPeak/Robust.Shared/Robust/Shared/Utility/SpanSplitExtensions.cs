// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.SpanSplitExtensions
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;

#nullable enable
namespace Robust.Shared.Utility;

internal static class SpanSplitExtensions
{
  public static bool SplitFindNext<T>(
    ref ReadOnlySpan<T> source,
    T splitOn,
    out ReadOnlySpan<T> splitValue)
    where T : IEquatable<T>
  {
    if (source.IsEmpty)
    {
      splitValue = ReadOnlySpan<T>.Empty;
      return false;
    }
    int length = source.IndexOf<T>(splitOn);
    if (length == -1)
    {
      splitValue = source;
      source = ReadOnlySpan<T>.Empty;
    }
    else
    {
      splitValue = source.Slice(0, length);
      ref ReadOnlySpan<T> local1 = ref source;
      ref ReadOnlySpan<T> local2 = ref source;
      int start = length + 1;
      ReadOnlySpan<T> readOnlySpan = local2.Slice(start, local2.Length - start);
      local1 = readOnlySpan;
    }
    return true;
  }
}
