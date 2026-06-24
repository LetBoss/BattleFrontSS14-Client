// Decompiled with JetBrains decompiler
// Type: Content.Shared.Atmos.Components.AtmosAlertsComputerEntry
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Atmos.Monitor;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared.Atmos.Components;

[NetSerializable]
[Serializable]
public struct AtmosAlertsComputerEntry(
  NetEntity entity,
  NetCoordinates coordinates,
  AtmosAlertsComputerGroup group,
  AtmosAlarmType alarmState,
  string entityName,
  string address)
{
  public NetEntity NetEntity = entity;
  public NetCoordinates Coordinates = coordinates;
  public AtmosAlertsComputerGroup Group = group;
  public AtmosAlarmType AlarmState = alarmState;
  public string EntityName = entityName;
  public string Address = address;
}
