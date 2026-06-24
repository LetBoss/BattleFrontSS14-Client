// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.Lobby.PubgLobbyClientSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._PUBG.Lobby;
using Content.Shared._PUBG.Match;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client._PUBG.Lobby;

public sealed class PubgLobbyClientSystem : EntitySystem
{
  public event Action? LobbyStatusUpdated;

  public bool InLobby { get; private set; }

  public int TotalPlayers { get; private set; }

  public int ReadyPlayers { get; private set; }

  public int TimeRemaining { get; private set; }

  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeNetworkEvent<LobbyStatusEvent>(new EntitySessionEventHandler<LobbyStatusEvent>(this.OnLobbyStatus), (Type[]) null, (Type[]) null);
  }

  public void RequestJoinMode(PubgMatchMode matchMode, bool preferFullSquad)
  {
    this.RaiseNetworkEvent((EntityEventArgs) new LobbyJoinModeMessage(matchMode, preferFullSquad));
  }

  private void OnLobbyStatus(LobbyStatusEvent msg, EntitySessionEventArgs args)
  {
    this.InLobby = msg.InLobby;
    this.TotalPlayers = msg.TotalPlayers;
    this.ReadyPlayers = msg.ReadyPlayers;
    this.TimeRemaining = msg.TimeRemaining;
    Action lobbyStatusUpdated = this.LobbyStatusUpdated;
    if (lobbyStatusUpdated == null)
      return;
    lobbyStatusUpdated();
  }
}
