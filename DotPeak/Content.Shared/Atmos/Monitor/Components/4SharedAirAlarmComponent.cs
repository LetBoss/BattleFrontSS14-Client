// Decompiled with JetBrains decompiler
// Type: Content.Shared.Atmos.Monitor.Components.AirAlarmUIState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Atmos.Monitor.Components;

[NetSerializable]
[Serializable]
public sealed class AirAlarmUIState : BoundUserInterfaceState
{
  public AirAlarmUIState(
    string address,
    int deviceCount,
    float pressureAverage,
    float temperatureAverage,
    List<(string, IAtmosDeviceData)> deviceData,
    AirAlarmMode mode,
    AtmosAlarmType alarmType,
    bool autoMode,
    bool panicWireCut)
  {
    this.Address = address;
    this.DeviceCount = deviceCount;
    this.PressureAverage = pressureAverage;
    this.TemperatureAverage = temperatureAverage;
    this.DeviceData = deviceData;
    this.Mode = mode;
    this.AlarmType = alarmType;
    this.AutoMode = autoMode;
    this.PanicWireCut = panicWireCut;
  }

  public string Address { get; }

  public int DeviceCount { get; }

  public float PressureAverage { get; }

  public float TemperatureAverage { get; }

  public List<(string, IAtmosDeviceData)> DeviceData { get; }

  public AirAlarmMode Mode { get; }

  public AtmosAlarmType AlarmType { get; }

  public bool AutoMode { get; }

  public bool PanicWireCut { get; }
}
