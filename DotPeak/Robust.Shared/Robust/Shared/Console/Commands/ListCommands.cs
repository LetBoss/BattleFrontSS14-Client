// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Console.Commands.ListCommands
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#nullable enable
namespace Robust.Shared.Console.Commands;

internal sealed class ListCommands : LocalizedCommands
{
  public override string Command => "list";

  public override void Execute(IConsoleShell shell, string argStr, string[] args)
  {
    string filter = "";
    if (args.Length == 1)
      filter = args[0];
    IConsoleHostInternal consoleHost = (IConsoleHostInternal) shell.ConsoleHost;
    StringBuilder stringBuilder1 = new StringBuilder(this.Loc.GetString("cmd-list-heading"));
    foreach (IConsoleCommand cmd in (IEnumerable<IConsoleCommand>) consoleHost.AvailableCommands.Values.Where<IConsoleCommand>((Func<IConsoleCommand, bool>) (p => p.Command.Contains(filter))).OrderBy<IConsoleCommand, string>((Func<IConsoleCommand, string>) (c => c.Command)))
    {
      string str = consoleHost.IsCmdServer(cmd) ? "S" : "C";
      StringBuilder stringBuilder2 = stringBuilder1;
      StringBuilder stringBuilder3 = stringBuilder2;
      StringBuilder.AppendInterpolatedStringHandler interpolatedStringHandler = new StringBuilder.AppendInterpolatedStringHandler(1, 3, stringBuilder2);
      interpolatedStringHandler.AppendFormatted(str);
      interpolatedStringHandler.AppendLiteral(" ");
      interpolatedStringHandler.AppendFormatted<string>(cmd.Command, -32);
      interpolatedStringHandler.AppendFormatted(cmd.Description);
      ref StringBuilder.AppendInterpolatedStringHandler local = ref interpolatedStringHandler;
      stringBuilder3.AppendLine(ref local);
    }
    string text = stringBuilder1.ToString().Trim(' ', '\n');
    shell.WriteLine(text);
  }

  public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
  {
    return args.Length == 1 ? CompletionResult.FromHint(this.Loc.GetString("cmd-list-arg-filter")) : CompletionResult.Empty;
  }
}
