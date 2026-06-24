using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;

namespace Content.Shared._RMC14.Weapons.Ranged;

[ByRefEvent]
public record struct GetGunDamageModifierEvent(FixedPoint2 Multiplier);
