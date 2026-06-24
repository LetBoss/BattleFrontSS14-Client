// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Configuration.ConfigUnmarkRollbackCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Console;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Robust.Shared.Configuration;

internal sealed class ConfigUnmarkRollbackCommand : IConsoleCommand
{
  [Dependency]
  private readonly IConfigurationManager _cfg;

  public string Command => "config_rollback_unmark";

  public string Description => "";

  public string Help => "";

  public void Execute(IConsoleShell shell, string argStr, string[] args)
  {
    int length = args.Length;
    if (length < 1 || length > 2)
      shell.WriteError(Loc.GetString("cmd-invalid-arg-number-error"));
    else
      this._cfg.UnmarkForRollback(args[0]);
  }

  public CompletionResult GetCompletion(IConsoleShell shell, string[] args)
  {
    return args.Length == 1 ? CompletionResult.FromOptions((IEnumerable<CompletionOption>) CVarCommandUtil.GetCVarCompletionOptions(this._cfg).OrderBy<CompletionOption, string>((Func<CompletionOption, string>) (c => c.Value))) : CompletionResult.Empty;
  }
}
