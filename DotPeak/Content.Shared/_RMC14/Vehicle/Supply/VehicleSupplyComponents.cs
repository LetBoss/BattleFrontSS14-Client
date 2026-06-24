// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vehicle.Supply.VehicleSupplyEntry
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Vehicle.Supply;

[DataDefinition]
[NetSerializable]
[Serializable]
public sealed class VehicleSupplyEntry : 
  ISerializationGenerated<VehicleSupplyEntry>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public string? Name;
  [DataField(null, false, 1, true, false, null)]
  public EntProtoId Vehicle;
  [DataField(null, false, 1, false, false, null)]
  public string? Unlock;
  [DataField(null, false, 1, false, false, null)]
  public List<EntProtoId> Hardpoints = new List<EntProtoId>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref VehicleSupplyEntry target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<VehicleSupplyEntry>(this, ref target, hookCtx, false, context))
      return;
    string target1 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.Name, ref target1, hookCtx, false, context))
      target1 = this.Name;
    target.Name = target1;
    EntProtoId target2 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.Vehicle, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntProtoId>(this.Vehicle, hookCtx, context);
    target.Vehicle = target2;
    string target3 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.Unlock, ref target3, hookCtx, false, context))
      target3 = this.Unlock;
    target.Unlock = target3;
    List<EntProtoId> target4 = (List<EntProtoId>) null;
    if (this.Hardpoints == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<EntProtoId>>(this.Hardpoints, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<List<EntProtoId>>(this.Hardpoints, hookCtx, context);
    target.Hardpoints = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref VehicleSupplyEntry target,
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
    VehicleSupplyEntry target1 = (VehicleSupplyEntry) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public VehicleSupplyEntry Instantiate() => new VehicleSupplyEntry();
}
