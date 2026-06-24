using System;
using Content.Shared.CCVar;
using Robust.Shared.Configuration;
using Robust.Shared.Console;
using Robust.Shared.IoC;

namespace Content.Client.Commands;

public sealed class GroupingEntityMenuCommand : LocalizedCommands
{
	[Dependency]
	private IConfigurationManager _configurationManager;

	public override string Command => "entitymenug";

	public override string Help => base.LocalizationManager.GetString("cmd-" + ((LocalizedCommands)this).Command + "-help", (ValueTuple<string, object>)("command", ((LocalizedCommands)this).Command), (ValueTuple<string, object>)("groupingTypesCount", 2));

	public override void Execute(IConsoleShell shell, string argStr, string[] args)
	{
		if (args.Length != 1)
		{
			shell.WriteLine(((LocalizedCommands)this).Help);
			return;
		}
		if (!int.TryParse(args[0], out var result))
		{
			shell.WriteError(base.LocalizationManager.GetString("cmd-" + ((LocalizedCommands)this).Command + "-error", (ValueTuple<string, object>)("arg", args[0])));
			return;
		}
		if (result < 0 || result > 1)
		{
			shell.WriteError(base.LocalizationManager.GetString("cmd-" + ((LocalizedCommands)this).Command + "-error", (ValueTuple<string, object>)("arg", args[0])));
			return;
		}
		CVarDef<int> entityMenuGroupingType = CCVars.EntityMenuGroupingType;
		_configurationManager.SetCVar<int>(entityMenuGroupingType, result, false);
		shell.WriteLine(base.LocalizationManager.GetString("cmd-" + ((LocalizedCommands)this).Command + "-notify", (ValueTuple<string, object>)("cvar", _configurationManager.GetCVar<int>(entityMenuGroupingType))));
	}
}
