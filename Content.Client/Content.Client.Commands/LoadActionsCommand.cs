using System;
using Content.Client.Actions;
using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Commands;

[AnyCommand]
public sealed class LoadActionsCommand : LocalizedCommands
{
	[Dependency]
	private IEntitySystemManager _entitySystemManager;

	public override string Command => "loadacts";

	public override string Help => base.LocalizationManager.GetString("cmd-" + ((LocalizedCommands)this).Command + "-help", (ValueTuple<string, object>)("command", ((LocalizedCommands)this).Command));

	public override void Execute(IConsoleShell shell, string argStr, string[] args)
	{
		if (args.Length != 1)
		{
			shell.WriteLine(((LocalizedCommands)this).Help);
			return;
		}
		try
		{
			_entitySystemManager.GetEntitySystem<ActionsSystem>().LoadActionAssignments(args[0], userData: true);
		}
		catch
		{
			shell.WriteError(base.LocalizationManager.GetString("cmd-" + ((LocalizedCommands)this).Command + "-error"));
		}
	}
}
