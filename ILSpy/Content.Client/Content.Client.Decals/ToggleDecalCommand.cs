using Robust.Shared.Console;
using Robust.Shared.IoC;

namespace Content.Client.Decals;

public sealed class ToggleDecalCommand : LocalizedEntityCommands
{
	[Dependency]
	private DecalSystem _decal;

	public override string Command => "toggledecals";

	public override void Execute(IConsoleShell shell, string argStr, string[] args)
	{
		_decal.ToggleOverlay();
	}
}
