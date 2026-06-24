using System;
using Content.Shared.Weapons.Ranged.Components;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Attachable;

[Serializable]
[DataRecord]
[NetSerializable]
public record struct AttachableWeaponFireModesModifierSet(AttachableModifierConditions? Conditions, SelectiveFire ExtraFireModes, SelectiveFire SetFireMode);
