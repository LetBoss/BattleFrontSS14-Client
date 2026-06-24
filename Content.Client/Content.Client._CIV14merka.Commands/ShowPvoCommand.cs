using Content.Client._CIV14merka.Pvo;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client._CIV14merka.Commands;

public sealed class ShowPvoCommand : IConsoleCommand
{
	[Dependency]
	private IEntityManager _entities;

	public string Command => "showpvo";

	public string Description => "тест. оверлей радиуса пво";

	public string Help => "Usage: " + Command;

	public void Execute(IConsoleShell shell, string argStr, string[] args)
	{
		if (args.Length != 0)
		{
			shell.WriteLine(Help);
			return;
		}
		bool value = _entities.System<CivPvoOverlaySystem>().Toggle();
		shell.WriteLine($"showpvo: {value}");
	}
}
