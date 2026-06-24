// Decompiled with JetBrains decompiler
// Type: Content.Shared.Anomaly.Effects.Components.GravityAnomalyComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Anomaly.Effects.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (SharedGravityAnomalySystem)})]
public sealed class GravityAnomalyComponent : 
  Component,
  ISerializationGenerated<GravityAnomalyComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public float MaxGravityWellRange = 10f;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public float MaxThrowRange = 5f;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public float MaxThrowStrength = 10f;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public float MaxRadiationIntensity = 3f;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public float MinAccel;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public float MaxAccel = 5f;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public float MinRadialAccel;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public float MaxRadialAccel = 5f;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public float MinSpeed = 0.1f;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public float MaxSpeed = 1f;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public float SpeedVariation = 0.1f;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public float SpaceRange = 3f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref GravityAnomalyComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (GravityAnomalyComponent) component;
    if (serialization.TryCustomCopy<GravityAnomalyComponent>(this, ref target, hookCtx, false, context))
      return;
    float num1 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaxGravityWellRange, ref num1, hookCtx, false, context))
      num1 = this.MaxGravityWellRange;
    target.MaxGravityWellRange = num1;
    float num2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaxThrowRange, ref num2, hookCtx, false, context))
      num2 = this.MaxThrowRange;
    target.MaxThrowRange = num2;
    float num3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaxThrowStrength, ref num3, hookCtx, false, context))
      num3 = this.MaxThrowStrength;
    target.MaxThrowStrength = num3;
    float num4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaxRadiationIntensity, ref num4, hookCtx, false, context))
      num4 = this.MaxRadiationIntensity;
    target.MaxRadiationIntensity = num4;
    float num5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MinAccel, ref num5, hookCtx, false, context))
      num5 = this.MinAccel;
    target.MinAccel = num5;
    float num6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaxAccel, ref num6, hookCtx, false, context))
      num6 = this.MaxAccel;
    target.MaxAccel = num6;
    float num7 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MinRadialAccel, ref num7, hookCtx, false, context))
      num7 = this.MinRadialAccel;
    target.MinRadialAccel = num7;
    float num8 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaxRadialAccel, ref num8, hookCtx, false, context))
      num8 = this.MaxRadialAccel;
    target.MaxRadialAccel = num8;
    float num9 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MinSpeed, ref num9, hookCtx, false, context))
      num9 = this.MinSpeed;
    target.MinSpeed = num9;
    float num10 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaxSpeed, ref num10, hookCtx, false, context))
      num10 = this.MaxSpeed;
    target.MaxSpeed = num10;
    float num11 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SpeedVariation, ref num11, hookCtx, false, context))
      num11 = this.SpeedVariation;
    target.SpeedVariation = num11;
    float num12 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SpaceRange, ref num12, hookCtx, false, context))
      num12 = this.SpaceRange;
    target.SpaceRange = num12;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref GravityAnomalyComponent target,
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
    GravityAnomalyComponent target1 = (GravityAnomalyComponent) target;
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
    GravityAnomalyComponent target1 = (GravityAnomalyComponent) target;
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
    GravityAnomalyComponent target1 = (GravityAnomalyComponent) target;
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
  virtual GravityAnomalyComponent Component.Instantiate() => new GravityAnomalyComponent();
}
