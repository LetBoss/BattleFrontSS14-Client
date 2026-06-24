// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Console.Commands.TestLog
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.IoC;
using Robust.Shared.Log;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Robust.Shared.Console.Commands;

internal sealed class TestLog : LocalizedCommands
{
  [Dependency]
  private readonly ILogManager _logManager;

  public override string Command => "testlog";

  public override void Execute(IConsoleShell shell, string argStr, string[] args)
  {
    if (args.Length != 3)
    {
      shell.WriteError("Invalid argument amount. Expected 3 arguments.");
    }
    else
    {
      string name = args[0];
      string str = args[1];
      string message = args[2];
      LogLevel logLevel;
      ref LogLevel local = ref logLevel;
      if (!Enum.TryParse<LogLevel>(str, out local))
      {
        shell.WriteLine("Failed to parse 2nd argument. Must be one of the values of the LogLevel enum.");
      }
      else
      {
        LogLevel level = logLevel;
        this._logManager.GetSawmill(name).Log(level, message);
      }
    }
  }

  public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
  {
    switch (args.Length)
    {
      case 1:
        return CompletionResult.FromHintOptions((IEnumerable<string>) this._logManager.AllSawmills.Select<ISawmill, string>((Func<ISawmill, string>) (c => c.Name)).OrderBy<string, string>((Func<string, string>) (c => c)), "<sawmill>");
      case 2:
        return CompletionResult.FromHintOptions((IEnumerable<string>) Enum.GetNames<LogLevel>(), "<level>");
      case 3:
        return CompletionResult.FromHint("<message>");
      default:
        return CompletionResult.Empty;
    }
  }
}
