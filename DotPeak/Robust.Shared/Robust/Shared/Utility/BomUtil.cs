// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.BomUtil
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;

#nullable disable
namespace Robust.Shared.Utility;

internal static class BomUtil
{
  public static Span<byte> SkipBom(Span<byte> span)
  {
    if (!BomUtil.HasBom(span))
      return span;
    ref Span<byte> local = ref span;
    return local.Slice(3, local.Length - 3);
  }

  public static bool HasBom(Span<byte> span)
  {
    return span[2] == (byte) 191 && span[1] == (byte) 187 && span[0] == (byte) 239;
  }
}
