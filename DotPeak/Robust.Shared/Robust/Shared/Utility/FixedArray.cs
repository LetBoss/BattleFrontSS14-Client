// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.FixedArray
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;

#nullable enable
namespace Robust.Shared.Utility;

internal static class FixedArray
{
  public static Span<T> Alloc2<T>(out FixedArray2<T> discard)
  {
    discard = new FixedArray2<T>();
    return discard.AsSpan;
  }

  public static Span<T> Alloc4<T>(out FixedArray4<T> discard)
  {
    discard = new FixedArray4<T>();
    return discard.AsSpan;
  }

  public static Span<T> Alloc8<T>(out FixedArray8<T> discard)
  {
    discard = new FixedArray8<T>();
    return discard.AsSpan;
  }

  public static Span<T> Alloc16<T>(out FixedArray16<T> discard)
  {
    discard = new FixedArray16<T>();
    return discard.AsSpan;
  }

  public static Span<T> Alloc32<T>(out FixedArray32<T> discard)
  {
    discard = new FixedArray32<T>();
    return discard.AsSpan;
  }

  public static Span<T> Alloc64<T>(out FixedArray64<T> discard)
  {
    discard = new FixedArray64<T>();
    return discard.AsSpan;
  }

  public static Span<T> Alloc128<T>(out FixedArray128<T> discard)
  {
    discard = new FixedArray128<T>();
    return discard.AsSpan;
  }
}
