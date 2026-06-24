// Decompiled with JetBrains decompiler
// Type: Content.Shared.PDA.PdaUpdateState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.CartridgeLoader;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.PDA;

[NetSerializable]
[Serializable]
public sealed class PdaUpdateState : CartridgeLoaderUiState
{
  public bool FlashlightEnabled;
  public bool HasPen;
  public bool HasPai;
  public PdaIdInfoText PdaOwnerInfo;
  public string? StationName;
  public bool HasUplink;
  public bool CanPlayMusic;
  public string? Address;

  public PdaUpdateState(
    List<NetEntity> programs,
    NetEntity? activeUI,
    bool flashlightEnabled,
    bool hasPen,
    bool hasPai,
    PdaIdInfoText pdaOwnerInfo,
    string? stationName,
    bool hasUplink = false,
    bool canPlayMusic = false,
    string? address = null)
    : base(programs, activeUI)
  {
    this.FlashlightEnabled = flashlightEnabled;
    this.HasPen = hasPen;
    this.HasPai = hasPai;
    this.PdaOwnerInfo = pdaOwnerInfo;
    this.HasUplink = hasUplink;
    this.CanPlayMusic = canPlayMusic;
    this.StationName = stationName;
    this.Address = address;
  }
}
