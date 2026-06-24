// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Commands.Math.SumCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Robust.Shared.Toolshed.Commands.Math;

[ToolshedCommand]
public sealed class SumCommand : ToolshedCommand
{
  [CommandImplementation(null)]
  [TakesPipedTypeAsGeneric]
  public T Sum<T>([PipedArgument] IEnumerable<T> input) where T : IAdditionOperators<T, T, T>
  {
    return input.Aggregate<T>((Func<T, T, T>) ((x, y) => x + y));
  }
}
