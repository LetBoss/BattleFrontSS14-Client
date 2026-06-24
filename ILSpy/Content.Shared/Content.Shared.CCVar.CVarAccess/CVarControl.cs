using System;
using Content.Shared.Administration;
using Robust.Shared.Reflection;

namespace Content.Shared.CCVar.CVarAccess;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
[Reflect(true)]
public sealed class CVarControl : Attribute
{
	public AdminFlags AdminFlags { get; }

	public object? Min { get; }

	public object? Max { get; }

	public CVarControl(AdminFlags adminFlags, object? min = null, object? max = null, string? helpText = null)
	{
		AdminFlags = adminFlags;
		Min = min;
		Max = max;
		if (min != null && max != null && min.GetType() != max.GetType())
		{
			throw new ArgumentException("Min and max must be of the same type.");
		}
		if ((min == null && max != null) || (min != null && max == null))
		{
			throw new ArgumentException("Min and max must both be null or both be set.");
		}
	}
}
