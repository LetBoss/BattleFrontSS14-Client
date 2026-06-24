using Robust.Shared.ContentPack;
using Robust.Shared.IoC;
using Robust.Shared.Utility;

namespace Robust.Shared.Console.Commands;

public sealed class VfsListCommand : LocalizedCommands
{
	[Dependency]
	private readonly IResourceManager _resourceManager;

	public override string Command => "vfs_ls";

	public override void Execute(IConsoleShell shell, string argStr, string[] args)
	{
		if (args.Length > 1)
		{
			shell.WriteError(LocalizationManager.GetString("cmd-vfs_ls-err-args"));
			return;
		}
		foreach (string item in _resourceManager.ContentGetDirectoryEntries(new ResPath(args[0])))
		{
			shell.WriteLine(item);
		}
	}

	public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
	{
		if (args.Length == 1)
		{
			return CompletionResult.FromHintOptions(CompletionHelper.ContentDirPath(args[0], _resourceManager), LocalizationManager.GetString("cmd-vfs_ls-hint-path"));
		}
		return CompletionResult.Empty;
	}
}
