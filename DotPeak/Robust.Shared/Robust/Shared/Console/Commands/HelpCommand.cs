// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Console.Commands.HelpCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Localization;
using Robust.Shared.Maths;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Robust.Shared.Console.Commands;

internal sealed class HelpCommand : LocalizedCommands
{
  private static readonly string Gold;
  private static readonly string Aqua;

  public override string Command => "help";

  public override void Execute(IConsoleShell shell, string argStr, string[] args)
  {
    HelpCommand.ExecuteStatic(shell, argStr, args, this.Loc);
  }

  public static void ExecuteStatic(
    IConsoleShell shell,
    string argStr,
    string[] args,
    ILocalizationManager loc)
  {
    switch (args.Length)
    {
      case 0:
        shell.WriteLine("\n  TOOLSHED\n /.\\\\\\\\\\\\\\\\\n/___\\\\\\\\\\\\\\\\\n|''''|'''''|\n| 8  | === |\n|_0__|_____|");
        shell.WriteMarkup($"\nFor a list of commands, run [color={HelpCommand.Gold}]cmd:list[/color].\nTo search for commands, run [color={HelpCommand.Gold}]cmd:list search \"[color={HelpCommand.Aqua}]query[/color]\"[/color].\nFor a breakdown of how a string of commands flows, run [color={HelpCommand.Gold}]explain [color={HelpCommand.Aqua}]commands here[/color][/color].\nFor help with old console commands, run [color={HelpCommand.Gold}]oldhelp[/color].\n");
        break;
      case 1:
        string key = args[0];
        IConsoleCommand consoleCommand;
        if (!shell.ConsoleHost.AvailableCommands.TryGetValue(key, out consoleCommand))
        {
          shell.WriteError(loc.GetString("cmd-help-unknown", ("command", (object) key)));
          break;
        }
        shell.WriteLine(loc.GetString("cmd-help-top", ("command", (object) consoleCommand.Command), ("description", (object) consoleCommand.Description)));
        shell.WriteLine(consoleCommand.Help);
        break;
      default:
        shell.WriteError(loc.GetString("cmd-help-invalid-args"));
        break;
    }
  }

  public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
  {
    return HelpCommand.GetCompletionStatic(shell, args, this.Loc);
  }

  public static CompletionResult GetCompletionStatic(
    IConsoleShell shell,
    string[] args,
    ILocalizationManager loc)
  {
    return args.Length == 1 ? CompletionResult.FromHintOptions((IEnumerable<CompletionOption>) shell.ConsoleHost.AvailableCommands.Values.OrderBy<IConsoleCommand, string>((Func<IConsoleCommand, string>) (c => c.Command)).Select<IConsoleCommand, CompletionOption>((Func<IConsoleCommand, CompletionOption>) (c => new CompletionOption(c.Command, c.Description))).ToArray<CompletionOption>(), loc.GetString("cmd-help-arg-cmdname")) : CompletionResult.Empty;
  }

  static HelpCommand()
  {
    Color gold = Color.Gold;
    HelpCommand.Gold = ((Color) ref gold).ToHex();
    Color aqua = Color.Aqua;
    HelpCommand.Aqua = ((Color) ref aqua).ToHex();
  }
}
