// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Commands.Misc.TypesCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.Toolshed.Commands.Misc;

[ToolshedCommand]
internal sealed class TypesCommand : ToolshedCommand
{
  [CommandImplementation("consumers")]
  public void Consumers(IInvocationContext ctx, [PipedArgument] object? input)
  {
    Type type = (object) (input as Type) != null ? (Type) input : input.GetType();
    ctx.WriteLine($"Valid intakers for {type.PrettyName()}:");
    foreach ((ToolshedCommand Cmd, string SubCommand) in ctx.Environment.CommandsTakingType(type))
    {
      if (SubCommand == null)
        ctx.WriteLine(Cmd.Name ?? "");
      else
        ctx.WriteLine($"{Cmd.Name}:{SubCommand}");
    }
  }

  [CommandImplementation("tree")]
  public IEnumerable<Type> Tree(IInvocationContext ctx, [PipedArgument] object? input)
  {
    return this.Toolshed.AllSteppedTypes((object) (input as Type) != null ? (Type) input : input.GetType());
  }

  [CommandImplementation("gettype")]
  public Type GetType([PipedArgument] object? input)
  {
    Type type = input?.GetType();
    return (object) type != null ? type : typeof (void);
  }

  [CommandImplementation("fullname")]
  public string FullName([PipedArgument] Type input) => input.FullName;
}
