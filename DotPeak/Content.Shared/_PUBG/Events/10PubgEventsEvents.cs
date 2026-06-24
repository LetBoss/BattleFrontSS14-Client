// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Events.PubgMarsEventStateInfo
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
public sealed class PubgMarsEventStateInfo
{
  public int Points { get; set; }

  public string BalanceLabelKey { get; set; } = "pubg-events-mars-token-balance";

  public string TaskRewardLabelKey { get; set; } = "pubg-events-task-token-reward";

  public PubgEventWalletInfo Wallet { get; set; } = new PubgEventWalletInfo();

  public List<PubgMarsMilestoneInfo> Milestones { get; set; } = new List<PubgMarsMilestoneInfo>();

  public List<PubgMarsTaskInfo> LoginTasks { get; set; } = new List<PubgMarsTaskInfo>();

  public List<PubgMarsTaskInfo> ChallengeTasks { get; set; } = new List<PubgMarsTaskInfo>();

  public string WeekKey { get; set; } = string.Empty;

  public DateTime WeekStartsAt { get; set; }

  public DateTime WeekEndsAt { get; set; }
}
