// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vehicle.VehicleEntryPoint
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Numerics;

#nullable enable
namespace Content.Shared._RMC14.Vehicle;

[DataDefinition]
public sealed class VehicleEntryPoint : 
  ISerializationGenerated<VehicleEntryPoint>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public Vector2 Offset;
  [DataField(null, false, 1, false, false, null)]
  public float Radius = 0.6f;
  [DataField(null, false, 1, false, false, null)]
  public Vector2? InteriorCoords;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref VehicleEntryPoint target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<VehicleEntryPoint>(this, ref target, hookCtx, false, context))
      return;
    Vector2 target1 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.Offset, ref target1, hookCtx, false, context))
      target1 = serialization.CreateCopy<Vector2>(this.Offset, hookCtx, context);
    target.Offset = target1;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Radius, ref target2, hookCtx, false, context))
      target2 = this.Radius;
    target.Radius = target2;
    Vector2? target3 = new Vector2?();
    if (!serialization.TryCustomCopy<Vector2?>(this.InteriorCoords, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<Vector2?>(this.InteriorCoords, hookCtx, context);
    target.InteriorCoords = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref VehicleEntryPoint target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    VehicleEntryPoint target1 = (VehicleEntryPoint) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public VehicleEntryPoint Instantiate() => new VehicleEntryPoint();
}
