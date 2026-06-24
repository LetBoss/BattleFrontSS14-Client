using Robust.Shared.GameObjects;

namespace Content.Shared.Slippery;

[ByRefEvent]
public readonly record struct SlipEvent(EntityUid Slipped);
