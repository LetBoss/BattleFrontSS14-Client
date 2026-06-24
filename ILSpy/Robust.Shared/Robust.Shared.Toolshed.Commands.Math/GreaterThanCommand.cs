using System.Numerics;

namespace Robust.Shared.Toolshed.Commands.Math;

[ToolshedCommand(Name = ">")]
public sealed class GreaterThanCommand : ToolshedCommand
{
	[CommandImplementation(null)]
	[TakesPipedTypeAsGeneric]
	public bool Comparison<T>([PipedArgument] T x, T y) where T : INumber<T>
	{
		return x > y;
	}
}
