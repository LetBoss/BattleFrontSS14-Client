using System;
using System.Globalization;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Placement;

namespace Robust.Shared.Console.Commands;

public sealed class SpawnCommand : LocalizedCommands
{
	[Dependency]
	private readonly IEntityManager _entityManager;

	public override string Command => "spawn";

	public override bool RequireServerOrSingleplayer => true;

	public override void Execute(IConsoleShell shell, string argStr, string[] args)
	{
		int num = args.Length;
		if ((num < 1 || num > 3) ? true : false)
		{
			shell.WriteError("Incorrect number of arguments. " + Help);
		}
		EntityUid entityUid = shell.Player?.AttachedEntity ?? EntityUid.Invalid;
		PlacementEntityEvent? placementEntityEvent = null;
		if (args.Length == 1 && entityUid != EntityUid.Invalid)
		{
			EntityCoordinates coordinates = _entityManager.GetComponent<TransformComponent>(entityUid).Coordinates;
			EntityUid editedEntity = _entityManager.SpawnEntity(args[0], coordinates);
			placementEntityEvent = new PlacementEntityEvent(editedEntity, coordinates, PlacementEventAction.Create, shell.Player?.UserId);
		}
		else if (args.Length == 2)
		{
			NetEntity nEntity = NetEntity.Parse(args[1].AsSpan());
			EntityCoordinates coordinates2 = _entityManager.GetComponent<TransformComponent>(_entityManager.GetEntity(nEntity)).Coordinates;
			EntityUid editedEntity2 = _entityManager.SpawnEntity(args[0], coordinates2);
			placementEntityEvent = new PlacementEntityEvent(editedEntity2, coordinates2, PlacementEventAction.Create, shell.Player?.UserId);
		}
		else if (entityUid != EntityUid.Invalid)
		{
			MapCoordinates coordinates3 = new MapCoordinates(float.Parse(args[1], CultureInfo.InvariantCulture), float.Parse(args[2], CultureInfo.InvariantCulture), _entityManager.GetComponent<TransformComponent>(entityUid).MapID);
			EntityUid entityUid2 = _entityManager.SpawnEntity(args[0], coordinates3);
			placementEntityEvent = new PlacementEntityEvent(entityUid2, _entityManager.GetComponent<TransformComponent>(entityUid2).Coordinates, PlacementEventAction.Create, shell.Player?.UserId);
		}
		if (placementEntityEvent.HasValue)
		{
			_entityManager.EventBus.RaiseEvent(EventSource.Local, placementEntityEvent.Value);
		}
	}
}
