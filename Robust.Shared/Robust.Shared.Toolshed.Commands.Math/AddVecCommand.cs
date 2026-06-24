using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Robust.Shared.Toolshed.Commands.Math;

[ToolshedCommand(Name = "+/")]
public sealed class AddVecCommand : ToolshedCommand
{
	[CommandImplementation(null)]
	[TakesPipedTypeAsGeneric]
	public IEnumerable<T> Operation<T>([PipedArgument] IEnumerable<T> x, T y) where T : IAdditionOperators<T, T, T>
	{
		return x.Select((T i) => i + y);
	}

	[CommandImplementation(null)]
	public IEnumerable<Vector2> Operation([PipedArgument] IEnumerable<Vector2> x, Vector2 y)
	{
		return x.Select((Vector2 i) => i + y);
	}
}
