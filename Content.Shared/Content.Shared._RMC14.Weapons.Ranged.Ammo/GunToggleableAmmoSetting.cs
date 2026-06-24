using System;
using Content.Shared.Damage;
using Robust.Shared.Localization;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

namespace Content.Shared._RMC14.Weapons.Ranged.Ammo;

[Serializable]
[DataRecord]
[NetSerializable]
public readonly record struct GunToggleableAmmoSetting(DamageSpecifier Damage, int ArmorPiercing, LocId Name, Rsi Icon);
