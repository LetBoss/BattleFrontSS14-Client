// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vehicle.VehicleHardpointLayerState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;

#nullable enable
namespace Content.Shared._RMC14.Vehicle;

[NetSerializable]
[DataDefinition]
[Serializable]
public record struct VehicleHardpointLayerState : 
  ISerializationGenerated<VehicleHardpointLayerState>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public string Layer { get; set; }

  [DataField(null, false, 1, false, false, null)]
  public string State { get; set; }

  public VehicleHardpointLayerState(string layer, string state)
  {
    // ISSUE: reference to a compiler-generated field
    this.\u003CLayer\u003Ek__BackingField = string.Empty;
    // ISSUE: reference to a compiler-generated field
    this.\u003CState\u003Ek__BackingField = string.Empty;
    this.Layer = layer;
    this.State = state;
  }

  public VehicleHardpointLayerState()
  {
    // ISSUE: reference to a compiler-generated field
    this.\u003CLayer\u003Ek__BackingField = string.Empty;
    // ISSUE: reference to a compiler-generated field
    this.\u003CState\u003Ek__BackingField = string.Empty;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref VehicleHardpointLayerState target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<VehicleHardpointLayerState>(this, ref target, hookCtx, false, context))
      return;
    string target1 = (string) null;
    if (this.Layer == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Layer, ref target1, hookCtx, false, context))
      target1 = this.Layer;
    string target2 = (string) null;
    if (this.State == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.State, ref target2, hookCtx, false, context))
      target2 = this.State;
    target = target with
    {
      Layer = target1,
      State = target2
    };
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref VehicleHardpointLayerState target,
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
    VehicleHardpointLayerState target1 = (VehicleHardpointLayerState) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public VehicleHardpointLayerState Instantiate() => new VehicleHardpointLayerState();
}
