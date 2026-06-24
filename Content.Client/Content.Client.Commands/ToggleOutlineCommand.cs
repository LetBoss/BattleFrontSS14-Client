using System;
using Content.Shared.Administration;
using Content.Shared.CCVar;
using Robust.Shared.Configuration;
using Robust.Shared.Console;
using Robust.Shared.IoC;

namespace Content.Client.Commands;

[AnyCommand]
public sealed class ToggleOutlineCommand : LocalizedCommands
{
	[Dependency]
	private IConfigurationManager _configurationManager;

	public override string Command => "toggleoutline";

	public override string Help => base.LocalizationManager.GetString("cmd-" + ((LocalizedCommands)this).Command + "-help", (ValueTuple<string, object>)("command", ((LocalizedCommands)this).Command));

	public override void Execute(IConsoleShell shell, string argStr, string[] args)
	{
		CVarDef<bool> outlineEnabled = CCVars.OutlineEnabled;
		bool cVar = _configurationManager.GetCVar<bool>(outlineEnabled);
		_configurationManager.SetCVar<bool>(outlineEnabled, !cVar, false);
		shell.WriteLine(base.LocalizationManager.GetString("cmd-" + ((LocalizedCommands)this).Command + "-notify", (ValueTuple<string, object>)("state", _configurationManager.GetCVar<bool>(outlineEnabled))));
	}
}
