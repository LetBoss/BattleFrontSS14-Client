// Decompiled with JetBrains decompiler
// Type: Content.Shared.EntityEffects.Effects.WearableReaction
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.FixedPoint;
using Content.Shared.Inventory;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.EntityEffects.Effects;

public sealed class WearableReaction : 
  EntityEffect,
  ISerializationGenerated<WearableReaction>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public float AmountThreshold = 1f;
  [DataField(null, false, 1, true, false, null)]
  public string Slot;
  [DataField(null, false, 1, true, false, null)]
  public string PrototypeID;

  protected override string? ReagentEffectGuidebookText(
    IPrototypeManager prototype,
    IEntitySystemManager entSys)
  {
    return (string) null;
  }

  public override void Effect(EntityEffectBaseArgs args)
  {
    if (!args.EntityManager.System<InventorySystem>().SpawnItemInSlot(args.TargetEntity, this.Slot, this.PrototypeID))
      return;
    EntityEffectReagentArgs effectReagentArgs = args as EntityEffectReagentArgs;
    if ((object) effectReagentArgs == null || effectReagentArgs.Reagent == null || effectReagentArgs.Quantity < (FixedPoint2) this.AmountThreshold)
      return;
    effectReagentArgs.Source?.RemoveReagent(effectReagentArgs.Reagent.ID, (FixedPoint2) this.AmountThreshold);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref WearableReaction target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EntityEffect target1 = (EntityEffect) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (WearableReaction) target1;
    if (serialization.TryCustomCopy<WearableReaction>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.AmountThreshold, ref target2, hookCtx, false, context))
      target2 = this.AmountThreshold;
    target.AmountThreshold = target2;
    string target3 = (string) null;
    if (this.Slot == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Slot, ref target3, hookCtx, false, context))
      target3 = this.Slot;
    target.Slot = target3;
    string target4 = (string) null;
    if (this.PrototypeID == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.PrototypeID, ref target4, hookCtx, false, context))
      target4 = this.PrototypeID;
    target.PrototypeID = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref WearableReaction target,
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
    WearableReaction target1 = (WearableReaction) target;
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
    WearableReaction target1 = (WearableReaction) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual WearableReaction EntityEffect.Instantiate() => new WearableReaction();
}
