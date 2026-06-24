using System;

namespace Robust.Shared.Exceptions;

[Serializable]
public sealed class TypeArgumentException : Exception
{
	public readonly string? TypeArgumentName;

	public TypeArgumentException()
	{
	}

	public TypeArgumentException(string message)
		: base(message)
	{
	}

	public TypeArgumentException(string message, Exception inner)
		: base(message, inner)
	{
	}

	public TypeArgumentException(string message, string name)
		: base(message)
	{
		TypeArgumentName = name;
	}

	public TypeArgumentException(string message, string name, Exception inner)
		: base(message, inner)
	{
		TypeArgumentName = name;
	}
}
