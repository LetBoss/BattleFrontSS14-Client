// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Chemistry.Effects.StabilizeTemperature
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Temperature;
using Content.Shared.EntityEffects;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Chemistry.Effects;

public sealed class StabilizeTemperature : 
  EntityEffect,
  ISerializationGenerated<StabilizeTemperature>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public float Stable;
  [DataField(null, false, 1, true, false, null)]
  public float Change;

  protected override string ReagentEffectGuidebookText(
    IPrototypeManager prototype,
    IEntitySystemManager entSys)
  {
    return $"Stabilizes the temperature of the body that it is in to {this.Stable} degrees, by {this.Change} degrees at a time";
  }

  public override void Effect(EntityEffectBaseArgs args)
  {
    SharedRMCTemperatureSystem entitySystem = args.EntityManager.EntitySysManager.GetEntitySystem<SharedRMCTemperatureSystem>();
    float temperature1 = entitySystem.GetTemperature(args.TargetEntity);
    if ((double) Math.Abs(temperature1 - this.Stable) < 0.01)
      return;
    float change = this.Change;
    EntityEffectReagentArgs effectReagentArgs = args as EntityEffectReagentArgs;
    if ((object) effectReagentArgs != null)
      change *= effectReagentArgs.Scale.Float();
    float temperature2 = (double) temperature1 > (double) this.Stable ? Math.Max(this.Stable, temperature1 - change) : Math.Min(this.Stable, temperature1 + change);
    entitySystem.ForceChangeTemperature(args.TargetEntity, temperature2);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref StabilizeTemperature target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EntityEffect target1 = (EntityEffect) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (StabilizeTemperature) target1;
    if (serialization.TryCustomCopy<StabilizeTemperature>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Stable, ref target2, hookCtx, false, context))
      target2 = this.Stable;
    target.Stable = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Change, ref target3, hookCtx, false, context))
      target3 = this.Change;
    target.Change = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref StabilizeTemperature target,
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
    StabilizeTemperature target1 = (StabilizeTemperature) target;
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
    StabilizeTemperature target1 = (StabilizeTemperature) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual StabilizeTemperature EntityEffect.Instantiate() => new StabilizeTemperature();
}
