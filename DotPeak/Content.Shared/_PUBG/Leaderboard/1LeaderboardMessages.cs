// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Leaderboard.LeaderboardResponseMessage
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._PUBG.Leaderboard;

[NetSerializable]
[Serializable]
public sealed class LeaderboardResponseMessage : EntityEventArgs
{
  public List<LeaderboardEntry> Players { get; }

  public int PlayerRank { get; }

  public int PlayerRating { get; }

  public string PlayerUsername { get; }

  public int TotalPlayers { get; }

  public LeaderboardResponseMessage(
    List<LeaderboardEntry> players,
    int playerRank,
    int playerRating,
    string playerUsername,
    int totalPlayers)
  {
    this.Players = players;
    this.PlayerRank = playerRank;
    this.PlayerRating = playerRating;
    this.PlayerUsername = playerUsername;
    this.TotalPlayers = totalPlayers;
  }
}
