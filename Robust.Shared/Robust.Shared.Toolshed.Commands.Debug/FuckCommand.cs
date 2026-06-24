using System;

namespace Robust.Shared.Toolshed.Commands.Debug;

[ToolshedCommand]
public sealed class FuckCommand : ToolshedCommand
{
	[CommandImplementation(null)]
	public object? Fuck([PipedArgument] object? value)
	{
		throw new Exception("fuck!");
	}

	[CommandImplementation(null)]
	public object? Fuck()
	{
		throw new Exception("fuck!");
	}
}
