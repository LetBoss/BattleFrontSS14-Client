// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Commands.Math.SignificandBitCountCommand
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
public sealed class SignificandBitCountCommand : ToolshedCommand
{
  [CommandImplementation(null)]
  [TakesPipedTypeAsGeneric]
  public int Operation<T>([PipedArgument] T x) where T : IFloatingPoint<T>
  {
    return x.GetSignificandBitLength();
  }

  [CommandImplementation(null)]
  [TakesPipedTypeAsGeneric]
  public IEnumerable<int> Operation<T>([PipedArgument] IEnumerable<T> x) where T : IFloatingPoint<T>
  {
    return x.Select<T, int>(new Func<T, int>(this.Operation<T>));
  }
}
