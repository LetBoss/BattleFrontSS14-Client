using Content.Shared.Damage;
using Content.Shared.Mobs.Components;
using Robust.Shared.GameObjects;

namespace Content.Shared.Mobs.Systems;

[ByRefEvent]
public readonly record struct MobThresholdChecked(EntityUid Target, MobStateComponent MobState, MobThresholdsComponent Threshold, DamageableComponent Damageable);
