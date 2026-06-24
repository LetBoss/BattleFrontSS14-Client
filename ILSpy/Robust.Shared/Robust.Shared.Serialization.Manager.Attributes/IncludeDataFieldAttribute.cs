using System;

namespace Robust.Shared.Serialization.Manager.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
[MeansImplicitAssignment]
public sealed class IncludeDataFieldAttribute : DataFieldBaseAttribute
{
	public IncludeDataFieldAttribute(bool readOnly = false, int priority = 1, bool serverOnly = false, Type? customTypeSerializer = null)
		: base(readOnly, priority, serverOnly, customTypeSerializer)
	{
	}

	public override string ToString()
	{
		return "[INCLUDE]";
	}
}
