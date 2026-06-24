// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.CivRoundEndSideInfo
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared._CIV14merka;

[NetSerializable]
[Serializable]
public sealed class CivRoundEndSideInfo
{
  public string TeamName { get; }

  public bool IsWinner { get; }

  public bool HasScore { get; }

  public int Score { get; }

  public string CommanderName { get; }

  public CivRoundEndSideInfo(
    string teamName,
    bool isWinner,
    bool hasScore,
    int score,
    string commanderName)
  {
    this.TeamName = teamName;
    this.IsWinner = isWinner;
    this.HasScore = hasScore;
    this.Score = score;
    this.CommanderName = commanderName;
  }
}
