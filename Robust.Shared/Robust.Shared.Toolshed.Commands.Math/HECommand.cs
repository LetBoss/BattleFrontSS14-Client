using System;

namespace Robust.Shared.Toolshed.Commands.Math;

[ToolshedCommand]
public sealed class HECommand : ToolshedCommand
{
	[CommandImplementation(null)]
	public Half Const()
	{
		return Half.E;
	}
}
