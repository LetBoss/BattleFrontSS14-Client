using System.Collections.Generic;
using Content.Shared.Damage;
using Robust.Shared.GameObjects;

namespace Content.Shared.Weapons.Melee.Events;

[ByRefEvent]
public record struct GetMeleeDamageEvent(EntityUid Weapon, DamageSpecifier Damage, List<DamageModifierSet> Modifiers, EntityUid User, bool ResistanceBypass = false);
