// Decompiled with JetBrains decompiler
// Type: Content.Shared.EntityEffects.EffectConditions.Hunger
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Nutrition.Components;
using Content.Shared.Nutrition.EntitySystems;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.EntityEffects.EffectConditions;

public sealed class Hunger : 
  EntityEffectCondition,
  ISerializationGenerated<Hunger>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public float Max = float.PositiveInfinity;
  [DataField(null, false, 1, false, false, null)]
  public float Min;

  public override bool Condition(EntityEffectBaseArgs args)
  {
    HungerComponent component;
    if (args.EntityManager.TryGetComponent<HungerComponent>(args.TargetEntity, out component))
    {
      float hunger = args.EntityManager.System<HungerSystem>().GetHunger(component);
      if ((double) hunger > (double) this.Min && (double) hunger < (double) this.Max)
        return true;
    }
    return false;
  }

  public override string GuidebookExplanation(IPrototypeManager prototype)
  {
    return Loc.GetString("reagent-effect-condition-guidebook-total-hunger", ("max", (object) (float) (float.IsPositiveInfinity(this.Max) ? 2147483648.0 : (double) this.Max)), ("min", (object) this.Min));
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref Hunger target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EntityEffectCondition target1 = (EntityEffectCondition) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (Hunger) target1;
    if (serialization.TryCustomCopy<Hunger>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Max, ref target2, hookCtx, false, context))
      target2 = this.Max;
    target.Max = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Min, ref target3, hookCtx, false, context))
      target3 = this.Min;
    target.Min = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref Hunger target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref EntityEffectCondition target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Hunger target1 = (Hunger) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (EntityEffectCondition) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Hunger target1 = (Hunger) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual Hunger EntityEffectCondition.Instantiate() => new Hunger();
}
