// Decompiled with JetBrains decompiler
// Type: Content.Shared.Atmos.Monitor.AtmosAlarmThresholdChange
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

#nullable disable
namespace Content.Shared.Atmos.Monitor;

public readonly struct AtmosAlarmThresholdChange(
  AtmosMonitorLimitType type,
  AlarmThresholdSetting? previous,
  AlarmThresholdSetting current)
{
  public readonly AtmosMonitorLimitType Type = type;
  public readonly AlarmThresholdSetting? Previous = previous;
  public readonly AlarmThresholdSetting Current = current;
}
