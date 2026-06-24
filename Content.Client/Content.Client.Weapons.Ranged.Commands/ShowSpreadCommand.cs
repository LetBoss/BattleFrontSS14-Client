using System;
using Content.Client.Weapons.Ranged.Systems;
using Robust.Shared.Console;
using Robust.Shared.IoC;

namespace Content.Client.Weapons.Ranged.Commands;

public sealed class ShowSpreadCommand : LocalizedEntityCommands
{
	[Dependency]
	private GunSystem _gunSystem;

	public override string Command => "showgunspread";

	public override void Execute(IConsoleShell shell, string argStr, string[] args)
	{
		GunSystem gunSystem = _gunSystem;
		gunSystem.SpreadOverlay = !gunSystem.SpreadOverlay;
		shell.WriteLine(((LocalizedCommands)this).Loc.GetString("cmd-showgunspread-status", (ValueTuple<string, object>)("status", _gunSystem.SpreadOverlay)));
	}
}
