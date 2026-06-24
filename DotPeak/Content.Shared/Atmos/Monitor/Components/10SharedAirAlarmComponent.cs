// Decompiled with JetBrains decompiler
// Type: Content.Shared.Atmos.Monitor.Components.AirAlarmUpdateAlarmThresholdMessage
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared.Atmos.Monitor.Components;

[NetSerializable]
[Serializable]
public sealed class AirAlarmUpdateAlarmThresholdMessage : BoundUserInterfaceMessage
{
  public string Address { get; }

  public AtmosAlarmThreshold Threshold { get; }

  public AtmosMonitorThresholdType Type { get; }

  public Content.Shared.Atmos.Gas? Gas { get; }

  public AirAlarmUpdateAlarmThresholdMessage(
    string address,
    AtmosMonitorThresholdType type,
    AtmosAlarmThreshold threshold,
    Content.Shared.Atmos.Gas? gas = null)
  {
    this.Address = address;
    this.Threshold = threshold;
    this.Type = type;
    this.Gas = gas;
  }
}
