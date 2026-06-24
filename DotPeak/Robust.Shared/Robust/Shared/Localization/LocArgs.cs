// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Localization.LocArgs
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.Localization;

public readonly struct LocArgs(
  IReadOnlyList<ILocValue> args,
  IReadOnlyDictionary<string, ILocValue> options)
{
  public IReadOnlyList<ILocValue> Args { get; } = args;

  public IReadOnlyDictionary<string, ILocValue> Options { get; } = options;
}
