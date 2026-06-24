using Robust.Shared.GameObjects;

namespace Content.Shared.Throwing;

[ByRefEvent]
public readonly record struct LandEvent(EntityUid? User, bool PlaySound);
