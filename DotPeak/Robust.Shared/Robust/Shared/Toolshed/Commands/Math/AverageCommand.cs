// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Commands.Math.AverageCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System.Collections.Generic;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Robust.Shared.Toolshed.Commands.Math;

[ToolshedCommand]
public sealed class AverageCommand : ToolshedCommand
{
  [CommandImplementation(null)]
  [TakesPipedTypeAsGeneric]
  public T Average<T>([PipedArgument] IEnumerable<T> input) where T : INumberBase<T>
  {
    T[] array = input.ToArray<T>();
    T zero = T.Zero;
    foreach (T obj in array)
      zero += obj;
    return zero / T.CreateChecked<int>(array.Length);
  }
}
