// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.Manager.Definition.DataDefinitionUtility
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;

#nullable enable
namespace Robust.Shared.Serialization.Manager.Definition;

public static class DataDefinitionUtility
{
  public static string AutoGenerateTag(string name)
  {
    ReadOnlySpan<char> readOnlySpan = name.AsSpan();
    return $"{char.ToLowerInvariant(readOnlySpan[0])}{readOnlySpan.Slice(1).ToString()}";
  }
}
