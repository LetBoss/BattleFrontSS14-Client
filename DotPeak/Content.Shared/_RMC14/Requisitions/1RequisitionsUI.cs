// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Requisitions.RequisitionsBuiState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Requisitions.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;

#nullable disable
namespace Content.Shared._RMC14.Requisitions;

[NetSerializable]
[Serializable]
public sealed class RequisitionsBuiState : BoundUserInterfaceState
{
  public RequisitionsElevatorMode? PlatformLowered;
  public bool Busy;
  public int Balance;
  public bool Full;

  public RequisitionsBuiState(
    RequisitionsElevatorMode? platformLowered,
    bool busy,
    int balance,
    bool full)
  {
    this.PlatformLowered = platformLowered;
    this.Busy = busy;
    this.Balance = balance;
    this.Full = full;
  }
}
