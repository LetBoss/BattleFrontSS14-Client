// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Chemistry.Effects.Positive.Hemogenic
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Body;
using Content.Shared.Body.Components;
using Content.Shared.Body.Systems;
using Content.Shared.Chemistry.Components;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.EntityEffects;
using Content.Shared.FixedPoint;
using Content.Shared.Nutrition.Components;
using Content.Shared.Nutrition.EntitySystems;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Chemistry.Effects.Positive;

public sealed class Hemogenic : 
  RMCChemicalEffect,
  ISerializationGenerated<Hemogenic>,
  ISerializationGenerated
{
  private static readonly ProtoId<DamageTypePrototype> BluntType = (ProtoId<DamageTypePrototype>) "Blunt";
  private static readonly ProtoId<DamageTypePrototype> PoisonType = (ProtoId<DamageTypePrototype>) "Poison";
  private static readonly ProtoId<DamageTypePrototype> AsphyxiationType = (ProtoId<DamageTypePrototype>) "Asphyxiation";

  protected override string ReagentEffectGuidebookText(
    IPrototypeManager prototype,
    IEntitySystemManager entSys)
  {
    string str = $"Restores [color=green]{this.PotencyPerSecond}[/color]cl of blood while not hungry.\nCauses [color=red]{this.PotencyPerSecond}[/color] nutrient loss per second.\nOverdoses cause [color=red]{this.PotencyPerSecond}[/color] toxin damage.\nCritical overdoses cause [color=red]{this.PotencyPerSecond * 5f}[/color] additional nutrient loss";
    if ((double) this.ActualPotency <= 3.0)
      return str;
    return $"Deals [color=red]{this.PotencyPerSecond}[/color] brute, [color=red]{this.PotencyPerSecond * 2f}[/color] airloss damage, and slows you down.\n{str}";
  }

  protected override void Tick(
    DamageableSystem damageable,
    FixedPoint2 potency,
    EntityEffectReagentArgs args)
  {
    IEntityManager entityManager = args.EntityManager;
    EntityUid targetEntity = args.TargetEntity;
    HungerSystem hungerSystem = entityManager.System<HungerSystem>();
    HungerComponent component1;
    if (!entityManager.TryGetComponent<HungerComponent>(targetEntity, out component1) || (double) hungerSystem.GetHunger(component1) < 200.0)
      return;
    hungerSystem.ModifyHunger(targetEntity, -this.PotencyPerSecond);
    BloodstreamComponent component2;
    if (entityManager.TryGetComponent<BloodstreamComponent>(targetEntity, out component2))
      entityManager.System<SharedBloodstreamSystem>().TryModifyBloodLevel((Entity<BloodstreamComponent>) (targetEntity, component2), potency);
    SharedRMCBloodstreamSystem bloodstreamSystem = entityManager.System<SharedRMCBloodstreamSystem>();
    Solution solution;
    if (((double) this.ActualPotency <= 3.0 || !bloodstreamSystem.TryGetBloodSolution(targetEntity, out solution) ? 0 : (solution.Volume > 570 ? 1 : 0)) == 0)
      return;
    damageable.TryChangeDamage(new EntityUid?(args.TargetEntity), new DamageSpecifier()
    {
      DamageDict = {
        [(string) Hemogenic.BluntType] = potency,
        [(string) Hemogenic.AsphyxiationType] = potency * 2
      }
    }, true, false);
  }

  protected override void TickOverdose(
    DamageableSystem damageable,
    FixedPoint2 potency,
    EntityEffectReagentArgs args)
  {
    damageable.TryChangeDamage(new EntityUid?(args.TargetEntity), new DamageSpecifier()
    {
      DamageDict = {
        [(string) Hemogenic.PoisonType] = potency
      }
    }, true, false);
  }

  protected override void TickCriticalOverdose(
    DamageableSystem damageable,
    FixedPoint2 potency,
    EntityEffectReagentArgs args)
  {
    IEntityManager entityManager = args.EntityManager;
    EntityUid targetEntity = args.TargetEntity;
    entityManager.System<HungerSystem>().ModifyHunger(targetEntity, (float) (-(double) this.PotencyPerSecond * 5.0));
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref Hemogenic target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    RMCChemicalEffect target1 = (RMCChemicalEffect) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (Hemogenic) target1;
    serialization.TryCustomCopy<Hemogenic>(this, ref target, hookCtx, false, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref Hemogenic target,
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
    Hemogenic target1 = (Hemogenic) target;
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
    Hemogenic target1 = (Hemogenic) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual Hemogenic RMCChemicalEffect.Instantiate() => new Hemogenic();
}
