using Robust.Shared.GameObjects;

namespace Content.Shared.Explosion.EntitySystems;

[ByRefEvent]
public record struct GrenadeContentThrownEvent(EntityUid Source);
