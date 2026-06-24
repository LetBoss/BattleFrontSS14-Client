// Decompiled with JetBrains decompiler
// Type: Content.Shared.EntityEffects.Effects.PlantMetabolism.PlantChangeStat
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.EntityEffects.Effects.PlantMetabolism;

public sealed class PlantChangeStat : 
  EventEntityEffect<PlantChangeStat>,
  ISerializationGenerated<PlantChangeStat>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public string TargetValue;
  [DataField(null, false, 1, false, false, null)]
  public float MinValue;
  [DataField(null, false, 1, false, false, null)]
  public float MaxValue;
  [DataField(null, false, 1, false, false, null)]
  public int Steps;

  protected override string? ReagentEffectGuidebookText(
    IPrototypeManager prototype,
    IEntitySystemManager entSys)
  {
    throw new NotImplementedException();
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PlantChangeStat target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EventEntityEffect<PlantChangeStat> target1 = (EventEntityEffect<PlantChangeStat>) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (PlantChangeStat) target1;
    if (serialization.TryCustomCopy<PlantChangeStat>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.TargetValue == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.TargetValue, ref target2, hookCtx, false, context))
      target2 = this.TargetValue;
    target.TargetValue = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MinValue, ref target3, hookCtx, false, context))
      target3 = this.MinValue;
    target.MinValue = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaxValue, ref target4, hookCtx, false, context))
      target4 = this.MaxValue;
    target.MaxValue = target4;
    int target5 = 0;
    if (!serialization.TryCustomCopy<int>(this.Steps, ref target5, hookCtx, false, context))
      target5 = this.Steps;
    target.Steps = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PlantChangeStat target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref EventEntityEffect<PlantChangeStat> target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    PlantChangeStat target1 = (PlantChangeStat) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (EventEntityEffect<PlantChangeStat>) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    PlantChangeStat target1 = (PlantChangeStat) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual PlantChangeStat EventEntityEffect<PlantChangeStat>.Instantiate() => new PlantChangeStat();
}
