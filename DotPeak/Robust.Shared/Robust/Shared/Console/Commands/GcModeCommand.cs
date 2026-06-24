// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Console.Commands.GcModeCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Collections.Generic;
using System.Runtime;

#nullable enable
namespace Robust.Shared.Console.Commands;

internal sealed class GcModeCommand : LocalizedCommands
{
  public override string Command => "gc_mode";

  public override void Execute(IConsoleShell shell, string argStr, string[] args)
  {
    GCLatencyMode latencyMode = GCSettings.LatencyMode;
    if (args.Length == 0)
    {
      shell.WriteLine(this.Loc.GetString("cmd-gc_mode-current", ("prevMode", (object) latencyMode.ToString())));
      shell.WriteLine(this.Loc.GetString("cmd-gc_mode-possible"));
      foreach (GCLatencyMode gcLatencyMode in Enum.GetValues<GCLatencyMode>())
        shell.WriteLine(this.Loc.GetString("cmd-gc_mode-option", ("mode", (object) gcLatencyMode.ToString())));
    }
    else
    {
      GCLatencyMode result;
      if (!Enum.TryParse<GCLatencyMode>(args[0], true, out result))
      {
        shell.WriteLine(this.Loc.GetString("cmd-gc_mode-unknown", ("arg", (object) args[0])));
      }
      else
      {
        shell.WriteLine(this.Loc.GetString("cmd-gc_mode-attempt", ("prevMode", (object) latencyMode.ToString()), ("mode", (object) result.ToString())));
        GCSettings.LatencyMode = result;
        shell.WriteLine(this.Loc.GetString("cmd-gc_mode-result", ("mode", (object) GCSettings.LatencyMode.ToString())));
      }
    }
  }

  public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
  {
    return args.Length == 1 ? CompletionResult.FromHintOptions((IEnumerable<string>) Enum.GetNames<GCLatencyMode>(), this.Loc.GetString("cmd-gc_mode-arg-type")) : CompletionResult.Empty;
  }
}
