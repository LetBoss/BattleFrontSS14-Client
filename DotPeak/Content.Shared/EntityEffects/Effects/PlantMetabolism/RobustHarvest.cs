// Decompiled with JetBrains decompiler
// Type: Content.Shared.EntityEffects.Effects.PlantMetabolism.RobustHarvest
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
namespace Content.Shared.EntityEffects.Effects.PlantMetabolism;

public sealed class RobustHarvest : 
  EventEntityEffect<RobustHarvest>,
  ISerializationGenerated<RobustHarvest>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public int PotencyLimit = 50;
  [DataField(null, false, 1, false, false, null)]
  public int PotencyIncrease = 3;
  [DataField(null, false, 1, false, false, null)]
  public int PotencySeedlessThreshold = 30;

  protected override string? ReagentEffectGuidebookText(
    IPrototypeManager prototype,
    IEntitySystemManager entSys)
  {
    return Loc.GetString("reagent-effect-guidebook-plant-robust-harvest", ("seedlesstreshold", (object) this.PotencySeedlessThreshold), ("limit", (object) this.PotencyLimit), ("increase", (object) this.PotencyIncrease), ("chance", (object) this.Probability));
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RobustHarvest target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EventEntityEffect<RobustHarvest> target1 = (EventEntityEffect<RobustHarvest>) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RobustHarvest) target1;
    if (serialization.TryCustomCopy<RobustHarvest>(this, ref target, hookCtx, false, context))
      return;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.PotencyLimit, ref target2, hookCtx, false, context))
      target2 = this.PotencyLimit;
    target.PotencyLimit = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.PotencyIncrease, ref target3, hookCtx, false, context))
      target3 = this.PotencyIncrease;
    target.PotencyIncrease = target3;
    int target4 = 0;
    if (!serialization.TryCustomCopy<int>(this.PotencySeedlessThreshold, ref target4, hookCtx, false, context))
      target4 = this.PotencySeedlessThreshold;
    target.PotencySeedlessThreshold = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RobustHarvest target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref EventEntityEffect<RobustHarvest> target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    RobustHarvest target1 = (RobustHarvest) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (EventEntityEffect<RobustHarvest>) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    RobustHarvest target1 = (RobustHarvest) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual RobustHarvest EventEntityEffect<RobustHarvest>.Instantiate() => new RobustHarvest();
}
