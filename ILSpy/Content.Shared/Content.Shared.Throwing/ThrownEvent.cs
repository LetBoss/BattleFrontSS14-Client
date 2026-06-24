using Robust.Shared.GameObjects;

namespace Content.Shared.Throwing;

[ByRefEvent]
public readonly record struct ThrownEvent(EntityUid? User, EntityUid Thrown);
