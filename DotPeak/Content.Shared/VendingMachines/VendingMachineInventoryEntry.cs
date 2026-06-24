// Decompiled with JetBrains decompiler
// Type: Content.Shared.VendingMachines.VendingMachineInventoryEntry
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Serialization;
using Robust.Shared.ViewVariables;
using System;

#nullable enable
namespace Content.Shared.VendingMachines;

[NetSerializable]
[Serializable]
public sealed class VendingMachineInventoryEntry
{
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public InventoryType Type;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public string ID;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public uint Amount;

  public VendingMachineInventoryEntry(InventoryType type, string id, uint amount)
  {
    this.Type = type;
    this.ID = id;
    this.Amount = amount;
  }

  public VendingMachineInventoryEntry(VendingMachineInventoryEntry entry)
  {
    this.Type = entry.Type;
    this.ID = entry.ID;
    this.Amount = entry.Amount;
  }
}
