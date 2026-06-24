// Decompiled with JetBrains decompiler
// Type: Content.Shared.Construction.Steps.TemperatureConstructionGraphStep
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Examine;
using Robust.Shared.Localization;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Construction.Steps;

[DataDefinition]
public sealed class TemperatureConstructionGraphStep : 
  ConstructionGraphStep,
  ISerializationGenerated<TemperatureConstructionGraphStep>,
  ISerializationGenerated
{
  [DataField("minTemperature", false, 1, false, false, null)]
  public float? MinTemperature;
  [DataField("maxTemperature", false, 1, false, false, null)]
  public float? MaxTemperature;

  public override void DoExamine(ExaminedEvent examinedEvent)
  {
    float num = this.MinTemperature.HasValue ? this.MinTemperature.Value : (this.MaxTemperature.HasValue ? this.MaxTemperature.Value : 0.0f);
    examinedEvent.PushMarkup(Loc.GetString("construction-temperature-default", new (string, object)[1]
    {
      ("temperature", (object) num)
    }));
  }

  public override ConstructionGuideEntry GenerateGuideEntry()
  {
    float num = this.MinTemperature.HasValue ? this.MinTemperature.Value : (this.MaxTemperature.HasValue ? this.MaxTemperature.Value : 0.0f);
    return new ConstructionGuideEntry()
    {
      Localization = "construction-presenter-temperature-step",
      Arguments = new (string, object)[1]
      {
        ("temperature", (object) num)
      }
    };
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref TemperatureConstructionGraphStep target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ConstructionGraphStep target1 = (ConstructionGraphStep) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (TemperatureConstructionGraphStep) target1;
    if (serialization.TryCustomCopy<TemperatureConstructionGraphStep>(this, ref target, hookCtx, false, context))
      return;
    float? nullable1 = new float?();
    if (!serialization.TryCustomCopy<float?>(this.MinTemperature, ref nullable1, hookCtx, false, context))
      nullable1 = this.MinTemperature;
    target.MinTemperature = nullable1;
    float? nullable2 = new float?();
    if (!serialization.TryCustomCopy<float?>(this.MaxTemperature, ref nullable2, hookCtx, false, context))
      nullable2 = this.MaxTemperature;
    target.MaxTemperature = nullable2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref TemperatureConstructionGraphStep target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref ConstructionGraphStep target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    TemperatureConstructionGraphStep target1 = (TemperatureConstructionGraphStep) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (ConstructionGraphStep) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    TemperatureConstructionGraphStep target1 = (TemperatureConstructionGraphStep) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual TemperatureConstructionGraphStep ConstructionGraphStep.Instantiate()
  {
    return new TemperatureConstructionGraphStep();
  }
}
