// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Console.IConsoleHost
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Player;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.Console;

[NotContentImplementable]
public interface IConsoleHost
{
  bool IsClient => !this.IsServer;

  bool IsServer { get; }

  IConsoleShell LocalShell { get; }

  IReadOnlyDictionary<string, IConsoleCommand> AvailableCommands { get; }

  event ConAnyCommandCallback AnyCommandExecuted;

  event EventHandler ClearText;

  void LoadConsoleCommands();

  void RegisterCommand(
    string command,
    string description,
    string help,
    ConCommandCallback callback,
    bool requireServerOrSingleplayer = false);

  void RegisterCommand(
    string command,
    string description,
    string help,
    ConCommandCallback callback,
    ConCommandCompletionCallback completionCallback,
    bool requireServerOrSingleplayer = false);

  void RegisterCommand(
    string command,
    string description,
    string help,
    ConCommandCallback callback,
    ConCommandCompletionAsyncCallback completionCallback,
    bool requireServerOrSingleplayer = false);

  void RegisterCommand(
    string command,
    ConCommandCallback callback,
    bool requireServerOrSingleplayer = false);

  void RegisterCommand(
    string command,
    ConCommandCallback callback,
    ConCommandCompletionCallback completionCallback,
    bool requireServerOrSingleplayer = false);

  void RegisterCommand(
    string command,
    ConCommandCallback callback,
    ConCommandCompletionAsyncCallback completionCallback,
    bool requireServerOrSingleplayer = false);

  void RegisterCommand(IConsoleCommand command);

  void BeginRegistrationRegion();

  void EndRegistrationRegion();

  void UnregisterCommand(string command);

  IConsoleShell GetSessionShell(ICommonSession session);

  void ExecuteCommand(string command);

  void AppendCommand(string command);

  void InsertCommand(string command);

  void CommandBufferExecute();

  void ExecuteCommand(ICommonSession? session, string command);

  void RemoteExecuteCommand(ICommonSession? session, string command);

  void WriteLine(ICommonSession? session, string text);

  void WriteLine(ICommonSession? session, FormattedMessage msg);

  void WriteError(ICommonSession? session, string text);

  void ClearLocalConsole();
}
