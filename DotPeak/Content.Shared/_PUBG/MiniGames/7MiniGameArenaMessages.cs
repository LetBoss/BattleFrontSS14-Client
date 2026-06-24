// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.MiniGames.MiniGameArenaInfo
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared._PUBG.MiniGames;

[NetSerializable]
[Serializable]
public sealed class MiniGameArenaInfo
{
  public string FileName { get; set; }

  public string DisplayName { get; set; }

  public DateTime DateCreated { get; set; }

  public int Team1SpawnCount { get; set; }

  public int Team2SpawnCount { get; set; }

  public MiniGameArenaInfo(
    string fileName,
    string displayName,
    DateTime dateCreated,
    int team1SpawnCount,
    int team2SpawnCount)
  {
    this.FileName = fileName;
    this.DisplayName = displayName;
    this.DateCreated = dateCreated;
    this.Team1SpawnCount = team1SpawnCount;
    this.Team2SpawnCount = team2SpawnCount;
  }
}
