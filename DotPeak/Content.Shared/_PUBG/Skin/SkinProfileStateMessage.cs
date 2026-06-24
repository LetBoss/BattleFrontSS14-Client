// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Skin.SkinProfileStateMessage
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._PUBG.Skin;

[NetSerializable]
[Serializable]
public sealed class SkinProfileStateMessage : EntityEventArgs
{
  public List<LeaderboardEntryInfo> Leaderboard { get; }

  public int PlayerRank { get; }

  public int PlayerRating { get; }

  public List<MatchHistoryInfo> MatchHistory { get; }

  public int TotalDeaths { get; }

  public SkinProfileStateMessage(
    List<LeaderboardEntryInfo> leaderboard,
    int playerRank,
    int playerRating,
    List<MatchHistoryInfo> matchHistory,
    int totalDeaths)
  {
    this.Leaderboard = leaderboard;
    this.PlayerRank = playerRank;
    this.PlayerRating = playerRating;
    this.MatchHistory = matchHistory;
    this.TotalDeaths = totalDeaths;
  }
}
