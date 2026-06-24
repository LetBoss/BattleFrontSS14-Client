// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vehicle.Supply.VehicleSupplyBuiState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Vehicle.Supply;

[NetSerializable]
[Serializable]
public sealed class VehicleSupplyBuiState : BoundUserInterfaceState
{
  public VehicleSupplyLiftMode? LiftMode;
  public bool Busy;
  public string? ActiveVehicleId;
  public string? SelectedVehicleId;
  public int SelectedCopyIndex;
  public VehicleSupplyPreviewState? Preview;
  public List<VehicleSupplyEntryState> Available;

  public VehicleSupplyBuiState(
    VehicleSupplyLiftMode? liftMode,
    bool busy,
    string? activeVehicleId,
    string? selectedVehicleId,
    int selectedCopyIndex,
    VehicleSupplyPreviewState? preview,
    List<VehicleSupplyEntryState> available)
  {
    this.LiftMode = liftMode;
    this.Busy = busy;
    this.ActiveVehicleId = activeVehicleId;
    this.SelectedVehicleId = selectedVehicleId;
    this.SelectedCopyIndex = selectedCopyIndex;
    this.Preview = preview;
    this.Available = available;
  }
}
