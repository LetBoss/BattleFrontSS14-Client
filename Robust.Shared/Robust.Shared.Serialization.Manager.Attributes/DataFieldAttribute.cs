using System;
using Robust.Shared.Analyzers;

namespace Robust.Shared.Serialization.Manager.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
[MeansImplicitAssignment]
[Virtual]
public class DataFieldAttribute : DataFieldBaseAttribute
{
	public readonly bool Required;

	public string? Tag { get; internal set; }

	public DataFieldAttribute(string? tag = null, bool readOnly = false, int priority = 1, bool required = false, bool serverOnly = false, Type? customTypeSerializer = null)
		: base(readOnly, priority, serverOnly, customTypeSerializer)
	{
		Tag = tag;
		Required = required;
	}

	public override string? ToString()
	{
		return Tag;
	}
}
