using System;
using Content.Client.Atmos.EntitySystems;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Commands;

internal sealed class AtvCBMCommand : LocalizedCommands
{
	[Dependency]
	private IEntitySystemManager _entitySystemManager;

	public override string Command => "atvcbm";

	public override string Help => base.LocalizationManager.GetString("cmd-" + ((LocalizedCommands)this).Command + "-help", (ValueTuple<string, object>)("command", ((LocalizedCommands)this).Command));

	public override void Execute(IConsoleShell shell, string argStr, string[] args)
	{
		bool result;
		if (args.Length != 1)
		{
			shell.WriteLine(((LocalizedCommands)this).Help);
		}
		else if (!bool.TryParse(args[0], out result))
		{
			shell.WriteError(base.LocalizationManager.GetString("cmd-" + ((LocalizedCommands)this).Command + "-error"));
		}
		else
		{
			_entitySystemManager.GetEntitySystem<AtmosDebugOverlaySystem>().CfgCBM = result;
		}
	}
}
