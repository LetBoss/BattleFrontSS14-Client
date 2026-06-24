using Robust.Shared.GameObjects;

namespace Content.Shared.Standing;

[ByRefEvent]
public record struct FellDownThrowAttemptEvent(EntityUid Thrower, bool Cancelled = false);
