// Decompiled with JetBrains decompiler
// Type: Content.Shared.EntityEffects.EffectConditions.ReagentThreshold
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Chemistry.Reagent;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.EntityEffects.EffectConditions;

public sealed class ReagentThreshold : 
  EntityEffectCondition,
  ISerializationGenerated<ReagentThreshold>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public FixedPoint2 Min = FixedPoint2.Zero;
  [DataField(null, false, 1, false, false, null)]
  public FixedPoint2 Max = FixedPoint2.MaxValue;
  [DataField(null, false, 1, false, false, null)]
  public string? Reagent;

  public override bool Condition(EntityEffectBaseArgs args)
  {
    EntityEffectReagentArgs effectReagentArgs = args as EntityEffectReagentArgs;
    if ((object) effectReagentArgs == null)
      throw new NotImplementedException();
    string id = this.Reagent ?? effectReagentArgs.Reagent?.ID;
    if (id == null)
      return true;
    FixedPoint2 fixedPoint2 = FixedPoint2.Zero;
    if (effectReagentArgs.Source != null)
      fixedPoint2 = effectReagentArgs.Source.GetTotalPrototypeQuantity(id);
    return fixedPoint2 >= this.Min && fixedPoint2 <= this.Max;
  }

  public override string GuidebookExplanation(IPrototypeManager prototype)
  {
    ReagentPrototype reagentPrototype = (ReagentPrototype) null;
    Content.Shared._RMC14.Chemistry.Reagent.Reagent reagent;
    if (this.Reagent != null && IoCManager.Resolve<IEntityManager>().System<RMCReagentSystem>().TryIndex((ProtoId<ReagentPrototype>) this.Reagent, out reagent))
      reagentPrototype = (ReagentPrototype) reagent;
    return Loc.GetString("reagent-effect-condition-guidebook-reagent-threshold", ("reagent", (object) (reagentPrototype?.LocalizedName ?? Loc.GetString("reagent-effect-condition-guidebook-this-reagent"))), ("max", (object) (float) (this.Max == FixedPoint2.MaxValue ? 2147483648.0 : (double) this.Max.Float())), ("min", (object) this.Min.Float()));
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ReagentThreshold target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EntityEffectCondition target1 = (EntityEffectCondition) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ReagentThreshold) target1;
    if (serialization.TryCustomCopy<ReagentThreshold>(this, ref target, hookCtx, false, context))
      return;
    FixedPoint2 target2 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.Min, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<FixedPoint2>(this.Min, hookCtx, context);
    target.Min = target2;
    FixedPoint2 target3 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.Max, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<FixedPoint2>(this.Max, hookCtx, context);
    target.Max = target3;
    string target4 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.Reagent, ref target4, hookCtx, false, context))
      target4 = this.Reagent;
    target.Reagent = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ReagentThreshold target,
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
    ReagentThreshold target1 = (ReagentThreshold) target;
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
    ReagentThreshold target1 = (ReagentThreshold) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual ReagentThreshold EntityEffectCondition.Instantiate() => new ReagentThreshold();
}
