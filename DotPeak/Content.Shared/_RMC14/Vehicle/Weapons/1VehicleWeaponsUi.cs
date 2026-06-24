// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vehicle.VehicleWeaponsUiEntry
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
public sealed class VehicleWeaponsUiEntry
{
  public readonly string SlotId;
  public readonly string HardpointType;
  public readonly NetEntity? MountedEntity;
  public readonly string? InstalledName;
  public readonly NetEntity? InstalledEntity;
  public readonly bool HasItem;
  public readonly bool Selectable;
  public readonly bool Selected;
  public readonly int AmmoCount;
  public readonly int AmmoCapacity;
  public readonly bool HasAmmo;
  public readonly int MagazineSize;
  public readonly int StoredMagazines;
  public readonly int MaxStoredMagazines;
  public readonly bool HasMagazineData;
  public readonly string? OperatorName;
  public readonly bool OperatorIsSelf;
  public readonly float Integrity;
  public readonly float MaxIntegrity;
  public readonly bool HasIntegrity;
  public readonly float CooldownRemaining;
  public readonly float CooldownTotal;
  public readonly bool IsOnCooldown;

  public VehicleWeaponsUiEntry(
    string slotId,
    string hardpointType,
    NetEntity? mountedEntity,
    string? installedName,
    NetEntity? installedEntity,
    bool hasItem,
    bool selectable,
    bool selected,
    int ammoCount,
    int ammoCapacity,
    bool hasAmmo,
    int magazineSize,
    int storedMagazines,
    int maxStoredMagazines,
    bool hasMagazineData,
    string? operatorName,
    bool operatorIsSelf,
    float integrity,
    float maxIntegrity,
    bool hasIntegrity,
    float cooldownRemaining,
    float cooldownTotal,
    bool isOnCooldown)
  {
    this.SlotId = slotId;
    this.HardpointType = hardpointType;
    this.MountedEntity = mountedEntity;
    this.InstalledName = installedName;
    this.InstalledEntity = installedEntity;
    this.HasItem = hasItem;
    this.Selectable = selectable;
    this.Selected = selected;
    this.AmmoCount = ammoCount;
    this.AmmoCapacity = ammoCapacity;
    this.HasAmmo = hasAmmo;
    this.MagazineSize = magazineSize;
    this.StoredMagazines = storedMagazines;
    this.MaxStoredMagazines = maxStoredMagazines;
    this.HasMagazineData = hasMagazineData;
    this.OperatorName = operatorName;
    this.OperatorIsSelf = operatorIsSelf;
    this.Integrity = integrity;
    this.MaxIntegrity = maxIntegrity;
    this.HasIntegrity = hasIntegrity;
    this.CooldownRemaining = cooldownRemaining;
    this.CooldownTotal = cooldownTotal;
    this.IsOnCooldown = isOnCooldown;
  }
}
