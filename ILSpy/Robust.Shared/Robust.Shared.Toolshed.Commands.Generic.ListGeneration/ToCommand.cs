using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Robust.Shared.Toolshed.Commands.Generic.ListGeneration;

[ToolshedCommand]
public sealed class ToCommand : ToolshedCommand
{
	[CommandImplementation(null)]
	[TakesPipedTypeAsGeneric]
	public IEnumerable<T> To<T>([PipedArgument] T start, T end) where T : INumber<T>
	{
		//IL_0029: Ignored invalid 'constrained' prefix
		return Enumerable.Range(int.CreateTruncating(start), 1 + int.CreateTruncating(end - start)).Select(INumberBase<T>.CreateTruncating);
	}
}
