using System;
using Robust.Shared.Analyzers;

namespace Robust.Shared.IoC.Exceptions;

[Serializable]
[Virtual]
public class UnregisteredDependencyException : Exception
{
	public readonly string? OwnerType;

	public readonly string? TargetType;

	public readonly string? FieldName;

	public UnregisteredDependencyException(Type owner, Type target, string fieldName)
		: base($"{owner} requested unregistered type with its field {target}: {fieldName}")
	{
		OwnerType = owner.AssemblyQualifiedName;
		TargetType = target.AssemblyQualifiedName;
		FieldName = fieldName;
	}
}
