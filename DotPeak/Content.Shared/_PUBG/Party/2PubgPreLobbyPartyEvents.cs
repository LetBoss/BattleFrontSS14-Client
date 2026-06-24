// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Party.PubgPreLobbyModeOverviewEntry
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._PUBG.Match;
using Robust.Shared.Serialization;
using System;

#nullable disable
namespace Content.Shared._PUBG.Party;

[NetSerializable]
[Serializable]
public sealed class PubgPreLobbyModeOverviewEntry
{
  public PubgMatchMode Mode { get; }

  public int InGamePlayers { get; }

  public int InLobbyPlayers { get; }

  public int? NextStartSeconds { get; }

  public PubgPreLobbyModeOverviewEntry(
    PubgMatchMode mode,
    int inGamePlayers,
    int inLobbyPlayers,
    int? nextStartSeconds)
  {
    this.Mode = mode;
    this.InGamePlayers = inGamePlayers;
    this.InLobbyPlayers = inLobbyPlayers;
    this.NextStartSeconds = nextStartSeconds;
  }
}
