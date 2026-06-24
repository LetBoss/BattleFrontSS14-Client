// Decompiled with JetBrains decompiler
// Type: Content.Shared.VendingMachines.VendingMachineComponentState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.VendingMachines;

[NetSerializable]
[Serializable]
public sealed class VendingMachineComponentState : ComponentState
{
  public Dictionary<string, VendingMachineInventoryEntry> Inventory = new Dictionary<string, VendingMachineInventoryEntry>();
  public Dictionary<string, VendingMachineInventoryEntry> EmaggedInventory = new Dictionary<string, VendingMachineInventoryEntry>();
  public Dictionary<string, VendingMachineInventoryEntry> ContrabandInventory = new Dictionary<string, VendingMachineInventoryEntry>();
  public bool Contraband;
  public TimeSpan? EjectEnd;
  public TimeSpan? DenyEnd;
  public TimeSpan? DispenseOnHitEnd;
}
