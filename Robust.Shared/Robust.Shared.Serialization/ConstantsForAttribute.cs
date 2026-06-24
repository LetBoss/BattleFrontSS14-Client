using System;

namespace Robust.Shared.Serialization;

[AttributeUsage(AttributeTargets.Enum, AllowMultiple = true)]
public sealed class ConstantsForAttribute : Attribute
{
	public Type Tag { get; }

	public ConstantsForAttribute(Type tag)
	{
		Tag = tag;
	}
}
