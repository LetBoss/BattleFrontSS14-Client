using Robust.Shared.GameObjects;

namespace Content.Shared.Throwing;

[ByRefEvent]
public record struct ThrowItemAttemptEvent(EntityUid User, bool Cancelled = false);
