using System;

namespace Robust.Shared.GameObjects;

public sealed class InvalidComponentNameException : Exception
{
	public InvalidComponentNameException(string message)
		: base(message)
	{
	}
}
