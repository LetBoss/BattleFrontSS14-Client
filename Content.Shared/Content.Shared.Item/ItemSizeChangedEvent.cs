using Robust.Shared.GameObjects;

namespace Content.Shared.Item;

[ByRefEvent]
public struct ItemSizeChangedEvent(EntityUid Entity)
{
	public EntityUid Entity = Entity;
}
