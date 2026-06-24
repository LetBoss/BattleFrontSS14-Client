using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;

namespace Content.Shared.Weapons.Melee.Events;

[ByRefEvent]
public record struct GetHeavyDamageModifierEvent(EntityUid Weapon, FixedPoint2 DamageModifier, float Multipliers, EntityUid User);
