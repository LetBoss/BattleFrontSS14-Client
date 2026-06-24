using System;

namespace Robust.Shared.GameObjects;

[Serializable]
public sealed class UnknownComponentException : Exception
{
	public UnknownComponentException()
	{
	}

	public UnknownComponentException(string message)
		: base(message)
	{
	}

	public UnknownComponentException(string message, Exception inner)
		: base(message, inner)
	{
	}
}
