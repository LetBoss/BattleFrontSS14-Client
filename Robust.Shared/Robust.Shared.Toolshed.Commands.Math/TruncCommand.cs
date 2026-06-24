using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Robust.Shared.Toolshed.Commands.Math;

[ToolshedCommand]
public sealed class TruncCommand : ToolshedCommand
{
	[CommandImplementation(null)]
	[TakesPipedTypeAsGeneric]
	public T Operation<T>([PipedArgument] T x) where T : IFloatingPoint<T>
	{
		return T.Truncate(x);
	}

	[CommandImplementation(null)]
	[TakesPipedTypeAsGeneric]
	public IEnumerable<T> Operation<T>([PipedArgument] IEnumerable<T> x) where T : IFloatingPoint<T>
	{
		return x.Select(Operation);
	}
}
