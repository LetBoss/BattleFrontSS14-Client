using System;

namespace Robust.Shared.Toolshed;

[AttributeUsage(AttributeTargets.Class)]
public sealed class ToolshedCommandAttribute : Attribute
{
	public string? Name;
}
