// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Chemistry.Effects.RMCChemicalEffect
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Body.Prototypes;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Damage;
using Content.Shared.EntityEffects;
using Content.Shared.FixedPoint;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Chemistry.Effects;

public abstract class RMCChemicalEffect : 
  EntityEffect,
  ISerializationGenerated<RMCChemicalEffect>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public float Potency;
  private float _moddedPotency;
  [DataField(null, false, 1, false, false, null)]
  public float NutFactor;
  [DataField(null, false, 1, false, false, null)]
  public float NutMetabolism;

  public float ActualPotency
  {
    get
    {
      return (float) (((double) this._moddedPotency != 0.0 ? (double) this._moddedPotency : (double) this.Potency) * 0.5);
    }
  }

  public float PotencyPerSecond => this.ActualPotency * 0.5f;

  public float NutrimentFactor => this.NutFactor * this.NutMetabolism;

  public override void Effect(EntityEffectBaseArgs args)
  {
    EntityEffectReagentArgs args1 = args as EntityEffectReagentArgs;
    if ((object) args1 == null)
      return;
    ReagentPrototype reagent = args1.Reagent;
    if (reagent == null)
      return;
    DamageableSystem damageable = args.EntityManager.System<DamageableSystem>();
    FixedPoint2 scale = args1.Scale;
    this._moddedPotency = this.Potency + RMCChemicalEffect.CalculateReagentBoost(args1);
    FixedPoint2 potency = (FixedPoint2) this.PotencyPerSecond * scale;
    this.Tick(damageable, potency, args1);
    FixedPoint2 fixedPoint2_1 = FixedPoint2.Zero;
    if (args1.Source != null)
      fixedPoint2_1 = args1.Source.GetTotalPrototypeQuantity(reagent.ID);
    if (reagent.Overdose.HasValue)
    {
      FixedPoint2 fixedPoint2_2 = fixedPoint2_1;
      FixedPoint2? overdose = reagent.Overdose;
      if ((overdose.HasValue ? (fixedPoint2_2 >= overdose.GetValueOrDefault() ? 1 : 0) : 0) != 0)
        this.TickOverdose(damageable, potency, args1);
    }
    if (!reagent.CriticalOverdose.HasValue)
      return;
    FixedPoint2 fixedPoint2_3 = fixedPoint2_1;
    FixedPoint2? criticalOverdose = reagent.CriticalOverdose;
    if ((criticalOverdose.HasValue ? (fixedPoint2_3 >= criticalOverdose.GetValueOrDefault() ? 1 : 0) : 0) == 0)
      return;
    this.TickCriticalOverdose(damageable, potency, args1);
  }

  private static float CalculateReagentBoost(EntityEffectReagentArgs args)
  {
    float boost = 0.0f;
    if (args.Reagent?.Metabolisms == null)
      return boost;
    foreach ((ProtoId<MetabolismGroupPrototype> _, ReagentEffectsEntry reagentEffectsEntry) in args.Reagent.Metabolisms)
    {
      foreach (EntityEffect effect in reagentEffectsEntry.Effects)
      {
        if (effect is RMCChemicalEffect rmcChemicalEffect)
          rmcChemicalEffect.ReagentBoost(args, ref boost);
      }
    }
    return boost;
  }

  protected virtual void ReagentBoost(EntityEffectReagentArgs args, ref float boost)
  {
  }

  protected virtual void Tick(
    DamageableSystem damageable,
    FixedPoint2 potency,
    EntityEffectReagentArgs args)
  {
  }

  protected virtual void TickOverdose(
    DamageableSystem damageable,
    FixedPoint2 potency,
    EntityEffectReagentArgs args)
  {
  }

  protected virtual void TickCriticalOverdose(
    DamageableSystem damageable,
    FixedPoint2 potency,
    EntityEffectReagentArgs args)
  {
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref RMCChemicalEffect target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EntityEffect target1 = (EntityEffect) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCChemicalEffect) target1;
    if (serialization.TryCustomCopy<RMCChemicalEffect>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Potency, ref target2, hookCtx, false, context))
      target2 = this.Potency;
    target.Potency = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.NutFactor, ref target3, hookCtx, false, context))
      target3 = this.NutFactor;
    target.NutFactor = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.NutMetabolism, ref target4, hookCtx, false, context))
      target4 = this.NutMetabolism;
    target.NutMetabolism = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref RMCChemicalEffect target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref EntityEffect target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    RMCChemicalEffect target1 = (RMCChemicalEffect) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (EntityEffect) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    RMCChemicalEffect target1 = (RMCChemicalEffect) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual RMCChemicalEffect EntityEffect.Instantiate() => throw new NotImplementedException();
}
