// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Commands.Math.CheckedToCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Toolshed.TypeParsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Robust.Shared.Toolshed.Commands.Math;

[ToolshedCommand]
public sealed class CheckedToCommand : ToolshedCommand
{
  private static Type[] _parsers = new Type[1]
  {
    typeof (TypeTypeParser)
  };

  public override Type[] TypeParameterParsers => CheckedToCommand._parsers;

  [CommandImplementation(null)]
  [TakesPipedTypeAsGeneric]
  public TOut Operation<TOut, T>([PipedArgument] T x)
    where TOut : INumberBase<TOut>
    where T : INumberBase<T>
  {
    return TOut.CreateChecked<T>(x);
  }

  [CommandImplementation(null)]
  [TakesPipedTypeAsGeneric]
  public IEnumerable<TOut> Operation<TOut, T>([PipedArgument] IEnumerable<T> x)
    where TOut : INumberBase<TOut>
    where T : INumberBase<T>
  {
    return x.Select<T, TOut>(new Func<T, TOut>(this.Operation<TOut, T>));
  }
}
