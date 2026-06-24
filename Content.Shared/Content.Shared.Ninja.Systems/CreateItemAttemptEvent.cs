using Robust.Shared.GameObjects;

namespace Content.Shared.Ninja.Systems;

[ByRefEvent]
public record struct CreateItemAttemptEvent(EntityUid User, bool Cancelled = false);
