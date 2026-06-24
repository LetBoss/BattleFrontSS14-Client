using System;
using Robust.Shared.Analyzers;

namespace Robust.Shared.IoC.Exceptions;

[Serializable]
[Virtual]
public class UnregisteredTypeException : Exception
{
	public readonly string? TypeName;

	public UnregisteredTypeException(Type type)
		: base($"Attempted to resolve unregistered type: {type}")
	{
		TypeName = type.AssemblyQualifiedName;
	}
}
