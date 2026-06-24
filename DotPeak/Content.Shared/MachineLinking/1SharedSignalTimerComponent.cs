// Decompiled with JetBrains decompiler
// Type: Content.Shared.MachineLinking.SignalTimerBoundUserInterfaceState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared.MachineLinking;

[NetSerializable]
[Serializable]
public sealed class SignalTimerBoundUserInterfaceState : BoundUserInterfaceState
{
  public string CurrentText;
  public string CurrentDelayMinutes;
  public string CurrentDelaySeconds;
  public bool ShowText;
  public TimeSpan TriggerTime;
  public bool TimerStarted;
  public bool HasAccess;

  public SignalTimerBoundUserInterfaceState(
    string currentText,
    string currentDelayMinutes,
    string currentDelaySeconds,
    bool showText,
    TimeSpan triggerTime,
    bool timerStarted,
    bool hasAccess)
  {
    this.CurrentText = currentText;
    this.CurrentDelayMinutes = currentDelayMinutes;
    this.CurrentDelaySeconds = currentDelaySeconds;
    this.ShowText = showText;
    this.TriggerTime = triggerTime;
    this.TimerStarted = timerStarted;
    this.HasAccess = hasAccess;
  }
}
