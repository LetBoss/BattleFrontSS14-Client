using System.IO;
using System.Text.RegularExpressions;
using Robust.Shared.ContentPack;
using Robust.Shared.IoC;
using Robust.Shared.Utility;

namespace Robust.Shared.Console.Commands;

internal sealed class ExecCommand : LocalizedCommands
{
	private static readonly Regex CommentRegex = new Regex("^\\s*#");

	[Dependency]
	private readonly IResourceManager _resources;

	public override string Command => "exec";

	public override void Execute(IConsoleShell shell, string argStr, string[] args)
	{
		if (args.Length < 1)
		{
			shell.WriteError("No file specified!");
			return;
		}
		ResPath path = new ResPath(args[0]).ToRootedPath();
		if (!_resources.UserData.Exists(path))
		{
			shell.WriteError("File does not exist.");
			return;
		}
		using StreamReader streamReader = _resources.UserData.OpenText(path);
		while (true)
		{
			string text = streamReader.ReadLine();
			if (text == null)
			{
				break;
			}
			if (!string.IsNullOrWhiteSpace(text) && !CommentRegex.IsMatch(text))
			{
				shell.ConsoleHost.AppendCommand(text);
			}
		}
	}

	public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
	{
		if (args.Length == 1)
		{
			string hint = base.Loc.GetString("cmd-exec-arg-filename");
			return CompletionResult.FromHintOptions(CompletionHelper.UserFilePath(args[0], _resources.UserData), hint);
		}
		return CompletionResult.Empty;
	}
}
