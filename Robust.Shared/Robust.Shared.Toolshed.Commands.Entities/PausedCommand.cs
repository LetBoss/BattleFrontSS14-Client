using System.Collections.Generic;
using System.Linq;
using Robust.Shared.GameObjects;

namespace Robust.Shared.Toolshed.Commands.Entities;

[ToolshedCommand]
internal sealed class PausedCommand : ToolshedCommand
{
	[CommandImplementation(null)]
	public IEnumerable<EntityUid> Paused([PipedArgument] IEnumerable<EntityUid> entities, [CommandInverted] bool inverted)
	{
		return entities.Where((EntityUid x) => Comp<MetaDataComponent>(x).EntityPaused ^ inverted);
	}
}
