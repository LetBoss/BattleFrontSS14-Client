using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Map.Components;

namespace Robust.Shared.Console.Commands;

internal sealed class RemoveGridCommand : LocalizedEntityCommands
{
	public override string Command => "rmgrid";

	public override bool RequireServerOrSingleplayer => true;

	public override void Execute(IConsoleShell shell, string argStr, string[] args)
	{
		if (args.Length != 1)
		{
			shell.WriteError("Wrong number of args.");
			return;
		}
		NetEntity nEntity = NetEntity.Parse(args[0].AsSpan());
		if (!EntityManager.TryGetEntity(nEntity, out var entity) || !EntityManager.HasComponent<MapGridComponent>(entity))
		{
			shell.WriteError($"Grid {entity} does not exist.");
		}
		else
		{
			EntityManager.DeleteEntity(entity);
			shell.WriteLine($"Grid {entity} was removed.");
		}
	}

	public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
	{
		if (args.Length != 1)
		{
			return CompletionResult.Empty;
		}
		return CompletionResult.FromHintOptions(CompletionHelper.Components<MapGridComponent>(args[0], EntityManager), LocalizationManager.GetString("generic-grid"));
	}
}
