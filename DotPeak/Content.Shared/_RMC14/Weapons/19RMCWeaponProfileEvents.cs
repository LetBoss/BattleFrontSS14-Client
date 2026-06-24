// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.RMCWeaponProfileIntegrityResponseEvent
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
public sealed class RMCWeaponProfileIntegrityResponseEvent : EntityEventArgs
{
  public int Nonce { get; }

  public int Token { get; }

  public bool Success { get; }

  public List<byte> ProbeIds { get; }

  public List<string> ProbeDigests { get; }

  public uint FlowToken { get; }

  public RMCWeaponProfileIntegrityResponseEvent(
    int nonce,
    int token,
    bool success,
    List<byte>? probeIds,
    List<string>? probeDigests,
    uint flowToken = 0)
  {
    this.Nonce = nonce;
    this.Token = token;
    this.Success = success;
    this.ProbeIds = probeIds ?? new List<byte>();
    this.ProbeDigests = probeDigests ?? new List<string>();
    this.FlowToken = flowToken;
  }
}
