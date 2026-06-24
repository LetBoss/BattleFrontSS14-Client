// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.RMCWeaponFocusRangeEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;

#nullable disable
namespace Content.Shared._RMC14.Weapons;

[NetSerializable]
[Serializable]
public sealed class RMCWeaponFocusRangeEvent : EntityEventArgs
{
  public int Nonce { get; }

  public float AimDistance { get; }

  public RMCWeaponFocusRangeEvent(int nonce, float aimDistance)
  {
    this.Nonce = nonce;
    this.AimDistance = aimDistance;
  }
}
