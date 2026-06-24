using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;

namespace Robust.Shared.Toolshed.Commands.Entities.World;

[ToolshedCommand]
internal sealed class MapPosCommand : ToolshedCommand
{
	private SharedTransformSystem? _xform;

	[CommandImplementation(null)]
	public EntityCoordinates MapPos([PipedArgument] EntityUid ent)
	{
		_xform = GetSys<SharedTransformSystem>();
		TransformComponent transformComponent = Transform(ent);
		Vector2 worldPosition = _xform.GetWorldPosition(transformComponent);
		return new EntityCoordinates(transformComponent.MapUid ?? EntityUid.Invalid, worldPosition);
	}

	[CommandImplementation(null)]
	public IEnumerable<EntityCoordinates> MapPos([PipedArgument] IEnumerable<EntityUid> input)
	{
		return input.Select(MapPos);
	}
}
