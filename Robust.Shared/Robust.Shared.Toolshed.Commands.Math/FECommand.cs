using System;

namespace Robust.Shared.Toolshed.Commands.Math;

[ToolshedCommand]
public sealed class FECommand : ToolshedCommand
{
	[CommandImplementation(null)]
	public float Const()
	{
		return (float)System.Math.E;
	}
}
