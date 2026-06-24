using System.Collections.Generic;
using System.Linq;
using Content.Client._PUBG.MiniGames;
using Content.Client.Gameplay;
using Content.Shared._PUBG.MiniGames;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Localization;
using Robust.Shared.Network;

namespace Content.Client._PUBG.UserInterface.MiniGames;

public sealed class MiniGameLobbyUIController : UIController, IOnStateEntered<GameplayState>, IOnStateExited<GameplayState>
{
	private MiniGameLobbyWindow? _window;

	private MiniGameArenaSelectWindow? _arenaSelectWindow;

	private bool _systemSubscribed;

	private bool _arenaSystemSubscribed;

	private MiniGameCustomArenaClientSystem ArenaSystem => base.EntityManager.System<MiniGameCustomArenaClientSystem>();

	private void EnsureSystemSubscribed()
	{
		if (!_systemSubscribed)
		{
			MiniGameLobbyClientSystem miniGameLobbyClientSystem = base.EntityManager.System<MiniGameLobbyClientSystem>();
			miniGameLobbyClientSystem.OnLobbyListReceived += OnLobbyList;
			miniGameLobbyClientSystem.OnLobbyStateReceived += OnLobbyState;
			miniGameLobbyClientSystem.OnLobbyChatReceived += OnLobbyChat;
			miniGameLobbyClientSystem.OnLobbyErrorReceived += OnLobbyError;
			miniGameLobbyClientSystem.OnLobbyClosedReceived += OnLobbyClosed;
			miniGameLobbyClientSystem.OnLobbyOpenReceived += OnLobbyOpen;
			miniGameLobbyClientSystem.MembershipChanged += OnMembershipChanged;
			_systemSubscribed = true;
		}
	}

	private void EnsureArenaSystemSubscribed()
	{
		if (!_arenaSystemSubscribed)
		{
			ArenaSystem.OnArenaListUpdated += OnArenaListUpdated;
			ArenaSystem.OnError += OnArenaError;
			ArenaSystem.OnUIOpen += OnArenaUIOpen;
			_arenaSystemSubscribed = true;
		}
	}

	public void OnStateEntered(GameplayState state)
	{
		EnsureSystemSubscribed();
		EnsureArenaSystemSubscribed();
	}

	public void OnStateExited(GameplayState state)
	{
		CloseWindow();
		CloseArenaSelectWindow();
		UnsubscribeFromSystem();
		UnsubscribeFromArenaSystem();
	}

	private void UnsubscribeFromSystem()
	{
		if (_systemSubscribed)
		{
			MiniGameLobbyClientSystem miniGameLobbyClientSystem = base.EntityManager.SystemOrNull<MiniGameLobbyClientSystem>();
			if (miniGameLobbyClientSystem != null)
			{
				miniGameLobbyClientSystem.OnLobbyListReceived -= OnLobbyList;
				miniGameLobbyClientSystem.OnLobbyStateReceived -= OnLobbyState;
				miniGameLobbyClientSystem.OnLobbyChatReceived -= OnLobbyChat;
				miniGameLobbyClientSystem.OnLobbyErrorReceived -= OnLobbyError;
				miniGameLobbyClientSystem.OnLobbyClosedReceived -= OnLobbyClosed;
				miniGameLobbyClientSystem.OnLobbyOpenReceived -= OnLobbyOpen;
				miniGameLobbyClientSystem.MembershipChanged -= OnMembershipChanged;
			}
			_systemSubscribed = false;
		}
	}

	private void UnsubscribeFromArenaSystem()
	{
		if (_arenaSystemSubscribed)
		{
			MiniGameCustomArenaClientSystem miniGameCustomArenaClientSystem = base.EntityManager.SystemOrNull<MiniGameCustomArenaClientSystem>();
			if (miniGameCustomArenaClientSystem != null)
			{
				miniGameCustomArenaClientSystem.OnArenaListUpdated -= OnArenaListUpdated;
				miniGameCustomArenaClientSystem.OnError -= OnArenaError;
				miniGameCustomArenaClientSystem.OnUIOpen -= OnArenaUIOpen;
			}
			_arenaSystemSubscribed = false;
		}
	}

	public void ToggleWindow()
	{
		if (_window == null)
		{
			EnsureWindow();
			MiniGameLobbyWindow? window = _window;
			if (window != null)
			{
				((BaseWindow)window).OpenCentered();
			}
			RequestLobbyList();
			ArenaSystem.RequestArenaList();
		}
		else
		{
			CloseWindow();
		}
	}

	public void OpenWindow()
	{
		if (_window == null)
		{
			EnsureWindow();
			MiniGameLobbyWindow? window = _window;
			if (window != null)
			{
				((BaseWindow)window).OpenCentered();
			}
			RequestLobbyList();
			ArenaSystem.RequestArenaList();
		}
	}

	private void EnsureWindow()
	{
		if (_window == null)
		{
			_window = base.UIManager.CreateWindow<MiniGameLobbyWindow>();
			((BaseWindow)_window).OnClose += OnWindowClosed;
			_window.OnRequestList += RequestLobbyList;
			_window.OnRequestDetails += RequestLobbyDetails;
			_window.OnCreateLobby += OnCreateLobby;
			_window.OnJoinLobby += OnJoinLobby;
			_window.OnLeaveLobby += OnLeaveLobby;
			_window.OnStartLobby += OnStartLobby;
			_window.OnSetLock += OnSetLock;
			_window.OnSetRounds += OnSetRounds;
			_window.OnKickPlayer += OnKickPlayer;
			_window.OnSendChat += OnSendChat;
			_window.OnCustomArena += OnCustomArena;
			MiniGameLobbyClientSystem miniGameLobbyClientSystem = base.EntityManager.System<MiniGameLobbyClientSystem>();
			_window.UpdateMembership(miniGameLobbyClientSystem.IsInLobby, miniGameLobbyClientSystem.CurrentLobbyId);
		}
	}

	private void OnWindowClosed()
	{
		CloseWindow();
	}

	private void CloseWindow()
	{
		if (_window != null)
		{
			_window.CloseCreateWindow();
			((BaseWindow)_window).OnClose -= OnWindowClosed;
			_window.OnRequestList -= RequestLobbyList;
			_window.OnRequestDetails -= RequestLobbyDetails;
			_window.OnCreateLobby -= OnCreateLobby;
			_window.OnJoinLobby -= OnJoinLobby;
			_window.OnLeaveLobby -= OnLeaveLobby;
			_window.OnStartLobby -= OnStartLobby;
			_window.OnSetLock -= OnSetLock;
			_window.OnSetRounds -= OnSetRounds;
			_window.OnKickPlayer -= OnKickPlayer;
			_window.OnSendChat -= OnSendChat;
			_window.OnCustomArena -= OnCustomArena;
			((BaseWindow)_window).Close();
			_window = null;
		}
	}

	private void CloseArenaSelectWindow()
	{
		if (_arenaSelectWindow != null)
		{
			((BaseWindow)_arenaSelectWindow).Close();
			_arenaSelectWindow = null;
		}
	}

	private void RequestLobbyList()
	{
		base.EntityManager.System<MiniGameLobbyClientSystem>().RequestLobbyList();
	}

	private void RequestLobbyDetails(int lobbyId)
	{
		base.EntityManager.System<MiniGameLobbyClientSystem>().RequestLobbyDetails(lobbyId);
	}

	private void OnCreateLobby(string name, string gameId, string submodeId, string mapId, int rounds, int maxPlayers, bool isLocked, string password)
	{
		base.EntityManager.System<MiniGameLobbyClientSystem>().CreateLobby(name, gameId, submodeId, mapId, rounds, maxPlayers, isLocked, password);
	}

	private void OnJoinLobby(int lobbyId, string password)
	{
		base.EntityManager.System<MiniGameLobbyClientSystem>().JoinLobby(lobbyId, password);
	}

	private void OnLeaveLobby(int lobbyId)
	{
		base.EntityManager.System<MiniGameLobbyClientSystem>().LeaveLobby(lobbyId);
	}

	private void OnStartLobby(int lobbyId)
	{
		base.EntityManager.System<MiniGameLobbyClientSystem>().StartLobby(lobbyId);
	}

	private void OnSetLock(int lobbyId, bool isLocked, string password, bool updatePassword)
	{
		base.EntityManager.System<MiniGameLobbyClientSystem>().SetLobbyLock(lobbyId, isLocked, password, updatePassword);
	}

	private void OnSetRounds(int lobbyId, int rounds)
	{
		base.EntityManager.System<MiniGameLobbyClientSystem>().SetLobbyRounds(lobbyId, rounds);
	}

	private void OnKickPlayer(int lobbyId, NetUserId targetUserId)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		base.EntityManager.System<MiniGameLobbyClientSystem>().KickPlayer(lobbyId, targetUserId);
	}

	private void OnSendChat(int lobbyId, string text)
	{
		base.EntityManager.System<MiniGameLobbyClientSystem>().SendChat(lobbyId, text);
	}

	private void OnCustomArena()
	{
		EnsureArenaSelectWindow();
		_arenaSelectWindow?.UpdateArenaList(ArenaSystem.CachedArenas.ToList());
		MiniGameArenaSelectWindow? arenaSelectWindow = _arenaSelectWindow;
		if (arenaSelectWindow != null)
		{
			((BaseWindow)arenaSelectWindow).OpenCentered();
		}
		ArenaSystem.RequestArenaList();
	}

	private void OnLobbyList(MiniGameLobbyListMessage msg)
	{
		_window?.UpdateLobbyList(msg.Lobbies);
	}

	private void OnLobbyState(MiniGameLobbyStateMessage msg)
	{
		if (msg.InGame && _window != null)
		{
			CloseWindow();
			return;
		}
		if (_window == null && !msg.InGame && msg.CurrentRound > 0)
		{
			EnsureWindow();
			MiniGameLobbyWindow? window = _window;
			if (window != null)
			{
				((BaseWindow)window).OpenCentered();
			}
		}
		_window?.UpdateLobbyState(msg);
	}

	private void OnLobbyChat(MiniGameLobbyChatMessage msg)
	{
		_window?.AppendChatMessage(msg.LobbyId, msg.Entry);
	}

	private void OnLobbyError(MiniGameLobbyErrorMessage msg)
	{
		_window?.ShowError(msg.LobbyId, Loc.GetString(msg.ErrorKey));
	}

	private void OnLobbyClosed(MiniGameLobbyClosedMessage msg)
	{
		_window?.HandleLobbyClosed(msg.LobbyId);
	}

	private void OnLobbyOpen(bool canCustomize)
	{
		OpenWindow();
		_window?.UpdateCustomArenaPermission(canCustomize);
	}

	private void OnMembershipChanged()
	{
		MiniGameLobbyClientSystem miniGameLobbyClientSystem = base.EntityManager.System<MiniGameLobbyClientSystem>();
		_window?.UpdateMembership(miniGameLobbyClientSystem.IsInLobby, miniGameLobbyClientSystem.CurrentLobbyId);
	}

	private void EnsureArenaSelectWindow()
	{
		if (_arenaSelectWindow == null)
		{
			_arenaSelectWindow = base.UIManager.CreateWindow<MiniGameArenaSelectWindow>();
			_arenaSelectWindow.OnSelectArena += delegate(string arenaName)
			{
				ArenaSystem.RequestEnterCustomization(arenaName);
			};
			_arenaSelectWindow.OnCreateNew += delegate
			{
				ArenaSystem.RequestEnterCustomization();
			};
		}
	}

	private void OnArenaListUpdated(List<MiniGameArenaInfo> arenas, int maxArenas)
	{
		_arenaSelectWindow?.UpdateArenaList(arenas);
		_window?.UpdateCustomArenas(arenas);
	}

	private void OnArenaError(string errorLocKey)
	{
		_arenaSelectWindow?.ShowError(errorLocKey);
	}

	private void OnArenaUIOpen()
	{
		CloseArenaSelectWindow();
		CloseWindow();
	}
}
