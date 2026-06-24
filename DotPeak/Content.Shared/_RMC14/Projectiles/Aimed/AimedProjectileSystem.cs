// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Projectiles.Aimed.AimedProjectileSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Armor;
using Content.Shared._RMC14.Atmos;
using Content.Shared._RMC14.BlurredVision;
using Content.Shared._RMC14.Projectiles.StoppingPower;
using Content.Shared._RMC14.Slow;
using Content.Shared._RMC14.Stun;
using Content.Shared._RMC14.Weapons.Ranged.AimedShot.FocusedShooting;
using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Content.Shared.Mobs.Systems;
using Content.Shared.Projectiles;
using Content.Shared.StatusEffect;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Shared._RMC14.Projectiles.Aimed;

public sealed class AimedProjectileSystem : EntitySystem
{
  private const float BigXenoSlowDurationMultiplier = 0.6f;
  private const float BigXenoBlindDurationMultiplier = 0.4f;
  private const string BlindKey = "Blinded";
  [Dependency]
  private RMCSizeStunSystem _sizeStun;
  [Dependency]
  private RMCSlowSystem _slow;
  [Dependency]
  private DamageableSystem _damageable;
  [Dependency]
  private StatusEffectsSystem _statusEffects;
  [Dependency]
  private MobThresholdSystem _mobThresholds;
  [Dependency]
  private RMCDazedSystem _dazed;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<AimedProjectileComponent, ProjectileHitEvent>(new EntityEventRefHandler<AimedProjectileComponent, ProjectileHitEvent>(this.OnAimedProjectileHit));
    this.SubscribeLocalEvent<AimedProjectileComponent, BeforeAreaDamageEvent>(new EntityEventRefHandler<AimedProjectileComponent, BeforeAreaDamageEvent>(this.OnBeforeAreaDamage));
  }

  private void OnAimedProjectileHit(
    Entity<AimedProjectileComponent> ent,
    ref ProjectileHitEvent args)
  {
    AimedShotEffectComponent comp1;
    if (!this.TryComp<AimedShotEffectComponent>((EntityUid) ent, out comp1) || args.Handled)
      return;
    RMCFocusedShootingComponent comp2;
    if (this.TryComp<RMCFocusedShootingComponent>(ent.Comp.Source, out comp2))
      this.CalculateFocusEffects(ent, args.Target, comp2, comp1);
    EntityUid target = args.Target;
    TimeSpan superSlowDuration = comp1.SuperSlowDuration;
    TimeSpan blindDuration = comp1.BlindDuration;
    RMCSizes size;
    this._sizeStun.TryGetSize((EntityUid) ent, out size);
    if (target != ent.Comp.Target)
      return;
    if (size >= RMCSizes.Big)
    {
      superSlowDuration *= 0.60000002384185791;
      blindDuration *= 0.40000000596046448;
    }
    int num1 = 0;
    DamageSpecifier damageSpecifier = args.Damage * comp1.ExtraHits + comp1.CurrentHealthDamage;
    CMArmorPiercingComponent comp3;
    if (this.TryComp<CMArmorPiercingComponent>((EntityUid) ent, out comp3))
      num1 = comp3.Amount;
    DamageableSystem damageable = this._damageable;
    EntityUid? uid = new EntityUid?(target);
    DamageSpecifier damage = damageSpecifier;
    EntityUid? nullable = new EntityUid?(ent.Comp.Source);
    int num2 = num1;
    EntityUid? origin = new EntityUid?();
    EntityUid? tool = nullable;
    int armorPiercing = num2;
    damageable.TryChangeDamage(uid, damage, origin: origin, tool: tool, armorPiercing: armorPiercing);
    this._slow.TrySlowdown(target, comp1.SlowDuration);
    this._slow.TrySuperSlowdown(target, superSlowDuration);
    this._statusEffects.TryAddStatusEffect<RMCBlindedComponent>(target, "Blinded", blindDuration, false);
    IgniteOnProjectileHitComponent comp4;
    if (!this.TryComp<IgniteOnProjectileHitComponent>((EntityUid) ent, out comp4))
      return;
    comp4.Duration += comp1.FireStacksOnHit;
  }

  private void OnBeforeAreaDamage(
    Entity<AimedProjectileComponent> ent,
    ref BeforeAreaDamageEvent args)
  {
    if (!(args.Target == ent.Comp.Target))
      return;
    args.Cancelled = true;
  }

  private void CalculateFocusEffects(
    Entity<AimedProjectileComponent> ent,
    EntityUid target,
    RMCFocusedShootingComponent focusEffect,
    AimedShotEffectComponent aimedEffect)
  {
    float num1 = 0.0f;
    bool flag = false;
    RMCSizes size;
    this._sizeStun.TryGetSize(target, out size);
    RMCStoppingPowerComponent comp1;
    if (this.TryComp<RMCStoppingPowerComponent>((EntityUid) ent, out comp1))
      num1 = comp1.CurrentStoppingPower;
    if ((double) num1 < (double) focusEffect.SlowThreshold)
      num1 = 0.0f;
    if (size >= RMCSizes.VerySmallXeno)
    {
      float num2 = focusEffect.CurrentHealthDamageSmallXeno;
      float num3 = focusEffect.BonusDamageXeno;
      if (size >= RMCSizes.Xeno)
      {
        num2 = focusEffect.CurrentHealthDamageXeno;
        num1 = Math.Max(num1 - 1f, 0.0f);
        flag = true;
      }
      if (size >= RMCSizes.Big)
      {
        num3 = focusEffect.BonusDamageBigXeno;
        num2 = focusEffect.CurrentHealthDamageBigXeno;
        num1 = Math.Max(num1 - 1f, 0.0f);
        flag = true;
      }
      DamageableComponent comp2;
      if (this.TryComp<DamageableComponent>(target, out comp2))
      {
        FixedPoint2? threshold;
        this._mobThresholds.TryGetIncapThreshold(target, out threshold);
        if (!threshold.HasValue)
          return;
        DamageSpecifier damageSpecifier = new DamageSpecifier();
        damageSpecifier.DamageDict.Add("Piercing", (threshold.Value - comp2.TotalDamage) * num2);
        if (flag)
        {
          damageSpecifier *= focusEffect.BaseFocusMultiplier + focusEffect.FocusMultiplier * (float) focusEffect.FocusCounter;
          num3 *= focusEffect.BaseFocusMultiplier + focusEffect.FocusMultiplier * (float) focusEffect.FocusCounter;
          num1 *= focusEffect.BaseFocusMultiplier + focusEffect.FocusMultiplier * (float) focusEffect.FocusCounter;
        }
        aimedEffect.ExtraHits = num3;
        aimedEffect.SuperSlowDuration = TimeSpan.FromSeconds((double) num1);
        aimedEffect.CurrentHealthDamage = damageSpecifier;
      }
    }
    if (size != RMCSizes.SmallXeno && (double) num1 > 0.0)
    {
      aimedEffect.SlowDuration = TimeSpan.FromSeconds((double) num1);
      if ((double) num1 > (double) focusEffect.SlowThreshold)
        aimedEffect.SuperSlowDuration = TimeSpan.FromSeconds((double) num1);
      if (comp1 != null && (double) comp1.CurrentStoppingPower > (double) focusEffect.DazeThreshold)
        this._dazed.TryDaze(target, TimeSpan.FromSeconds((double) focusEffect.DazeDuration));
    }
    this.Dirty(ent.Owner, (IComponent) aimedEffect);
  }
}
