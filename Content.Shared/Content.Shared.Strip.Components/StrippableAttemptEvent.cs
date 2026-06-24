using Robust.Shared.GameObjects;

namespace Content.Shared.Strip.Components;

[ByRefEvent]
public record struct StrippableAttemptEvent(EntityUid User, EntityUid Target, bool Cancelled = false);
