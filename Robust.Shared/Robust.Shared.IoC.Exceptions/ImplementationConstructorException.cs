using System;
using Robust.Shared.Analyzers;

namespace Robust.Shared.IoC.Exceptions;

[Serializable]
[Virtual]
public class ImplementationConstructorException : Exception
{
	public readonly string? typeName;

	public ImplementationConstructorException(Type type, Exception? inner)
		: base($"{type} threw an exception inside its constructor.", inner)
	{
		typeName = type.AssemblyQualifiedName;
	}
}
