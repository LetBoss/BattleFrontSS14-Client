// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Lobby.LobbyStatusEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;

#nullable disable
namespace Content.Shared._PUBG.Lobby;

[NetSerializable]
[Serializable]
public sealed class LobbyStatusEvent : EntityEventArgs
{
  public bool InLobby { get; }

  public int TotalPlayers { get; }

  public int ReadyPlayers { get; }

  public int TimeRemaining { get; }

  public LobbyStatusEvent(bool inLobby, int totalPlayers, int readyPlayers, int timeRemaining = 0)
  {
    this.InLobby = inLobby;
    this.TotalPlayers = totalPlayers;
    this.ReadyPlayers = readyPlayers;
    this.TimeRemaining = timeRemaining;
  }
}
