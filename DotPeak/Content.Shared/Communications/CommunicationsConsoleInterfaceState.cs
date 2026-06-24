// Decompiled with JetBrains decompiler
// Type: Content.Shared.Communications.CommunicationsConsoleInterfaceState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Communications;

[NetSerializable]
[Serializable]
public sealed class CommunicationsConsoleInterfaceState : BoundUserInterfaceState
{
  public readonly bool CanAnnounce;
  public readonly bool CanBroadcast = true;
  public readonly bool CanCall;
  public readonly TimeSpan? ExpectedCountdownEnd;
  public readonly bool CountdownStarted;
  public List<string>? AlertLevels;
  public string CurrentAlert;
  public float CurrentAlertDelay;

  public CommunicationsConsoleInterfaceState(
    bool canAnnounce,
    bool canCall,
    List<string>? alertLevels,
    string currentAlert,
    float currentAlertDelay,
    TimeSpan? expectedCountdownEnd = null)
  {
    this.CanAnnounce = canAnnounce;
    this.CanCall = canCall;
    this.ExpectedCountdownEnd = expectedCountdownEnd;
    this.CountdownStarted = expectedCountdownEnd.HasValue;
    this.AlertLevels = alertLevels;
    this.CurrentAlert = currentAlert;
    this.CurrentAlertDelay = currentAlertDelay;
  }
}
