// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Calendar.PubgCalendarStateMessage
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._PUBG.Calendar;

[NetSerializable]
[Serializable]
public sealed class PubgCalendarStateMessage : EntityEventArgs
{
  public string MonthKey { get; }

  public int Year { get; }

  public int Month { get; }

  public DateTime StartsAt { get; }

  public DateTime EndsAt { get; }

  public DateTime ServerNowUtc { get; }

  public List<PubgCalendarDayInfo> Days { get; }

  public int NextDayToClaim { get; }

  public bool CanClaimToday { get; }

  public DateTime? LastClaimAt { get; }

  public List<int> ClaimedDays { get; }

  public int MaxUnlockDay { get; }

  public PubgCalendarStateMessage(
    string monthKey,
    int year,
    int month,
    DateTime startsAt,
    DateTime endsAt,
    DateTime serverNowUtc,
    List<PubgCalendarDayInfo> days,
    int nextDayToClaim,
    bool canClaimToday,
    DateTime? lastClaimAt,
    List<int> claimedDays,
    int maxUnlockDay)
  {
    this.MonthKey = monthKey;
    this.Year = year;
    this.Month = month;
    this.StartsAt = startsAt;
    this.EndsAt = endsAt;
    this.ServerNowUtc = serverNowUtc;
    this.Days = days;
    this.NextDayToClaim = nextDayToClaim;
    this.CanClaimToday = canClaimToday;
    this.LastClaimAt = lastClaimAt;
    this.ClaimedDays = claimedDays;
    this.MaxUnlockDay = maxUnlockDay;
  }
}
