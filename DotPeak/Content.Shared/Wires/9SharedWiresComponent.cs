// Decompiled with JetBrains decompiler
// Type: Content.Shared.Wires.WiresBoundUserInterfaceState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared.Wires;

[NetSerializable]
[Serializable]
public sealed class WiresBoundUserInterfaceState : BoundUserInterfaceState
{
  public string BoardName { get; }

  public string? SerialNumber { get; }

  public ClientWire[] WiresList { get; }

  public StatusEntry[] Statuses { get; }

  public int WireSeed { get; }

  public WiresBoundUserInterfaceState(
    ClientWire[] wiresList,
    StatusEntry[] statuses,
    string boardName,
    string? serialNumber,
    int wireSeed)
  {
    this.BoardName = boardName;
    this.SerialNumber = serialNumber;
    this.WireSeed = wireSeed;
    this.WiresList = wiresList;
    this.Statuses = statuses;
  }
}
