// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Commands.Generic.TeeCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Toolshed.Syntax;
using Robust.Shared.Toolshed.TypeParsers;
using System;
using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.Toolshed.Commands.Generic;

[ToolshedCommand]
public sealed class TeeCommand : ToolshedCommand
{
  private static Type[] _parsers = new Type[1]
  {
    typeof (MapBlockOutputParser)
  };

  public override Type[] TypeParameterParsers => TeeCommand._parsers;

  [CommandImplementation(null)]
  [TakesPipedTypeAsGeneric]
  public IEnumerable<TIn> Tee<TOut, TIn>(
    IInvocationContext ctx,
    [PipedArgument] IEnumerable<TIn> value,
    Block<TIn, TOut> block)
  {
    foreach (TIn input in value)
    {
      block.Invoke(input, ctx);
      if (ctx.HasErrors)
        break;
      yield return input;
    }
  }
}
