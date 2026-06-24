// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Skin.LeaderboardEntryInfo
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared._PUBG.Skin;

[NetSerializable]
[Serializable]
public sealed class LeaderboardEntryInfo
{
  public int Rank { get; set; }

  public string Username { get; set; } = string.Empty;

  public int Rating { get; set; }

  public int Games { get; set; }

  public int Wins { get; set; }

  public int Kills { get; set; }

  public int DamageDealt { get; set; }

  public int SurvivalTime { get; set; }
}
