// Decompiled with JetBrains decompiler
// Type: Content.Shared.EntityEffects.EffectConditions.TotalDamage
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.EntityEffects.EffectConditions;

public sealed class TotalDamage : 
  EntityEffectCondition,
  ISerializationGenerated<TotalDamage>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public FixedPoint2 Max = FixedPoint2.MaxValue;
  [DataField(null, false, 1, false, false, null)]
  public FixedPoint2 Min = FixedPoint2.Zero;

  public override bool Condition(EntityEffectBaseArgs args)
  {
    DamageableComponent component;
    if (args.EntityManager.TryGetComponent<DamageableComponent>(args.TargetEntity, out component))
    {
      FixedPoint2 totalDamage = component.TotalDamage;
      if (totalDamage > this.Min && totalDamage < this.Max)
        return true;
    }
    return false;
  }

  public override string GuidebookExplanation(IPrototypeManager prototype)
  {
    return Loc.GetString("reagent-effect-condition-guidebook-total-damage", ("max", (object) (float) (this.Max == FixedPoint2.MaxValue ? 2147483648.0 : (double) this.Max.Float())), ("min", (object) this.Min.Float()));
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref TotalDamage target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EntityEffectCondition target1 = (EntityEffectCondition) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (TotalDamage) target1;
    if (serialization.TryCustomCopy<TotalDamage>(this, ref target, hookCtx, false, context))
      return;
    FixedPoint2 target2 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.Max, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<FixedPoint2>(this.Max, hookCtx, context);
    target.Max = target2;
    FixedPoint2 target3 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.Min, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<FixedPoint2>(this.Min, hookCtx, context);
    target.Min = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref TotalDamage target,
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
    TotalDamage target1 = (TotalDamage) target;
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
    TotalDamage target1 = (TotalDamage) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual TotalDamage EntityEffectCondition.Instantiate() => new TotalDamage();
}
