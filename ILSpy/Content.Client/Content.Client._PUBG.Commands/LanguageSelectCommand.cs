using Content.Client._PUBG.UserInterface.LanguageSelect;
using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.IoC;

namespace Content.Client._PUBG.Commands;

[AnyCommand]
public sealed class LanguageSelectCommand : IConsoleCommand
{
	public string Command => "languageselect";

	public string Description => "language";

	public string Help => "Usage: languageselect";

	public void Execute(IConsoleShell shell, string argStr, string[] args)
	{
		IoCManager.Resolve<LanguageSelectManager>().ShowLanguageSelect();
	}
}
