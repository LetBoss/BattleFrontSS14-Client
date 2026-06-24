using Robust.Shared.GameObjects;

namespace Robust.Shared.Toolshed.Commands.Values;

[ToolshedCommand]
internal sealed class EntCommand : ToolshedCommand
{
	[CommandImplementation(null)]
	public EntityUid Ent(EntityUid uid)
	{
		return uid;
	}
}
