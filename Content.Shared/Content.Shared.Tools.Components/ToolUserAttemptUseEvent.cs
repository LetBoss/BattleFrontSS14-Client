using Robust.Shared.GameObjects;

namespace Content.Shared.Tools.Components;

[ByRefEvent]
public struct ToolUserAttemptUseEvent(EntityUid? target)
{
	public EntityUid? Target = target;

	public bool Cancelled = false;
}
