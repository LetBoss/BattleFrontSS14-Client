using Robust.Shared.GameObjects;

namespace Content.Shared.Sticky;

[ByRefEvent]
public record struct EntityUnstuckEvent(EntityUid Target, EntityUid User);
