// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Chemistry.Effects.Neutral.Ketogenic
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Body;
using Content.Shared._RMC14.Stun;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.Drunk;
using Content.Shared.EntityEffects;
using Content.Shared.FixedPoint;
using Content.Shared.Nutrition.EntitySystems;
using Content.Shared.StatusEffect;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Chemistry.Effects.Neutral;

public sealed class Ketogenic : 
  RMCChemicalEffect,
  ISerializationGenerated<Ketogenic>,
  ISerializationGenerated
{
  private static readonly ProtoId<DamageTypePrototype> PoisonType = (ProtoId<DamageTypePrototype>) "Poison";
  private static readonly ProtoId<StatusEffectPrototype> Unconscious = (ProtoId<StatusEffectPrototype>) nameof (Unconscious);

  protected override string ReagentEffectGuidebookText(
    IPrototypeManager prototype,
    IEntitySystemManager entSys)
  {
    return $"Removes [color=red]{this.PotencyPerSecond * 5f}[/color] nutrients, causing hunger over time.\nIncreases alcohol metabolism rate by [color=green]{this.PotencyPerSecond}[/color] units.\nOverdoses cause [color=red]{this.PotencyPerSecond * 5f}[/color] nutrition loss, [color=red]{this.PotencyPerSecond}[/color] toxin damage, and a [color=red]{(double) this.ActualPotency * 2.5}%[/color] chance of vomiting.\nCritical overdoses will knock you unconscious for [color=red]10[/color] seconds";
  }

  protected override void Tick(
    DamageableSystem damageable,
    FixedPoint2 potency,
    EntityEffectReagentArgs args)
  {
    IEntityManager entityManager = args.EntityManager;
    EntityUid targetEntity = args.TargetEntity;
    entityManager.System<HungerSystem>().ModifyHunger(targetEntity, (float) (-(double) this.PotencyPerSecond * 5.0));
    if (!args.EntityManager.System<SharedRMCBloodstreamSystem>().RemoveBloodstreamAlcohols(args.TargetEntity, potency))
      return;
    args.EntityManager.System<SharedDrunkSystem>().TryApplyDrunkenness(args.TargetEntity, this.PotencyPerSecond * 5f);
  }

  protected override void TickOverdose(
    DamageableSystem damageable,
    FixedPoint2 potency,
    EntityEffectReagentArgs args)
  {
    IEntityManager entityManager = args.EntityManager;
    EntityUid targetEntity = args.TargetEntity;
    entityManager.System<HungerSystem>().ModifyHunger(targetEntity, (float) (-(double) this.PotencyPerSecond * 5.0));
    damageable.TryChangeDamage(new EntityUid?(targetEntity), new DamageSpecifier()
    {
      DamageDict = {
        [(string) Ketogenic.PoisonType] = potency
      }
    }, true, false);
    if (!IoCManager.Resolve<IRobustRandom>().Prob(0.025f * this.ActualPotency))
      return;
    RMCVomitEvent toRaise = new RMCVomitEvent(targetEntity);
    entityManager.EventBus.RaiseEvent<RMCVomitEvent>(EventSource.Local, ref toRaise);
  }

  protected override void TickCriticalOverdose(
    DamageableSystem damageable,
    FixedPoint2 potency,
    EntityEffectReagentArgs args)
  {
    args.EntityManager.System<StatusEffectsSystem>().TryAddStatusEffect<RMCUnconsciousComponent>(args.TargetEntity, (string) Ketogenic.Unconscious, TimeSpan.FromSeconds(10L), false);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref Ketogenic target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    RMCChemicalEffect target1 = (RMCChemicalEffect) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (Ketogenic) target1;
    serialization.TryCustomCopy<Ketogenic>(this, ref target, hookCtx, false, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref Ketogenic target,
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
    Ketogenic target1 = (Ketogenic) target;
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
    Ketogenic target1 = (Ketogenic) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual Ketogenic RMCChemicalEffect.Instantiate() => new Ketogenic();
}
