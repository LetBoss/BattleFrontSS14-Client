// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Console.Commands.LogSetLevelCommand
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

internal sealed class LogSetLevelCommand : LocalizedCommands
{
  private const string LevelNull = "null";
  [Dependency]
  private readonly ILogManager _logManager;

  public override string Command => "loglevel";

  public override void Execute(IConsoleShell shell, string argStr, string[] args)
  {
    if (args.Length != 2)
    {
      shell.WriteError("Invalid argument amount. Expected 2 arguments.");
    }
    else
    {
      string name = args[0];
      string str = args[1];
      LogLevel? nullable;
      if (str == "null")
      {
        nullable = new LogLevel?();
      }
      else
      {
        LogLevel result;
        if (!Enum.TryParse<LogLevel>(str, out result))
        {
          shell.WriteLine("Failed to parse 2nd argument. Must be one of the values of the LogLevel enum.");
          return;
        }
        nullable = new LogLevel?(result);
      }
      Logger.GetSawmill(name).Level = nullable;
    }
  }

  public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
  {
    switch (args.Length)
    {
      case 1:
        return CompletionResult.FromHintOptions((IEnumerable<string>) this._logManager.AllSawmills.Select<ISawmill, string>((Func<ISawmill, string>) (c => c.Name)).OrderBy<string, string>((Func<string, string>) (c => c)), "<sawmill>");
      case 2:
        // ISSUE: object of a compiler-generated type is created
        return CompletionResult.FromHintOptions(((IEnumerable<string>) Enum.GetNames<LogLevel>()).Union<string>((IEnumerable<string>) new \u003C\u003Ez__ReadOnlySingleElementList<string>("null")), "<level>");
      default:
        return CompletionResult.Empty;
    }
  }
}
