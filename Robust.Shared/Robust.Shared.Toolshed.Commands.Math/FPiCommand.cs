using System;

namespace Robust.Shared.Toolshed.Commands.Math;

[ToolshedCommand]
public sealed class FPiCommand : ToolshedCommand
{
	[CommandImplementation(null)]
	public float Const()
	{
		return (float)System.Math.PI;
	}
}
