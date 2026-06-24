using System;

namespace Robust.Shared.Toolshed.Commands.Math;

[ToolshedCommand]
public sealed class DECommand : ToolshedCommand
{
	[CommandImplementation(null)]
	public double Const()
	{
		return System.Math.E;
	}
}
