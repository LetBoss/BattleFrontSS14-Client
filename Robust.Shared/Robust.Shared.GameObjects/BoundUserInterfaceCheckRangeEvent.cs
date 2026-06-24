using System;

namespace Robust.Shared.GameObjects;

[ByRefEvent]
public struct BoundUserInterfaceCheckRangeEvent(Entity<TransformComponent> target, Enum uiKey, InterfaceData data, Entity<TransformComponent> actor)
{
	public readonly EntityUid Target = target;

	public readonly Enum UiKey = uiKey;

	public readonly InterfaceData Data = data;

	public readonly Entity<TransformComponent> Actor = actor;

	public BoundUserInterfaceRangeResult Result = BoundUserInterfaceRangeResult.Default;
}
