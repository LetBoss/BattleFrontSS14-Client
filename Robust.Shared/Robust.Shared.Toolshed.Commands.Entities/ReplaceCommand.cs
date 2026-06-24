using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;

namespace Robust.Shared.Toolshed.Commands.Entities;

[ToolshedCommand]
internal sealed class ReplaceCommand : ToolshedCommand
{
	[CommandImplementation(null)]
	public IEnumerable<EntityUid> Replace([PipedArgument] IEnumerable<EntityUid> input, EntProtoId prototype)
	{
		foreach (EntityUid item in input)
		{
			TransformComponent transformComponent = Transform(item);
			EntityCoordinates coordinates = transformComponent.Coordinates;
			Angle localRotation = transformComponent.LocalRotation;
			QDel(item);
			EntityUid entityUid = Spawn(prototype, coordinates);
			Transform(entityUid).LocalRotation = localRotation;
			yield return entityUid;
		}
	}
}
