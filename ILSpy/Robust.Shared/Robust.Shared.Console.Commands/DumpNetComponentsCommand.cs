using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Robust.Shared.Console.Commands;

internal sealed class DumpNetComponentsCommand : LocalizedCommands
{
	[Dependency]
	private readonly IComponentFactory _componentFactory;

	public override string Command => "dump_net_comps";

	public override void Execute(IConsoleShell shell, string argStr, string[] args)
	{
		IReadOnlyList<ComponentRegistration> networkedComponents = _componentFactory.NetworkedComponents;
		if (networkedComponents == null)
		{
			shell.WriteError(base.Loc.GetString("cmd-dump_net_comps-error-writeable"));
			return;
		}
		shell.WriteLine(base.Loc.GetString("cmd-dump_net_comps-header"));
		for (int i = 0; i < networkedComponents.Count; i++)
		{
			ComponentRegistration componentRegistration = networkedComponents[i];
			shell.WriteLine($"  [{i,4}] {componentRegistration.Name,-16} {componentRegistration.Type.Name}");
		}
	}
}
