// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.BattlePass.BattlePassStateMessage
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._PUBG.BattlePass;

[NetSerializable]
[Serializable]
public sealed class BattlePassStateMessage : EntityEventArgs
{
  public string SeasonId { get; }

  public string SeasonName { get; }

  public int CurrentXp { get; }

  public int CurrentLevel { get; }

  public bool HasPremium { get; }

  public List<BattlePassLevelInfo> Levels { get; }

  public List<string> ClaimedRewardIds { get; }

  public List<BattlePassTaskInfo> Tasks { get; }

  public DateTime TasksEndAt { get; }

  public int SkipsRemaining { get; }

  public BattlePassStateMessage(
    string seasonId,
    string seasonName,
    int currentXp,
    int currentLevel,
    bool hasPremium,
    List<BattlePassLevelInfo> levels,
    List<string> claimedRewardIds,
    List<BattlePassTaskInfo> tasks,
    DateTime tasksEndAt,
    int skipsRemaining)
  {
    this.SeasonId = seasonId;
    this.SeasonName = seasonName;
    this.CurrentXp = currentXp;
    this.CurrentLevel = currentLevel;
    this.HasPremium = hasPremium;
    this.Levels = levels;
    this.ClaimedRewardIds = claimedRewardIds;
    this.Tasks = tasks;
    this.TasksEndAt = tasksEndAt;
    this.SkipsRemaining = skipsRemaining;
  }
}
