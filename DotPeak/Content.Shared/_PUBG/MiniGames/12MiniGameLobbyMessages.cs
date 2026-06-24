// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.MiniGames.MiniGameLobbyStateMessage
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._PUBG.MiniGames;

[NetSerializable]
[Serializable]
public sealed class MiniGameLobbyStateMessage : EntityEventArgs
{
  public int LobbyId { get; }

  public string Name { get; }

  public string LeaderName { get; }

  public string GameName { get; }

  public string SubmodeName { get; }

  public string MapName { get; }

  public int CurrentPlayers { get; }

  public int MaxPlayers { get; }

  public bool IsLocked { get; }

  public bool HasPassword { get; }

  public int RoundsTotal { get; }

  public int CurrentRound { get; }

  public int MaxRounds { get; }

  public bool InGame { get; }

  public List<MiniGameLobbyPlayerInfo> Players { get; }

  public List<MiniGameLobbyChatEntry> ChatHistory { get; }

  public MiniGameLobbyStateMessage(
    int lobbyId,
    string name,
    string leaderName,
    string gameName,
    string submodeName,
    string mapName,
    int currentPlayers,
    int maxPlayers,
    bool isLocked,
    bool hasPassword,
    int roundsTotal,
    int currentRound,
    int maxRounds,
    bool inGame,
    List<MiniGameLobbyPlayerInfo> players,
    List<MiniGameLobbyChatEntry> chatHistory)
  {
    this.LobbyId = lobbyId;
    this.Name = name;
    this.LeaderName = leaderName;
    this.GameName = gameName;
    this.SubmodeName = submodeName;
    this.MapName = mapName;
    this.CurrentPlayers = currentPlayers;
    this.MaxPlayers = maxPlayers;
    this.IsLocked = isLocked;
    this.HasPassword = hasPassword;
    this.RoundsTotal = roundsTotal;
    this.CurrentRound = currentRound;
    this.MaxRounds = maxRounds;
    this.InGame = inGame;
    this.Players = players;
    this.ChatHistory = chatHistory;
  }
}
