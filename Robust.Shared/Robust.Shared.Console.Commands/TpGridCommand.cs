using System;
using System.Globalization;
using System.Numerics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;

namespace Robust.Shared.Console.Commands;

internal sealed class TpGridCommand : LocalizedEntityCommands
{
	[Dependency]
	private readonly IEntityManager _ent;

	[Dependency]
	private readonly SharedMapSystem _map;

	public override string Command => "tpgrid";

	public override bool RequireServerOrSingleplayer => true;

	public override void Execute(IConsoleShell shell, string argStr, string[] args)
	{
		int num = args.Length;
		if ((num < 3 || num > 4) ? true : false)
		{
			shell.WriteError(base.Loc.GetString("cmd-invalid-arg-number-error"));
			return;
		}
		if (!NetEntity.TryParse(args[0].AsSpan(), out var entity))
		{
			shell.WriteError(base.Loc.GetString("cmd-parse-failure-uid", ("arg", args[0])));
			return;
		}
		if (!_ent.TryGetEntity(entity, out var entity2) || !_ent.HasComponent<MapGridComponent>(entity2) || _ent.HasComponent<MapComponent>(entity2))
		{
			shell.WriteError(base.Loc.GetString("cmd-parse-failure-grid", ("arg", args[0])));
			return;
		}
		float x = float.Parse(args[1], CultureInfo.InvariantCulture);
		float y = float.Parse(args[2], CultureInfo.InvariantCulture);
		MapId mapId = _ent.GetComponent<TransformComponent>(entity2.Value).MapID;
		if (args.Length > 3)
		{
			if (!int.TryParse(args[3], out var result))
			{
				shell.WriteError(base.Loc.GetString("cmd-parse-failure-mapid", ("arg", args[3])));
				return;
			}
			mapId = new MapId(result);
		}
		EntityUid map = _map.GetMap(mapId);
		if (map == EntityUid.Invalid)
		{
			shell.WriteError(base.Loc.GetString("cmd-parse-failure-mapid", ("arg", mapId.Value)));
			return;
		}
		EntityCoordinates value = new EntityCoordinates(map, new Vector2(x, y));
		_ent.System<SharedTransformSystem>().SetCoordinates(entity2.Value, value);
	}

	public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
	{
		return args.Length switch
		{
			1 => CompletionResult.FromHintOptions(CompletionHelper.Components<MapGridComponent>(args[^1], _ent), "<GridUid>"), 
			2 => CompletionResult.FromHint("<x>"), 
			3 => CompletionResult.FromHint("<y>"), 
			4 => CompletionResult.FromHintOptions(CompletionHelper.MapIds(_ent), "[MapId]"), 
			_ => CompletionResult.Empty, 
		};
	}
}
