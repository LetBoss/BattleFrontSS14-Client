// Decompiled with JetBrains decompiler
// Type: Content.Shared.SensorMonitoring.SensorMonitoringIncrementalUpdate
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
public sealed class SensorMonitoringIncrementalUpdate : BoundUserInterfaceMessage
{
  public TimeSpan RelTime;
  public SensorMonitoringIncrementalUpdate.SensorData[] Sensors = Array.Empty<SensorMonitoringIncrementalUpdate.SensorData>();
  public int[] RemovedSensors = Array.Empty<int>();

  [NetSerializable]
  [Serializable]
  public sealed class SensorData
  {
    public int NetId;
    public SensorMonitoringIncrementalUpdate.SensorStream[] Streams = Array.Empty<SensorMonitoringIncrementalUpdate.SensorStream>();
  }

  [NetSerializable]
  [Serializable]
  public sealed class SensorStream
  {
    public int NetId;
    public SensorUnit Unit;
    public SensorSample[] Samples = Array.Empty<SensorSample>();
  }
}
