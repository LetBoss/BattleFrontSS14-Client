// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Commands.Math.StepNextCommand
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
public sealed class StepNextCommand : ToolshedCommand
{
  [CommandImplementation(null)]
  [TakesPipedTypeAsGeneric]
  public T Operation<T>([PipedArgument] T x) where T : IFloatingPointIeee754<T>
  {
    return T.BitIncrement(x);
  }

  [CommandImplementation(null)]
  [TakesPipedTypeAsGeneric]
  public IEnumerable<T> Operation<T>([PipedArgument] IEnumerable<T> x) where T : IFloatingPointIeee754<T>
  {
    return x.Select<T, T>(new Func<T, T>(this.Operation<T>));
  }
}
