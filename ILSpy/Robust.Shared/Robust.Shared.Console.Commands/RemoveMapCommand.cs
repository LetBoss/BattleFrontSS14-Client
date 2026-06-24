using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;

namespace Robust.Shared.Console.Commands;

internal sealed class RemoveMapCommand : LocalizedEntityCommands
{
	[Dependency]
	private readonly IEntitySystemManager _systems;

	public override string Command => "rmmap";

	public override bool RequireServerOrSingleplayer => true;

	public override void Execute(IConsoleShell shell, string argStr, string[] args)
	{
		if (args.Length != 1)
		{
			shell.WriteError("Wrong number of args.");
			return;
		}
		MapId mapId = new MapId(int.Parse(args[0]));
		SharedMapSystem entitySystem = _systems.GetEntitySystem<SharedMapSystem>();
		if (!entitySystem.MapExists(mapId))
		{
			shell.WriteError($"Map {mapId.Value} does not exist.");
		}
		else
		{
			entitySystem.DeleteMap(mapId);
			shell.WriteLine($"Map {mapId.Value} was removed.");
		}
	}

	public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
	{
		if (args.Length != 1)
		{
			return CompletionResult.Empty;
		}
		return CompletionResult.FromHintOptions(CompletionHelper.MapIds(args[0], EntityManager), LocalizationManager.GetString("generic-map"));
	}
}
