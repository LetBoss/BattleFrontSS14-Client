using System;
using Content.Shared.FixedPoint;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Attachable;

[Serializable]
[DataRecord]
[NetSerializable]
public record struct AttachableWeaponRangedModifierSet(AttachableModifierConditions? Conditions, FixedPoint2 AccuracyAddMult, FixedPoint2 DamageFalloffAddMult, double BurstScatterAddMult, int ShotsPerBurstFlat, FixedPoint2 DamageAddMult, float RecoilFlat, double ScatterFlat, float FireDelayFlat, float ProjectileSpeedFlat, float RangeFlat);
