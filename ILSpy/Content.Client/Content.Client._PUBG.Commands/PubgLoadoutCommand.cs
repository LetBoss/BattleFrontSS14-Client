using Content.Client._PUBG.UserInterface.Systems.Loadout;
using Content.Shared.Administration;
using Robust.Client.UserInterface;
using Robust.Shared.Console;
using Robust.Shared.IoC;

namespace Content.Client._PUBG.Commands;

[AnyCommand]
public sealed class PubgLoadoutCommand : IConsoleCommand
{
	public string Command => "pubg_loadout";

	public string Description => "Open PUBG loadout UI";

	public string Help => "Usage: pubg_loadout";

	public void Execute(IConsoleShell shell, string argStr, string[] args)
	{
		IoCManager.Resolve<IUserInterfaceManager>().GetUIController<PubgLoadoutUIController>().ToggleForTest();
	}
}
