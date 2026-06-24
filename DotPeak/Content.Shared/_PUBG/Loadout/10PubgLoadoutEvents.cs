// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Loadout.PubgLoadoutWeaponState
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
public sealed class PubgLoadoutWeaponState
{
  public NetEntity Entity { get; }

  public string Name { get; }

  public List<PubgLoadoutWeaponSlotState> Slots { get; }

  public PubgWeaponStats Stats { get; }

  public PubgLoadoutWeaponState(
    NetEntity entity,
    string name,
    List<PubgLoadoutWeaponSlotState> slots,
    PubgWeaponStats stats)
  {
    this.Entity = entity;
    this.Name = name;
    this.Slots = slots;
    this.Stats = stats;
  }
}
