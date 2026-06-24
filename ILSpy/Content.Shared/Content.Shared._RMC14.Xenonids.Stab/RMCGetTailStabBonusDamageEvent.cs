using Content.Shared.Damage;
using Robust.Shared.GameObjects;

namespace Content.Shared._RMC14.Xenonids.Stab;

[ByRefEvent]
public record struct RMCGetTailStabBonusDamageEvent(DamageSpecifier Damage);
