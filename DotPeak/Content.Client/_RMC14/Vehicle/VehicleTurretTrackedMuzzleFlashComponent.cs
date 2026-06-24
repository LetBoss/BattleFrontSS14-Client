// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Vehicle.VehicleTurretTrackedMuzzleFlashComponent
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using System;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Client._RMC14.Vehicle;

[RegisterComponent]
public sealed class VehicleTurretTrackedMuzzleFlashComponent : 
  Component,
  ISerializationGenerated<VehicleTurretTrackedMuzzleFlashComponent>,
  ISerializationGenerated
{
  public EntityUid Weapon;
  public Vector2 Offset = Vector2.Zero;
  public Angle RotationOffset = Angle.Zero;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref VehicleTurretTrackedMuzzleFlashComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (VehicleTurretTrackedMuzzleFlashComponent) component;
    serialization.TryCustomCopy<VehicleTurretTrackedMuzzleFlashComponent>(this, ref target, hookCtx, false, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref VehicleTurretTrackedMuzzleFlashComponent target,
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
    VehicleTurretTrackedMuzzleFlashComponent target1 = (VehicleTurretTrackedMuzzleFlashComponent) target;
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
    VehicleTurretTrackedMuzzleFlashComponent target1 = (VehicleTurretTrackedMuzzleFlashComponent) target;
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
    VehicleTurretTrackedMuzzleFlashComponent target1 = (VehicleTurretTrackedMuzzleFlashComponent) target;
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
  virtual VehicleTurretTrackedMuzzleFlashComponent Component.Instantiate()
  {
    return new VehicleTurretTrackedMuzzleFlashComponent();
  }
}
