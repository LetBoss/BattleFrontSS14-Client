using Robust.Shared.GameObjects;

namespace Content.Shared._RMC14.Xenonids.Leap;

[ByRefEvent]
public record struct XenoLeapHitAttempt(EntityUid Leaper, bool Cancelled = false);
