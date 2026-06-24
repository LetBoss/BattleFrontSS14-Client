// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vehicle.VehicleDeployGatedHardpointsComponent
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
[Access(new Type[] {typeof (VehicleDeploySystem)})]
public sealed class VehicleDeployGatedHardpointsComponent : 
  Component,
  ISerializationGenerated<VehicleDeployGatedHardpointsComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public List<string> BlockedHardpoints = new List<string>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref VehicleDeployGatedHardpointsComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (VehicleDeployGatedHardpointsComponent) target1;
    if (serialization.TryCustomCopy<VehicleDeployGatedHardpointsComponent>(this, ref target, hookCtx, false, context))
      return;
    List<string> target2 = (List<string>) null;
    if (this.BlockedHardpoints == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<string>>(this.BlockedHardpoints, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<List<string>>(this.BlockedHardpoints, hookCtx, context);
    target.BlockedHardpoints = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref VehicleDeployGatedHardpointsComponent target,
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
    VehicleDeployGatedHardpointsComponent target1 = (VehicleDeployGatedHardpointsComponent) target;
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
    VehicleDeployGatedHardpointsComponent target1 = (VehicleDeployGatedHardpointsComponent) target;
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
    VehicleDeployGatedHardpointsComponent target1 = (VehicleDeployGatedHardpointsComponent) target;
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
  virtual VehicleDeployGatedHardpointsComponent Component.Instantiate()
  {
    return new VehicleDeployGatedHardpointsComponent();
  }
}
