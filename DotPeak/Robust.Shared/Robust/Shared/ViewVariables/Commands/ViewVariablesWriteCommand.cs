// Decompiled with JetBrains decompiler
// Type: Robust.Shared.ViewVariables.Commands.ViewVariablesWriteCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Console;

#nullable enable
namespace Robust.Shared.ViewVariables.Commands;

public sealed class ViewVariablesWriteCommand : ViewVariablesBaseCommand
{
  public const string Comm = "vvwrite";

  public override string Command => "vvwrite";

  public override void Execute(IConsoleShell shell, string argStr, string[] args)
  {
    if (args.Length != 2)
    {
      shell.WriteError("Incorrect number of arguments!");
    }
    else
    {
      string path = args[0];
      string str1 = args[1];
      if (this._netMan.IsClient)
      {
        if (!path.StartsWith("/c"))
        {
          this._vvm.WriteRemotePath(path, str1);
          return;
        }
        string str2 = path;
        path = str2.Substring(2, str2.Length - 2);
      }
      this._vvm.WritePath(path, str1);
    }
  }
}
