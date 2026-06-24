using Content.Shared.Damage;
using Robust.Shared.GameObjects;

namespace Content.Shared._RMC14.Projectiles;

[ByRefEvent]
public record struct BeforeAreaDamageEvent(EntityUid Target, DamageSpecifier Damage, bool Cancelled = false);
