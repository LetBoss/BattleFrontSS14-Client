using System;

namespace Robust.Shared.Analyzers;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class ObsoleteInheritanceAttribute : Attribute
{
	public string? Message { get; }

	public ObsoleteInheritanceAttribute()
	{
	}

	public ObsoleteInheritanceAttribute(string message)
	{
		Message = message;
	}
}
