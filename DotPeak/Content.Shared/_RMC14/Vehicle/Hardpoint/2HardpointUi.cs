// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vehicle.HardpointBoundUserInterfaceState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Vehicle;

[NetSerializable]
[Serializable]
public sealed class HardpointBoundUserInterfaceState : BoundUserInterfaceState
{
  public readonly List<HardpointUiEntry> Hardpoints;
  public readonly float FrameIntegrity;
  public readonly float FrameMaxIntegrity;
  public readonly bool HasFrameIntegrity;
  public readonly string? Error;

  public HardpointBoundUserInterfaceState(
    List<HardpointUiEntry> hardpoints,
    float frameIntegrity,
    float frameMaxIntegrity,
    bool hasFrameIntegrity,
    string? error)
  {
    this.Hardpoints = hardpoints;
    this.FrameIntegrity = frameIntegrity;
    this.FrameMaxIntegrity = frameMaxIntegrity;
    this.HasFrameIntegrity = hasFrameIntegrity;
    this.Error = error;
  }
}
