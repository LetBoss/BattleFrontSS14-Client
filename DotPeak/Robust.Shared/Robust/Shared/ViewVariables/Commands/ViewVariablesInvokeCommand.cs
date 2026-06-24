// Decompiled with JetBrains decompiler
// Type: Robust.Shared.ViewVariables.Commands.ViewVariablesInvokeCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Console;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Robust.Shared.ViewVariables.Commands;

public sealed class ViewVariablesInvokeCommand : ViewVariablesBaseCommand
{
  public const string Comm = "vvinvoke";

  public override string Command => "vvinvoke";

  public override void Execute(IConsoleShell shell, string argStr, string[] args)
  {
    if (args.Length == 0)
    {
      shell.WriteError("Not enough arguments!");
    }
    else
    {
      string path = args[0];
      string arguments = string.Join(string.Empty, RuntimeHelpers.GetSubArray<string>(args, Range.StartAt((Index) 1)));
      if (this._netMan.IsClient)
      {
        if (!path.StartsWith("/c"))
        {
          this._vvm.InvokeRemotePath(path, arguments);
          return;
        }
        string str = path;
        path = str.Substring(2, str.Length - 2);
      }
      object obj = this._vvm.InvokePath(path, arguments);
      shell.WriteLine(obj?.ToString() ?? "null");
    }
  }
}
