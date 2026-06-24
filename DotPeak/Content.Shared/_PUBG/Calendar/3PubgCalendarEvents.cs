// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Calendar.PubgCalendarClaimResultMessage
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared._PUBG.Calendar;

[NetSerializable]
[Serializable]
public sealed class PubgCalendarClaimResultMessage : EntityEventArgs
{
  public bool Success { get; }

  public string? Error { get; }

  public int ClaimedDay { get; }

  public string? RewardType { get; }

  public string? RewardValue { get; }

  public int? DurationDays { get; }

  public int NextDayToClaim { get; }

  public bool CanClaimToday { get; }

  public PubgCalendarClaimResultMessage(
    bool success,
    string? error = null,
    int claimedDay = 0,
    string? rewardType = null,
    string? rewardValue = null,
    int? durationDays = null,
    int nextDayToClaim = 0,
    bool canClaimToday = false)
  {
    this.Success = success;
    this.Error = error;
    this.ClaimedDay = claimedDay;
    this.RewardType = rewardType;
    this.RewardValue = rewardValue;
    this.DurationDays = durationDays;
    this.NextDayToClaim = nextDayToClaim;
    this.CanClaimToday = canClaimToday;
  }
}
