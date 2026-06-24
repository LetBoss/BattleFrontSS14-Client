using System;
using Content.Client.Atmos.EntitySystems;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Commands;

internal sealed class AtvRangeCommand : LocalizedCommands
{
	[Dependency]
	private IEntitySystemManager _entitySystemManager;

	public override string Command => "atvrange";

	public override string Help => base.LocalizationManager.GetString("cmd-" + ((LocalizedCommands)this).Command + "-help", (ValueTuple<string, object>)("command", ((LocalizedCommands)this).Command));

	public override void Execute(IConsoleShell shell, string argStr, string[] args)
	{
		if (args.Length != 2)
		{
			shell.WriteLine(((LocalizedCommands)this).Help);
			return;
		}
		if (!float.TryParse(args[0], out var result))
		{
			shell.WriteError(base.LocalizationManager.GetString("cmd-" + ((LocalizedCommands)this).Command + "-error-start"));
			return;
		}
		if (!float.TryParse(args[1], out var result2))
		{
			shell.WriteError(base.LocalizationManager.GetString("cmd-" + ((LocalizedCommands)this).Command + "-error-end"));
			return;
		}
		if (result == result2)
		{
			shell.WriteError(base.LocalizationManager.GetString("cmd-" + ((LocalizedCommands)this).Command + "-error-zero"));
			return;
		}
		AtmosDebugOverlaySystem entitySystem = _entitySystemManager.GetEntitySystem<AtmosDebugOverlaySystem>();
		entitySystem.CfgBase = result;
		entitySystem.CfgScale = result2 - result;
	}
}
