// Decompiled with JetBrains decompiler
// Type: Content.Shared.Kitchen.ReagentGrinderInterfaceState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Chemistry.Reagent;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared.Kitchen;

[NetSerializable]
[Serializable]
public sealed class ReagentGrinderInterfaceState : BoundUserInterfaceState
{
  public bool IsBusy;
  public bool HasBeakerIn;
  public bool Powered;
  public bool CanJuice;
  public bool CanGrind;
  public NetEntity[] ChamberContents;
  public ReagentQuantity[]? ReagentQuantities;
  public GrinderAutoMode AutoMode;

  public ReagentGrinderInterfaceState(
    bool isBusy,
    bool hasBeaker,
    bool powered,
    bool canJuice,
    bool canGrind,
    GrinderAutoMode autoMode,
    NetEntity[] chamberContents,
    ReagentQuantity[]? heldBeakerContents)
  {
    this.IsBusy = isBusy;
    this.HasBeakerIn = hasBeaker;
    this.Powered = powered;
    this.CanJuice = canJuice;
    this.CanGrind = canGrind;
    this.AutoMode = autoMode;
    this.ChamberContents = chamberContents;
    this.ReagentQuantities = heldBeakerContents;
  }
}
