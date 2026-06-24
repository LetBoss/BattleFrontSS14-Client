// Decompiled with JetBrains decompiler
// Type: Content.Shared.Shuttles.BUIStates.NavInterfaceState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Shuttles.BUIStates;

[NetSerializable]
[Serializable]
public sealed class NavInterfaceState
{
  public float MaxRange;
  public NetCoordinates? Coordinates;
  public Robust.Shared.Maths.Angle? Angle;
  public Dictionary<NetEntity, List<DockingPortState>> Docks;
  public bool RotateWithEntity = true;

  public NavInterfaceState(
    float maxRange,
    NetCoordinates? coordinates,
    Robust.Shared.Maths.Angle? angle,
    Dictionary<NetEntity, List<DockingPortState>> docks)
  {
    this.MaxRange = maxRange;
    this.Coordinates = coordinates;
    this.Angle = angle;
    this.Docks = docks;
  }
}
