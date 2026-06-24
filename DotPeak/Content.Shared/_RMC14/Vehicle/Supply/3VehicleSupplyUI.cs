// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vehicle.Supply.VehicleSupplyPreviewState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Vehicle.Supply;

[NetSerializable]
[Serializable]
public sealed class VehicleSupplyPreviewState
{
  public string VehicleId;
  public int CopyIndex;
  public List<VehicleHardpointLayerState> Layers;
  public List<VehicleSupplyPreviewOverlay> Overlays;

  public VehicleSupplyPreviewState(
    string vehicleId,
    int copyIndex,
    List<VehicleHardpointLayerState> layers,
    List<VehicleSupplyPreviewOverlay> overlays)
  {
    this.VehicleId = vehicleId;
    this.CopyIndex = copyIndex;
    this.Layers = layers;
    this.Overlays = overlays;
  }
}
