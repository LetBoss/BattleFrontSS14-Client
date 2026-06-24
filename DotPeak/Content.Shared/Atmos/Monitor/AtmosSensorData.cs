// Decompiled with JetBrains decompiler
// Type: Content.Shared.Atmos.Monitor.AtmosSensorData
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Atmos.Monitor.Components;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Atmos.Monitor;

[NetSerializable]
[Serializable]
public sealed class AtmosSensorData : IAtmosDeviceData
{
  public AtmosSensorData(
    float pressure,
    float temperature,
    float totalMoles,
    AtmosAlarmType alarmState,
    Dictionary<Gas, float> gases,
    AtmosAlarmThreshold pressureThreshold,
    AtmosAlarmThreshold temperatureThreshold,
    Dictionary<Gas, AtmosAlarmThreshold> gasThresholds)
  {
    this.Pressure = pressure;
    this.Temperature = temperature;
    this.TotalMoles = totalMoles;
    this.AlarmState = alarmState;
    this.Gases = gases;
    this.PressureThreshold = pressureThreshold;
    this.TemperatureThreshold = temperatureThreshold;
    this.GasThresholds = gasThresholds;
  }

  public bool Enabled { get; set; }

  public bool Dirty { get; set; }

  public bool IgnoreAlarms { get; set; }

  public float Pressure { get; }

  public float Temperature { get; }

  public float TotalMoles { get; }

  public AtmosAlarmType AlarmState { get; }

  public Dictionary<Gas, float> Gases { get; }

  public AtmosAlarmThreshold PressureThreshold { get; }

  public AtmosAlarmThreshold TemperatureThreshold { get; }

  public Dictionary<Gas, AtmosAlarmThreshold> GasThresholds { get; }
}
