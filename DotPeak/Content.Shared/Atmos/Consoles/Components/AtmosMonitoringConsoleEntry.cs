// Decompiled with JetBrains decompiler
// Type: Content.Shared.Atmos.Components.AtmosMonitoringConsoleEntry
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Atmos.Components;

[NetSerializable]
[Serializable]
public struct AtmosMonitoringConsoleEntry
{
  public NetEntity NetEntity;
  public NetCoordinates Coordinates;
  public int NetId;
  public string EntityName;
  public string Address;
  public float TemperatureData;
  public float PressureData;
  public float TotalMolData;
  public Dictionary<Gas, float> GasData;
  public Color Color;
  public bool IsPowered;

  public AtmosMonitoringConsoleEntry(
    NetEntity entity,
    NetCoordinates coordinates,
    int netId,
    string entityName,
    string address)
  {
    this.TemperatureData = 0.0f;
    this.PressureData = 0.0f;
    this.TotalMolData = 0.0f;
    this.Color = new Color();
    this.NetId = -1;
    this.GasData = new Dictionary<Gas, float>();
    this.IsPowered = true;
    this.NetEntity = entity;
    this.Coordinates = coordinates;
    this.NetId = netId;
    this.EntityName = entityName;
    this.Address = address;
  }
}
