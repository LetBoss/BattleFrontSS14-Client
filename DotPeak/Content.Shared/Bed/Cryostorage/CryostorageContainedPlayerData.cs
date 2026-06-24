// Decompiled with JetBrains decompiler
// Type: Content.Shared.Bed.Cryostorage.CryostorageContainedPlayerData
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Bed.Cryostorage;

[NetSerializable]
[Serializable]
public record struct CryostorageContainedPlayerData
{
  public string PlayerName;
  public NetEntity PlayerEnt;
  public Dictionary<string, string> ItemSlots;
  public Dictionary<string, string> HeldItems;

  public CryostorageContainedPlayerData()
  {
    this.PlayerName = string.Empty;
    this.PlayerEnt = NetEntity.Invalid;
    this.ItemSlots = new Dictionary<string, string>();
    this.HeldItems = new Dictionary<string, string>();
  }

  [CompilerGenerated]
  public override readonly int GetHashCode()
  {
    return ((EqualityComparer<string>.Default.GetHashCode(this.PlayerName) * -1521134295 + EqualityComparer<NetEntity>.Default.GetHashCode(this.PlayerEnt)) * -1521134295 + EqualityComparer<Dictionary<string, string>>.Default.GetHashCode(this.ItemSlots)) * -1521134295 + EqualityComparer<Dictionary<string, string>>.Default.GetHashCode(this.HeldItems);
  }

  [CompilerGenerated]
  public readonly bool Equals(CryostorageContainedPlayerData other)
  {
    return EqualityComparer<string>.Default.Equals(this.PlayerName, other.PlayerName) && EqualityComparer<NetEntity>.Default.Equals(this.PlayerEnt, other.PlayerEnt) && EqualityComparer<Dictionary<string, string>>.Default.Equals(this.ItemSlots, other.ItemSlots) && EqualityComparer<Dictionary<string, string>>.Default.Equals(this.HeldItems, other.HeldItems);
  }
}
