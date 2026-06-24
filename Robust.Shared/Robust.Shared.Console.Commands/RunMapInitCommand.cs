using System.Globalization;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;

namespace Robust.Shared.Console.Commands;

internal sealed class RunMapInitCommand : LocalizedEntityCommands
{
	[Dependency]
	private readonly SharedMapSystem _mapSystem;

	public override string Command => "mapinit";

	public override bool RequireServerOrSingleplayer => true;

	public override void Execute(IConsoleShell shell, string argStr, string[] args)
	{
		if (args.Length != 1)
		{
			shell.WriteError("Wrong number of args.");
			return;
		}
		string s = args[0];
		MapId mapId = new MapId(int.Parse(s, CultureInfo.InvariantCulture));
		if (!_mapSystem.MapExists(mapId))
		{
			shell.WriteError("Map does not exist!");
		}
		else if (_mapSystem.IsInitialized(mapId))
		{
			shell.WriteError("Map is already initialized!");
		}
		else
		{
			_mapSystem.InitializeMap(mapId);
		}
	}
}
