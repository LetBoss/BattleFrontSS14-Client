// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vehicle.RMCVehicleAmmoLoaderUiEntry
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared._RMC14.Vehicle;

[NetSerializable]
[Serializable]
public sealed class RMCVehicleAmmoLoaderUiEntry
{
  public readonly string SlotId;
  public readonly string HardpointType;
  public readonly string? InstalledName;
  public readonly NetEntity? InstalledEntity;
  public readonly int ChamberedRounds;
  public readonly int MagazineSize;
  public readonly int StoredMagazines;
  public readonly int MaxStoredMagazines;
  public readonly bool CanLoad;
  public readonly string? LoadedAmmoType;

  public RMCVehicleAmmoLoaderUiEntry(
    string slotId,
    string hardpointType,
    string? installedName,
    NetEntity? installedEntity,
    int chamberedRounds,
    int magazineSize,
    int storedMagazines,
    int maxStoredMagazines,
    bool canLoad,
    string? loadedAmmoType = null)
  {
    this.SlotId = slotId;
    this.HardpointType = hardpointType;
    this.InstalledName = installedName;
    this.InstalledEntity = installedEntity;
    this.ChamberedRounds = chamberedRounds;
    this.MagazineSize = magazineSize;
    this.StoredMagazines = storedMagazines;
    this.MaxStoredMagazines = maxStoredMagazines;
    this.CanLoad = canLoad;
    this.LoadedAmmoType = loadedAmmoType;
  }
}
