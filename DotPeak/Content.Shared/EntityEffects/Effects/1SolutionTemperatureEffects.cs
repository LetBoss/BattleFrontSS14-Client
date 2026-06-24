// Decompiled with JetBrains decompiler
// Type: Content.Shared.EntityEffects.Effects.AdjustSolutionTemperatureEffect
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Chemistry.Components;
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
public sealed class AdjustSolutionTemperatureEffect : 
  EntityEffect,
  ISerializationGenerated<AdjustSolutionTemperatureEffect>,
  ISerializationGenerated
{
  [DataField("delta", false, 1, true, false, null)]
  private float _delta;
  [DataField("minTemp", false, 1, false, false, null)]
  private float _minTemp;
  [DataField("maxTemp", false, 1, false, false, null)]
  private float _maxTemp = float.PositiveInfinity;
  [DataField("scaled", false, 1, false, false, null)]
  private bool _scaled;

  protected override string? ReagentEffectGuidebookText(
    IPrototypeManager prototype,
    IEntitySystemManager entSys)
  {
    return Loc.GetString("reagent-effect-guidebook-adjust-solution-temperature-effect", ("chance", (object) this.Probability), ("deltasign", (object) MathF.Sign(this._delta)), ("mintemp", (object) this._minTemp), ("maxtemp", (object) this._maxTemp));
  }

  public override void Effect(EntityEffectBaseArgs args)
  {
    EntityEffectReagentArgs effectReagentArgs = args as EntityEffectReagentArgs;
    Solution solution = (object) effectReagentArgs != null ? effectReagentArgs.Source : throw new NotImplementedException();
    if (solution == null || solution.Volume == 0)
      return;
    float num = this._scaled ? this._delta * (float) effectReagentArgs.Quantity : this._delta;
    solution.Temperature = Math.Clamp(solution.Temperature + num, this._minTemp, this._maxTemp);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref AdjustSolutionTemperatureEffect target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EntityEffect target1 = (EntityEffect) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (AdjustSolutionTemperatureEffect) target1;
    if (serialization.TryCustomCopy<AdjustSolutionTemperatureEffect>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this._delta, ref target2, hookCtx, false, context))
      target2 = this._delta;
    target._delta = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this._minTemp, ref target3, hookCtx, false, context))
      target3 = this._minTemp;
    target._minTemp = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this._maxTemp, ref target4, hookCtx, false, context))
      target4 = this._maxTemp;
    target._maxTemp = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this._scaled, ref target5, hookCtx, false, context))
      target5 = this._scaled;
    target._scaled = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref AdjustSolutionTemperatureEffect target,
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
    AdjustSolutionTemperatureEffect target1 = (AdjustSolutionTemperatureEffect) target;
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
    AdjustSolutionTemperatureEffect target1 = (AdjustSolutionTemperatureEffect) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual AdjustSolutionTemperatureEffect EntityEffect.Instantiate()
  {
    return new AdjustSolutionTemperatureEffect();
  }
}
