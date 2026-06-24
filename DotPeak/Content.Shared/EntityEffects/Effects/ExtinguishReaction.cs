// Decompiled with JetBrains decompiler
// Type: Content.Shared.EntityEffects.Effects.ExtinguishReaction
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Atmos;
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

public sealed class ExtinguishReaction : 
  EntityEffect,
  ISerializationGenerated<ExtinguishReaction>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public float FireStacksAdjustment = -1.5f;

  protected override string? ReagentEffectGuidebookText(
    IPrototypeManager prototype,
    IEntitySystemManager entSys)
  {
    return Loc.GetString("reagent-effect-guidebook-extinguish-reaction", ("chance", (object) this.Probability));
  }

  public override void Effect(EntityEffectBaseArgs args)
  {
    ExtinguishEvent args1 = new ExtinguishEvent()
    {
      FireStacksAdjustment = this.FireStacksAdjustment
    };
    EntityEffectReagentArgs effectReagentArgs = args as EntityEffectReagentArgs;
    if ((object) effectReagentArgs != null)
      args1.FireStacksAdjustment *= (float) effectReagentArgs.Quantity;
    args.EntityManager.EventBus.RaiseLocalEvent<ExtinguishEvent>(args.TargetEntity, ref args1);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ExtinguishReaction target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EntityEffect target1 = (EntityEffect) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ExtinguishReaction) target1;
    if (serialization.TryCustomCopy<ExtinguishReaction>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.FireStacksAdjustment, ref target2, hookCtx, false, context))
      target2 = this.FireStacksAdjustment;
    target.FireStacksAdjustment = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ExtinguishReaction target,
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
    ExtinguishReaction target1 = (ExtinguishReaction) target;
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
    ExtinguishReaction target1 = (ExtinguishReaction) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual ExtinguishReaction EntityEffect.Instantiate() => new ExtinguishReaction();
}
