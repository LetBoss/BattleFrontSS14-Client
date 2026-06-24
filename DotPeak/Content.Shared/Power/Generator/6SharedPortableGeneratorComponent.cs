// Decompiled with JetBrains decompiler
// Type: Content.Shared.Power.Generator.PortableGeneratorComponentBuiState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared.Power.Generator;

[NetSerializable]
[Serializable]
public sealed class PortableGeneratorComponentBuiState : BoundUserInterfaceState
{
  public float RemainingFuel;
  public bool Clogged;
  public (float Load, float Supply)? NetworkStats;
  public float TargetPower;
  public float MaximumPower;
  public float OptimalPower;
  public bool On;

  public PortableGeneratorComponentBuiState(
    FuelGeneratorComponent component,
    float remainingFuel,
    bool clogged,
    (float Demand, float Supply)? networkStats)
  {
    this.RemainingFuel = remainingFuel;
    this.Clogged = clogged;
    this.TargetPower = component.TargetPower;
    this.MaximumPower = component.MaxTargetPower;
    this.OptimalPower = component.OptimalPower;
    this.On = component.On;
    this.NetworkStats = networkStats;
  }
}
