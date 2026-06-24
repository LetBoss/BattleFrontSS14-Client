using Robust.Shared.GameObjects;

namespace Content.Shared.Sticky;

[ByRefEvent]
public record struct EntityStuckEvent(EntityUid Target, EntityUid User);
