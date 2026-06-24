// Decompiled with JetBrains decompiler
// Type: Content.Shared.Ame.Components.AmeControllerBoundUserInterfaceState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;

#nullable disable
namespace Content.Shared.Ame.Components;

[NetSerializable]
[Serializable]
public sealed class AmeControllerBoundUserInterfaceState : BoundUserInterfaceState
{
  public readonly bool HasPower;
  public readonly bool IsMaster;
  public readonly bool Injecting;
  public readonly bool HasFuelJar;
  public readonly int FuelAmount;
  public readonly int InjectionAmount;
  public readonly int CoreCount;
  public readonly float CurrentPowerSupply;
  public readonly float TargetedPowerSupply;

  public AmeControllerBoundUserInterfaceState(
    bool hasPower,
    bool isMaster,
    bool injecting,
    bool hasFuelJar,
    int fuelAmount,
    int injectionAmount,
    int coreCount,
    float currentPowerSupply,
    float targetedPowerSupply)
  {
    this.HasPower = hasPower;
    this.IsMaster = isMaster;
    this.Injecting = injecting;
    this.HasFuelJar = hasFuelJar;
    this.FuelAmount = fuelAmount;
    this.InjectionAmount = injectionAmount;
    this.CoreCount = coreCount;
    this.CurrentPowerSupply = currentPowerSupply;
    this.TargetedPowerSupply = targetedPowerSupply;
  }
}
