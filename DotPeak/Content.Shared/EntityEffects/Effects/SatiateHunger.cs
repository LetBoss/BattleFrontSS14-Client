// Decompiled with JetBrains decompiler
// Type: Content.Shared.EntityEffects.Effects.SatiateHunger
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Nutrition.Components;
using Content.Shared.Nutrition.EntitySystems;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.EntityEffects.Effects;

public sealed class SatiateHunger : 
  EntityEffect,
  ISerializationGenerated<SatiateHunger>,
  ISerializationGenerated
{
  private const float DefaultNutritionFactor = 3f;

  [DataField("factor", false, 1, false, false, null)]
  public float NutritionFactor { get; set; } = 3f;

  public override void Effect(EntityEffectBaseArgs args)
  {
    IEntityManager entityManager = args.EntityManager;
    HungerComponent component;
    if (!entityManager.TryGetComponent<HungerComponent>(args.TargetEntity, out component))
      return;
    EntityEffectReagentArgs effectReagentArgs = args as EntityEffectReagentArgs;
    if ((object) effectReagentArgs != null)
      entityManager.System<HungerSystem>().ModifyHunger(effectReagentArgs.TargetEntity, this.NutritionFactor * (float) effectReagentArgs.Quantity, component);
    else
      entityManager.System<HungerSystem>().ModifyHunger(args.TargetEntity, this.NutritionFactor, component);
  }

  protected override string? ReagentEffectGuidebookText(
    IPrototypeManager prototype,
    IEntitySystemManager entSys)
  {
    return Loc.GetString("reagent-effect-guidebook-satiate-hunger", ("chance", (object) this.Probability), ("relative", (object) (float) ((double) this.NutritionFactor / 3.0)));
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SatiateHunger target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EntityEffect target1 = (EntityEffect) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (SatiateHunger) target1;
    if (serialization.TryCustomCopy<SatiateHunger>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.NutritionFactor, ref target2, hookCtx, false, context))
      target2 = this.NutritionFactor;
    target.NutritionFactor = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SatiateHunger target,
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
    SatiateHunger target1 = (SatiateHunger) target;
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
    SatiateHunger target1 = (SatiateHunger) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual SatiateHunger EntityEffect.Instantiate() => new SatiateHunger();
}
