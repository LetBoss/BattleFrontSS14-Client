using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client._RMC14.Areas;

public sealed class ShowAreasCommand : IConsoleCommand
{
	[Dependency]
	private IEntityManager _entities;

	public string Command => "showareas";

	public string Description => "Shows areas depending on their properties.";

	public string Help => $"Usage: {Command} disable | {Command} cas";

	public void Execute(IConsoleShell shell, string argStr, string[] args)
	{
		if (args.Length == 0 || args.Length > 1)
		{
			shell.WriteLine(Help);
			return;
		}
		AreasCommandSystem areasCommandSystem = _entities.System<AreasCommandSystem>();
		string text = args[0].ToLowerInvariant();
		if (!(text == "cas"))
		{
			if (text == "disable")
			{
				areasCommandSystem.Enabled = false;
				shell.WriteLine("Disabled area visualizer");
			}
			else
			{
				shell.WriteLine(Help);
			}
		}
		else
		{
			areasCommandSystem.ShowCAS = !areasCommandSystem.ShowCAS;
			shell.WriteLine($"Showing areas with {"ShowCAS"}: {areasCommandSystem.ShowCAS}");
			areasCommandSystem.Enabled = true;
		}
	}
}
