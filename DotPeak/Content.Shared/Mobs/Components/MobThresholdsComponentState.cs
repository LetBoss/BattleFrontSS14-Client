// Decompiled with JetBrains decompiler
// Type: Content.Shared.Mobs.Components.MobThresholdsComponentState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Alert;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Mobs.Components;

[NetSerializable]
[Serializable]
public sealed class MobThresholdsComponentState : ComponentState
{
  public Dictionary<FixedPoint2, MobState> UnsortedThresholds;
  public bool TriggersAlerts;
  public MobState CurrentThresholdState;
  public Dictionary<MobState, ProtoId<AlertPrototype>> StateAlertDict;
  public bool ShowOverlays;
  public bool AllowRevives;
  public bool DisplayDamageInAlert;

  public MobThresholdsComponentState(
    Dictionary<FixedPoint2, MobState> unsortedThresholds,
    bool triggersAlerts,
    MobState currentThresholdState,
    Dictionary<MobState, ProtoId<AlertPrototype>> stateAlertDict,
    bool showOverlays,
    bool allowRevives,
    bool displayDamageInAlert)
  {
    this.UnsortedThresholds = unsortedThresholds;
    this.TriggersAlerts = triggersAlerts;
    this.CurrentThresholdState = currentThresholdState;
    this.StateAlertDict = stateAlertDict;
    this.ShowOverlays = showOverlays;
    this.AllowRevives = allowRevives;
    this.DisplayDamageInAlert = displayDamageInAlert;
  }
}
