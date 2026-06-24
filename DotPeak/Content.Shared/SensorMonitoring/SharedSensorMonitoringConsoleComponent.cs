// Decompiled with JetBrains decompiler
// Type: Content.Shared.SensorMonitoring.SensorMonitoringConsoleBoundInterfaceState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared.SensorMonitoring;

[NetSerializable]
[Serializable]
public sealed class SensorMonitoringConsoleBoundInterfaceState : BoundUserInterfaceState
{
  public TimeSpan RetentionTime;
  public SensorMonitoringConsoleBoundInterfaceState.SensorData[] Sensors = Array.Empty<SensorMonitoringConsoleBoundInterfaceState.SensorData>();

  [NetSerializable]
  [Serializable]
  public sealed class SensorData
  {
    public int NetId;
    public string Name = "";
    public string Address = "";
    public SensorDeviceType DeviceType;
    public SensorMonitoringConsoleBoundInterfaceState.SensorStream[] Streams = Array.Empty<SensorMonitoringConsoleBoundInterfaceState.SensorStream>();
  }

  [NetSerializable]
  [Serializable]
  public sealed class SensorStream
  {
    public int NetId;
    public string Name = "";
    public SensorUnit Unit;
    public SensorSample[] Samples = Array.Empty<SensorSample>();
  }
}
