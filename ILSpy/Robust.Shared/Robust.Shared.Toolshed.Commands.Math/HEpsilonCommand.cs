using System;

namespace Robust.Shared.Toolshed.Commands.Math;

[ToolshedCommand]
public sealed class HEpsilonCommand : ToolshedCommand
{
	[CommandImplementation(null)]
	public Half Const()
	{
		return Half.Epsilon;
	}
}
