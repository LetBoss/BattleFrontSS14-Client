using Robust.Shared.GameObjects;

namespace Content.Shared._RMC14.Xenonids.Lunge;

[ByRefEvent]
public record struct XenoLungeHitAttempt(EntityUid Lunging, bool Cancelled = false);
