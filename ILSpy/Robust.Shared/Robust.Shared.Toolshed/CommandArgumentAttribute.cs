using System;

namespace Robust.Shared.Toolshed;

[AttributeUsage(AttributeTargets.Parameter)]
public sealed class CommandArgumentAttribute : Attribute
{
	public bool Unparseable { get; }

	public Type? CustomParser { get; }

	public CommandArgumentAttribute(Type? customParser = null, bool unparseable = false)
	{
		Unparseable = unparseable;
		if (!(customParser == null))
		{
			CustomParser = customParser;
		}
	}
}
