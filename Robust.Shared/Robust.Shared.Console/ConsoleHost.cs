using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Robust.Shared.IoC;
using Robust.Shared.IoC.Exceptions;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Reflection;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;

namespace Robust.Shared.Console;

public abstract class ConsoleHost : IConsoleHost
{
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

		public RegisteredCommand(string command, string description, string help, ConCommandCallback callback, bool requireServerOrSingleplayer = false)
		{
			Command = command;
			Description = description;
			Help = help;
			Callback = callback;
			RequireServerOrSingleplayer = requireServerOrSingleplayer;
		}

		public RegisteredCommand(string command, string description, string help, ConCommandCallback callback, ConCommandCompletionCallback completionCallback, bool requireServerOrSingleplayer = false)
			: this(command, description, help, callback, requireServerOrSingleplayer)
		{
			CompletionCallback = completionCallback;
		}

		public RegisteredCommand(string command, string description, string help, ConCommandCallback callback, ConCommandCompletionAsyncCallback completionCallback, bool requireServerOrSingleplayer = false)
			: this(command, description, help, callback, requireServerOrSingleplayer)
		{
			CompletionCallbackAsync = completionCallback;
		}

		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			Callback(shell, argStr, args);
		}

		public ValueTask<CompletionResult> GetCompletionAsync(IConsoleShell shell, string[] args, string argStr, CancellationToken cancel)
		{
			if (CompletionCallbackAsync != null)
			{
				return CompletionCallbackAsync(shell, args, argStr);
			}
			if (CompletionCallback != null)
			{
				return ValueTask.FromResult(CompletionCallback(shell, args));
			}
			return ValueTask.FromResult(CompletionResult.Empty);
		}

		public CompletionResult GetCompletion(IConsoleShell shell, string[] args)
		{
			return CompletionCallback?.Invoke(shell, args) ?? CompletionResult.Empty;
		}
	}

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

	[ViewVariables]
	protected readonly Dictionary<string, IConsoleCommand> RegisteredCommands = new Dictionary<string, IConsoleCommand>();

	[ViewVariables]
	private readonly HashSet<string> _autoRegisteredCommands = new HashSet<string>();

	private bool _isInRegistrationRegion;

	private readonly CommandBuffer _commandBuffer = new CommandBuffer();

	protected ISawmill Sawmill => LogManager.GetSawmill("con");

	public bool IsServer { get; }

	public IConsoleShell LocalShell { get; }

	public virtual IReadOnlyDictionary<string, IConsoleCommand> AvailableCommands => RegisteredCommands;

	public abstract event ConAnyCommandCallback? AnyCommandExecuted;

	public event EventHandler? ClearText;

	protected ConsoleHost(bool isServer)
	{
		IsServer = isServer;
		LocalShell = new ConsoleShell(this, null, isLocal: true);
	}

	public void LoadConsoleCommands()
	{
		foreach (Type allChild in ReflectionManager.GetAllChildren<IConsoleCommand>())
		{
			if (!allChild.IsAssignableTo(typeof(IEntityConsoleCommand)))
			{
				IConsoleCommand consoleCommand = (IConsoleCommand)_typeFactory.CreateInstanceUnchecked(allChild, oneOff: true);
				if (AvailableCommands.TryGetValue(consoleCommand.Command, out IConsoleCommand value))
				{
					throw new InvalidImplementationException(consoleCommand.GetType(), typeof(IConsoleCommand), $"Command name already registered: {consoleCommand.Command}, previous: {value.GetType()}");
				}
				RegisteredCommands[consoleCommand.Command] = consoleCommand;
				_autoRegisteredCommands.Add(consoleCommand.Command);
			}
		}
	}

	protected virtual void UpdateAvailableCommands()
	{
	}

	public void BeginRegistrationRegion()
	{
		if (_isInRegistrationRegion)
		{
			throw new InvalidOperationException("Cannot enter registration region twice!");
		}
		_isInRegistrationRegion = true;
	}

	public void EndRegistrationRegion()
	{
		if (!_isInRegistrationRegion)
		{
			throw new InvalidOperationException("Was not in registration region.");
		}
		_isInRegistrationRegion = false;
		UpdateAvailableCommands();
	}

	public void RegisterCommand(string command, string description, string help, ConCommandCallback callback, bool requireServerOrSingleplayer = false)
	{
		if (RegisteredCommands.ContainsKey(command))
		{
			throw new InvalidOperationException("Command already registered: " + command);
		}
		RegisteredCommand command2 = new RegisteredCommand(command, description, help, callback, requireServerOrSingleplayer);
		RegisterCommand(command2);
	}

	public void RegisterCommand(string command, string description, string help, ConCommandCallback callback, ConCommandCompletionCallback completionCallback, bool requireServerOrSingleplayer = false)
	{
		if (RegisteredCommands.ContainsKey(command))
		{
			throw new InvalidOperationException("Command already registered: " + command);
		}
		RegisteredCommand command2 = new RegisteredCommand(command, description, help, callback, completionCallback, requireServerOrSingleplayer);
		RegisterCommand(command2);
	}

	public void RegisterCommand(string command, string description, string help, ConCommandCallback callback, ConCommandCompletionAsyncCallback completionCallback, bool requireServerOrSingleplayer = false)
	{
		if (RegisteredCommands.ContainsKey(command))
		{
			throw new InvalidOperationException("Command already registered: " + command);
		}
		RegisteredCommand command2 = new RegisteredCommand(command, description, help, callback, completionCallback, requireServerOrSingleplayer);
		RegisterCommand(command2);
	}

	public void RegisterCommand(string command, ConCommandCallback callback, bool requireServerOrSingleplayer = false)
	{
		string value;
		string description = (LocalizationManager.TryGetString("cmd-" + command + "-desc", out value) ? value : "");
		string value2;
		string help = (LocalizationManager.TryGetString("cmd-" + command + "-help", out value2) ? value2 : "");
		RegisterCommand(command, description, help, callback, requireServerOrSingleplayer);
	}

	public void RegisterCommand(string command, ConCommandCallback callback, ConCommandCompletionCallback completionCallback, bool requireServerOrSingleplayer = false)
	{
		string value;
		string description = (LocalizationManager.TryGetString("cmd-" + command + "-desc", out value) ? value : "");
		string value2;
		string help = (LocalizationManager.TryGetString("cmd-" + command + "-help", out value2) ? value2 : "");
		RegisterCommand(command, description, help, callback, completionCallback, requireServerOrSingleplayer);
	}

	public void RegisterCommand(string command, ConCommandCallback callback, ConCommandCompletionAsyncCallback completionCallback, bool requireServerOrSingleplayer = false)
	{
		string value;
		string description = (LocalizationManager.TryGetString("cmd-" + command + "-desc", out value) ? value : "");
		string value2;
		string help = (LocalizationManager.TryGetString("cmd-" + command + "-help", out value2) ? value2 : "");
		RegisterCommand(command, description, help, callback, completionCallback, requireServerOrSingleplayer);
	}

	public void RegisterCommand(IConsoleCommand command)
	{
		RegisteredCommands.Add(command.Command, command);
		if (!_isInRegistrationRegion)
		{
			UpdateAvailableCommands();
		}
	}

	public void UnregisterCommand(string command)
	{
		if (!RegisteredCommands.TryGetValue(command, out IConsoleCommand _))
		{
			throw new KeyNotFoundException("Command " + command + " is not registered.");
		}
		if (_autoRegisteredCommands.Contains(command))
		{
			throw new InvalidOperationException("You cannot unregister commands that have been registered automatically.");
		}
		RegisteredCommands.Remove(command);
		if (!_isInRegistrationRegion)
		{
			UpdateAvailableCommands();
		}
	}

	public abstract void ExecuteCommand(ICommonSession? session, string command);

	public abstract void RemoteExecuteCommand(ICommonSession? session, string command);

	public abstract void WriteLine(ICommonSession? session, string text);

	public abstract void WriteLine(ICommonSession? session, FormattedMessage msg);

	public abstract void WriteError(ICommonSession? session, string text);

	public void ClearLocalConsole()
	{
		this.ClearText?.Invoke(this, EventArgs.Empty);
	}

	public IConsoleShell GetSessionShell(ICommonSession session)
	{
		if (!IsServer)
		{
			return LocalShell;
		}
		if ((int)session.Status >= 4)
		{
			throw new InvalidOperationException("Tried to get the session shell of a disconnected peer.");
		}
		return new ConsoleShell(this, session, isLocal: false);
	}

	public void ExecuteCommand(string command)
	{
		ExecuteCommand(null, command);
	}

	public void AppendCommand(string command)
	{
		_commandBuffer.Append(command);
	}

	public void InsertCommand(string command)
	{
		_commandBuffer.Insert(command);
	}

	public void CommandBufferExecute()
	{
		_commandBuffer.Tick(_timing.TickRate);
		string command;
		while (_commandBuffer.TryGetCommand(out command))
		{
			try
			{
				ExecuteCommand(command);
			}
			catch (Exception ex)
			{
				LocalShell.WriteError(ex.Message);
			}
		}
	}
}
