using System;

namespace Robust.Shared.Toolshed.Commands.Math;

[ToolshedCommand]
public sealed class HTauCommand : ToolshedCommand
{
	[CommandImplementation(null)]
	public Half Const()
	{
		return Half.Tau;
	}
}
