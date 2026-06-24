using System;
using Content.Shared.Damage;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Attachable;

[Serializable]
[DataRecord]
[NetSerializable]
public record struct AttachableWeaponMeleeModifierSet(AttachableModifierConditions? Conditions, DamageSpecifier? BonusDamage);
