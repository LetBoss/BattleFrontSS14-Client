// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Commands.Generic.Variables.ArrowCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Toolshed.Syntax;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Robust.Shared.Toolshed.Commands.Generic.Variables;

[ToolshedCommand(Name = "=>")]
public sealed class ArrowCommand : ToolshedCommand
{
  [CommandImplementation(null)]
  [TakesPipedTypeAsGeneric]
  public T Arrow<T>(IInvocationContext ctx, [PipedArgument] T input, WriteableVarRef<T> var)
  {
    ctx.WriteVar(var.Inner.VarName, (object) input);
    return input;
  }

  [CommandImplementation(null)]
  [TakesPipedTypeAsGeneric]
  public List<T> Arrow<T>(
    IInvocationContext ctx,
    [PipedArgument] IEnumerable<T> input,
    WriteableVarRef<List<T>> var)
  {
    List<T> list = input.ToList<T>();
    ctx.WriteVar(var.Inner.VarName, (object) list);
    return list;
  }
}
