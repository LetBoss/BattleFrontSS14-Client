using System;
using System.Linq;
using Robust.Shared.IoC;
using Robust.Shared.Log;

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
			return;
		}
		string name = args[0];
		string text = args[1];
		LogLevel? level;
		if (text == "null")
		{
			level = null;
		}
		else
		{
			if (!Enum.TryParse<LogLevel>(text, out var result))
			{
				shell.WriteLine("Failed to parse 2nd argument. Must be one of the values of the LogLevel enum.");
				return;
			}
			level = result;
		}
		Logger.GetSawmill(name).Level = level;
	}

	public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
	{
		return args.Length switch
		{
			1 => CompletionResult.FromHintOptions(from c in _logManager.AllSawmills
				select c.Name into c
				orderby c
				select c, "<sawmill>"), 
			2 => CompletionResult.FromHintOptions(Enum.GetNames<LogLevel>().Union(new _003C_003Ez__ReadOnlySingleElementList<string>("null")), "<level>"), 
			_ => CompletionResult.Empty, 
		};
	}
}
