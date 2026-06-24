namespace Robust.Shared.Toolshed.Commands.Generic;

[ToolshedCommand]
public sealed class IsNullCommand : ToolshedCommand
{
	[CommandImplementation(null)]
	public bool IsNull([PipedArgument] object? input, [CommandInverted] bool inverted)
	{
		return (input == null) ^ inverted;
	}
}
