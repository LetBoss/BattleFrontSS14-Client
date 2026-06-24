using System;

namespace Robust.Shared.ViewVariables;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field)]
public sealed class ViewVariablesAttribute : Attribute
{
	public readonly VVAccess Access;

	public ViewVariablesAttribute()
	{
	}

	public ViewVariablesAttribute(VVAccess access)
	{
		Access = access;
	}
}
