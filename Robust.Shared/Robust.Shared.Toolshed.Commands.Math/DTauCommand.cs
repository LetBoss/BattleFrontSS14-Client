using System;

namespace Robust.Shared.Toolshed.Commands.Math;

[ToolshedCommand]
public sealed class DTauCommand : ToolshedCommand
{
	[CommandImplementation(null)]
	public double Const()
	{
		return System.Math.PI * 2.0;
	}
}
