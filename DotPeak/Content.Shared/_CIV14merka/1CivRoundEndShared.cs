// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.CivRoundEndSummary
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._CIV14merka.Stats;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._CIV14merka;

[NetSerializable]
[Serializable]
public sealed class CivRoundEndSummary
{
  public string WinnerText { get; }

  public string ModeText { get; }

  public string MapText { get; }

  public string ReasonText { get; }

  public string DurationText { get; }

  public string Team1Name { get; }

  public string Team2Name { get; }

  public int Team1AliveCount { get; }

  public int Team2AliveCount { get; }

  public bool HasScore { get; }

  public int Team1Score { get; }

  public int Team2Score { get; }

  public int LobbyReturnSeconds { get; }

  public List<CivRoundTopAward> TopAwards { get; }

  public List<CivRoundEndSideInfo> Sides { get; }

  public CivRoundEndSummary(
    string winnerText,
    string modeText,
    string mapText,
    string reasonText,
    string durationText,
    string team1Name,
    string team2Name,
    int team1AliveCount,
    int team2AliveCount,
    bool hasScore,
    int team1Score,
    int team2Score,
    int lobbyReturnSeconds,
    List<CivRoundTopAward>? topAwards = null,
    List<CivRoundEndSideInfo>? sides = null)
  {
    this.WinnerText = winnerText;
    this.ModeText = modeText;
    this.MapText = mapText;
    this.ReasonText = reasonText;
    this.DurationText = durationText;
    this.Team1Name = team1Name;
    this.Team2Name = team2Name;
    this.Team1AliveCount = team1AliveCount;
    this.Team2AliveCount = team2AliveCount;
    this.HasScore = hasScore;
    this.Team1Score = team1Score;
    this.Team2Score = team2Score;
    this.LobbyReturnSeconds = lobbyReturnSeconds;
    this.TopAwards = topAwards ?? new List<CivRoundTopAward>();
    this.Sides = sides ?? new List<CivRoundEndSideInfo>();
  }
}
