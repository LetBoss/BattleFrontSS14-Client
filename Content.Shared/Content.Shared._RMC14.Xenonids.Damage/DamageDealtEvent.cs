using Content.Shared.Damage;
using Robust.Shared.GameObjects;

namespace Content.Shared._RMC14.Xenonids.Damage;

[ByRefEvent]
public record struct DamageDealtEvent(EntityUid? Origin, DamageSpecifier? DamageDelta);
