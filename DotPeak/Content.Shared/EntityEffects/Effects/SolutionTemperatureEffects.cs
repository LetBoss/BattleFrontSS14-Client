// Decompiled with JetBrains decompiler
// Type: Content.Shared.EntityEffects.Effects.SetSolutionTemperatureEffect
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
public sealed class SetSolutionTemperatureEffect : 
  EntityEffect,
  ISerializationGenerated<SetSolutionTemperatureEffect>,
  ISerializationGenerated
{
  [DataField("temperature", false, 1, true, false, null)]
  private float _temperature;

  protected override string? ReagentEffectGuidebookText(
    IPrototypeManager prototype,
    IEntitySystemManager entSys)
  {
    return Loc.GetString("reagent-effect-guidebook-set-solution-temperature-effect", ("chance", (object) this.Probability), ("temperature", (object) this._temperature));
  }

  public override void Effect(EntityEffectBaseArgs args)
  {
    Solution source = (args as EntityEffectReagentArgs ?? throw new NotImplementedException()).Source;
    if (source == null)
      return;
    source.Temperature = this._temperature;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SetSolutionTemperatureEffect target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EntityEffect target1 = (EntityEffect) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (SetSolutionTemperatureEffect) target1;
    if (serialization.TryCustomCopy<SetSolutionTemperatureEffect>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this._temperature, ref target2, hookCtx, false, context))
      target2 = this._temperature;
    target._temperature = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SetSolutionTemperatureEffect target,
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
    SetSolutionTemperatureEffect target1 = (SetSolutionTemperatureEffect) target;
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
    SetSolutionTemperatureEffect target1 = (SetSolutionTemperatureEffect) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual SetSolutionTemperatureEffect EntityEffect.Instantiate()
  {
    return new SetSolutionTemperatureEffect();
  }
}
