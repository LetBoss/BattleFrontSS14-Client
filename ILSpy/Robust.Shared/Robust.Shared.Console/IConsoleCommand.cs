using System.Threading;
using System.Threading.Tasks;

namespace Robust.Shared.Console;

public interface IConsoleCommand
{
	string Command { get; }

	string Description { get; }

	string Help { get; }

	bool RequireServerOrSingleplayer => false;

	void Execute(IConsoleShell shell, string argStr, string[] args);

	CompletionResult GetCompletion(IConsoleShell shell, string[] args)
	{
		return CompletionResult.Empty;
	}

	ValueTask<CompletionResult> GetCompletionAsync(IConsoleShell shell, string[] args, string argStr, CancellationToken cancel)
	{
		return ValueTask.FromResult(GetCompletion(shell, args));
	}
}
