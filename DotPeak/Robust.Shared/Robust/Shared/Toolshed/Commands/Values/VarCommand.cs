// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Commands.Values.VarCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Toolshed.Syntax;
using Robust.Shared.Toolshed.TypeParsers;
using System;

#nullable enable
namespace Robust.Shared.Toolshed.Commands.Values;

[ToolshedCommand]
public sealed class VarCommand : ToolshedCommand
{
  private static Type[] _parsers = new Type[1]
  {
    typeof (VarTypeParser)
  };

  public override Type[] TypeParameterParsers => VarCommand._parsers;

  [CommandImplementation(null)]
  public T Var<T>(IInvocationContext ctx, VarRef<T> var) => var.Evaluate(ctx);
}
