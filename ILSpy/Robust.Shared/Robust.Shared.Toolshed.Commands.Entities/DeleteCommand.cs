using System.Collections.Generic;
using Robust.Shared.GameObjects;

namespace Robust.Shared.Toolshed.Commands.Entities;

[ToolshedCommand]
internal sealed class DeleteCommand : ToolshedCommand
{
	[CommandImplementation(null)]
	public void Delete([PipedArgument] IEnumerable<EntityUid> entities)
	{
		foreach (EntityUid entity in entities)
		{
			Del(entity);
		}
	}

	[CommandImplementation(null)]
	public void Delete(EntityUid entity)
	{
		Del(entity);
	}
}
