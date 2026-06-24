using System;

namespace Robust.Shared.Analyzers;

[AttributeUsage(AttributeTargets.Method)]
public sealed class PreferNonGenericVariantForAttribute : Attribute
{
	public readonly Type[] ForTypes;

	public PreferNonGenericVariantForAttribute(params Type[] forTypes)
	{
		ForTypes = forTypes;
	}
}
