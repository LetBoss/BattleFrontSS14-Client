// Decompiled with JetBrains decompiler
// Type: Content.Shared.Atmos.Monitor.AtmosAlarmThresholdPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

#nullable enable
namespace Content.Shared.Atmos.Monitor;

[Prototype("alarmThreshold", 1)]
public sealed class AtmosAlarmThresholdPrototype : IPrototype
{
  [DataField("ignore", false, 1, false, false, null)]
  public bool Ignore;
  [DataField("upperBound", false, 1, false, false, null)]
  public AlarmThresholdSetting UpperBound = AlarmThresholdSetting.Disabled;
  [DataField("lowerBound", false, 1, false, false, null)]
  public AlarmThresholdSetting LowerBound = AlarmThresholdSetting.Disabled;
  [DataField("upperWarnAround", false, 1, false, false, null)]
  public AlarmThresholdSetting UpperWarningPercentage = AlarmThresholdSetting.Disabled;
  [DataField("lowerWarnAround", false, 1, false, false, null)]
  public AlarmThresholdSetting LowerWarningPercentage = AlarmThresholdSetting.Disabled;

  [IdDataField(1, null)]
  public string ID { get; private set; }
}
