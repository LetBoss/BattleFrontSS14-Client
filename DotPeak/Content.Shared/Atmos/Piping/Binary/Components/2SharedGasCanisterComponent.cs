// Decompiled with JetBrains decompiler
// Type: Content.Shared.Atmos.Piping.Binary.Components.GasCanisterBoundUserInterfaceState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;

#nullable disable
namespace Content.Shared.Atmos.Piping.Binary.Components;

[NetSerializable]
[Serializable]
public sealed class GasCanisterBoundUserInterfaceState : BoundUserInterfaceState
{
  public float CanisterPressure { get; }

  public bool PortStatus { get; }

  public float TankPressure { get; }

  public GasCanisterBoundUserInterfaceState(
    float canisterPressure,
    bool portStatus,
    float tankPressure)
  {
    this.CanisterPressure = canisterPressure;
    this.PortStatus = portStatus;
    this.TankPressure = tankPressure;
  }
}
