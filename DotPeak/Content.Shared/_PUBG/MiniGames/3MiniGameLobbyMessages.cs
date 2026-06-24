// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.MiniGames.MiniGameLobbyCreateMessage
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared._PUBG.MiniGames;

[NetSerializable]
[Serializable]
public sealed class MiniGameLobbyCreateMessage : EntityEventArgs
{
  public string Name { get; }

  public string GameId { get; }

  public string SubmodeId { get; }

  public string MapId { get; }

  public int Rounds { get; }

  public int MaxPlayers { get; }

  public bool IsLocked { get; }

  public string Password { get; }

  public MiniGameLobbyCreateMessage(
    string name,
    string gameId,
    string submodeId,
    string mapId,
    int rounds,
    int maxPlayers,
    bool isLocked,
    string password)
  {
    this.Name = name;
    this.GameId = gameId;
    this.SubmodeId = submodeId;
    this.MapId = mapId;
    this.Rounds = rounds;
    this.MaxPlayers = maxPlayers;
    this.IsLocked = isLocked;
    this.Password = password;
  }
}
