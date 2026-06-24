// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vehicle.RMCVehicleDamageProfileComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Vehicle;

[RegisterComponent]
[Access(new Type[] {typeof (RMCVehicleDamageProfileSystem)})]
public sealed class RMCVehicleDamageProfileComponent : 
  Component,
  ISerializationGenerated<RMCVehicleDamageProfileComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public List<RMCVehicleDamageScaleRule> Rules = new List<RMCVehicleDamageScaleRule>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCVehicleDamageProfileComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCVehicleDamageProfileComponent) target1;
    if (serialization.TryCustomCopy<RMCVehicleDamageProfileComponent>(this, ref target, hookCtx, false, context))
      return;
    List<RMCVehicleDamageScaleRule> target2 = (List<RMCVehicleDamageScaleRule>) null;
    if (this.Rules == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<RMCVehicleDamageScaleRule>>(this.Rules, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<List<RMCVehicleDamageScaleRule>>(this.Rules, hookCtx, context);
    target.Rules = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCVehicleDamageProfileComponent target,
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
    RMCVehicleDamageProfileComponent target1 = (RMCVehicleDamageProfileComponent) target;
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
    RMCVehicleDamageProfileComponent target1 = (RMCVehicleDamageProfileComponent) target;
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
    RMCVehicleDamageProfileComponent target1 = (RMCVehicleDamageProfileComponent) target;
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
  virtual RMCVehicleDamageProfileComponent Component.Instantiate()
  {
    return new RMCVehicleDamageProfileComponent();
  }
}
