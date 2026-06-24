using Content.Client.NPC.HTN;
using Robust.Shared.Console;
using Robust.Shared.IoC;

namespace Content.Client.NPC;

public sealed class ShowHtnCommand : LocalizedEntityCommands
{
	[Dependency]
	private HTNSystem _htnSystem;

	public override string Command => "showhtn";

	public override void Execute(IConsoleShell shell, string argStr, string[] args)
	{
		HTNSystem htnSystem = _htnSystem;
		htnSystem.EnableOverlay = !htnSystem.EnableOverlay;
	}
}
