// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vehicle.Supply.VehicleSupplyEntryState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared._RMC14.Vehicle.Supply;

[NetSerializable]
[Serializable]
public sealed class VehicleSupplyEntryState
{
  public string Id;
  public string Name;
  public int Count;

  public VehicleSupplyEntryState(string id, string name, int count)
  {
    this.Id = id;
    this.Name = name;
    this.Count = count;
  }
}
