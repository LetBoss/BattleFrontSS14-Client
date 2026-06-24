// Decompiled with JetBrains decompiler
// Type: Content.Shared.Shuttles.BUIStates.ShuttleMapInterfaceState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Shuttles.Systems;
using Content.Shared.Shuttles.UI.MapObjects;
using Content.Shared.Timing;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Shuttles.BUIStates;

[NetSerializable]
[Serializable]
public sealed class ShuttleMapInterfaceState
{
  public readonly FTLState FTLState;
  public StartEndTime FTLTime;
  public List<ShuttleBeaconObject> Destinations;
  public List<ShuttleExclusionObject> Exclusions;

  public ShuttleMapInterfaceState(
    FTLState ftlState,
    StartEndTime ftlTime,
    List<ShuttleBeaconObject> destinations,
    List<ShuttleExclusionObject> exclusions)
  {
    this.FTLState = ftlState;
    this.FTLTime = ftlTime;
    this.Destinations = destinations;
    this.Exclusions = exclusions;
  }
}
