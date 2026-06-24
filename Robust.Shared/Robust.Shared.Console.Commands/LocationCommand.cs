using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Player;

namespace Robust.Shared.Console.Commands;

internal sealed class LocationCommand : LocalizedEntityCommands
{
	[Dependency]
	private readonly IEntityManager _ent;

	[Dependency]
	private readonly SharedTransformSystem _transform;

	public override string Command => "loc";

	public override void Execute(IConsoleShell shell, string argStr, string[] args)
	{
		ICommonSession player = shell.Player;
		if (player != null)
		{
			EntityUid? attachedEntity = player.AttachedEntity;
			if (attachedEntity.HasValue)
			{
				EntityUid valueOrDefault = attachedEntity.GetValueOrDefault();
				EntityCoordinates coordinates = _ent.GetComponent<TransformComponent>(valueOrDefault).Coordinates;
				MapId mapId = _transform.GetMapId(coordinates);
				EntityUid? grid = _transform.GetGrid(coordinates);
				shell.WriteLine($"MapID:{mapId} GridUid:{grid} X:{coordinates.X:N2} Y:{coordinates.Y:N2}");
			}
		}
	}
}
