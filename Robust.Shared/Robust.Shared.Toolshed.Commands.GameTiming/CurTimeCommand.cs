using System;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Robust.Shared.Toolshed.Commands.GameTiming;

[ToolshedCommand]
public sealed class CurTimeCommand : ToolshedCommand
{
	[Dependency]
	private readonly IGameTiming _gameTiming;

	[CommandImplementation(null)]
	public TimeSpan CurTime()
	{
		return _gameTiming.CurTime;
	}
}
