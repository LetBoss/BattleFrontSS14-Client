using System;
using System.Collections.Generic;
using Robust.Shared.IoC;
using Robust.Shared.Toolshed;

namespace Robust.Shared.GameObjects;

[ToolshedCommand]
internal sealed class EntitySystemUpdateOrderCommand : ToolshedCommand
{
	[Dependency]
	private readonly IEntitySystemManager _entitySystemManager;

	[CommandImplementation("tick")]
	public IEnumerable<Type> Tick()
	{
		return ((EntitySystemManager)_entitySystemManager).TickUpdateOrder;
	}

	[CommandImplementation("frame")]
	public IEnumerable<Type> Frame()
	{
		return ((EntitySystemManager)_entitySystemManager).FrameUpdateOrder;
	}
}
