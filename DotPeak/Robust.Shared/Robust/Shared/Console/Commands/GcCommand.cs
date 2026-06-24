// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Console.Commands.GcCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Utility;
using System;

#nullable enable
namespace Robust.Shared.Console.Commands;

internal sealed class GcCommand : LocalizedCommands
{
  public override string Command => "gc";

  public override void Execute(IConsoleShell shell, string argStr, string[] args)
  {
    if (args.Length == 0)
    {
      GC.Collect();
    }
    else
    {
      int result;
      if (Parse.TryInt32(args[0].AsSpan(), out result))
        GC.Collect(result);
      else
        shell.WriteError(this.Loc.GetString("cmd-gc-failed-parse"));
    }
  }

  public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
  {
    return args.Length == 1 ? CompletionResult.FromHint(this.Loc.GetString("cmd-gc-arg-generation")) : CompletionResult.Empty;
  }
}
