// Decompiled with JetBrains decompiler
// Type: Content.Shared.Atmos.Piping.Portable.Components.SpaceHeaterBoundUserInterfaceState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;

#nullable disable
namespace Content.Shared.Atmos.Piping.Portable.Components;

[NetSerializable]
[Serializable]
public sealed class SpaceHeaterBoundUserInterfaceState : BoundUserInterfaceState
{
  public float MinTemperature { get; }

  public float MaxTemperature { get; }

  public float TargetTemperature { get; }

  public bool Enabled { get; }

  public SpaceHeaterMode Mode { get; }

  public SpaceHeaterPowerLevel PowerLevel { get; }

  public SpaceHeaterBoundUserInterfaceState(
    float minTemperature,
    float maxTemperature,
    float temperature,
    bool enabled,
    SpaceHeaterMode mode,
    SpaceHeaterPowerLevel powerLevel)
  {
    this.MinTemperature = minTemperature;
    this.MaxTemperature = maxTemperature;
    this.TargetTemperature = temperature;
    this.Enabled = enabled;
    this.Mode = mode;
    this.PowerLevel = powerLevel;
  }
}
