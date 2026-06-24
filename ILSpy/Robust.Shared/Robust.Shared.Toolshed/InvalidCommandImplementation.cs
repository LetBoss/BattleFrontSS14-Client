using System;

namespace Robust.Shared.Toolshed;

public sealed class InvalidCommandImplementation : Exception
{
	public InvalidCommandImplementation(string message)
		: base(message)
	{
	}
}
