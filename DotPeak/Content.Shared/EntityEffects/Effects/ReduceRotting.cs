// Decompiled with JetBrains decompiler
// Type: Content.Shared.EntityEffects.Effects.ReduceRotting
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Atmos.Rotting;
using Content.Shared.FixedPoint;
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

public sealed class ReduceRotting : 
  EntityEffect,
  ISerializationGenerated<ReduceRotting>,
  ISerializationGenerated
{
  [DataField("seconds", false, 1, false, false, null)]
  public double RottingAmount = 10.0;

  protected override string? ReagentEffectGuidebookText(
    IPrototypeManager prototype,
    IEntitySystemManager entSys)
  {
    return Loc.GetString("reagent-effect-guidebook-reduce-rotting", ("chance", (object) this.Probability), ("time", (object) this.RottingAmount));
  }

  public override void Effect(EntityEffectBaseArgs args)
  {
    EntityEffectReagentArgs effectReagentArgs = args as EntityEffectReagentArgs;
    if ((object) effectReagentArgs != null && effectReagentArgs.Scale != (FixedPoint2) 1f)
      return;
    args.EntityManager.EntitySysManager.GetEntitySystem<SharedRottingSystem>().ReduceAccumulator(args.TargetEntity, TimeSpan.FromSeconds(this.RottingAmount));
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ReduceRotting target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EntityEffect target1 = (EntityEffect) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ReduceRotting) target1;
    if (serialization.TryCustomCopy<ReduceRotting>(this, ref target, hookCtx, false, context))
      return;
    double target2 = 0.0;
    if (!serialization.TryCustomCopy<double>(this.RottingAmount, ref target2, hookCtx, false, context))
      target2 = this.RottingAmount;
    target.RottingAmount = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ReduceRotting target,
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
    ReduceRotting target1 = (ReduceRotting) target;
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
    ReduceRotting target1 = (ReduceRotting) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual ReduceRotting EntityEffect.Instantiate() => new ReduceRotting();
}
