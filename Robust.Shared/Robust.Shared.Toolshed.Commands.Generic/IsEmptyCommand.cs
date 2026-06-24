using System.Collections;
using System.Linq;

namespace Robust.Shared.Toolshed.Commands.Generic;

[ToolshedCommand]
public sealed class IsEmptyCommand : ToolshedCommand
{
	[CommandImplementation(null)]
	[TakesPipedTypeAsGeneric]
	public bool IsEmpty<T>([PipedArgument] T? input, [CommandInverted] bool inverted)
	{
		if (input == null)
		{
			return !inverted;
		}
		if (input is IEnumerable source)
		{
			return !source.Cast<object>().Any() ^ inverted;
		}
		return inverted;
	}
}
