using System.Threading.Tasks;

namespace Robust.Shared.Console;

public delegate ValueTask<CompletionResult> ConCommandCompletionAsyncCallback(IConsoleShell shell, string[] args, string argStr);
