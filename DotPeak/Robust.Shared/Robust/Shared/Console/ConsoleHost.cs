// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Console.ConsoleHost
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Enums;
using Robust.Shared.IoC;
using Robust.Shared.IoC.Exceptions;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Reflection;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace Robust.Shared.Console;

public abstract class ConsoleHost : IConsoleHost
{
  protected const string SawmillName = "con";
  [Dependency]
  protected readonly ILogManager LogManager;
  [Dependency]
  private readonly IReflectionManager ReflectionManager;
  [Dependency]
  protected readonly INetManager NetManager;
  [Dependency]
  private readonly IDynamicTypeFactoryInternal _typeFactory;
  [Dependency]
  private readonly IGameTiming _timing;
  [Dependency]
  protected readonly ILocalizationManager LocalizationManager;
  [Robust.Shared.ViewVariables.ViewVariables]
  protected readonly Dictionary<string, IConsoleCommand> RegisteredCommands = new Dictionary<string, IConsoleCommand>();
  [Robust.Shared.ViewVariables.ViewVariables]
  private readonly HashSet<string> _autoRegisteredCommands = new HashSet<string>();
  private bool _isInRegistrationRegion;
  private readonly CommandBuffer _commandBuffer = new CommandBuffer();

  protected ISawmill Sawmill => this.LogManager.GetSawmill("con");

  public bool IsServer { get; }

  public IConsoleShell LocalShell { get; }

  public virtual IReadOnlyDictionary<string, IConsoleCommand> AvailableCommands
  {
    get => (IReadOnlyDictionary<string, IConsoleCommand>) this.RegisteredCommands;
  }

  public abstract event ConAnyCommandCallback? AnyCommandExecuted;

  protected ConsoleHost(bool isServer)
  {
    this.IsServer = isServer;
    this.LocalShell = (IConsoleShell) new ConsoleShell((IConsoleHost) this, (ICommonSession) null, true);
  }

  public event EventHandler? ClearText;

  public void LoadConsoleCommands()
  {
    foreach (Type allChild in this.ReflectionManager.GetAllChildren<IConsoleCommand>())
    {
      if (!allChild.IsAssignableTo(typeof (IEntityConsoleCommand)))
      {
        IConsoleCommand instanceUnchecked = (IConsoleCommand) this._typeFactory.CreateInstanceUnchecked(allChild, true);
        IConsoleCommand consoleCommand;
        if (this.AvailableCommands.TryGetValue(instanceUnchecked.Command, out consoleCommand))
          throw new InvalidImplementationException(instanceUnchecked.GetType(), typeof (IConsoleCommand), $"Command name already registered: {instanceUnchecked.Command}, previous: {consoleCommand.GetType()}");
        this.RegisteredCommands[instanceUnchecked.Command] = instanceUnchecked;
        this._autoRegisteredCommands.Add(instanceUnchecked.Command);
      }
    }
  }

  protected virtual void UpdateAvailableCommands()
  {
  }

  public void BeginRegistrationRegion()
  {
    this._isInRegistrationRegion = !this._isInRegistrationRegion ? true : throw new InvalidOperationException("Cannot enter registration region twice!");
  }

  public void EndRegistrationRegion()
  {
    this._isInRegistrationRegion = this._isInRegistrationRegion ? false : throw new InvalidOperationException("Was not in registration region.");
    this.UpdateAvailableCommands();
  }

  public void RegisterCommand(
    string command,
    string description,
    string help,
    ConCommandCallback callback,
    bool requireServerOrSingleplayer = false)
  {
    if (this.RegisteredCommands.ContainsKey(command))
      throw new InvalidOperationException("Command already registered: " + command);
    this.RegisterCommand((IConsoleCommand) new ConsoleHost.RegisteredCommand(command, description, help, callback, requireServerOrSingleplayer));
  }

  public void RegisterCommand(
    string command,
    string description,
    string help,
    ConCommandCallback callback,
    ConCommandCompletionCallback completionCallback,
    bool requireServerOrSingleplayer = false)
  {
    if (this.RegisteredCommands.ContainsKey(command))
      throw new InvalidOperationException("Command already registered: " + command);
    this.RegisterCommand((IConsoleCommand) new ConsoleHost.RegisteredCommand(command, description, help, callback, completionCallback, requireServerOrSingleplayer));
  }

  public void RegisterCommand(
    string command,
    string description,
    string help,
    ConCommandCallback callback,
    ConCommandCompletionAsyncCallback completionCallback,
    bool requireServerOrSingleplayer = false)
  {
    if (this.RegisteredCommands.ContainsKey(command))
      throw new InvalidOperationException("Command already registered: " + command);
    this.RegisterCommand((IConsoleCommand) new ConsoleHost.RegisteredCommand(command, description, help, callback, completionCallback, requireServerOrSingleplayer));
  }

  public void RegisterCommand(
    string command,
    ConCommandCallback callback,
    bool requireServerOrSingleplayer = false)
  {
    string str1;
    string description = this.LocalizationManager.TryGetString($"cmd-{command}-desc", out str1) ? str1 : "";
    string str2;
    string help = this.LocalizationManager.TryGetString($"cmd-{command}-help", out str2) ? str2 : "";
    this.RegisterCommand(command, description, help, callback, requireServerOrSingleplayer);
  }

  public void RegisterCommand(
    string command,
    ConCommandCallback callback,
    ConCommandCompletionCallback completionCallback,
    bool requireServerOrSingleplayer = false)
  {
    string str1;
    string description = this.LocalizationManager.TryGetString($"cmd-{command}-desc", out str1) ? str1 : "";
    string str2;
    string help = this.LocalizationManager.TryGetString($"cmd-{command}-help", out str2) ? str2 : "";
    this.RegisterCommand(command, description, help, callback, completionCallback, requireServerOrSingleplayer);
  }

  public void RegisterCommand(
    string command,
    ConCommandCallback callback,
    ConCommandCompletionAsyncCallback completionCallback,
    bool requireServerOrSingleplayer = false)
  {
    string str1;
    string description = this.LocalizationManager.TryGetString($"cmd-{command}-desc", out str1) ? str1 : "";
    string str2;
    string help = this.LocalizationManager.TryGetString($"cmd-{command}-help", out str2) ? str2 : "";
    this.RegisterCommand(command, description, help, callback, completionCallback, requireServerOrSingleplayer);
  }

  public void RegisterCommand(IConsoleCommand command)
  {
    this.RegisteredCommands.Add(command.Command, command);
    if (this._isInRegistrationRegion)
      return;
    this.UpdateAvailableCommands();
  }

  public void UnregisterCommand(string command)
  {
    if (!this.RegisteredCommands.TryGetValue(command, out IConsoleCommand _))
      throw new KeyNotFoundException($"Command {command} is not registered.");
    if (this._autoRegisteredCommands.Contains(command))
      throw new InvalidOperationException("You cannot unregister commands that have been registered automatically.");
    this.RegisteredCommands.Remove(command);
    if (this._isInRegistrationRegion)
      return;
    this.UpdateAvailableCommands();
  }

  public abstract void ExecuteCommand(ICommonSession? session, string command);

  public abstract void RemoteExecuteCommand(ICommonSession? session, string command);

  public abstract void WriteLine(ICommonSession? session, string text);

  public abstract void WriteLine(ICommonSession? session, FormattedMessage msg);

  public abstract void WriteError(ICommonSession? session, string text);

  public void ClearLocalConsole()
  {
    EventHandler clearText = this.ClearText;
    if (clearText == null)
      return;
    clearText((object) this, EventArgs.Empty);
  }

  public IConsoleShell GetSessionShell(ICommonSession session)
  {
    if (!this.IsServer)
      return this.LocalShell;
    return session.Status < SessionStatus.Disconnected ? (IConsoleShell) new ConsoleShell((IConsoleHost) this, session, false) : throw new InvalidOperationException("Tried to get the session shell of a disconnected peer.");
  }

  public void ExecuteCommand(string command) => this.ExecuteCommand((ICommonSession) null, command);

  public void AppendCommand(string command) => this._commandBuffer.Append(command);

  public void InsertCommand(string command) => this._commandBuffer.Insert(command);

  public void CommandBufferExecute()
  {
    this._commandBuffer.Tick(this._timing.TickRate);
    string command;
    while (this._commandBuffer.TryGetCommand(out command))
    {
      try
      {
        this.ExecuteCommand(command);
      }
      catch (Exception ex)
      {
        this.LocalShell.WriteError(ex.Message);
      }
    }
  }

  [Reflect(false)]
  public sealed class RegisteredCommand : IConsoleCommand
  {
    public ConCommandCallback Callback { get; }

    public ConCommandCompletionCallback? CompletionCallback { get; }

    public ConCommandCompletionAsyncCallback? CompletionCallbackAsync { get; }

    public string Command { get; }

    public string Description { get; }

    public string Help { get; }

    public bool RequireServerOrSingleplayer { get; init; }

    public RegisteredCommand(
      string command,
      string description,
      string help,
      ConCommandCallback callback,
      bool requireServerOrSingleplayer = false)
    {
      this.Command = command;
      this.Description = description;
      this.Help = help;
      this.Callback = callback;
      this.RequireServerOrSingleplayer = requireServerOrSingleplayer;
    }

    public RegisteredCommand(
      string command,
      string description,
      string help,
      ConCommandCallback callback,
      ConCommandCompletionCallback completionCallback,
      bool requireServerOrSingleplayer = false)
      : this(command, description, help, callback, requireServerOrSingleplayer)
    {
      this.CompletionCallback = completionCallback;
    }

    public RegisteredCommand(
      string command,
      string description,
      string help,
      ConCommandCallback callback,
      ConCommandCompletionAsyncCallback completionCallback,
      bool requireServerOrSingleplayer = false)
      : this(command, description, help, callback, requireServerOrSingleplayer)
    {
      this.CompletionCallbackAsync = completionCallback;
    }

    public void Execute(IConsoleShell shell, string argStr, string[] args)
    {
      this.Callback(shell, argStr, args);
    }

    public ValueTask<CompletionResult> GetCompletionAsync(
      IConsoleShell shell,
      string[] args,
      string argStr,
      CancellationToken cancel)
    {
      if (this.CompletionCallbackAsync != null)
        return this.CompletionCallbackAsync(shell, args, argStr);
      return this.CompletionCallback != null ? ValueTask.FromResult<CompletionResult>(this.CompletionCallback(shell, args)) : ValueTask.FromResult<CompletionResult>(CompletionResult.Empty);
    }

    public CompletionResult GetCompletion(IConsoleShell shell, string[] args)
    {
      ConCommandCompletionCallback completionCallback = this.CompletionCallback;
      CompletionResult completionResult = completionCallback != null ? completionCallback(shell, args) : (CompletionResult) null;
      return (object) completionResult != null ? completionResult : CompletionResult.Empty;
    }
  }
}
