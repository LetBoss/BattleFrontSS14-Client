// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.PubgAmmoUpdateEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared._PUBG;

[NetSerializable]
[Serializable]
public sealed class PubgAmmoUpdateEvent : EntityEventArgs
{
  public int CurrentAmmo { get; }

  public int MaxAmmo { get; }

  public int ReserveAmmo { get; }

  public string AmmoType { get; }

  public PubgAmmoUpdateEvent(int currentAmmo, int maxAmmo, int reserveAmmo, string ammoType = "")
  {
    this.CurrentAmmo = currentAmmo;
    this.MaxAmmo = maxAmmo;
    this.ReserveAmmo = reserveAmmo;
    this.AmmoType = ammoType;
  }
}
