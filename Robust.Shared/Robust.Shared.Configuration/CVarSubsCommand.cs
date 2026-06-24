using System;
using System.Linq;
using Robust.Shared.Console;
using Robust.Shared.IoC;

namespace Robust.Shared.Configuration;

internal sealed class CVarSubsCommand : LocalizedCommands
{
	[Dependency]
	private readonly IConfigurationManager _cfg;

	public override string Command => "cvar_subs";

	public override void Execute(IConsoleShell shell, string argStr, string[] args)
	{
		if (args.Length < 1)
		{
			shell.WriteError(base.Loc.GetString("cmd-cvar_subs-invalid-args"));
			return;
		}
		string name = args[0];
		foreach (Delegate sub in ((ConfigurationManager)_cfg).GetSubs(name))
		{
			shell.WriteLine(ShowDelegateInfo(sub));
		}
	}

	private static string ShowDelegateInfo(Delegate del)
	{
		return $"{del}: {del.Method} -> {del.Target}";
	}

	public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
	{
		if (args.Length == 1)
		{
			return CompletionResult.FromHintOptions(from c in _cfg.GetRegisteredCVars()
				select new CompletionOption(c) into c
				orderby c.Value
				select c, base.Loc.GetString("cmd-cvar_subs-arg-name"));
		}
		return CompletionResult.Empty;
	}
}
