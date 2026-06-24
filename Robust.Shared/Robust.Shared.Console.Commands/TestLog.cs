using System;
using System.Linq;
using Robust.Shared.IoC;
using Robust.Shared.Log;

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
			return;
		}
		string name = args[0];
		string value = args[1];
		string message = args[2];
		if (!Enum.TryParse<LogLevel>(value, out var result))
		{
			shell.WriteLine("Failed to parse 2nd argument. Must be one of the values of the LogLevel enum.");
			return;
		}
		LogLevel level = result;
		_logManager.GetSawmill(name).Log(level, message);
	}

	public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
	{
		return args.Length switch
		{
			1 => CompletionResult.FromHintOptions(from c in _logManager.AllSawmills
				select c.Name into c
				orderby c
				select c, "<sawmill>"), 
			2 => CompletionResult.FromHintOptions(Enum.GetNames<LogLevel>(), "<level>"), 
			3 => CompletionResult.FromHint("<message>"), 
			_ => CompletionResult.Empty, 
		};
	}
}
