// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Loadout.PubgLoadoutStateMessage
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._PUBG.Loadout;

[NetSerializable]
[Serializable]
public sealed class PubgLoadoutStateMessage : EntityEventArgs
{
  public List<PubgLoadoutItemState> GroundItems { get; }

  public List<PubgLoadoutItemState> InventoryItems { get; }

  public List<PubgLoadoutWeaponState> Weapons { get; }

  public bool HasRigContainer { get; }

  public bool HasBackpackContainer { get; }

  public int Revision { get; }

  public PubgLoadoutStateMessage(
    List<PubgLoadoutItemState> groundItems,
    List<PubgLoadoutItemState> inventoryItems,
    List<PubgLoadoutWeaponState> weapons,
    bool hasRigContainer,
    bool hasBackpackContainer,
    int revision)
  {
    this.GroundItems = groundItems;
    this.InventoryItems = inventoryItems;
    this.Weapons = weapons;
    this.HasRigContainer = hasRigContainer;
    this.HasBackpackContainer = hasBackpackContainer;
    this.Revision = revision;
  }
}
