// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.UserInterface.MiniGames.MiniGameLobbyUIController
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._PUBG.MiniGames;
using Content.Client.Gameplay;
using Content.Shared._PUBG.MiniGames;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Localization;
using Robust.Shared.Network;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Client._PUBG.UserInterface.MiniGames;

public sealed class MiniGameLobbyUIController : 
  UIController,
  IOnStateEntered<GameplayState>,
  IOnStateExited<GameplayState>
{
  private MiniGameLobbyWindow? _window;
  private MiniGameArenaSelectWindow? _arenaSelectWindow;
  private bool _systemSubscribed;
  private bool _arenaSystemSubscribed;

  private MiniGameCustomArenaClientSystem ArenaSystem
  {
    get => this.EntityManager.System<MiniGameCustomArenaClientSystem>();
  }

  private void EnsureSystemSubscribed()
  {
    if (this._systemSubscribed)
      return;
    MiniGameLobbyClientSystem lobbyClientSystem = this.EntityManager.System<MiniGameLobbyClientSystem>();
    lobbyClientSystem.OnLobbyListReceived += new Action<MiniGameLobbyListMessage>(this.OnLobbyList);
    lobbyClientSystem.OnLobbyStateReceived += new Action<MiniGameLobbyStateMessage>(this.OnLobbyState);
    lobbyClientSystem.OnLobbyChatReceived += new Action<MiniGameLobbyChatMessage>(this.OnLobbyChat);
    lobbyClientSystem.OnLobbyErrorReceived += new Action<MiniGameLobbyErrorMessage>(this.OnLobbyError);
    lobbyClientSystem.OnLobbyClosedReceived += new Action<MiniGameLobbyClosedMessage>(this.OnLobbyClosed);
    lobbyClientSystem.OnLobbyOpenReceived += new Action<bool>(this.OnLobbyOpen);
    lobbyClientSystem.MembershipChanged += new Action(this.OnMembershipChanged);
    this._systemSubscribed = true;
  }

  private void EnsureArenaSystemSubscribed()
  {
    if (this._arenaSystemSubscribed)
      return;
    this.ArenaSystem.OnArenaListUpdated += new Action<List<MiniGameArenaInfo>, int>(this.OnArenaListUpdated);
    this.ArenaSystem.OnError += new Action<string>(this.OnArenaError);
    this.ArenaSystem.OnUIOpen += new Action(this.OnArenaUIOpen);
    this._arenaSystemSubscribed = true;
  }

  public void OnStateEntered(GameplayState state)
  {
    this.EnsureSystemSubscribed();
    this.EnsureArenaSystemSubscribed();
  }

  public void OnStateExited(GameplayState state)
  {
    this.CloseWindow();
    this.CloseArenaSelectWindow();
    this.UnsubscribeFromSystem();
    this.UnsubscribeFromArenaSystem();
  }

  private void UnsubscribeFromSystem()
  {
    if (!this._systemSubscribed)
      return;
    MiniGameLobbyClientSystem lobbyClientSystem = this.EntityManager.SystemOrNull<MiniGameLobbyClientSystem>();
    if (lobbyClientSystem != null)
    {
      lobbyClientSystem.OnLobbyListReceived -= new Action<MiniGameLobbyListMessage>(this.OnLobbyList);
      lobbyClientSystem.OnLobbyStateReceived -= new Action<MiniGameLobbyStateMessage>(this.OnLobbyState);
      lobbyClientSystem.OnLobbyChatReceived -= new Action<MiniGameLobbyChatMessage>(this.OnLobbyChat);
      lobbyClientSystem.OnLobbyErrorReceived -= new Action<MiniGameLobbyErrorMessage>(this.OnLobbyError);
      lobbyClientSystem.OnLobbyClosedReceived -= new Action<MiniGameLobbyClosedMessage>(this.OnLobbyClosed);
      lobbyClientSystem.OnLobbyOpenReceived -= new Action<bool>(this.OnLobbyOpen);
      lobbyClientSystem.MembershipChanged -= new Action(this.OnMembershipChanged);
    }
    this._systemSubscribed = false;
  }

  private void UnsubscribeFromArenaSystem()
  {
    if (!this._arenaSystemSubscribed)
      return;
    MiniGameCustomArenaClientSystem arenaClientSystem = this.EntityManager.SystemOrNull<MiniGameCustomArenaClientSystem>();
    if (arenaClientSystem != null)
    {
      arenaClientSystem.OnArenaListUpdated -= new Action<List<MiniGameArenaInfo>, int>(this.OnArenaListUpdated);
      arenaClientSystem.OnError -= new Action<string>(this.OnArenaError);
      arenaClientSystem.OnUIOpen -= new Action(this.OnArenaUIOpen);
    }
    this._arenaSystemSubscribed = false;
  }

  public void ToggleWindow()
  {
    if (this._window == null)
    {
      this.EnsureWindow();
      ((BaseWindow) this._window)?.OpenCentered();
      this.RequestLobbyList();
      this.ArenaSystem.RequestArenaList();
    }
    else
      this.CloseWindow();
  }

  public void OpenWindow()
  {
    if (this._window != null)
      return;
    this.EnsureWindow();
    ((BaseWindow) this._window)?.OpenCentered();
    this.RequestLobbyList();
    this.ArenaSystem.RequestArenaList();
  }

  private void EnsureWindow()
  {
    if (this._window != null)
      return;
    this._window = this.UIManager.CreateWindow<MiniGameLobbyWindow>();
    ((BaseWindow) this._window).OnClose += new Action(this.OnWindowClosed);
    this._window.OnRequestList += new Action(this.RequestLobbyList);
    this._window.OnRequestDetails += new Action<int>(this.RequestLobbyDetails);
    this._window.OnCreateLobby += new Action<string, string, string, string, int, int, bool, string>(this.OnCreateLobby);
    this._window.OnJoinLobby += new Action<int, string>(this.OnJoinLobby);
    this._window.OnLeaveLobby += new Action<int>(this.OnLeaveLobby);
    this._window.OnStartLobby += new Action<int>(this.OnStartLobby);
    this._window.OnSetLock += new Action<int, bool, string, bool>(this.OnSetLock);
    this._window.OnSetRounds += new Action<int, int>(this.OnSetRounds);
    this._window.OnKickPlayer += new Action<int, NetUserId>(this.OnKickPlayer);
    this._window.OnSendChat += new Action<int, string>(this.OnSendChat);
    this._window.OnCustomArena += new Action(this.OnCustomArena);
    MiniGameLobbyClientSystem lobbyClientSystem = this.EntityManager.System<MiniGameLobbyClientSystem>();
    this._window.UpdateMembership(lobbyClientSystem.IsInLobby, lobbyClientSystem.CurrentLobbyId);
  }

  private void OnWindowClosed() => this.CloseWindow();

  private void CloseWindow()
  {
    if (this._window == null)
      return;
    this._window.CloseCreateWindow();
    ((BaseWindow) this._window).OnClose -= new Action(this.OnWindowClosed);
    this._window.OnRequestList -= new Action(this.RequestLobbyList);
    this._window.OnRequestDetails -= new Action<int>(this.RequestLobbyDetails);
    this._window.OnCreateLobby -= new Action<string, string, string, string, int, int, bool, string>(this.OnCreateLobby);
    this._window.OnJoinLobby -= new Action<int, string>(this.OnJoinLobby);
    this._window.OnLeaveLobby -= new Action<int>(this.OnLeaveLobby);
    this._window.OnStartLobby -= new Action<int>(this.OnStartLobby);
    this._window.OnSetLock -= new Action<int, bool, string, bool>(this.OnSetLock);
    this._window.OnSetRounds -= new Action<int, int>(this.OnSetRounds);
    this._window.OnKickPlayer -= new Action<int, NetUserId>(this.OnKickPlayer);
    this._window.OnSendChat -= new Action<int, string>(this.OnSendChat);
    this._window.OnCustomArena -= new Action(this.OnCustomArena);
    ((BaseWindow) this._window).Close();
    this._window = (MiniGameLobbyWindow) null;
  }

  private void CloseArenaSelectWindow()
  {
    if (this._arenaSelectWindow == null)
      return;
    ((BaseWindow) this._arenaSelectWindow).Close();
    this._arenaSelectWindow = (MiniGameArenaSelectWindow) null;
  }

  private void RequestLobbyList()
  {
    this.EntityManager.System<MiniGameLobbyClientSystem>().RequestLobbyList();
  }

  private void RequestLobbyDetails(int lobbyId)
  {
    this.EntityManager.System<MiniGameLobbyClientSystem>().RequestLobbyDetails(lobbyId);
  }

  private void OnCreateLobby(
    string name,
    string gameId,
    string submodeId,
    string mapId,
    int rounds,
    int maxPlayers,
    bool isLocked,
    string password)
  {
    this.EntityManager.System<MiniGameLobbyClientSystem>().CreateLobby(name, gameId, submodeId, mapId, rounds, maxPlayers, isLocked, password);
  }

  private void OnJoinLobby(int lobbyId, string password)
  {
    this.EntityManager.System<MiniGameLobbyClientSystem>().JoinLobby(lobbyId, password);
  }

  private void OnLeaveLobby(int lobbyId)
  {
    this.EntityManager.System<MiniGameLobbyClientSystem>().LeaveLobby(lobbyId);
  }

  private void OnStartLobby(int lobbyId)
  {
    this.EntityManager.System<MiniGameLobbyClientSystem>().StartLobby(lobbyId);
  }

  private void OnSetLock(int lobbyId, bool isLocked, string password, bool updatePassword)
  {
    this.EntityManager.System<MiniGameLobbyClientSystem>().SetLobbyLock(lobbyId, isLocked, password, updatePassword);
  }

  private void OnSetRounds(int lobbyId, int rounds)
  {
    this.EntityManager.System<MiniGameLobbyClientSystem>().SetLobbyRounds(lobbyId, rounds);
  }

  private void OnKickPlayer(int lobbyId, NetUserId targetUserId)
  {
    this.EntityManager.System<MiniGameLobbyClientSystem>().KickPlayer(lobbyId, targetUserId);
  }

  private void OnSendChat(int lobbyId, string text)
  {
    this.EntityManager.System<MiniGameLobbyClientSystem>().SendChat(lobbyId, text);
  }

  private void OnCustomArena()
  {
    this.EnsureArenaSelectWindow();
    this._arenaSelectWindow?.UpdateArenaList(this.ArenaSystem.CachedArenas.ToList<MiniGameArenaInfo>());
    ((BaseWindow) this._arenaSelectWindow)?.OpenCentered();
    this.ArenaSystem.RequestArenaList();
  }

  private void OnLobbyList(MiniGameLobbyListMessage msg)
  {
    this._window?.UpdateLobbyList(msg.Lobbies);
  }

  private void OnLobbyState(MiniGameLobbyStateMessage msg)
  {
    if (msg.InGame && this._window != null)
    {
      this.CloseWindow();
    }
    else
    {
      if (this._window == null && !msg.InGame && msg.CurrentRound > 0)
      {
        this.EnsureWindow();
        ((BaseWindow) this._window)?.OpenCentered();
      }
      this._window?.UpdateLobbyState(msg);
    }
  }

  private void OnLobbyChat(MiniGameLobbyChatMessage msg)
  {
    this._window?.AppendChatMessage(msg.LobbyId, msg.Entry);
  }

  private void OnLobbyError(MiniGameLobbyErrorMessage msg)
  {
    this._window?.ShowError(msg.LobbyId, Loc.GetString(msg.ErrorKey));
  }

  private void OnLobbyClosed(MiniGameLobbyClosedMessage msg)
  {
    this._window?.HandleLobbyClosed(msg.LobbyId);
  }

  private void OnLobbyOpen(bool canCustomize)
  {
    this.OpenWindow();
    this._window?.UpdateCustomArenaPermission(canCustomize);
  }

  private void OnMembershipChanged()
  {
    MiniGameLobbyClientSystem lobbyClientSystem = this.EntityManager.System<MiniGameLobbyClientSystem>();
    this._window?.UpdateMembership(lobbyClientSystem.IsInLobby, lobbyClientSystem.CurrentLobbyId);
  }

  private void EnsureArenaSelectWindow()
  {
    if (this._arenaSelectWindow != null)
      return;
    this._arenaSelectWindow = this.UIManager.CreateWindow<MiniGameArenaSelectWindow>();
    this._arenaSelectWindow.OnSelectArena += (Action<string>) (arenaName => this.ArenaSystem.RequestEnterCustomization(arenaName));
    this._arenaSelectWindow.OnCreateNew += (Action) (() => this.ArenaSystem.RequestEnterCustomization());
  }

  private void OnArenaListUpdated(List<MiniGameArenaInfo> arenas, int maxArenas)
  {
    this._arenaSelectWindow?.UpdateArenaList(arenas);
    this._window?.UpdateCustomArenas(arenas);
  }

  private void OnArenaError(string errorLocKey) => this._arenaSelectWindow?.ShowError(errorLocKey);

  private void OnArenaUIOpen()
  {
    this.CloseArenaSelectWindow();
    this.CloseWindow();
  }
}
