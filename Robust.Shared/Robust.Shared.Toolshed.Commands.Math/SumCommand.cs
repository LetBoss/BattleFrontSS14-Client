using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Robust.Shared.Toolshed.Commands.Math;

[ToolshedCommand]
public sealed class SumCommand : ToolshedCommand
{
	[CommandImplementation(null)]
	[TakesPipedTypeAsGeneric]
	public T Sum<T>([PipedArgument] IEnumerable<T> input) where T : IAdditionOperators<T, T, T>
	{
		return input.Aggregate((T x, T y) => x + y);
	}
}
