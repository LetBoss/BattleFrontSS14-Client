// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Chemistry.Effects.Positive.Anticorrosive
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Damage;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.EntityEffects;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Chemistry.Effects.Positive;

public sealed class Anticorrosive : 
  RMCChemicalEffect,
  ISerializationGenerated<Anticorrosive>,
  ISerializationGenerated
{
  private static readonly ProtoId<DamageGroupPrototype> BurnGroup = (ProtoId<DamageGroupPrototype>) "Burn";
  private static readonly ProtoId<DamageTypePrototype> BluntType = (ProtoId<DamageTypePrototype>) "Blunt";
  private static readonly ProtoId<DamageTypePrototype> PoisonType = (ProtoId<DamageTypePrototype>) "Poison";

  protected override string ReagentEffectGuidebookText(
    IPrototypeManager prototype,
    IEntitySystemManager entSys)
  {
    float potencyPerSecond = this.PotencyPerSecond;
    if ((double) this.ActualPotency > 2.0)
      potencyPerSecond += this.PotencyPerSecond * 0.5f;
    return $"Heals [color=green]{potencyPerSecond}[/color] burn damage.\nOverdoses cause [color=red]{this.PotencyPerSecond}[/color] brute and [color=red]{this.PotencyPerSecond}[/color] toxin damage.\nCritical overdoses cause [color=red]{this.PotencyPerSecond * 5f}[/color] brute and [color=red]{this.PotencyPerSecond * 5f}[/color] toxin damage";
  }

  protected override void Tick(
    DamageableSystem damageable,
    FixedPoint2 potency,
    EntityEffectReagentArgs args)
  {
    SharedRMCDamageableSystem damageableSystem = args.EntityManager.System<SharedRMCDamageableSystem>();
    DamageSpecifier damage1 = damageableSystem.DistributeHealingCached((Entity<DamageableComponent>) args.TargetEntity, Anticorrosive.BurnGroup, potency);
    damageable.TryChangeDamage(new EntityUid?(args.TargetEntity), damage1, true, false);
    if ((double) this.ActualPotency <= 2.0)
      return;
    DamageSpecifier damage2 = damageableSystem.DistributeHealingCached((Entity<DamageableComponent>) args.TargetEntity, Anticorrosive.BurnGroup, potency * 0.5f);
    damageable.TryChangeDamage(new EntityUid?(args.TargetEntity), damage2, true, false);
  }

  protected override void TickOverdose(
    DamageableSystem damageable,
    FixedPoint2 potency,
    EntityEffectReagentArgs args)
  {
    damageable.TryChangeDamage(new EntityUid?(args.TargetEntity), new DamageSpecifier()
    {
      DamageDict = {
        [(string) Anticorrosive.BluntType] = potency,
        [(string) Anticorrosive.PoisonType] = potency
      }
    }, true, false);
  }

  protected override void TickCriticalOverdose(
    DamageableSystem damageable,
    FixedPoint2 potency,
    EntityEffectReagentArgs args)
  {
    damageable.TryChangeDamage(new EntityUid?(args.TargetEntity), new DamageSpecifier()
    {
      DamageDict = {
        [(string) Anticorrosive.BluntType] = potency * 5,
        [(string) Anticorrosive.PoisonType] = potency * 5
      }
    }, true, false);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref Anticorrosive target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    RMCChemicalEffect target1 = (RMCChemicalEffect) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (Anticorrosive) target1;
    serialization.TryCustomCopy<Anticorrosive>(this, ref target, hookCtx, false, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref Anticorrosive target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref RMCChemicalEffect target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Anticorrosive target1 = (Anticorrosive) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (RMCChemicalEffect) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Anticorrosive target1 = (Anticorrosive) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual Anticorrosive RMCChemicalEffect.Instantiate() => new Anticorrosive();
}
