using System;
using System.Linq;
using Content.Client._CIV14merka.Lobby.UI;
using Content.Client._CIV14merka.UserInterface.Systems.Hud;
using Content.Client.GameTicking.Managers;
using Content.Client.Lobby;
using Content.Client.Message;
using Content.Client.Playtime;
using Content.Client.UserInterface.Systems.Chat;
using Content.Client.Voting;
using Content.Shared._CIV14merka;
using Content.Shared.CCVar;
using Content.Shared.DeadSpace.CCCCVars;
using Robust.Client.Console;
using Robust.Client.ResourceManagement;
using Robust.Client.State;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Configuration;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Timing;

namespace Content.Client._CIV14merka.Lobby;

public sealed class CivLobbyState : State
{
	[Dependency]
	private IClientConsoleHost _consoleHost;

	[Dependency]
	private IEntityManager _entityManager;

	[Dependency]
	private IResourceCache _resourceCache;

	[Dependency]
	private IUserInterfaceManager _userInterfaceManager;

	[Dependency]
	private IGameTiming _gameTiming;

	[Dependency]
	private IVoteManager _voteManager;

	[Dependency]
	private ClientsidePlaytimeTrackingManager _playtimeTracking;

	[Dependency]
	private IConfigurationManager _cfg;

	private ClientGameTicker _gameTicker;

	private CivRosterSystem _rosterSystem;

	private CivHudEventsSystem _hudEvents;

	private CivRosterControl? _rosterControl;

	private TimeSpan _nextBackgroundChange = TimeSpan.Zero;

	private const float BackgroundChangeIntervalSeconds = 300f;

	private CivSettingNoticeWindow? _settingNotice;

	protected override Type? LinkedScreenType { get; } = typeof(CivLobbyGui);

	public CivLobbyGui? Lobby { get; private set; }

	protected override void Startup()
	{
		if (_userInterfaceManager.ActiveScreen != null)
		{
			Lobby = (CivLobbyGui)(object)_userInterfaceManager.ActiveScreen;
			ChatUIController uIController = _userInterfaceManager.GetUIController<ChatUIController>();
			_gameTicker = _entityManager.System<ClientGameTicker>();
			_rosterSystem = _entityManager.System<CivRosterSystem>();
			_hudEvents = _entityManager.System<CivHudEventsSystem>();
			uIController.SetMainChat(setting: true);
			_voteManager.SetPopupContainer((Control)(object)Lobby.VoteContainer);
			LayoutContainer.SetAnchorPreset((Control)(object)Lobby, (LayoutPreset)15, false);
			UpdateLobbyUi();
			UpdateLobbyBackground();
			LoadMainScreen();
			_nextBackgroundChange = _gameTiming.CurTime + TimeSpan.FromSeconds(300.0);
			((BaseButton)Lobby.CharacterSetup).OnPressed += OnSetupPressed;
			((BaseButton)Lobby.ReadyButton).OnPressed += OnReadyPressed;
			((BaseButton)Lobby.ReadyButton).OnToggled += OnReadyToggled;
			((BaseButton)Lobby.ModeMenuButton).OnPressed += OnModeMenuPressed;
			((BaseButton)Lobby.AllowAutoLeaderCheck).OnToggled += OnAllowAutoLeaderToggled;
			CivRosterControl civRosterControl = new CivRosterControl();
			((Control)civRosterControl).HorizontalExpand = true;
			((Control)civRosterControl).VerticalExpand = true;
			_rosterControl = civRosterControl;
			((Control)Lobby.RosterHost).AddChild((Control)(object)_rosterControl);
			_rosterSystem.AttachInlineControl(_rosterControl);
			_gameTicker.InfoBlobUpdated += UpdateLobbyUi;
			_gameTicker.LobbyStatusUpdated += LobbyStatusUpdated;
			_gameTicker.Civ14LobbyStatusUpdated += UpdateLobbyUi;
			_rosterSystem.StateUpdated += OnRosterStateUpdated;
			Lobby.UpdateAssignment(_rosterSystem.GetState());
			_rosterSystem.RequestState();
			_cfg.OnValueChanged<string>(CCCCVars.Background, (Action<string>)OnBackgroundChanged, true);
			TryShowSettingNotices();
		}
	}

	private void TryShowSettingNotices()
	{
		if (_settingNotice == null && !_cfg.GetCVar<bool>(CCVars.Civ14ForeignNamesPromptSeen))
		{
			_settingNotice = new CivSettingNoticeWindow(Loc.GetString("civ-setting-notice-foreign-names-title"), Loc.GetString("civ-setting-notice-foreign-names-message"), Loc.GetString("civ-setting-notice-foreign-names-enable"), Loc.GetString("civ-setting-notice-foreign-names-disable"));
			_settingNotice.ChoiceMade += delegate(bool enabled)
			{
				_cfg.SetCVar<bool>(CCVars.Civ14ShowForeignNames, enabled, false);
				_cfg.SetCVar<bool>(CCVars.Civ14ForeignNamesPromptSeen, true, false);
				_cfg.SaveToFile();
			};
			((BaseWindow)_settingNotice).OnClose += delegate
			{
				_settingNotice = null;
			};
			((BaseWindow)_settingNotice).OpenCentered();
		}
	}

	protected override void Shutdown()
	{
		_userInterfaceManager.GetUIController<ChatUIController>().SetMainChat(setting: false);
		_gameTicker.InfoBlobUpdated -= UpdateLobbyUi;
		_gameTicker.LobbyStatusUpdated -= LobbyStatusUpdated;
		_gameTicker.Civ14LobbyStatusUpdated -= UpdateLobbyUi;
		_rosterSystem.StateUpdated -= OnRosterStateUpdated;
		if (_settingNotice != null)
		{
			((BaseWindow)_settingNotice).Close();
			_settingNotice = null;
		}
		_voteManager.ClearPopupContainer();
		_rosterSystem.DetachInlineControl();
		if (_rosterControl != null)
		{
			CivLobbyGui? lobby = Lobby;
			if (lobby != null)
			{
				((Control)lobby.RosterHost).RemoveChild((Control)(object)_rosterControl);
			}
			_rosterControl = null;
		}
		if (Lobby != null)
		{
			((BaseButton)Lobby.CharacterSetup).OnPressed -= OnSetupPressed;
			((BaseButton)Lobby.ReadyButton).OnPressed -= OnReadyPressed;
			((BaseButton)Lobby.ReadyButton).OnToggled -= OnReadyToggled;
			((BaseButton)Lobby.ModeMenuButton).OnPressed -= OnModeMenuPressed;
			((BaseButton)Lobby.AllowAutoLeaderCheck).OnToggled -= OnAllowAutoLeaderToggled;
		}
		Lobby = null;
	}

	public void SwitchState(CivLobbyGui.LobbyGuiState state)
	{
		Lobby?.SwitchState(state);
	}

	public override void FrameUpdate(FrameEventArgs e)
	{
		if (Lobby == null)
		{
			return;
		}
		if (_gameTiming.CurTime >= _nextBackgroundChange)
		{
			RequestBackgroundChange();
			_nextBackgroundChange = _gameTiming.CurTime + TimeSpan.FromSeconds(300.0);
		}
		CivRosterStateEvent state = _rosterSystem.GetState();
		if (IsCivRoundActive(state))
		{
			TimeSpan timeSpan = _gameTiming.CurTime.Subtract(_gameTicker.RoundStartTimeSpan);
			Lobby.StationTime.Text = Loc.GetString("lobby-state-player-status-round-time", new(string, object)[2]
			{
				("hours", timeSpan.Hours),
				("minutes", timeSpan.Minutes)
			});
			CivHudStatusEvent lastStatus = _hudEvents.LastStatus;
			if (lastStatus != null && (state.RoundMode == Civ14RoundMode.PointCapture || state.RoundMode == Civ14RoundMode.Front))
			{
				CivRosterTeamEntry civRosterTeamEntry = state.Teams.FirstOrDefault((CivRosterTeamEntry t) => t.TeamId == 1);
				CivRosterTeamEntry civRosterTeamEntry2 = state.Teams.FirstOrDefault((CivRosterTeamEntry t) => t.TeamId == 2);
				Lobby.StartTime.Text = Loc.GetString("civ-lobby-score", new(string, object)[4]
				{
					("team1", civRosterTeamEntry?.TeamName ?? "1"),
					("s1", lastStatus.Team1Score),
					("s2", lastStatus.Team2Score),
					("team2", civRosterTeamEntry2?.TeamName ?? "2")
				});
				((Control)Lobby.StripeBack).Visible = true;
			}
			else
			{
				Lobby.StartTime.Text = string.Empty;
				((Control)Lobby.StripeBack).Visible = false;
			}
			return;
		}
		Lobby.StationTime.Text = Loc.GetString("lobby-state-player-status-round-not-started");
		UpdateCivLobbyInfo();
		if (!_gameTicker.IsCiv14CountdownActive)
		{
			Lobby.StartTime.Text = string.Empty;
			((Control)Lobby.StripeBack).Visible = false;
			return;
		}
		string item;
		if (_gameTicker.Paused)
		{
			item = Loc.GetString("lobby-state-paused");
		}
		else
		{
			if (_gameTicker.Civ14StartTime < _gameTiming.CurTime)
			{
				Lobby.StartTime.Text = Loc.GetString("lobby-state-soon");
				return;
			}
			TimeSpan timeSpan2 = _gameTicker.Civ14StartTime - _gameTiming.CurTime;
			double totalSeconds = timeSpan2.TotalSeconds;
			item = ((totalSeconds < 0.0) ? Loc.GetString((totalSeconds < -5.0) ? "lobby-state-right-now-question" : "lobby-state-right-now-confirmation") : ((!(timeSpan2.TotalHours >= 1.0)) ? $"{timeSpan2.Minutes}:{timeSpan2.Seconds:D2}" : $"{Math.Floor(timeSpan2.TotalHours)}:{timeSpan2.Minutes:D2}:{timeSpan2.Seconds:D2}"));
		}
		Lobby.StartTime.Text = Loc.GetString("lobby-state-round-start-countdown-text", new(string, object)[1] { ("timeLeft", item) });
		((Control)Lobby.StripeBack).Visible = true;
	}

	private void OnModeMenuPressed(ButtonEventArgs args)
	{
		_gameTicker.OpenModeMenu();
	}

	private void OnSetupPressed(ButtonEventArgs args)
	{
		if (!IsReadyLockedBySquad(_rosterSystem.GetState()))
		{
			SetReady(newReady: false);
		}
		Lobby?.SwitchState(CivLobbyGui.LobbyGuiState.CharacterSetup);
	}

	private void OnReadyPressed(ButtonEventArgs args)
	{
	}

	private void OnReadyToggled(ButtonToggledEventArgs args)
	{
		SetReady(args.Pressed);
	}

	private void OnBackgroundChanged(string background)
	{
		LoadMainScreen();
	}

	private void LobbyStatusUpdated()
	{
		UpdateLobbyBackground();
		UpdateLobbyUi();
	}

	private void UpdateLobbyUi()
	{
		if (Lobby != null)
		{
			CivRosterStateEvent state = _rosterSystem.GetState();
			bool flag = IsReadyLockedBySquad(state);
			if (IsCivRoundActive(state))
			{
				Lobby.ReadyButton.Text = (state.RejoinBlockedForCurrentRound ? Loc.GetString("civ-lobby-ready-rejoin-closed") : Loc.GetString("lobby-state-ready-button-join-state"));
				((BaseButton)Lobby.ReadyButton).ToggleMode = false;
				((BaseButton)Lobby.ReadyButton).Pressed = false;
				((BaseButton)Lobby.ReadyButton).Disabled = state.RejoinBlockedForCurrentRound;
				((BaseButton)Lobby.ObserveButton).Disabled = false;
				Lobby.SetReadyPulse(pulse: false);
			}
			else
			{
				Lobby.StartTime.Text = string.Empty;
				((BaseButton)Lobby.ReadyButton).Pressed = flag || _gameTicker.AreWeReady;
				Lobby.ReadyButton.Text = Loc.GetString(((BaseButton)Lobby.ReadyButton).Pressed ? "lobby-state-player-status-ready" : "lobby-state-player-status-not-ready");
				((BaseButton)Lobby.ReadyButton).ToggleMode = true;
				((BaseButton)Lobby.ReadyButton).Disabled = flag;
				((BaseButton)Lobby.ObserveButton).Disabled = true;
				Lobby.SetReadyPulse(((BaseButton)Lobby.ReadyButton).Pressed);
			}
			float playtimeMinutesToday = _playtimeTracking.PlaytimeMinutesToday;
			if (playtimeMinutesToday > 60f)
			{
				((Control)Lobby.PlaytimeComment).Visible = true;
				double num = Math.Round(playtimeMinutesToday / 60f, 1);
				string text = ((playtimeMinutesToday < 180f) ? "lobby-state-playtime-comment-normal" : ((playtimeMinutesToday < 360f) ? "lobby-state-playtime-comment-concerning" : ((!(playtimeMinutesToday < 720f)) ? "lobby-state-playtime-comment-selfdestructive" : "lobby-state-playtime-comment-grasstouchless")));
				string text2 = text;
				Lobby.PlaytimeComment.SetMarkup(Loc.GetString(text2, new(string, object)[1] { ("hours", num) }));
			}
			else
			{
				((Control)Lobby.PlaytimeComment).Visible = false;
			}
			UpdateCivLobbyInfo();
		}
	}

	private void UpdateCivLobbyInfo()
	{
		if (Lobby != null)
		{
			Lobby.TdmReadyLabel.Text = Loc.GetString("civ-lobby-info-mode", new(string, object)[2]
			{
				("mode", _gameTicker.Civ14ModeName),
				("ready", _gameTicker.Civ14ReadyCount)
			});
			Lobby.TdmMapLabel.Text = (_gameTicker.IsCiv14MapRandom ? Loc.GetString("civ-lobby-info-map-random") : Loc.GetString("civ-lobby-info-map", new(string, object)[1] { ("map", _gameTicker.Civ14MapName) }));
		}
	}

	private void UpdateLobbyBackground()
	{
		if (Lobby != null)
		{
			if (_gameTicker.LobbyBackground != null)
			{
				Lobby.Background.Texture = TextureResource.op_Implicit(_resourceCache.GetResource<TextureResource>(_gameTicker.LobbyBackground, true));
			}
			else
			{
				Lobby.Background.Texture = null;
			}
		}
	}

	private void RequestBackgroundChange()
	{
		((IConsoleHost)_consoleHost).ExecuteCommand("lobbybackground");
	}

	private void SetReady(bool newReady)
	{
		if (!IsCivRoundActive(_rosterSystem.GetState()) && (newReady || !IsReadyLockedBySquad(_rosterSystem.GetState())))
		{
			((IConsoleHost)_consoleHost).ExecuteCommand($"toggleready {newReady}");
		}
	}

	private static bool IsReadyLockedBySquad(CivRosterStateEvent state)
	{
		if (state.Enabled && !state.RoundInProgress && state.SelectedTeamId.HasValue)
		{
			return state.SelectedSquadId.HasValue;
		}
		return false;
	}

	private static bool IsCivRoundActive(CivRosterStateEvent state)
	{
		return state.LateJoinActive;
	}

	private void LoadMainScreen()
	{
		if (Lobby != null)
		{
			if (!Enum.TryParse<BackgroundType>(_cfg.GetCVar<string>(CCCCVars.Background), out var result))
			{
				result = BackgroundType.Image;
			}
			switch (result)
			{
			case BackgroundType.Image:
				((Control)Lobby.ParallaxControl).Visible = false;
				((Control)Lobby.Background).Visible = true;
				break;
			case BackgroundType.Parallax:
				((Control)Lobby.ParallaxControl).Visible = true;
				((Control)Lobby.Background).Visible = false;
				break;
			}
		}
	}

	private void OnRosterStateUpdated(CivRosterStateEvent state)
	{
		Lobby?.UpdateAssignment(state);
		if (Lobby != null)
		{
			((BaseButton)Lobby.AllowAutoLeaderCheck).Pressed = state.AllowAutoLeader;
		}
		UpdateLobbyUi();
	}

	private void OnAllowAutoLeaderToggled(ButtonToggledEventArgs args)
	{
		_rosterSystem.SetAllowAutoLeader(args.Pressed);
	}
}
