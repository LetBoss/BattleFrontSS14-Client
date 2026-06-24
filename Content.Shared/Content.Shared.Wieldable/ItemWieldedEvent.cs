using Robust.Shared.GameObjects;

namespace Content.Shared.Wieldable;

[ByRefEvent]
public readonly record struct ItemWieldedEvent(EntityUid User);
