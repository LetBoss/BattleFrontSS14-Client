// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vehicle.Supply.VehicleSupplyPreviewOverlay
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Serialization;
using System;
using System.Numerics;

#nullable enable
namespace Content.Shared._RMC14.Vehicle.Supply;

[NetSerializable]
[Serializable]
public sealed class VehicleSupplyPreviewOverlay
{
  public string Rsi;
  public string State;
  public int Order;
  public Vector2 BaseOffset;
  public bool UseDirectional;
  public Vector2 North;
  public Vector2 East;
  public Vector2 South;
  public Vector2 West;

  public VehicleSupplyPreviewOverlay(
    string rsi,
    string state,
    int order,
    Vector2 baseOffset,
    bool useDirectional,
    Vector2 north,
    Vector2 east,
    Vector2 south,
    Vector2 west)
  {
    this.Rsi = rsi;
    this.State = state;
    this.Order = order;
    this.BaseOffset = baseOffset;
    this.UseDirectional = useDirectional;
    this.North = north;
    this.East = east;
    this.South = south;
    this.West = west;
  }
}
