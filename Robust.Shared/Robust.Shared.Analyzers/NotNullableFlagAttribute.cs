using System;

namespace Robust.Shared.Analyzers;

[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.GenericParameter)]
public sealed class NotNullableFlagAttribute : Attribute
{
	public readonly string TypeParameterName;

	public NotNullableFlagAttribute(string typeParameterName)
	{
		TypeParameterName = typeParameterName;
	}
}
