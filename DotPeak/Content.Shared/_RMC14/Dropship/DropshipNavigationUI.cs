// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Dropship.DropshipNavigationDestinationsBuiState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Doors.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Dropship;

[NetSerializable]
[Serializable]
public sealed class DropshipNavigationDestinationsBuiState(
  NetEntity? flyBy,
  List<Destination> destinations,
  Dictionary<DoorLocation, bool> doorLockStatus) : BoundUserInterfaceState
{
  public readonly NetEntity? FlyBy = flyBy;
  public readonly List<Destination> Destinations = destinations;
  public readonly Dictionary<DoorLocation, bool> DoorLockStatus = doorLockStatus;
}
