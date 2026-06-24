using System;

namespace Robust.Shared.Serialization;

[AttributeUsage(AttributeTargets.Enum, AllowMultiple = true, Inherited = false)]
public sealed class FlagsForAttribute : Attribute
{
	private readonly Type _tag;

	public Type Tag => _tag;

	public FlagsForAttribute(Type tag)
	{
		_tag = tag;
	}
}
