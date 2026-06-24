using System.Collections.Generic;
using System.Linq;

namespace Robust.Shared.Toolshed.Commands.Generic;

[ToolshedCommand]
public sealed class AnyCommand : ToolshedCommand
{
	[CommandImplementation(null)]
	[TakesPipedTypeAsGeneric]
	public bool Any<T>([PipedArgument] IEnumerable<T> input)
	{
		return input.Any();
	}
}
