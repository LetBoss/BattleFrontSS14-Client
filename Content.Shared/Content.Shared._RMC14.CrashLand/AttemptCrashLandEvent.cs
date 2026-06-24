using Robust.Shared.GameObjects;

namespace Content.Shared._RMC14.CrashLand;

[ByRefEvent]
public record struct AttemptCrashLandEvent(EntityUid Crashing, bool Cancelled = false);
