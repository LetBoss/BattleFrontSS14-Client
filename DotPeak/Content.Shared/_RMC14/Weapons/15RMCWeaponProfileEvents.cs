// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.RMCWeaponProfileFrameResponseEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared._RMC14.Weapons;

[NetSerializable]
[Serializable]
public sealed class RMCWeaponProfileFrameResponseEvent : EntityEventArgs
{
  public int Nonce { get; }

  public int Token { get; }

  public RMCWeaponProfileFrameMode Mode { get; }

  public bool Success { get; }

  public byte[] Payload { get; }

  public RMCWeaponProfileFrameResponseEvent(
    int nonce,
    int token,
    RMCWeaponProfileFrameMode mode,
    bool success,
    byte[]? payload)
  {
    this.Nonce = nonce;
    this.Token = token;
    this.Mode = mode;
    this.Success = success;
    this.Payload = payload ?? Array.Empty<byte>();
  }
}
