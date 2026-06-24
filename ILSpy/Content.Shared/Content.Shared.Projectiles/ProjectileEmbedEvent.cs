using Robust.Shared.GameObjects;

namespace Content.Shared.Projectiles;

[ByRefEvent]
public readonly record struct ProjectileEmbedEvent(EntityUid? Shooter, EntityUid Weapon, EntityUid Embedded);
