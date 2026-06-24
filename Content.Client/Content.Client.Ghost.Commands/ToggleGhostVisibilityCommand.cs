using Robust.Shared.Console;
using Robust.Shared.IoC;

namespace Content.Client.Ghost.Commands;

public sealed class ToggleGhostVisibilityCommand : LocalizedEntityCommands
{
	[Dependency]
	private GhostSystem _ghost;

	public override string Command => "toggleghostvisibility";

	public override void Execute(IConsoleShell shell, string argStr, string[] args)
	{
		if (args.Length != 0 && bool.TryParse(args[0], out var result))
		{
			_ghost.ToggleGhostVisibility(result);
		}
		else
		{
			_ghost.ToggleGhostVisibility();
		}
	}
}
