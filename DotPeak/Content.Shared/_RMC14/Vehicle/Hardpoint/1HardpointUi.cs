// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vehicle.HardpointUiEntry
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
public sealed class HardpointUiEntry
{
  public readonly string SlotId;
  public readonly string HardpointType;
  public readonly string? InstalledName;
  public readonly NetEntity? InstalledEntity;
  public readonly float Integrity;
  public readonly float MaxIntegrity;
  public readonly bool HasIntegrity;
  public readonly bool HasItem;
  public readonly bool Required;
  public readonly bool Removing;

  public HardpointUiEntry(
    string slotId,
    string hardpointType,
    string? installedName,
    NetEntity? installedEntity,
    float integrity,
    float maxIntegrity,
    bool hasIntegrity,
    bool hasItem,
    bool required,
    bool removing)
  {
    this.SlotId = slotId;
    this.HardpointType = hardpointType;
    this.InstalledName = installedName;
    this.InstalledEntity = installedEntity;
    this.Integrity = integrity;
    this.MaxIntegrity = maxIntegrity;
    this.HasIntegrity = hasIntegrity;
    this.HasItem = hasItem;
    this.Required = required;
    this.Removing = removing;
  }
}
