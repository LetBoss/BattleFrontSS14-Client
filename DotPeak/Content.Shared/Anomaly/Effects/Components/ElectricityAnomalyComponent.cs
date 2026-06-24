// Decompiled with JetBrains decompiler
// Type: Content.Shared.Anomaly.Effects.Components.ElectricityAnomalyComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Anomaly.Effects.Components;

[RegisterComponent]
public sealed class ElectricityAnomalyComponent : 
  Component,
  ISerializationGenerated<ElectricityAnomalyComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public int MinBoltCount = 2;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public int MaxBoltCount = 5;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public float MaxElectrocuteRange = 7f;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public float MaxElectrocuteDamage = 20f;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public TimeSpan MaxElectrocuteDuration = TimeSpan.FromSeconds(8L);
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public float PassiveElectrocutionChance = 0.05f;
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [Robust.Shared.ViewVariables.ViewVariables]
  public TimeSpan NextSecond = TimeSpan.Zero;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public float EmpEnergyConsumption = 100000f;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public float EmpDisabledDuration = 60f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ElectricityAnomalyComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (ElectricityAnomalyComponent) component;
    if (serialization.TryCustomCopy<ElectricityAnomalyComponent>(this, ref target, hookCtx, false, context))
      return;
    int num1 = 0;
    if (!serialization.TryCustomCopy<int>(this.MinBoltCount, ref num1, hookCtx, false, context))
      num1 = this.MinBoltCount;
    target.MinBoltCount = num1;
    int num2 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxBoltCount, ref num2, hookCtx, false, context))
      num2 = this.MaxBoltCount;
    target.MaxBoltCount = num2;
    float num3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaxElectrocuteRange, ref num3, hookCtx, false, context))
      num3 = this.MaxElectrocuteRange;
    target.MaxElectrocuteRange = num3;
    float num4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaxElectrocuteDamage, ref num4, hookCtx, false, context))
      num4 = this.MaxElectrocuteDamage;
    target.MaxElectrocuteDamage = num4;
    TimeSpan timeSpan1 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.MaxElectrocuteDuration, ref timeSpan1, hookCtx, false, context))
      timeSpan1 = serialization.CreateCopy<TimeSpan>(this.MaxElectrocuteDuration, hookCtx, context, false);
    target.MaxElectrocuteDuration = timeSpan1;
    float num5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.PassiveElectrocutionChance, ref num5, hookCtx, false, context))
      num5 = this.PassiveElectrocutionChance;
    target.PassiveElectrocutionChance = num5;
    TimeSpan timeSpan2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextSecond, ref timeSpan2, hookCtx, false, context))
      timeSpan2 = serialization.CreateCopy<TimeSpan>(this.NextSecond, hookCtx, context, false);
    target.NextSecond = timeSpan2;
    float num6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.EmpEnergyConsumption, ref num6, hookCtx, false, context))
      num6 = this.EmpEnergyConsumption;
    target.EmpEnergyConsumption = num6;
    float num7 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.EmpDisabledDuration, ref num7, hookCtx, false, context))
      num7 = this.EmpDisabledDuration;
    target.EmpDisabledDuration = num7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ElectricityAnomalyComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ElectricityAnomalyComponent target1 = (ElectricityAnomalyComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ElectricityAnomalyComponent target1 = (ElectricityAnomalyComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ElectricityAnomalyComponent target1 = (ElectricityAnomalyComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    base.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual ElectricityAnomalyComponent Component.Instantiate() => new ElectricityAnomalyComponent();
}
