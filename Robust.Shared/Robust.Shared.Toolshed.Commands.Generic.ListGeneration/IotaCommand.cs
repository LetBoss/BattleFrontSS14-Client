using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Robust.Shared.Toolshed.Commands.Generic.ListGeneration;

[ToolshedCommand]
public sealed class IotaCommand : ToolshedCommand
{
	[CommandImplementation(null)]
	[TakesPipedTypeAsGeneric]
	public IEnumerable<T> Iota<T>([PipedArgument] T count) where T : INumber<T>
	{
		//IL_0016: Ignored invalid 'constrained' prefix
		return Enumerable.Range(1, int.CreateTruncating(count)).Select(INumberBase<T>.CreateTruncating);
	}
}
