using Robust.Shared.Player;
using Robust.Shared.Utility;

namespace Robust.Shared.Console;

public sealed class ConsoleShell : IConsoleShell
{
	public IConsoleHost ConsoleHost { get; }

	public bool IsServer => ConsoleHost.IsServer;

	public ICommonSession? Player { get; }

	public bool IsLocal { get; }

	public ConsoleShell(IConsoleHost host, ICommonSession? session, bool isLocal)
	{
		ConsoleHost = host;
		Player = session;
		IsLocal = isLocal;
	}

	public void ExecuteCommand(string command)
	{
		ConsoleHost.ExecuteCommand(Player, command);
	}

	public void RemoteExecuteCommand(string command)
	{
		ConsoleHost.RemoteExecuteCommand(Player, command);
	}

	public void WriteLine(string text)
	{
		ConsoleHost.WriteLine(Player, text);
	}

	public void WriteLine(FormattedMessage message)
	{
		ConsoleHost.WriteLine(Player, message);
	}

	public void WriteError(string text)
	{
		ConsoleHost.WriteError(Player, text);
	}

	public void Clear()
	{
		if (Player == null)
		{
			ConsoleHost.ClearLocalConsole();
		}
	}
}
