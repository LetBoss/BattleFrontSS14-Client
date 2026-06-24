using System.Linq;
using Robust.Shared.Console;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Robust.Shared.Configuration;

internal sealed class ConfigUnmarkRollbackCommand : IConsoleCommand
{
	[Dependency]
	private readonly IConfigurationManager _cfg;

	public string Command => "config_rollback_unmark";

	public string Description => "";

	public string Help => "";

	public void Execute(IConsoleShell shell, string argStr, string[] args)
	{
		int num = args.Length;
		if ((num < 1 || num > 2) ? true : false)
		{
			shell.WriteError(Loc.GetString("cmd-invalid-arg-number-error"));
			return;
		}
		_cfg.UnmarkForRollback(args[0]);
	}

	public CompletionResult GetCompletion(IConsoleShell shell, string[] args)
	{
		if (args.Length == 1)
		{
			return CompletionResult.FromOptions(from c in CVarCommandUtil.GetCVarCompletionOptions(_cfg)
				orderby c.Value
				select c);
		}
		return CompletionResult.Empty;
	}
}
