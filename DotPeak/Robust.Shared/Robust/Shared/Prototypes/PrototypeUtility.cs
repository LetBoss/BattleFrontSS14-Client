// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Prototypes.PrototypeUtility
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;

#nullable enable
namespace Robust.Shared.Prototypes;

public static class PrototypeUtility
{
  public const string PrototypeNameEnding = "Prototype";

  public static string CalculatePrototypeName(string type)
  {
    ReadOnlySpan<char> readOnlySpan = type.AsSpan();
    if (!type.EndsWith("Prototype"))
      return $"{char.ToLowerInvariant(readOnlySpan[0])}{readOnlySpan.Slice(1).ToString()}";
    return $"{char.ToLowerInvariant(readOnlySpan[0])}{readOnlySpan.Slice(1, readOnlySpan.Length - "Prototype".Length - 1).ToString()}";
  }
}
