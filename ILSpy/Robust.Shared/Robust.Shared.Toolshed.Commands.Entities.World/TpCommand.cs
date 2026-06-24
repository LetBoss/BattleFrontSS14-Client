using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;

namespace Robust.Shared.Toolshed.Commands.Entities.World;

[ToolshedCommand]
internal sealed class TpCommand : ToolshedCommand
{
	private SharedTransformSystem? _xform;

	[CommandImplementation("coords")]
	public EntityUid TpCoords([PipedArgument] EntityUid teleporter, [CommandArgument(null, true)] EntityCoordinates target)
	{
		if (_xform == null)
		{
			_xform = GetSys<SharedTransformSystem>();
		}
		_xform.SetCoordinates(teleporter, target);
		return teleporter;
	}

	[CommandImplementation("coords")]
	public IEnumerable<EntityUid> TpCoords([PipedArgument] IEnumerable<EntityUid> teleporters, [CommandArgument(null, true)] EntityCoordinates target)
	{
		return teleporters.Select((EntityUid x) => TpCoords(x, target));
	}

	[CommandImplementation("to")]
	public EntityUid TpTo([PipedArgument] EntityUid teleporter, EntityUid target)
	{
		if (_xform == null)
		{
			_xform = GetSys<SharedTransformSystem>();
		}
		_xform.SetCoordinates(teleporter, Transform(target).Coordinates);
		return teleporter;
	}

	[CommandImplementation("to")]
	public IEnumerable<EntityUid> TpTo([PipedArgument] IEnumerable<EntityUid> teleporters, EntityUid target)
	{
		return teleporters.Select((EntityUid x) => TpTo(x, target));
	}

	[CommandImplementation("into")]
	public EntityUid TpInto([PipedArgument] EntityUid teleporter, EntityUid target)
	{
		if (_xform == null)
		{
			_xform = GetSys<SharedTransformSystem>();
		}
		_xform.SetCoordinates(teleporter, new EntityCoordinates(target, Vector2.Zero));
		return teleporter;
	}

	[CommandImplementation("into")]
	public IEnumerable<EntityUid> TpInto([PipedArgument] IEnumerable<EntityUid> teleporters, EntityUid target)
	{
		return teleporters.Select((EntityUid x) => TpInto(x, target));
	}
}
