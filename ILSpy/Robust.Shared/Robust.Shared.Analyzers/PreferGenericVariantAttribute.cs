using System;

namespace Robust.Shared.Analyzers;

[AttributeUsage(AttributeTargets.Method)]
public sealed class PreferGenericVariantAttribute : Attribute
{
	public readonly string GenericVariant;

	public PreferGenericVariantAttribute(string genericVariant = null)
	{
		GenericVariant = genericVariant;
	}
}
