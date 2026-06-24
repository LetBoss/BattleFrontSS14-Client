using System;
using Content.Client.Credits;
using Content.Shared.Administration;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Console;

namespace Content.Client.Commands;

[AnyCommand]
public sealed class CreditsCommand : LocalizedCommands
{
	public override string Command => "credits";

	public override string Help => base.LocalizationManager.GetString("cmd-" + ((LocalizedCommands)this).Command + "-help", (ValueTuple<string, object>)("command", ((LocalizedCommands)this).Command));

	public override void Execute(IConsoleShell shell, string argStr, string[] args)
	{
		((BaseWindow)new CreditsWindow()).Open();
	}
}
