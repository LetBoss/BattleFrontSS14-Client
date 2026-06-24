// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Commands.Math.IsNegativeCommand
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
public sealed class IsNegativeCommand : ToolshedCommand
{
  [CommandImplementation(null)]
  [TakesPipedTypeAsGeneric]
  public bool Operation<T>([PipedArgument] T x) where T : INumberBase<T> => T.IsNegative(x);

  [CommandImplementation(null)]
  [TakesPipedTypeAsGeneric]
  public IEnumerable<bool> Operation<T>([PipedArgument] IEnumerable<T> x) where T : INumberBase<T>
  {
    return x.Select<T, bool>(new Func<T, bool>(this.Operation<T>));
  }
}
