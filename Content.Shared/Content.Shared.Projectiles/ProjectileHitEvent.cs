using Content.Shared.Damage;
using Robust.Shared.GameObjects;

namespace Content.Shared.Projectiles;

[ByRefEvent]
public record struct ProjectileHitEvent(DamageSpecifier Damage, EntityUid Target, EntityUid? Shooter = null, bool Handled = false);
