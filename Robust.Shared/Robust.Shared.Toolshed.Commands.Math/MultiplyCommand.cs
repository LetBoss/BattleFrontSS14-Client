using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Robust.Shared.Toolshed.Commands.Math;

[ToolshedCommand(Name = "*")]
public sealed class MultiplyCommand : ToolshedCommand
{
	[CommandImplementation(null)]
	[TakesPipedTypeAsGeneric]
	public T Operation<T>([PipedArgument] T x, T y) where T : IMultiplyOperators<T, T, T>
	{
		return x * y;
	}

	[CommandImplementation(null)]
	[TakesPipedTypeAsGeneric]
	public IEnumerable<T> Operation<T>([PipedArgument] IEnumerable<T> x, IEnumerable<T> y) where T : IMultiplyOperators<T, T, T>
	{
		return x.Zip(y).Select(delegate((T First, T Second) inp)
		{
			var (x2, y2) = inp;
			return Operation(x2, y2);
		});
	}

	[CommandImplementation(null)]
	public Vector2 Operation([PipedArgument] Vector2 x, Vector2 y)
	{
		return x * y;
	}

	[CommandImplementation(null)]
	public IEnumerable<Vector2> Operation([PipedArgument] IEnumerable<Vector2> x, IEnumerable<Vector2> y)
	{
		return x.Zip(y).Select(delegate((Vector2 First, Vector2 Second) inp)
		{
			var (x2, y2) = inp;
			return Operation(x2, y2);
		});
	}
}
