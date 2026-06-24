using System.Collections.Generic;
using System.Linq;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;

namespace Robust.Shared.Toolshed.Commands.Entities.World;

[ToolshedCommand]
internal sealed class PosCommand : ToolshedCommand
{
	[CommandImplementation(null)]
	public EntityCoordinates Pos([PipedArgument] EntityUid ent)
	{
		return Transform(ent).Coordinates;
	}

	[CommandImplementation(null)]
	public IEnumerable<EntityCoordinates> Pos([PipedArgument] IEnumerable<EntityUid> input)
	{
		return input.Select(Pos);
	}

	[CommandImplementation(null)]
	public EntityCoordinates Pos(IInvocationContext ctx)
	{
		EntityUid? entityUid = ExecutingEntity(ctx);
		if (entityUid.HasValue)
		{
			EntityUid valueOrDefault = entityUid.GetValueOrDefault();
			return Transform(valueOrDefault).Coordinates;
		}
		return EntityCoordinates.Invalid;
	}
}
