using Robust.Shared.GameObjects;

namespace Content.Shared.Throwing;

[ByRefEvent]
public record struct CatchAttemptEvent(EntityUid Item, float CatchChance, bool Cancelled = false);
