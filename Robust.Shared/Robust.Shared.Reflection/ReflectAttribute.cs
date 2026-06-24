using System;

namespace Robust.Shared.Reflection;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class ReflectAttribute : Attribute
{
	public const bool DEFAULT_DISCOVERABLE = true;

	public bool Discoverable { get; }

	public ReflectAttribute(bool discoverable)
	{
		Discoverable = discoverable;
	}
}
