using System.Collections.Generic;
using System.Linq;

namespace Robust.Shared.Toolshed.Commands.Math;

[ToolshedCommand]
public sealed class AppendCommand : ToolshedCommand
{
	[CommandImplementation(null)]
	[TakesPipedTypeAsGeneric]
	public IEnumerable<T> Append<T>([PipedArgument] IEnumerable<T> x, T y)
	{
		return x.Append(y);
	}
}
