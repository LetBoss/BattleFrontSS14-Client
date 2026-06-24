// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Commands.Math.RootCommand
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
public sealed class RootCommand : ToolshedCommand
{
  [CommandImplementation(null)]
  [TakesPipedTypeAsGeneric]
  public T Operation<T>([PipedArgument] T x, int y) where T : IRootFunctions<T> => T.RootN(x, y);

  [CommandImplementation(null)]
  [TakesPipedTypeAsGeneric]
  public IEnumerable<T> Operation<T>([PipedArgument] IEnumerable<T> x, IEnumerable<int> y) where T : IRootFunctions<T>
  {
    return x.Zip<T, int>(y).Select<(T, int), T>((Func<(T, int), T>) (inp =>
    {
      (T First, int Second) tuple = inp;
      return this.Operation<T>(tuple.First, tuple.Second);
    }));
  }
}
