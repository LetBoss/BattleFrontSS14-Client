// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Commands.Math.AddVecCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Robust.Shared.Toolshed.Commands.Math;

[ToolshedCommand(Name = "+/")]
public sealed class AddVecCommand : ToolshedCommand
{
  [CommandImplementation(null)]
  [TakesPipedTypeAsGeneric]
  public IEnumerable<T> Operation<T>([PipedArgument] IEnumerable<T> x, T y) where T : IAdditionOperators<T, T, T>
  {
    return x.Select<T, T>((Func<T, T>) (i => i + y));
  }

  [CommandImplementation(null)]
  public IEnumerable<Vector2> Operation([PipedArgument] IEnumerable<Vector2> x, Vector2 y)
  {
    return x.Select<Vector2, Vector2>((Func<Vector2, Vector2>) (i => i + y));
  }
}
