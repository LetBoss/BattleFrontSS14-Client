using Robust.Shared.GameObjects;

namespace Content.Shared.Bed.Sleep;

[ByRefEvent]
public record struct TryingToSleepEvent(EntityUid uid, bool Cancelled = false);
