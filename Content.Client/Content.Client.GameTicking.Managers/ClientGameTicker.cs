using System;
using System.Collections.Generic;
using Content.Client._CIV14merka.Lobby;
using Content.Client._CIV14merka.ModeMenu;
using Content.Client._PUBG.Events;
using Content.Client._PUBG.Lobby;
using Content.Client.Administration.Managers;
using Content.Client.Gameplay;
using Content.Client.Lobby;
using Content.Client.RoundEnd;
using Content.Shared._CIV14merka;
using Content.Shared.GameTicking;
using Content.Shared.GameWindow;
using Content.Shared.Roles;
using Robust.Client.Graphics;
using Robust.Client.State;
using Robust.Client.UserInterface;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Prototypes;
using Robust.Shared.ViewVariables;

namespace Content.Client.GameTicking.Managers;

public sealed class ClientGameTicker : SharedGameTicker
{
	[Dependency]
	private IStateManager _stateManager;

	[Dependency]
	private IClientAdminManager _admin;

	[Dependency]
	private IClyde _clyde;

	[Dependency]
	private IUserInterfaceManager _userInterfaceManager;

	private Dictionary<NetEntity, Dictionary<ProtoId<JobPrototype>, int?>> _jobsAvailable = new Dictionary<NetEntity, Dictionary<ProtoId<JobPrototype>, int?>>();

	private Dictionary<NetEntity, string> _stationNames = new Dictionary<NetEntity, string>();

	[ViewVariables]
	public bool AreWeReady { get; private set; }

	[ViewVariables]
	public bool IsGameStarted { get; private set; }

	[ViewVariables]
	public ResolvedSoundSpecifier? RestartSound { get; private set; }

	[ViewVariables]
	public string? LobbyBackground { get; private set; }

	[ViewVariables]
	public bool DisallowedLateJoin { get; private set; }

	[ViewVariables]
	public string? ServerInfoBlob { get; private set; }

	[ViewVariables]
	public TimeSpan StartTime { get; private set; }

	[ViewVariables]
	public bool Paused { get; private set; }

	[ViewVariables]
	public int ServerOnlineCount { get; private set; }

	[ViewVariables]
	public int PubgModeOnlineCount { get; private set; }

	[ViewVariables]
	public int Civ14ModeOnlineCount { get; private set; }

	[ViewVariables]
	public bool IsPubgModeSelectable { get; private set; } = true;

	[ViewVariables]
	public bool IsCiv14ModeSelectable { get; private set; } = true;

	[ViewVariables]
	public TimeSpan Civ14StartTime { get; private set; }

	[ViewVariables]
	public bool IsCiv14CountdownActive { get; private set; }

	[ViewVariables]
	public int Civ14ReadyCount { get; private set; }

	[ViewVariables]
	public string Civ14ModeName { get; private set; } = "Захват базы";

	[ViewVariables]
	public string Civ14MapName { get; private set; } = string.Empty;

	[ViewVariables]
	public bool IsCiv14MapRandom { get; private set; } = true;

	[ViewVariables]
	public IReadOnlyDictionary<NetEntity, Dictionary<ProtoId<JobPrototype>, int?>> JobsAvailable => _jobsAvailable;

	[ViewVariables]
	public IReadOnlyDictionary<NetEntity, string> StationNames => _stationNames;

	public event Action? InfoBlobUpdated;

	public event Action? LobbyStatusUpdated;

	public event Action? LobbyLateJoinStatusUpdated;

	public event Action? ModeMenuStatusUpdated;

	public event Action? Civ14LobbyStatusUpdated;

	public event Action<IReadOnlyDictionary<NetEntity, Dictionary<ProtoId<JobPrototype>, int?>>>? LobbyJobsAvailableUpdated;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeNetworkEvent<TickerJoinLobbyEvent>((EntityEventHandler<TickerJoinLobbyEvent>)JoinLobby, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<TickerJoinGameEvent>((EntityEventHandler<TickerJoinGameEvent>)JoinGame, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<TickerConnectionStatusEvent>((EntityEventHandler<TickerConnectionStatusEvent>)ConnectionStatus, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<TickerLobbyStatusEvent>((EntityEventHandler<TickerLobbyStatusEvent>)LobbyStatus, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<TickerLobbyInfoEvent>((EntityEventHandler<TickerLobbyInfoEvent>)LobbyInfo, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<TickerLobbyCountdownEvent>((EntityEventHandler<TickerLobbyCountdownEvent>)LobbyCountdown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<RoundEndMessageEvent>((EntityEventHandler<RoundEndMessageEvent>)RoundEnd, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<RequestWindowAttentionEvent>((EntityEventHandler<RequestWindowAttentionEvent>)OnAttentionRequest, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<TickerLateJoinStatusEvent>((EntityEventHandler<TickerLateJoinStatusEvent>)LateJoinStatus, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<TickerJobsAvailableEvent>((EntityEventHandler<TickerJobsAvailableEvent>)UpdateJobsAvailable, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<ModeMenuStatusEvent>((EntityEventHandler<ModeMenuStatusEvent>)OnModeMenuStatus, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<CivLobbyStatusEvent>((EntityEventHandler<CivLobbyStatusEvent>)OnCiv14LobbyStatus, (Type[])null, (Type[])null);
		_admin.AdminStatusUpdated += OnAdminUpdated;
		OnAdminUpdated();
	}

	public override void Shutdown()
	{
		_admin.AdminStatusUpdated -= OnAdminUpdated;
		base.Shutdown();
	}

	private void OnAdminUpdated()
	{
		((EntitySystem)((EntitySystem)this).EntityManager.System<SharedMapSystem>()).Log.Level = (LogLevel)(_admin.IsAdmin() ? 2 : 3);
	}

	private void OnAttentionRequest(RequestWindowAttentionEvent ev)
	{
		_clyde.RequestWindowAttention();
	}

	private void LateJoinStatus(TickerLateJoinStatusEvent message)
	{
		DisallowedLateJoin = message.Disallowed;
		this.LobbyLateJoinStatusUpdated?.Invoke();
	}

	private void UpdateJobsAvailable(TickerJobsAvailableEvent message)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		_jobsAvailable.Clear();
		foreach (var (key, value) in message.JobsAvailableByStation)
		{
			_jobsAvailable[key] = value;
		}
		_stationNames.Clear();
		foreach (KeyValuePair<NetEntity, string> stationName in message.StationNames)
		{
			_stationNames[stationName.Key] = stationName.Value;
		}
		this.LobbyJobsAvailableUpdated?.Invoke(JobsAvailable);
	}

	private void JoinLobby(TickerJoinLobbyEvent message)
	{
		OpenModeMenu();
	}

	public void OpenModeMenu()
	{
		_stateManager.RequestStateChange<ModeSelectState>();
	}

	public void ClearSelectedModeAndOpenModeMenu()
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new ModeMenuSelectRequestEvent(ModeMenuMode.None));
		OpenModeMenu();
	}

	public void SelectPubgMode()
	{
		if (IsPubgModeSelectable)
		{
			((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new ModeMenuSelectRequestEvent(ModeMenuMode.Pubg));
			OpenPubgLobby();
		}
	}

	public void SelectCiv14Mode()
	{
		if (IsCiv14ModeSelectable)
		{
			((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new ModeMenuSelectRequestEvent(ModeMenuMode.Civ14));
			OpenCivLobby();
		}
	}

	public void OpenPubgLobby()
	{
		if (_stateManager.CurrentState is GameplayState)
		{
			if (ShouldOpenPreLobbyHubForCompletedTasks())
			{
				_stateManager.RequestStateChange<PubgPreLobbyHubState>();
			}
			else
			{
				_stateManager.RequestStateChange<LobbyState>();
			}
		}
		else
		{
			_stateManager.RequestStateChange<PubgPreLobbyHubState>();
		}
	}

	public void OpenCivLobby()
	{
		_stateManager.RequestStateChange<CivLobbyState>();
	}

	private bool ShouldOpenPreLobbyHubForCompletedTasks()
	{
		PubgEventsSystem pubgEventsSystem = ((EntitySystem)this).EntityManager.SystemOrNull<PubgEventsSystem>();
		if (pubgEventsSystem != null && pubgEventsSystem.HasClaimableTasksInCache())
		{
			return true;
		}
		return false;
	}

	private void ConnectionStatus(TickerConnectionStatusEvent message)
	{
		base.RoundStartTimeSpan = message.RoundStartTimeSpan;
	}

	private void LobbyStatus(TickerLobbyStatusEvent message)
	{
		StartTime = message.StartTime;
		base.RoundStartTimeSpan = message.RoundStartTimeSpan;
		IsGameStarted = message.IsRoundStarted;
		AreWeReady = message.YouAreReady;
		LobbyBackground = message.LobbyBackground;
		Paused = message.Paused;
		this.LobbyStatusUpdated?.Invoke();
	}

	private void LobbyInfo(TickerLobbyInfoEvent message)
	{
		ServerInfoBlob = message.TextBlob;
		this.InfoBlobUpdated?.Invoke();
	}

	private void OnModeMenuStatus(ModeMenuStatusEvent message)
	{
		ServerOnlineCount = message.ServerOnlineCount;
		PubgModeOnlineCount = message.PubgOnlineCount;
		Civ14ModeOnlineCount = message.Civ14OnlineCount;
		IsPubgModeSelectable = message.PubgEnabled;
		IsCiv14ModeSelectable = message.Civ14Enabled;
		this.ModeMenuStatusUpdated?.Invoke();
	}

	private void OnCiv14LobbyStatus(CivLobbyStatusEvent message)
	{
		Civ14StartTime = message.StartTime;
		IsCiv14CountdownActive = message.CountdownActive;
		Civ14ReadyCount = message.ReadyCount;
		Civ14ModeName = message.RoundModeName;
		Civ14MapName = message.MapName;
		IsCiv14MapRandom = message.RandomMapSelection;
		this.Civ14LobbyStatusUpdated?.Invoke();
	}

	private void JoinGame(TickerJoinGameEvent message)
	{
		_stateManager.RequestStateChange<GameplayState>();
	}

	private void LobbyCountdown(TickerLobbyCountdownEvent message)
	{
		StartTime = message.StartTime;
		Paused = message.Paused;
	}

	private void RoundEnd(RoundEndMessageEvent message)
	{
		RestartSound = message.RestartSound;
		_userInterfaceManager.GetUIController<RoundEndSummaryUIController>().OpenRoundEndSummaryWindow(message);
	}
}
