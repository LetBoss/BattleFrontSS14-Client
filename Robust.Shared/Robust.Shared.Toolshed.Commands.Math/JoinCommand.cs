using System.Collections.Generic;
using System.Linq;

namespace Robust.Shared.Toolshed.Commands.Math;

[ToolshedCommand]
public sealed class JoinCommand : ToolshedCommand
{
	[CommandImplementation(null)]
	public string Join([PipedArgument] string x, string y)
	{
		return x + y;
	}

	[CommandImplementation(null)]
	[TakesPipedTypeAsGeneric]
	public IEnumerable<T> Join<T>([PipedArgument] IEnumerable<T> x, IEnumerable<T> y)
	{
		return x.Concat(y);
	}
}
