// Decompiled with JetBrains decompiler
// Type: Content.Shared.VendingMachines.VendingMachineEjectMessage
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared.VendingMachines;

[NetSerializable]
[Serializable]
public sealed class VendingMachineEjectMessage : BoundUserInterfaceMessage
{
  public readonly InventoryType Type;
  public readonly string ID;

  public VendingMachineEjectMessage(InventoryType type, string id)
  {
    this.Type = type;
    this.ID = id;
  }
}
