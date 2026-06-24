// Decompiled with JetBrains decompiler
// Type: Content.Shared.Power.BatteryBuiState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;

#nullable disable
namespace Content.Shared.Power;

[NetSerializable]
[Serializable]
public sealed class BatteryBuiState : BoundUserInterfaceState
{
  public bool CanCharge;
  public bool CanDischarge;
  public bool SupplyingNetworkHasPower;
  public bool LoadingNetworkHasPower;
  public float CurrentReceiving;
  public float CurrentSupply;
  public float MaxChargeRate;
  public float MinMaxChargeRate;
  public float MaxMaxChargeRate;
  public float Efficiency;
  public float MaxSupply;
  public float MinMaxSupply;
  public float MaxMaxSupply;
  public float Charge;
  public float Capacity;
}
