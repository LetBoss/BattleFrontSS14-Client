using Content.Shared.Damage;
using Robust.Shared.GameObjects;

namespace Content.Shared._RMC14.Medical.Defibrillator;

[ByRefEvent]
public record struct RMCDefibrillatorDamageModifyEvent(EntityUid Target, DamageSpecifier Heal);
