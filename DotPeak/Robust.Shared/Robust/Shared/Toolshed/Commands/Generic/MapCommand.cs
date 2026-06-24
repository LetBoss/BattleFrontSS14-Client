// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Commands.Generic.MapCommand
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
public sealed class MapCommand : ToolshedCommand
{
  private static Type[] _parsers = new Type[1]
  {
    typeof (MapBlockOutputParser)
  };

  public override Type[] TypeParameterParsers => MapCommand._parsers;

  [CommandImplementation(null)]
  [TakesPipedTypeAsGeneric]
  public IEnumerable<TOut> Map<TOut, TIn>(
    IInvocationContext ctx,
    [PipedArgument] IEnumerable<TIn> value,
    Block<TIn, TOut> block)
  {
    foreach (TIn input in value)
    {
      TOut @out = block.Invoke(input, ctx);
      if ((object) @out != null)
        yield return @out;
      if (ctx.HasErrors)
        break;
    }
  }
}
