using Robust.Shared.Console;
using Robust.Shared.IoC;

namespace Content.Client.NetworkConfigurator.Systems;

public sealed class ClearAllNetworkLinkOverlays : LocalizedEntityCommands
{
	[Dependency]
	private NetworkConfiguratorSystem _network;

	public override string Command => "clearnetworklinkoverlays";

	public override void Execute(IConsoleShell shell, string argStr, string[] args)
	{
		_network.ClearAllOverlays();
	}
}
