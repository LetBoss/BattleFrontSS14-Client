using System;
using Content.Client.SubFloor;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Commands;

internal sealed class ShowSubFloor : LocalizedCommands
{
	[Dependency]
	private IEntitySystemManager _entitySystemManager;

	public override string Command => "showsubfloor";

	public override string Help => base.LocalizationManager.GetString("cmd-" + ((LocalizedCommands)this).Command + "-help", (ValueTuple<string, object>)("command", ((LocalizedCommands)this).Command));

	public override void Execute(IConsoleShell shell, string argStr, string[] args)
	{
		SubFloorHideSystem entitySystem = _entitySystemManager.GetEntitySystem<SubFloorHideSystem>();
		entitySystem.ShowAll = !entitySystem.ShowAll;
	}
}
