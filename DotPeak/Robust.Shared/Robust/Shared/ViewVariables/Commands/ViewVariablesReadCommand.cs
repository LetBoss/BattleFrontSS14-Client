// Decompiled with JetBrains decompiler
// Type: Robust.Shared.ViewVariables.Commands.ViewVariablesReadCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Console;

#nullable enable
namespace Robust.Shared.ViewVariables.Commands;

public sealed class ViewVariablesReadCommand : ViewVariablesBaseCommand
{
  public const string Comm = "vvread";

  public override string Command => "vvread";

  public override async void Execute(IConsoleShell shell, string argStr, string[] args)
  {
    ViewVariablesReadCommand variablesReadCommand = this;
    if (args.Length == 0)
    {
      shell.WriteError("Not enough arguments!");
    }
    else
    {
      string path = args[0];
      if (variablesReadCommand._netMan.IsClient)
      {
        if (!path.StartsWith("/c"))
        {
          IConsoleShell consoleShell = shell;
          consoleShell.WriteLine(await variablesReadCommand._vvm.ReadRemotePath(path) ?? "null");
          consoleShell = (IConsoleShell) null;
          return;
        }
        string str = path;
        path = str.Substring(2, str.Length - 2);
      }
      shell.WriteLine(variablesReadCommand._vvm.ReadPathSerialized(path) ?? "null");
    }
  }
}
