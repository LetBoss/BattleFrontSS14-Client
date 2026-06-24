// Decompiled with JetBrains decompiler
// Type: Content.Shared.Singularity.Components.ParticleAcceleratorUIState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;

#nullable disable
namespace Content.Shared.Singularity.Components;

[NetSerializable]
[Serializable]
public sealed class ParticleAcceleratorUIState : BoundUserInterfaceState
{
  public bool Assembled;
  public bool Enabled;
  public ParticleAcceleratorPowerState State;
  public int PowerDraw;
  public int PowerReceive;
  public bool EmitterStarboardExists;
  public bool EmitterForeExists;
  public bool EmitterPortExists;
  public bool PowerBoxExists;
  public bool FuelChamberExists;
  public bool EndCapExists;
  public bool InterfaceBlock;
  public ParticleAcceleratorPowerState MaxLevel;
  public bool WirePowerBlock;

  public ParticleAcceleratorUIState(
    bool assembled,
    bool enabled,
    ParticleAcceleratorPowerState state,
    int powerReceive,
    int powerDraw,
    bool emitterStarboardExists,
    bool emitterForeExists,
    bool emitterPortExists,
    bool powerBoxExists,
    bool fuelChamberExists,
    bool endCapExists,
    bool interfaceBlock,
    ParticleAcceleratorPowerState maxLevel,
    bool wirePowerBlock)
  {
    this.Assembled = assembled;
    this.Enabled = enabled;
    this.State = state;
    this.PowerDraw = powerDraw;
    this.PowerReceive = powerReceive;
    this.EmitterStarboardExists = emitterStarboardExists;
    this.EmitterForeExists = emitterForeExists;
    this.EmitterPortExists = emitterPortExists;
    this.PowerBoxExists = powerBoxExists;
    this.FuelChamberExists = fuelChamberExists;
    this.EndCapExists = endCapExists;
    this.InterfaceBlock = interfaceBlock;
    this.MaxLevel = maxLevel;
    this.WirePowerBlock = wirePowerBlock;
  }
}
