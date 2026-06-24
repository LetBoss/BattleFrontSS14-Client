// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.MiniGames.MiniGameLobbyClientSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._PUBG.MiniGames;
using Robust.Shared.GameObjects;
using Robust.Shared.Network;
using System;

#nullable enable
namespace Content.Client._PUBG.MiniGames;

public sealed class MiniGameLobbyClientSystem : EntitySystem
{
  public event Action<MiniGameLobbyListMessage>? OnLobbyListReceived;

  public event Action<MiniGameLobbyStateMessage>? OnLobbyStateReceived;

  public event Action<MiniGameLobbyChatMessage>? OnLobbyChatReceived;

  public event Action<MiniGameLobbyErrorMessage>? OnLobbyErrorReceived;

  public event Action<MiniGameLobbyClosedMessage>? OnLobbyClosedReceived;

  public event Action<bool>? OnLobbyOpenReceived;

  public event Action? MembershipChanged;

  public bool IsInLobby { get; private set; }

  public int CurrentLobbyId { get; private set; }

  public bool CanCustomizeArenas { get; private set; }

  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeNetworkEvent<MiniGameLobbyListMessage>(new EntityEventHandler<MiniGameLobbyListMessage>(this.OnLobbyList), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<MiniGameLobbyStateMessage>(new EntityEventHandler<MiniGameLobbyStateMessage>(this.OnLobbyState), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<MiniGameLobbyChatMessage>(new EntityEventHandler<MiniGameLobbyChatMessage>(this.OnLobbyChat), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<MiniGameLobbyErrorMessage>(new EntityEventHandler<MiniGameLobbyErrorMessage>(this.OnLobbyError), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<MiniGameLobbyClosedMessage>(new EntityEventHandler<MiniGameLobbyClosedMessage>(this.OnLobbyClosed), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<MiniGameLobbyMembershipMessage>(new EntityEventHandler<MiniGameLobbyMembershipMessage>(this.OnMembershipChanged), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<MiniGameLobbyOpenMessage>(new EntityEventHandler<MiniGameLobbyOpenMessage>(this.OnLobbyOpen), (Type[]) null, (Type[]) null);
  }

  public void RequestLobbyList()
  {
    this.RaiseNetworkEvent((EntityEventArgs) new MiniGameLobbyListRequestMessage());
  }

  public void RequestLobbyDetails(int lobbyId)
  {
    this.RaiseNetworkEvent((EntityEventArgs) new MiniGameLobbyDetailsRequestMessage(lobbyId));
  }

  public void CreateLobby(
    string name,
    string gameId,
    string submodeId,
    string mapId,
    int rounds,
    int maxPlayers,
    bool isLocked,
    string password)
  {
    this.RaiseNetworkEvent((EntityEventArgs) new MiniGameLobbyCreateMessage(name, gameId, submodeId, mapId, rounds, maxPlayers, isLocked, password));
  }

  public void JoinLobby(int lobbyId, string password)
  {
    this.RaiseNetworkEvent((EntityEventArgs) new MiniGameLobbyJoinMessage(lobbyId, password));
  }

  public void LeaveLobby(int lobbyId)
  {
    this.RaiseNetworkEvent((EntityEventArgs) new MiniGameLobbyLeaveMessage(lobbyId));
  }

  public void KickPlayer(int lobbyId, NetUserId targetUserId)
  {
    this.RaiseNetworkEvent((EntityEventArgs) new MiniGameLobbyKickMessage(lobbyId, targetUserId));
  }

  public void StartLobby(int lobbyId)
  {
    this.RaiseNetworkEvent((EntityEventArgs) new MiniGameLobbyStartMessage(lobbyId));
  }

  public void SetLobbyLock(int lobbyId, bool isLocked, string password, bool updatePassword)
  {
    this.RaiseNetworkEvent((EntityEventArgs) new MiniGameLobbySetLockMessage(lobbyId, isLocked, password, updatePassword));
  }

  public void SetLobbyRounds(int lobbyId, int rounds)
  {
    this.RaiseNetworkEvent((EntityEventArgs) new MiniGameLobbySetRoundsMessage(lobbyId, rounds));
  }

  public void SendChat(int lobbyId, string text)
  {
    this.RaiseNetworkEvent((EntityEventArgs) new MiniGameLobbyChatSendMessage(lobbyId, text));
  }

  private void OnLobbyList(MiniGameLobbyListMessage msg)
  {
    Action<MiniGameLobbyListMessage> lobbyListReceived = this.OnLobbyListReceived;
    if (lobbyListReceived == null)
      return;
    lobbyListReceived(msg);
  }

  private void OnLobbyState(MiniGameLobbyStateMessage msg)
  {
    Action<MiniGameLobbyStateMessage> lobbyStateReceived = this.OnLobbyStateReceived;
    if (lobbyStateReceived == null)
      return;
    lobbyStateReceived(msg);
  }

  private void OnLobbyChat(MiniGameLobbyChatMessage msg)
  {
    Action<MiniGameLobbyChatMessage> lobbyChatReceived = this.OnLobbyChatReceived;
    if (lobbyChatReceived == null)
      return;
    lobbyChatReceived(msg);
  }

  private void OnLobbyError(MiniGameLobbyErrorMessage msg)
  {
    Action<MiniGameLobbyErrorMessage> lobbyErrorReceived = this.OnLobbyErrorReceived;
    if (lobbyErrorReceived == null)
      return;
    lobbyErrorReceived(msg);
  }

  private void OnLobbyClosed(MiniGameLobbyClosedMessage msg)
  {
    if (this.IsInLobby && this.CurrentLobbyId == msg.LobbyId)
    {
      this.IsInLobby = false;
      this.CurrentLobbyId = 0;
      Action membershipChanged = this.MembershipChanged;
      if (membershipChanged != null)
        membershipChanged();
    }
    Action<MiniGameLobbyClosedMessage> lobbyClosedReceived = this.OnLobbyClosedReceived;
    if (lobbyClosedReceived == null)
      return;
    lobbyClosedReceived(msg);
  }

  private void OnLobbyOpen(MiniGameLobbyOpenMessage msg)
  {
    this.CanCustomizeArenas = msg.CanCustomizeArenas;
    Action<bool> lobbyOpenReceived = this.OnLobbyOpenReceived;
    if (lobbyOpenReceived == null)
      return;
    lobbyOpenReceived(this.CanCustomizeArenas);
  }

  private void OnMembershipChanged(MiniGameLobbyMembershipMessage msg)
  {
    this.IsInLobby = msg.IsInLobby;
    this.CurrentLobbyId = msg.LobbyId;
    Action membershipChanged = this.MembershipChanged;
    if (membershipChanged == null)
      return;
    membershipChanged();
  }
}
