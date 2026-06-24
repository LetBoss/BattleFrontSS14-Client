// Decompiled with JetBrains decompiler
// Type: Content.Shared.EntityEffects.Effects.ModifyBloodLevel
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

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

public sealed class ModifyBloodLevel : 
  EventEntityEffect<ModifyBloodLevel>,
  ISerializationGenerated<ModifyBloodLevel>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public bool Scaled;
  [DataField(null, false, 1, false, false, null)]
  public FixedPoint2 Amount = (FixedPoint2) 1f;

  protected override string? ReagentEffectGuidebookText(
    IPrototypeManager prototype,
    IEntitySystemManager entSys)
  {
    return Loc.GetString("reagent-effect-guidebook-modify-blood-level", ("chance", (object) this.Probability), ("deltasign", (object) MathF.Sign(this.Amount.Float())));
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ModifyBloodLevel target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EventEntityEffect<ModifyBloodLevel> target1 = (EventEntityEffect<ModifyBloodLevel>) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ModifyBloodLevel) target1;
    if (serialization.TryCustomCopy<ModifyBloodLevel>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.Scaled, ref target2, hookCtx, false, context))
      target2 = this.Scaled;
    target.Scaled = target2;
    FixedPoint2 target3 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.Amount, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<FixedPoint2>(this.Amount, hookCtx, context);
    target.Amount = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ModifyBloodLevel target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref EventEntityEffect<ModifyBloodLevel> target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ModifyBloodLevel target1 = (ModifyBloodLevel) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (EventEntityEffect<ModifyBloodLevel>) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ModifyBloodLevel target1 = (ModifyBloodLevel) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual ModifyBloodLevel EventEntityEffect<ModifyBloodLevel>.Instantiate()
  {
    return new ModifyBloodLevel();
  }
}
