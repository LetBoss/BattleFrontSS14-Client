// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Commands.Math.PowCommand
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
public sealed class PowCommand : ToolshedCommand
{
  [CommandImplementation(null)]
  [TakesPipedTypeAsGeneric]
  public T Operation<T>([PipedArgument] T x, T y) where T : IPowerFunctions<T> => T.Pow(x, y);

  [CommandImplementation(null)]
  [TakesPipedTypeAsGeneric]
  public IEnumerable<T> Operation<T>([PipedArgument] IEnumerable<T> x, IEnumerable<T> y) where T : IPowerFunctions<T>
  {
    return x.Zip<T, T>(y).Select<(T, T), T>((Func<(T, T), T>) (inp =>
    {
      (T First, T Second) tuple = inp;
      return this.Operation<T>(tuple.First, tuple.Second);
    }));
  }
}
