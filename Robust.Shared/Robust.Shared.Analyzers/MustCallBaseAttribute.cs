using System;

namespace Robust.Shared.Analyzers;

[AttributeUsage(AttributeTargets.Method)]
public sealed class MustCallBaseAttribute(bool onlyOverrides = false) : Attribute
{
	public bool OnlyOverrides { get; } = onlyOverrides;
}
