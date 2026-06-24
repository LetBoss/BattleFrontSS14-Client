using Robust.Shared.GameObjects;

namespace Content.Shared.Slippery;

[ByRefEvent]
public record struct SlipCausingAttemptEvent(bool Cancelled);
