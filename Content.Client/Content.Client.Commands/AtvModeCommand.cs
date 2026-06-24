using System;
using Content.Client.Atmos.EntitySystems;
using Content.Shared.Atmos;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Commands;

internal sealed class AtvModeCommand : LocalizedCommands
{
	[Dependency]
	private IEntitySystemManager _entitySystemManager;

	public override string Command => "atvmode";

	public override string Help => base.LocalizationManager.GetString("cmd-" + ((LocalizedCommands)this).Command + "-help", (ValueTuple<string, object>)("command", ((LocalizedCommands)this).Command));

	public override void Execute(IConsoleShell shell, string argStr, string[] args)
	{
		if (args.Length < 1)
		{
			shell.WriteLine(((LocalizedCommands)this).Help);
			return;
		}
		if (!Enum.TryParse<AtmosDebugOverlayMode>(args[0], out var result))
		{
			shell.WriteError(base.LocalizationManager.GetString("cmd-" + ((LocalizedCommands)this).Command + "-error-invalid"));
			return;
		}
		int x = 0;
		float cfgBase = 0f;
		float cfgScale = 207.85599f;
		if (result == AtmosDebugOverlayMode.GasMoles)
		{
			if (args.Length != 2)
			{
				shell.WriteError(base.LocalizationManager.GetString("cmd-" + ((LocalizedCommands)this).Command + "-error-target-gas"));
				return;
			}
			if (!AtmosCommandUtils.TryParseGasID(args[1], out x))
			{
				shell.WriteError(base.LocalizationManager.GetString("cmd-" + ((LocalizedCommands)this).Command + "-error-out-of-range"));
				return;
			}
		}
		else
		{
			if (args.Length != 1)
			{
				shell.WriteLine(base.LocalizationManager.GetString("cmd-" + ((LocalizedCommands)this).Command + "-error-info"));
				return;
			}
			if (result == AtmosDebugOverlayMode.Temperature)
			{
				cfgBase = 373.15f;
				cfgScale = -160f;
			}
		}
		AtmosDebugOverlaySystem entitySystem = _entitySystemManager.GetEntitySystem<AtmosDebugOverlaySystem>();
		entitySystem.CfgMode = result;
		entitySystem.CfgSpecificGas = x;
		entitySystem.CfgBase = cfgBase;
		entitySystem.CfgScale = cfgScale;
	}
}
