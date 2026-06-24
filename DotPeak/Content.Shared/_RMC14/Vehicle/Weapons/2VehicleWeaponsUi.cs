// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vehicle.VehicleWeaponsUiState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Vehicle;

[NetSerializable]
[Serializable]
public sealed class VehicleWeaponsUiState : BoundUserInterfaceState
{
  public readonly NetEntity Vehicle;
  public readonly List<VehicleWeaponsUiEntry> Hardpoints;
  public readonly bool CanToggleStabilization;
  public readonly bool StabilizationEnabled;
  public readonly bool CanToggleAuto;
  public readonly bool AutoEnabled;

  public VehicleWeaponsUiState(
    NetEntity vehicle,
    List<VehicleWeaponsUiEntry> hardpoints,
    bool canToggleStabilization,
    bool stabilizationEnabled,
    bool canToggleAuto,
    bool autoEnabled)
  {
    this.Vehicle = vehicle;
    this.Hardpoints = hardpoints;
    this.CanToggleStabilization = canToggleStabilization;
    this.StabilizationEnabled = stabilizationEnabled;
    this.CanToggleAuto = canToggleAuto;
    this.AutoEnabled = autoEnabled;
  }
}
