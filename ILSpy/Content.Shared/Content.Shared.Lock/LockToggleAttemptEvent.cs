using Robust.Shared.GameObjects;

namespace Content.Shared.Lock;

[ByRefEvent]
public record struct LockToggleAttemptEvent(EntityUid User, bool Silent = false, bool Cancelled = false);
