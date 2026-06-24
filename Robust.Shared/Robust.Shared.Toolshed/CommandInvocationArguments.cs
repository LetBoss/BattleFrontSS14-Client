using System.Collections.Generic;

namespace Robust.Shared.Toolshed;

internal sealed class CommandInvocationArguments
{
	public required object? PipedArgument;

	public required CommandArgumentBundle Bundle;

	public required IInvocationContext Context { get; set; }

	public Dictionary<string, object?>? Arguments => Bundle.Arguments;

	public bool Inverted => Bundle.Inverted;
}
