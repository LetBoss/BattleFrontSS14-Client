// Decompiled with JetBrains decompiler
// Type: Content.Shared.EntityEffects.Effects.PlantMutateExudeGasses
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.EntityEffects.Effects;

public sealed class PlantMutateExudeGasses : 
  EventEntityEffect<PlantMutateExudeGasses>,
  ISerializationGenerated<PlantMutateExudeGasses>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public float MinValue = 0.01f;
  [DataField(null, false, 1, false, false, null)]
  public float MaxValue = 0.5f;

  protected override string? ReagentEffectGuidebookText(
    IPrototypeManager prototype,
    IEntitySystemManager entSys)
  {
    return "TODO";
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PlantMutateExudeGasses target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EventEntityEffect<PlantMutateExudeGasses> target1 = (EventEntityEffect<PlantMutateExudeGasses>) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (PlantMutateExudeGasses) target1;
    if (serialization.TryCustomCopy<PlantMutateExudeGasses>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MinValue, ref target2, hookCtx, false, context))
      target2 = this.MinValue;
    target.MinValue = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaxValue, ref target3, hookCtx, false, context))
      target3 = this.MaxValue;
    target.MaxValue = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PlantMutateExudeGasses target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref EventEntityEffect<PlantMutateExudeGasses> target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    PlantMutateExudeGasses target1 = (PlantMutateExudeGasses) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (EventEntityEffect<PlantMutateExudeGasses>) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    PlantMutateExudeGasses target1 = (PlantMutateExudeGasses) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual PlantMutateExudeGasses EventEntityEffect<PlantMutateExudeGasses>.Instantiate()
  {
    return new PlantMutateExudeGasses();
  }
}
