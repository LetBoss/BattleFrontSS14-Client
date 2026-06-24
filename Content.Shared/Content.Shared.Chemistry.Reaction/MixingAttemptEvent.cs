using Robust.Shared.GameObjects;

namespace Content.Shared.Chemistry.Reaction;

[ByRefEvent]
public record struct MixingAttemptEvent(EntityUid Mixed, bool Cancelled = false);
