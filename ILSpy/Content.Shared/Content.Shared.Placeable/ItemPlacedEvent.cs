using Robust.Shared.GameObjects;

namespace Content.Shared.Placeable;

[ByRefEvent]
public readonly record struct ItemPlacedEvent(EntityUid OtherEntity);
