using System;

namespace Robust.Shared.Toolshed;

[AttributeUsage(AttributeTargets.Method)]
public sealed class CommandImplementationAttribute : Attribute
{
	public readonly string? SubCommand;

	public CommandImplementationAttribute(string? subCommand = null)
	{
		SubCommand = subCommand;
	}
}
