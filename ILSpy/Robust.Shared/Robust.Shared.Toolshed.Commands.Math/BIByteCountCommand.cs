using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Robust.Shared.Toolshed.Commands.Math;

[ToolshedCommand]
public sealed class BIByteCountCommand : ToolshedCommand
{
	[CommandImplementation(null)]
	[TakesPipedTypeAsGeneric]
	public int Operation<T>([PipedArgument] T x) where T : IBinaryInteger<T>
	{
		return x.GetByteCount();
	}

	[CommandImplementation(null)]
	[TakesPipedTypeAsGeneric]
	public IEnumerable<int> Operation<T>([PipedArgument] IEnumerable<T> x) where T : IBinaryInteger<T>
	{
		return x.Select(Operation);
	}
}
