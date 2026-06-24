// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.RMCWeaponDrawSkewEvent
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
public sealed class RMCWeaponDrawSkewEvent : EntityEventArgs
{
  public bool Enabled { get; }

  public float ShiftTiles { get; }

  public float SwapIntervalSeconds { get; }

  public bool SyncLagEnabled { get; }

  public float SyncLagDelaySeconds { get; }

  public float SyncLagJitterSeconds { get; }

  public float SyncLagMaxOffsetTiles { get; }

  public RMCWeaponDrawSkewEvent(
    bool enabled,
    float shiftTiles,
    float swapIntervalSeconds,
    bool syncLagEnabled,
    float syncLagDelaySeconds,
    float syncLagJitterSeconds,
    float syncLagMaxOffsetTiles)
  {
    this.Enabled = enabled;
    this.ShiftTiles = shiftTiles;
    this.SwapIntervalSeconds = swapIntervalSeconds;
    this.SyncLagEnabled = syncLagEnabled;
    this.SyncLagDelaySeconds = syncLagDelaySeconds;
    this.SyncLagJitterSeconds = syncLagJitterSeconds;
    this.SyncLagMaxOffsetTiles = syncLagMaxOffsetTiles;
  }
}
