using System.Collections.Generic;
using System.Linq;

namespace Robust.Shared.Toolshed.Commands.Generic;

[ToolshedCommand]
public sealed class CountCommand : ToolshedCommand
{
	[CommandImplementation(null)]
	[TakesPipedTypeAsGeneric]
	public int Count<T>([PipedArgument] IEnumerable<T> input)
	{
		return input.Count();
	}
}
