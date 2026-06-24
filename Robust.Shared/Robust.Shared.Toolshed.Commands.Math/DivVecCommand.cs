using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Robust.Shared.Toolshed.Commands.Math;

[ToolshedCommand(Name = "//")]
public sealed class DivVecCommand : ToolshedCommand
{
	[CommandImplementation(null)]
	[TakesPipedTypeAsGeneric]
	public IEnumerable<T> Operation<T>([PipedArgument] IEnumerable<T> x, T y) where T : INumberBase<T>
	{
		if (T.IsZero(y))
		{
			return x.Select((T _) => T.Zero);
		}
		return x.Select((T i) => i / y);
	}
}
