// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Configuration.CVarSubsCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Console;
using Robust.Shared.IoC;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Robust.Shared.Configuration;

internal sealed class CVarSubsCommand : LocalizedCommands
{
  [Dependency]
  private readonly IConfigurationManager _cfg;

  public override string Command => "cvar_subs";

  public override void Execute(IConsoleShell shell, string argStr, string[] args)
  {
    if (args.Length < 1)
    {
      shell.WriteError(this.Loc.GetString("cmd-cvar_subs-invalid-args"));
    }
    else
    {
      foreach (Delegate sub in ((ConfigurationManager) this._cfg).GetSubs(args[0]))
        shell.WriteLine(CVarSubsCommand.ShowDelegateInfo(sub));
    }
  }

  private static string ShowDelegateInfo(Delegate del) => $"{del}: {del.Method} -> {del.Target}";

  public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
  {
    return args.Length == 1 ? CompletionResult.FromHintOptions((IEnumerable<CompletionOption>) this._cfg.GetRegisteredCVars().Select<string, CompletionOption>((Func<string, CompletionOption>) (c => new CompletionOption(c))).OrderBy<CompletionOption, string>((Func<CompletionOption, string>) (c => c.Value)), this.Loc.GetString("cmd-cvar_subs-arg-name")) : CompletionResult.Empty;
  }
}
