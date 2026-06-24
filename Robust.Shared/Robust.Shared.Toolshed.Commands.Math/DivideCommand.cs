using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Robust.Shared.Toolshed.Commands.Math;

[ToolshedCommand(Name = "/")]
public sealed class DivideCommand : ToolshedCommand
{
	[CommandImplementation(null)]
	[TakesPipedTypeAsGeneric]
	public T Operation<T>([PipedArgument] T x, T y) where T : INumberBase<T>
	{
		if (T.IsZero(y))
		{
			return T.Zero;
		}
		return x / y;
	}

	[CommandImplementation(null)]
	[TakesPipedTypeAsGeneric]
	public IEnumerable<T> Operation<T>([PipedArgument] IEnumerable<T> x, IEnumerable<T> y) where T : INumberBase<T>
	{
		return x.Zip(y).Select(delegate((T First, T Second) inp)
		{
			var (x2, y2) = inp;
			return Operation(x2, y2);
		});
	}
}
