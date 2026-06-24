using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Robust.Shared.Toolshed.Commands.Math;

[ToolshedCommand]
public sealed class ASinPiCommand : ToolshedCommand
{
	[CommandImplementation(null)]
	[TakesPipedTypeAsGeneric]
	public T Operation<T>([PipedArgument] T x) where T : ITrigonometricFunctions<T>
	{
		return T.AsinPi(x);
	}

	[CommandImplementation(null)]
	[TakesPipedTypeAsGeneric]
	public IEnumerable<T> Operation<T>([PipedArgument] IEnumerable<T> x) where T : ITrigonometricFunctions<T>
	{
		return x.Select(Operation);
	}
}
