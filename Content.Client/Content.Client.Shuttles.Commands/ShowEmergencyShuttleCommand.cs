using System;
using Content.Client.Shuttles.Systems;
using Robust.Shared.Console;
using Robust.Shared.IoC;

namespace Content.Client.Shuttles.Commands;

public sealed class ShowEmergencyShuttleCommand : LocalizedEntityCommands
{
	[Dependency]
	private ShuttleSystem _shuttle;

	public override string Command => "showemergencyshuttle";

	public override void Execute(IConsoleShell shell, string argStr, string[] args)
	{
		ShuttleSystem shuttle = _shuttle;
		shuttle.EnableShuttlePosition = !shuttle.EnableShuttlePosition;
		shell.WriteLine(((LocalizedCommands)this).Loc.GetString("cmd-showemergencyshuttle-status", (ValueTuple<string, object>)("status", _shuttle.EnableShuttlePosition)));
	}
}
