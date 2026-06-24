// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Commands.Misc.CmdCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.Toolshed.Commands.Misc;

[ToolshedCommand]
public sealed class CmdCommand : ToolshedCommand
{
  [CommandImplementation("list")]
  public IEnumerable<CommandSpec> List(IInvocationContext ctx)
  {
    return (IEnumerable<CommandSpec>) ctx.Environment.AllCommands();
  }

  [CommandImplementation("moo")]
  public string Moo() => "Have you mooed today?";

  [CommandImplementation("descloc")]
  public string GetLocStr([PipedArgument] CommandSpec cmd) => cmd.DescLocStr();

  [CommandImplementation("info")]
  public CommandSpec Info(CommandSpec cmd) => cmd;
}
