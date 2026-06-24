using System;
using Robust.Shared.Analyzers;

namespace Robust.Shared.IoC.Exceptions;

[Virtual]
public class InvalidImplementationException : Exception
{
	private readonly string message;

	private readonly Type type;

	private readonly Type parent;

	public override string Message => $"{type} incorrectly implements {parent}: {message}";

	public InvalidImplementationException(Type type, Type parent, string message)
	{
		this.type = type;
		this.parent = parent;
		this.message = message;
	}
}
