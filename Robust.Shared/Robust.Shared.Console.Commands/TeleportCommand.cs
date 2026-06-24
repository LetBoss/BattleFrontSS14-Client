using System.Numerics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Player;

namespace Robust.Shared.Console.Commands;

internal sealed class TeleportCommand : LocalizedEntityCommands
{
	[Dependency]
	private readonly IMapManager _map;

	[Dependency]
	private readonly IEntityManager _entityManager;

	[Dependency]
	private readonly SharedTransformSystem _transform;

	[Dependency]
	private readonly SharedMapSystem _mapSystem;

	public override string Command => "tp";

	public override bool RequireServerOrSingleplayer => true;

	public override void Execute(IConsoleShell shell, string argStr, string[] args)
	{
		ICommonSession player = shell.Player;
		if (player == null)
		{
			return;
		}
		EntityUid? attachedEntity = player.AttachedEntity;
		if (!attachedEntity.HasValue)
		{
			return;
		}
		EntityUid valueOrDefault = attachedEntity.GetValueOrDefault();
		if (args.Length < 2 || !float.TryParse(args[0], out var result) || !float.TryParse(args[1], out var result2))
		{
			shell.WriteError(Help);
			return;
		}
		TransformComponent component = _entityManager.GetComponent<TransformComponent>(valueOrDefault);
		Vector2 vector = new Vector2(result, result2);
		_transform.AttachToGridOrMap(valueOrDefault, component);
		int result3;
		MapId mapId = ((args.Length != 3 || !int.TryParse(args[2], out result3)) ? component.MapID : new MapId(result3));
		if (!_mapSystem.MapExists(mapId))
		{
			shell.WriteError($"Map {mapId} doesn't exist!");
			return;
		}
		EntityUid? uid2;
		if (_map.TryFindGridAt(mapId, vector, out EntityUid uid, out MapGridComponent _))
		{
			Vector2 position = Vector2.Transform(vector, _transform.GetInvWorldMatrix(uid));
			_transform.SetCoordinates(valueOrDefault, component, new EntityCoordinates(uid, position));
		}
		else if (_mapSystem.TryGetMap(mapId, out uid2))
		{
			_transform.SetWorldPosition((Owner: valueOrDefault, Comp: component), vector);
			_transform.SetParent(valueOrDefault, component, uid2.Value);
		}
		shell.WriteLine($"Teleported {shell.Player} to {mapId}:{result},{result2}.");
	}

	public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
	{
		return args.Length switch
		{
			1 => CompletionResult.FromHint("<x>"), 
			2 => CompletionResult.FromHint("<y>"), 
			3 => CompletionResult.FromHintOptions(CompletionHelper.MapIds(_entityManager), "[MapId]"), 
			_ => CompletionResult.Empty, 
		};
	}
}
