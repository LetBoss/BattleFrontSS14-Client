using System;

namespace Robust.Shared.Analyzers;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class PreferOtherTypeAttribute(Type genericType, Type replacementType) : Attribute
{
	public readonly Type GenericArgument = genericType;

	public readonly Type ReplacementType = replacementType;
}
