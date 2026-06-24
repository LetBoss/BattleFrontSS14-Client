using Robust.Shared.Analyzers;
using Robust.Shared.Player;
using Robust.Shared.Utility;

namespace Robust.Shared.Console;

[NotContentImplementable]
public interface IConsoleShell
{
	IConsoleHost ConsoleHost { get; }

	bool IsClient => !IsServer;

	bool IsLocal { get; }

	bool IsServer { get; }

	ICommonSession? Player { get; }

	void ExecuteCommand(string command);

	void RemoteExecuteCommand(string command);

	void WriteLine(string text);

	void WriteLine(FormattedMessage message);

	void WriteMarkup(string markup)
	{
		WriteLine(FormattedMessage.FromMarkupPermissive(markup));
	}

	void WriteError(string text);

	void Clear();
}
