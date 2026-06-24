using System;

namespace Robust.Shared.Toolshed.Commands.Math;

[ToolshedCommand(Name = "==")]
public sealed class EqualCommand : ToolshedCommand
{
	[CommandImplementation(null)]
	[TakesPipedTypeAsGeneric]
	public bool Comparison<T>([PipedArgument] T x, T y) where T : IEquatable<T>
	{
		return x.Equals(y);
	}
}
