// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.RMCWeaponProfileIntegrityRequestEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Weapons;

[NetSerializable]
[Serializable]
public sealed class RMCWeaponProfileIntegrityRequestEvent : EntityEventArgs
{
  public int Nonce { get; }

  public int Token { get; }

  public int ChallengeSalt { get; }

  public List<byte> ProbeIds { get; }

  public RMCWeaponProfileIntegrityRequestEvent(
    int nonce,
    int token,
    int challengeSalt,
    List<byte>? probeIds)
  {
    this.Nonce = nonce;
    this.Token = token;
    this.ChallengeSalt = challengeSalt;
    this.ProbeIds = probeIds ?? new List<byte>();
  }
}
