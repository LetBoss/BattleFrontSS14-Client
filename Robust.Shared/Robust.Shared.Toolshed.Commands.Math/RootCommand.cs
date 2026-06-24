using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Robust.Shared.Toolshed.Commands.Math;

[ToolshedCommand]
public sealed class RootCommand : ToolshedCommand
{
	[CommandImplementation(null)]
	[TakesPipedTypeAsGeneric]
	public T Operation<T>([PipedArgument] T x, int y) where T : IRootFunctions<T>
	{
		return T.RootN(x, y);
	}

	[CommandImplementation(null)]
	[TakesPipedTypeAsGeneric]
	public IEnumerable<T> Operation<T>([PipedArgument] IEnumerable<T> x, IEnumerable<int> y) where T : IRootFunctions<T>
	{
		return x.Zip(y).Select(delegate((T First, int Second) inp)
		{
			var (x2, y2) = inp;
			return Operation(x2, y2);
		});
	}
}
