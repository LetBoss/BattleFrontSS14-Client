using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Robust.Shared.Toolshed.Commands.Math;

[ToolshedCommand]
public sealed class IsPositiveCommand : ToolshedCommand
{
	[CommandImplementation(null)]
	[TakesPipedTypeAsGeneric]
	public bool Operation<T>([PipedArgument] T x) where T : INumberBase<T>
	{
		return T.IsPositive(x);
	}

	[CommandImplementation(null)]
	[TakesPipedTypeAsGeneric]
	public IEnumerable<bool> Operation<T>([PipedArgument] IEnumerable<T> x) where T : INumberBase<T>
	{
		return x.Select(Operation);
	}
}
