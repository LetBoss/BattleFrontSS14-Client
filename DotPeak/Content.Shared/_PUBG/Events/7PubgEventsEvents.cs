// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Events.PubgEventCardHubSummaryInfo
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._PUBG.Events;

[NetSerializable]
[Serializable]
public sealed class PubgEventCardHubSummaryInfo
{
  public int ProgressCurrent { get; set; }

  public int ProgressTarget { get; set; }

  public int? NextRewardThreshold { get; set; }

  public int NextRewardIn { get; set; }

  public int DailyCompleted { get; set; }

  public int DailyTotal { get; set; }

  public List<int> MilestoneThresholds { get; set; } = new List<int>();
}
