// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vehicle.VehicleEnterComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Vehicle;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (VehicleSystem)})]
public sealed class VehicleEnterComponent : 
  Component,
  ISerializationGenerated<VehicleEnterComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public ResPath InteriorPath;
  [DataField(null, false, 1, false, false, null)]
  public int MaxPassengers;
  [DataField(null, false, 1, false, false, null)]
  public int MaxXenos;
  [DataField(null, false, 1, false, false, null)]
  public List<VehicleEntryPoint> EntryPoints = new List<VehicleEntryPoint>();
  [DataField(null, false, 1, false, false, null)]
  public float EnterDoAfter;
  [DataField(null, false, 1, false, false, null)]
  public float ExitDoAfter;
  [DataField(null, false, 1, false, false, null)]
  public Vector2 ExitOffset = Vector2.Zero;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref VehicleEnterComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (VehicleEnterComponent) target1;
    if (serialization.TryCustomCopy<VehicleEnterComponent>(this, ref target, hookCtx, false, context))
      return;
    ResPath target2 = new ResPath();
    if (!serialization.TryCustomCopy<ResPath>(this.InteriorPath, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<ResPath>(this.InteriorPath, hookCtx, context);
    target.InteriorPath = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxPassengers, ref target3, hookCtx, false, context))
      target3 = this.MaxPassengers;
    target.MaxPassengers = target3;
    int target4 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxXenos, ref target4, hookCtx, false, context))
      target4 = this.MaxXenos;
    target.MaxXenos = target4;
    List<VehicleEntryPoint> target5 = (List<VehicleEntryPoint>) null;
    if (this.EntryPoints == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<VehicleEntryPoint>>(this.EntryPoints, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<List<VehicleEntryPoint>>(this.EntryPoints, hookCtx, context);
    target.EntryPoints = target5;
    float target6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.EnterDoAfter, ref target6, hookCtx, false, context))
      target6 = this.EnterDoAfter;
    target.EnterDoAfter = target6;
    float target7 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ExitDoAfter, ref target7, hookCtx, false, context))
      target7 = this.ExitDoAfter;
    target.ExitDoAfter = target7;
    Vector2 target8 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.ExitOffset, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<Vector2>(this.ExitOffset, hookCtx, context);
    target.ExitOffset = target8;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref VehicleEnterComponent target,
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
    VehicleEnterComponent target1 = (VehicleEnterComponent) target;
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
    VehicleEnterComponent target1 = (VehicleEnterComponent) target;
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
    VehicleEnterComponent target1 = (VehicleEnterComponent) target;
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
  virtual VehicleEnterComponent Component.Instantiate() => new VehicleEnterComponent();
}
