using System;
using Content.Shared._PUBG.MiniGames;
using Robust.Shared.GameObjects;
using Robust.Shared.Network;

namespace Content.Client._PUBG.MiniGames;

public sealed class MiniGameLobbyClientSystem : EntitySystem
{
	public bool IsInLobby { get; private set; }

	public int CurrentLobbyId { get; private set; }

	public bool CanCustomizeArenas { get; private set; }

	public event Action<MiniGameLobbyListMessage>? OnLobbyListReceived;

	public event Action<MiniGameLobbyStateMessage>? OnLobbyStateReceived;

	public event Action<MiniGameLobbyChatMessage>? OnLobbyChatReceived;

	public event Action<MiniGameLobbyErrorMessage>? OnLobbyErrorReceived;

	public event Action<MiniGameLobbyClosedMessage>? OnLobbyClosedReceived;

	public event Action<bool>? OnLobbyOpenReceived;

	public event Action? MembershipChanged;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeNetworkEvent<MiniGameLobbyListMessage>((EntityEventHandler<MiniGameLobbyListMessage>)OnLobbyList, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<MiniGameLobbyStateMessage>((EntityEventHandler<MiniGameLobbyStateMessage>)OnLobbyState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<MiniGameLobbyChatMessage>((EntityEventHandler<MiniGameLobbyChatMessage>)OnLobbyChat, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<MiniGameLobbyErrorMessage>((EntityEventHandler<MiniGameLobbyErrorMessage>)OnLobbyError, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<MiniGameLobbyClosedMessage>((EntityEventHandler<MiniGameLobbyClosedMessage>)OnLobbyClosed, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<MiniGameLobbyMembershipMessage>((EntityEventHandler<MiniGameLobbyMembershipMessage>)OnMembershipChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<MiniGameLobbyOpenMessage>((EntityEventHandler<MiniGameLobbyOpenMessage>)OnLobbyOpen, (Type[])null, (Type[])null);
	}

	public void RequestLobbyList()
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new MiniGameLobbyListRequestMessage());
	}

	public void RequestLobbyDetails(int lobbyId)
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new MiniGameLobbyDetailsRequestMessage(lobbyId));
	}

	public void CreateLobby(string name, string gameId, string submodeId, string mapId, int rounds, int maxPlayers, bool isLocked, string password)
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new MiniGameLobbyCreateMessage(name, gameId, submodeId, mapId, rounds, maxPlayers, isLocked, password));
	}

	public void JoinLobby(int lobbyId, string password)
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new MiniGameLobbyJoinMessage(lobbyId, password));
	}

	public void LeaveLobby(int lobbyId)
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new MiniGameLobbyLeaveMessage(lobbyId));
	}

	public void KickPlayer(int lobbyId, NetUserId targetUserId)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new MiniGameLobbyKickMessage(lobbyId, targetUserId));
	}

	public void StartLobby(int lobbyId)
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new MiniGameLobbyStartMessage(lobbyId));
	}

	public void SetLobbyLock(int lobbyId, bool isLocked, string password, bool updatePassword)
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new MiniGameLobbySetLockMessage(lobbyId, isLocked, password, updatePassword));
	}

	public void SetLobbyRounds(int lobbyId, int rounds)
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new MiniGameLobbySetRoundsMessage(lobbyId, rounds));
	}

	public void SendChat(int lobbyId, string text)
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new MiniGameLobbyChatSendMessage(lobbyId, text));
	}

	private void OnLobbyList(MiniGameLobbyListMessage msg)
	{
		this.OnLobbyListReceived?.Invoke(msg);
	}

	private void OnLobbyState(MiniGameLobbyStateMessage msg)
	{
		this.OnLobbyStateReceived?.Invoke(msg);
	}

	private void OnLobbyChat(MiniGameLobbyChatMessage msg)
	{
		this.OnLobbyChatReceived?.Invoke(msg);
	}

	private void OnLobbyError(MiniGameLobbyErrorMessage msg)
	{
		this.OnLobbyErrorReceived?.Invoke(msg);
	}

	private void OnLobbyClosed(MiniGameLobbyClosedMessage msg)
	{
		if (IsInLobby && CurrentLobbyId == msg.LobbyId)
		{
			IsInLobby = false;
			CurrentLobbyId = 0;
			this.MembershipChanged?.Invoke();
		}
		this.OnLobbyClosedReceived?.Invoke(msg);
	}

	private void OnLobbyOpen(MiniGameLobbyOpenMessage msg)
	{
		CanCustomizeArenas = msg.CanCustomizeArenas;
		this.OnLobbyOpenReceived?.Invoke(CanCustomizeArenas);
	}

	private void OnMembershipChanged(MiniGameLobbyMembershipMessage msg)
	{
		IsInLobby = msg.IsInLobby;
		CurrentLobbyId = msg.LobbyId;
		this.MembershipChanged?.Invoke();
	}
}
