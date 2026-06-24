using Content.Shared.Damage;
using Robust.Shared.GameObjects;

namespace Content.Shared._RMC14.Damage;

[ByRefEvent]
public record struct DamageStateCritBeforeDamageEvent(DamageSpecifier Damage);
