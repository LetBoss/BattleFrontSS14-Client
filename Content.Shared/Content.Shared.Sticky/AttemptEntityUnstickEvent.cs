using Robust.Shared.GameObjects;

namespace Content.Shared.Sticky;

[ByRefEvent]
public record struct AttemptEntityUnstickEvent(EntityUid Target, EntityUid User, bool Cancelled = false);
