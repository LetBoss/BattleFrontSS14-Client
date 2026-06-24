using System;
using Robust.Shared.Utility;

namespace Robust.Shared.Console.Commands;

internal sealed class GcCommand : LocalizedCommands
{
	public override string Command => "gc";

	public override void Execute(IConsoleShell shell, string argStr, string[] args)
	{
		int result;
		if (args.Length == 0)
		{
			GC.Collect();
		}
		else if (Parse.TryInt32(args[0].AsSpan(), out result))
		{
			GC.Collect(result);
		}
		else
		{
			shell.WriteError(base.Loc.GetString("cmd-gc-failed-parse"));
		}
	}

	public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
	{
		if (args.Length == 1)
		{
			return CompletionResult.FromHint(base.Loc.GetString("cmd-gc-arg-generation"));
		}
		return CompletionResult.Empty;
	}
}
