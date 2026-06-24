// Decompiled with JetBrains decompiler
// Type: Content.Shared.Inventory.Events.InventoryEquipActEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared.Inventory.Events;

[NetSerializable]
[Serializable]
public sealed class InventoryEquipActEvent : EntityEventArgs
{
  public readonly NetEntity Uid;
  public readonly NetEntity ItemUid;
  public readonly string Slot;
  public readonly bool Silent;
  public readonly bool Force;

  public InventoryEquipActEvent(
    NetEntity uid,
    NetEntity itemUid,
    string slot,
    bool silent = false,
    bool force = false)
  {
    this.Uid = uid;
    this.ItemUid = itemUid;
    this.Slot = slot;
    this.Silent = silent;
    this.Force = force;
  }
}
