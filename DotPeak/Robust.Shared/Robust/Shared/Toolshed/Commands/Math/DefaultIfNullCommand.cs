// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Commands.Math.DefaultIfNullCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Toolshed.Syntax;
using Robust.Shared.Toolshed.TypeParsers;
using System;

#nullable enable
namespace Robust.Shared.Toolshed.Commands.Math;

[ToolshedCommand(Name = "?")]
public sealed class DefaultIfNullCommand : ToolshedCommand
{
  private static Type[] _parsers = new Type[1]
  {
    typeof (MapBlockOutputParser)
  };

  public override Type[] TypeParameterParsers => DefaultIfNullCommand._parsers;

  [CommandImplementation(null)]
  [TakesPipedTypeAsGeneric]
  public TOut? DefaultIfNull<TOut, TIn>(
    IInvocationContext ctx,
    [PipedArgument] TIn? value,
    Block<TIn, TOut> follower)
    where TIn : unmanaged
  {
    return !value.HasValue ? default (TOut) : follower.Invoke(value.Value, ctx);
  }
}
