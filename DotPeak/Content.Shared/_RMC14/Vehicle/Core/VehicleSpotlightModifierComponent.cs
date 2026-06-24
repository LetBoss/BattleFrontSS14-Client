// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vehicle.VehicleSpotlightModifierComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Vehicle;

[RegisterComponent]
public sealed class VehicleSpotlightModifierComponent : 
  Component,
  ISerializationGenerated<VehicleSpotlightModifierComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public float RadiusMultiplier = 1f;
  [DataField(null, false, 1, false, false, null)]
  public float RadiusAdd;
  [DataField(null, false, 1, false, false, null)]
  public float EnergyMultiplier = 1f;
  [DataField(null, false, 1, false, false, null)]
  public float EnergyAdd;
  [DataField(null, false, 1, false, false, null)]
  public float SoftnessMultiplier = 1f;
  [DataField(null, false, 1, false, false, null)]
  public float SoftnessAdd;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref VehicleSpotlightModifierComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (VehicleSpotlightModifierComponent) target1;
    if (serialization.TryCustomCopy<VehicleSpotlightModifierComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.RadiusMultiplier, ref target2, hookCtx, false, context))
      target2 = this.RadiusMultiplier;
    target.RadiusMultiplier = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.RadiusAdd, ref target3, hookCtx, false, context))
      target3 = this.RadiusAdd;
    target.RadiusAdd = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.EnergyMultiplier, ref target4, hookCtx, false, context))
      target4 = this.EnergyMultiplier;
    target.EnergyMultiplier = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.EnergyAdd, ref target5, hookCtx, false, context))
      target5 = this.EnergyAdd;
    target.EnergyAdd = target5;
    float target6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SoftnessMultiplier, ref target6, hookCtx, false, context))
      target6 = this.SoftnessMultiplier;
    target.SoftnessMultiplier = target6;
    float target7 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SoftnessAdd, ref target7, hookCtx, false, context))
      target7 = this.SoftnessAdd;
    target.SoftnessAdd = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref VehicleSpotlightModifierComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    VehicleSpotlightModifierComponent target1 = (VehicleSpotlightModifierComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    VehicleSpotlightModifierComponent target1 = (VehicleSpotlightModifierComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    VehicleSpotlightModifierComponent target1 = (VehicleSpotlightModifierComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual VehicleSpotlightModifierComponent Component.Instantiate()
  {
    return new VehicleSpotlightModifierComponent();
  }
}
