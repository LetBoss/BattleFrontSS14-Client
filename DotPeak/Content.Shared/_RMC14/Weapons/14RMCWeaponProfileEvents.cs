// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.RMCWeaponProfileFrameRequestEvent
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
public sealed class RMCWeaponProfileFrameRequestEvent : EntityEventArgs
{
  public int Nonce { get; }

  public int Token { get; }

  public RMCWeaponProfileFrameMode Mode { get; }

  public bool UploadPayload { get; }

  public bool RequireLivenessMarker { get; }

  public byte LivenessGrid { get; }

  public byte LivenessCellX { get; }

  public byte LivenessCellY { get; }

  public byte LivenessSizePercent { get; }

  public byte LivenessRed { get; }

  public byte LivenessGreen { get; }

  public byte LivenessBlue { get; }

  public RMCWeaponProfileFrameRequestEvent(
    int nonce,
    int token,
    RMCWeaponProfileFrameMode mode,
    bool uploadPayload,
    bool requireLivenessMarker,
    byte livenessGrid,
    byte livenessCellX,
    byte livenessCellY,
    byte livenessSizePercent,
    byte livenessRed,
    byte livenessGreen,
    byte livenessBlue)
  {
    this.Nonce = nonce;
    this.Token = token;
    this.Mode = mode;
    this.UploadPayload = uploadPayload;
    this.RequireLivenessMarker = requireLivenessMarker;
    this.LivenessGrid = livenessGrid;
    this.LivenessCellX = livenessCellX;
    this.LivenessCellY = livenessCellY;
    this.LivenessSizePercent = livenessSizePercent;
    this.LivenessRed = livenessRed;
    this.LivenessGreen = livenessGreen;
    this.LivenessBlue = livenessBlue;
  }
}
