// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vehicle.VehicleSlotPath
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared._RMC14.Vehicle;

[NetSerializable]
[Serializable]
public readonly record struct VehicleSlotPath(string Root, string? Child = null)
{
  public bool IsValid => !string.IsNullOrWhiteSpace(this.Root);

  public bool IsNested => !string.IsNullOrWhiteSpace(this.Child);

  public string ToCompositeId()
  {
    return !this.IsNested ? this.Root : VehicleTurretSlotIds.Compose(this.Root, this.Child);
  }

  public VehicleSlotPath Append(string child)
  {
    return !this.IsNested ? new VehicleSlotPath(this.Root, child) : new VehicleSlotPath(this.Root, VehicleTurretSlotIds.Compose(this.Child, child));
  }

  public static bool TryParse(string? value, out VehicleSlotPath path)
  {
    path = new VehicleSlotPath();
    if (string.IsNullOrWhiteSpace(value))
      return false;
    string parentSlotId;
    string childSlotId;
    if (VehicleTurretSlotIds.TryParse(value, out parentSlotId, out childSlotId))
    {
      path = new VehicleSlotPath(parentSlotId, childSlotId);
      return true;
    }
    path = new VehicleSlotPath(value);
    return true;
  }
}
