using Content.Client.UserInterface.Systems.EscapeMenu;
using Content.Shared.Administration;
using Robust.Client.UserInterface;
using Robust.Shared.Console;
using Robust.Shared.IoC;

namespace Content.Client.Changelog;

[AnyCommand]
public sealed class ChangelogCommand : LocalizedCommands
{
	[Dependency]
	private IUserInterfaceManager _uiManager;

	public override string Command => "changelog";

	public override void Execute(IConsoleShell shell, string argStr, string[] args)
	{
		_uiManager.GetUIController<ChangelogUIController>().OpenWindow();
	}
}
