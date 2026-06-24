using System.Numerics;

namespace Robust.Shared.Toolshed.Commands.Math;

[ToolshedCommand(Name = "<")]
public sealed class LessThanCommand : ToolshedCommand
{
	[CommandImplementation(null)]
	[TakesPipedTypeAsGeneric]
	public bool Comparison<T>([PipedArgument] T x, T y) where T : IComparisonOperators<T, T, bool>
	{
		return x > y;
	}
}
