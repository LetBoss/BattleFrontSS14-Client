using Content.Shared.Administration;
using Content.Shared.CCVar;
using Robust.Shared.Configuration;
using Robust.Shared.Console;
using Robust.Shared.IoC;

namespace Content.Client._PUBG.Commands;

[AnyCommand]
public sealed class LanguageAutoCommand : IConsoleCommand
{
	public string Command => "languageauto";

	public string Description => "Reset language setting to auto.";

	public string Help => "Usage: languageauto";

	public void Execute(IConsoleShell shell, string argStr, string[] args)
	{
		IConfigurationManager obj = IoCManager.Resolve<IConfigurationManager>();
		obj.SetCVar<string>(CCVars.Language, "auto", false);
		obj.SaveToFile();
		shell.WriteLine("locale.language set to auto");
	}
}
