using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;

namespace Robust.Shared.Console.Commands;

internal sealed class AddMapCommand : LocalizedEntityCommands
{
	[Dependency]
	private readonly SharedMapSystem _mapSystem;

	public override string Command => "addmap";

	public override bool RequireServerOrSingleplayer => true;

	public override void Execute(IConsoleShell shell, string argStr, string[] args)
	{
		if (args.Length >= 1)
		{
			MapId mapId = new MapId(int.Parse(args[0]));
			if (!_mapSystem.MapExists(mapId))
			{
				bool runMapInit = args.Length < 2 || !bool.Parse(args[1]);
				EntityManager.System<SharedMapSystem>().CreateMap(mapId, runMapInit);
				shell.WriteLine($"Map with ID {mapId} created.");
			}
			else
			{
				shell.WriteError($"Map with ID {mapId} already exists!");
			}
		}
	}

	public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
	{
		switch (args.Length)
		{
		case 1:
		{
			MapId nextMapId = _mapSystem.GetNextMapId();
			return CompletionResult.FromHintOptions(new _003C_003Ez__ReadOnlySingleElementList<CompletionOption>(new CompletionOption($"{nextMapId}")), LocalizationManager.GetString("generic-mapid"));
		}
		case 2:
			return CompletionResult.FromHint(LocalizationManager.GetString("cmd-addmap-hint-2"));
		default:
			return CompletionResult.Empty;
		}
	}
}
