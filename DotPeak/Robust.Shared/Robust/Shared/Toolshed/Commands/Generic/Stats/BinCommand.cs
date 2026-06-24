// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Commands.Generic.Stats.BinCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.Toolshed.Commands.Generic.Stats;

[ToolshedCommand]
public sealed class BinCommand : ToolshedCommand
{
  [CommandImplementation(null)]
  [TakesPipedTypeAsGeneric]
  public IDictionary<T, int> Bin<T>([PipedArgument] IEnumerable<T> input) where T : IComparable<T>
  {
    Dictionary<T, int> dictionary = new Dictionary<T, int>();
    foreach (T key in input)
    {
      dictionary.TryAdd(key, 0);
      dictionary[key]++;
    }
    return (IDictionary<T, int>) dictionary;
  }
}
