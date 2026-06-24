using System;
using Content.Client.Mapping;
using Content.Client.Markers;
using Robust.Client.Graphics;
using Robust.Client.State;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Commands;

internal sealed class MappingClientSideSetupCommand : LocalizedCommands
{
	[Dependency]
	private IEntitySystemManager _entitySystemManager;

	[Dependency]
	private ILightManager _lightManager;

	public override string Command => "mappingclientsidesetup";

	public override string Help => base.LocalizationManager.GetString("cmd-" + ((LocalizedCommands)this).Command + "-help", (ValueTuple<string, object>)("command", ((LocalizedCommands)this).Command));

	public override void Execute(IConsoleShell shell, string argStr, string[] args)
	{
		if (!_lightManager.LockConsoleAccess)
		{
			_entitySystemManager.GetEntitySystem<MarkerSystem>().MarkersVisible = true;
			_lightManager.Enabled = false;
			shell.ExecuteCommand("showsubfloor");
			IoCManager.Resolve<IStateManager>().RequestStateChange<MappingState>();
		}
	}
}
