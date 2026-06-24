// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Commands.Math.AddCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Robust.Shared.Toolshed.Commands.Math;

[ToolshedCommand(Name = "+")]
public sealed class AddCommand : ToolshedCommand
{
  [CommandImplementation(null)]
  [TakesPipedTypeAsGeneric]
  public T Operation<T>([PipedArgument] T x, T y) where T : IAdditionOperators<T, T, T> => x + y;

  [CommandImplementation(null)]
  [TakesPipedTypeAsGeneric]
  public IEnumerable<T> Operation<T>([PipedArgument] IEnumerable<T> x, IEnumerable<T> y) where T : IAdditionOperators<T, T, T>
  {
    return x.Zip<T, T>(y).Select<(T, T), T>((Func<(T, T), T>) (inp =>
    {
      (T First, T Second) tuple = inp;
      return this.Operation<T>(tuple.First, tuple.Second);
    }));
  }

  [CommandImplementation(null)]
  public Vector2 Operation([PipedArgument] Vector2 x, Vector2 y) => x + y;

  [CommandImplementation(null)]
  public IEnumerable<Vector2> Operation([PipedArgument] IEnumerable<Vector2> x, IEnumerable<Vector2> y)
  {
    return x.Zip<Vector2, Vector2>(y).Select<(Vector2, Vector2), Vector2>((Func<(Vector2, Vector2), Vector2>) (inp =>
    {
      (Vector2 First, Vector2 Second) tuple = inp;
      return this.Operation(tuple.First, tuple.Second);
    }));
  }
}
