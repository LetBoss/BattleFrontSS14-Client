using Robust.Shared.GameObjects;

namespace Content.Shared.Sticky;

[ByRefEvent]
public record struct AttemptEntityStickEvent(EntityUid Target, EntityUid User, bool Cancelled = false);
