using System;
using Content.Client.Markers;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Commands;

internal sealed class ShowMarkersCommand : LocalizedCommands
{
	[Dependency]
	private IEntitySystemManager _entitySystemManager;

	public override string Command => "showmarkers";

	public override string Help => base.LocalizationManager.GetString("cmd-" + ((LocalizedCommands)this).Command + "-help", (ValueTuple<string, object>)("command", ((LocalizedCommands)this).Command));

	public override void Execute(IConsoleShell shell, string argStr, string[] args)
	{
		MarkerSystem entitySystem = _entitySystemManager.GetEntitySystem<MarkerSystem>();
		entitySystem.MarkersVisible = !entitySystem.MarkersVisible;
	}
}
