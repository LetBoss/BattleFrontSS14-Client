using Robust.Shared.GameObjects;

namespace Content.Shared.Lock;

[ByRefEvent]
public record struct UserLockToggleAttemptEvent(EntityUid Target, bool Silent = false, bool Cancelled = false);
