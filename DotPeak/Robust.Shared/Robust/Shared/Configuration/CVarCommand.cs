// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Configuration.CVarCommand
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

internal sealed class CVarCommand : LocalizedCommands
{
  [Dependency]
  private readonly IConfigurationManager _cfg;

  public override string Command => "cvar";

  public override void Execute(IConsoleShell shell, string argStr, string[] args)
  {
    int length = args.Length;
    if (length < 1 || length > 2)
    {
      shell.WriteError(this.Loc.GetString("cmd-cvar-invalid-args"));
    }
    else
    {
      string name = args[0];
      if (name == "?")
      {
        IOrderedEnumerable<string> values = this._cfg.GetRegisteredCVars().OrderBy<string, string>((Func<string, string>) (c => c));
        shell.WriteLine(string.Join("\n", (IEnumerable<string>) values));
      }
      else if (!this._cfg.IsCVarRegistered(name))
        shell.WriteError(this.Loc.GetString("cmd-cvar-not-registered", ("cvar", (object) name)));
      else if (args.Length == 1)
      {
        object cvar = this._cfg.GetCVar<object>(name);
        shell.WriteLine(cvar.ToString());
      }
      else
      {
        string input = args[1];
        Type cvarType = this._cfg.GetCVarType(name);
        try
        {
          object obj = CVarCommandUtil.ParseObject(cvarType, input);
          this._cfg.SetCVar(name, obj);
        }
        catch (FormatException ex)
        {
          shell.WriteError(this.Loc.GetString("cmd-cvar-parse-error", ("type", (object) cvarType)));
        }
      }
    }
  }

  public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
  {
    if (args.Length == 1)
    {
      string Hint = this.Loc.GetString("cmd-cvar-compl-list");
      return CompletionResult.FromHintOptions((IEnumerable<CompletionOption>) CVarCommandUtil.GetCVarCompletionOptions(this._cfg).Union<CompletionOption>((IEnumerable<CompletionOption>) new CompletionOption[1]
      {
        new CompletionOption("?", Hint)
      }).OrderBy<CompletionOption, string>((Func<CompletionOption, string>) (c => c.Value)), this.Loc.GetString("cmd-cvar-arg-name"));
    }
    string name = args[0];
    return !this._cfg.IsCVarRegistered(name) ? CompletionResult.Empty : CompletionResult.FromHint($"<{this._cfg.GetCVarType(name).Name}>");
  }
}
