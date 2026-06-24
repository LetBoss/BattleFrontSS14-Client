// Decompiled with JetBrains decompiler
// Type: Content.Shared.EntityEffects.Effects.EmpReactionEffect
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

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

[DataDefinition]
public sealed class EmpReactionEffect : 
  EventEntityEffect<EmpReactionEffect>,
  ISerializationGenerated<EmpReactionEffect>,
  ISerializationGenerated
{
  [DataField("rangePerUnit", false, 1, false, false, null)]
  public float EmpRangePerUnit = 0.5f;
  [DataField("maxRange", false, 1, false, false, null)]
  public float EmpMaxRange = 10f;
  [DataField(null, false, 1, false, false, null)]
  public float EnergyConsumption = 12500f;
  [DataField("duration", false, 1, false, false, null)]
  public float DisableDuration = 15f;

  protected override string? ReagentEffectGuidebookText(
    IPrototypeManager prototype,
    IEntitySystemManager entSys)
  {
    return Loc.GetString("reagent-effect-guidebook-emp-reaction-effect", ("chance", (object) this.Probability));
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref EmpReactionEffect target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EventEntityEffect<EmpReactionEffect> target1 = (EventEntityEffect<EmpReactionEffect>) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (EmpReactionEffect) target1;
    if (serialization.TryCustomCopy<EmpReactionEffect>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.EmpRangePerUnit, ref target2, hookCtx, false, context))
      target2 = this.EmpRangePerUnit;
    target.EmpRangePerUnit = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.EmpMaxRange, ref target3, hookCtx, false, context))
      target3 = this.EmpMaxRange;
    target.EmpMaxRange = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.EnergyConsumption, ref target4, hookCtx, false, context))
      target4 = this.EnergyConsumption;
    target.EnergyConsumption = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.DisableDuration, ref target5, hookCtx, false, context))
      target5 = this.DisableDuration;
    target.DisableDuration = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref EmpReactionEffect target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref EventEntityEffect<EmpReactionEffect> target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EmpReactionEffect target1 = (EmpReactionEffect) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (EventEntityEffect<EmpReactionEffect>) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EmpReactionEffect target1 = (EmpReactionEffect) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual EmpReactionEffect EventEntityEffect<EmpReactionEffect>.Instantiate()
  {
    return new EmpReactionEffect();
  }
}
