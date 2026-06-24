// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Commands.Math.DivVecCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Robust.Shared.Toolshed.Commands.Math;

[ToolshedCommand(Name = "//")]
public sealed class DivVecCommand : ToolshedCommand
{
  [CommandImplementation(null)]
  [TakesPipedTypeAsGeneric]
  public IEnumerable<T> Operation<T>([PipedArgument] IEnumerable<T> x, T y) where T : INumberBase<T>
  {
    return T.IsZero(y) ? x.Select<T, T>((Func<T, T>) (_ => T.Zero)) : x.Select<T, T>((Func<T, T>) (i => i / y));
  }
}
