using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Robust.Shared.Toolshed.Commands.Math;

[ToolshedCommand]
public sealed class ShortestBitLengthCommand : ToolshedCommand
{
	[CommandImplementation(null)]
	[TakesPipedTypeAsGeneric]
	public int Operation<T>([PipedArgument] T x) where T : IBinaryInteger<T>
	{
		return x.GetShortestBitLength();
	}

	[CommandImplementation(null)]
	[TakesPipedTypeAsGeneric]
	public IEnumerable<int> Operation<T>([PipedArgument] IEnumerable<T> x) where T : IBinaryInteger<T>
	{
		return x.Select(Operation);
	}
}
