using System;
using System.Collections.Generic;
using System.Linq;
using Content.Client._PUBG.Friends;
using Content.Client._PUBG.Lobby;
using Content.Client._PUBG.Party;
using Content.Client._PUBG.Reputation;
using Content.Client.GameTicking.Managers;
using Content.Client.Lobby.UI;
using Content.Client.Message;
using Content.Client.Playtime;
using Content.Client.UserInterface.Systems.Chat;
using Content.Client.Voting;
using Content.Shared._PUBG.Match;
using Content.Shared._PUBG.Party;
using Content.Shared.Chat;
using Content.Shared.DeadSpace.CCCCVars;
using Content.Shared.Preferences;
using Robust.Client.Player;
using Robust.Client.ResourceManagement;
using Robust.Client.State;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Timing;

namespace Content.Client.Lobby;

public sealed class LobbyState : State
{
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

	[Dependency]
	private IClientPreferencesManager _preferencesManager;

	[Dependency]
	private IPlayerManager _playerManager;

	[Dependency]
	private IStateManager _stateManager;

	private ClientGameTicker _gameTicker;

	private PubgMatchMode? _selectedMode;

	private bool _preferFullSquad = true;

	private static readonly TimeSpan JoinRequestCooldown = TimeSpan.FromSeconds(1L);

	private TimeSpan _nextJoinRequestAllowed;

	public LobbyGui? Lobby;

	protected override Type? LinkedScreenType { get; } = typeof(LobbyGui);

	protected override void Startup()
	{
		if (_userInterfaceManager.ActiveScreen != null)
		{
			Lobby = (LobbyGui)(object)_userInterfaceManager.ActiveScreen;
			ChatUIController uIController = _userInterfaceManager.GetUIController<ChatUIController>();
			_gameTicker = _entityManager.System<ClientGameTicker>();
			uIController.SetMainChat(setting: true);
			uIController.SetPreferredChannel(ChatSelectChannel.Lobby);
			_voteManager.SetPopupContainer((Control)(object)Lobby.VoteContainer);
			LayoutContainer.SetAnchorPreset((Control)(object)Lobby, (LayoutPreset)15, false);
			UpdateLobbyUi();
			LoadLobbyBackgroundMode();
			((BaseButton)Lobby.CharacterPreview.CharacterSetupButton).OnPressed += OnSetupPressed;
			((BaseButton)Lobby.CustomizeButton).OnPressed += OnSetupPressed;
			((BaseButton)Lobby.SoloButton).OnPressed += OnSoloPressed;
			((BaseButton)Lobby.DuoButton).OnPressed += OnDuoPressed;
			((BaseButton)Lobby.SquadButton).OnPressed += OnSquadPressed;
			((BaseButton)Lobby.FiftyFiftyButton).OnPressed += OnFiftyFiftyPressed;
			((BaseButton)Lobby.FullSquadCheckBox).OnToggled += OnFullSquadToggled;
			((BaseButton)Lobby.StartModeButton).OnPressed += OnStartModePressed;
			((BaseButton)Lobby.LeavePartyButton).OnPressed += OnLeavePartyPressed;
			((BaseButton)Lobby.FriendsButton).OnPressed += OnFriendsPressed;
			((BaseButton)Lobby.BackButton).OnPressed += OnBackPressed;
			PubgPreLobbyPartyClientSystem pubgPreLobbyPartyClientSystem = _entityManager.System<PubgPreLobbyPartyClientSystem>();
			pubgPreLobbyPartyClientSystem.PartyStateUpdated += OnPreLobbyPartyUpdated;
			pubgPreLobbyPartyClientSystem.RequestState();
			pubgPreLobbyPartyClientSystem.ModeOverviewUpdated += OnModeOverviewUpdated;
			pubgPreLobbyPartyClientSystem.RequestModeOverview();
			UpdateModeButtonsForParty();
			UpdateSelectedModeUi();
			UpdatePlayerLabels();
			UpdatePartyPreviews();
			UpdateModeOverviewPanel();
			_entityManager.System<PubgFriendsClientSystem>().PendingRequestsUpdated += OnFriendsPendingUpdated;
			UpdateFriendsButton(0);
			PubgReputationClientSystem pubgReputationClientSystem = _entityManager.System<PubgReputationClientSystem>();
			pubgReputationClientSystem.OnReputationUpdated += OnReputationUpdated;
			pubgReputationClientSystem.RequestState();
			_gameTicker.InfoBlobUpdated += UpdateLobbyUi;
			_gameTicker.LobbyStatusUpdated += LobbyStatusUpdated;
			_gameTicker.LobbyLateJoinStatusUpdated += LobbyLateJoinStatusUpdated;
			_cfg.OnValueChanged<string>(CCCCVars.Background, (Action<string>)OnBackgroundChanged, true);
		}
	}

	private void OnBackgroundChanged(string obj)
	{
		LoadLobbyBackgroundMode();
	}

	protected override void Shutdown()
	{
		_userInterfaceManager.GetUIController<ChatUIController>().SetMainChat(setting: false);
		_gameTicker.InfoBlobUpdated -= UpdateLobbyUi;
		_gameTicker.LobbyStatusUpdated -= LobbyStatusUpdated;
		_gameTicker.LobbyLateJoinStatusUpdated -= LobbyLateJoinStatusUpdated;
		_voteManager.ClearPopupContainer();
		((BaseButton)Lobby.CharacterPreview.CharacterSetupButton).OnPressed -= OnSetupPressed;
		((BaseButton)Lobby.CustomizeButton).OnPressed -= OnSetupPressed;
		((BaseButton)Lobby.SoloButton).OnPressed -= OnSoloPressed;
		((BaseButton)Lobby.DuoButton).OnPressed -= OnDuoPressed;
		((BaseButton)Lobby.SquadButton).OnPressed -= OnSquadPressed;
		((BaseButton)Lobby.FiftyFiftyButton).OnPressed -= OnFiftyFiftyPressed;
		((BaseButton)Lobby.FullSquadCheckBox).OnToggled -= OnFullSquadToggled;
		((BaseButton)Lobby.StartModeButton).OnPressed -= OnStartModePressed;
		((BaseButton)Lobby.LeavePartyButton).OnPressed -= OnLeavePartyPressed;
		((BaseButton)Lobby.FriendsButton).OnPressed -= OnFriendsPressed;
		((BaseButton)Lobby.BackButton).OnPressed -= OnBackPressed;
		PubgPreLobbyPartyClientSystem pubgPreLobbyPartyClientSystem = default(PubgPreLobbyPartyClientSystem);
		if (_entityManager.EntitySysManager.TryGetEntitySystem<PubgPreLobbyPartyClientSystem>(ref pubgPreLobbyPartyClientSystem))
		{
			pubgPreLobbyPartyClientSystem.PartyStateUpdated -= OnPreLobbyPartyUpdated;
			pubgPreLobbyPartyClientSystem.ModeOverviewUpdated -= OnModeOverviewUpdated;
		}
		PubgFriendsClientSystem pubgFriendsClientSystem = default(PubgFriendsClientSystem);
		if (_entityManager.EntitySysManager.TryGetEntitySystem<PubgFriendsClientSystem>(ref pubgFriendsClientSystem))
		{
			pubgFriendsClientSystem.PendingRequestsUpdated -= OnFriendsPendingUpdated;
		}
		PubgReputationClientSystem pubgReputationClientSystem = default(PubgReputationClientSystem);
		if (_entityManager.EntitySysManager.TryGetEntitySystem<PubgReputationClientSystem>(ref pubgReputationClientSystem))
		{
			pubgReputationClientSystem.OnReputationUpdated -= OnReputationUpdated;
		}
		Lobby = null;
	}

	public void SwitchState(LobbyGui.LobbyGuiState state)
	{
		Lobby?.SwitchState(state);
	}

	private void OnSetupPressed(ButtonEventArgs args)
	{
		Lobby?.SwitchState(LobbyGui.LobbyGuiState.CharacterSetup);
	}

	private void OnFriendsPressed(ButtonEventArgs args)
	{
		_entityManager.System<PubgFriendsClientSystem>().ToggleWindow();
	}

	private void OnBackPressed(ButtonEventArgs args)
	{
		_gameTicker.ClearSelectedModeAndOpenModeMenu();
	}

	private void OnFriendsPendingUpdated(int count)
	{
		UpdateFriendsButton(count);
	}

	private void UpdateFriendsButton(int count)
	{
		if (Lobby != null)
		{
			Lobby.FriendsButton.Text = ((count > 0) ? Loc.GetString("pubg-lobby-friends-button-count", new(string, object)[1] { ("count", count) }) : Loc.GetString("pubg-lobby-friends-button"));
		}
	}

	public override void FrameUpdate(FrameEventArgs e)
	{
		if (_gameTicker.IsGameStarted)
		{
			Lobby.StartTime.Text = string.Empty;
			((Control)Lobby.StripeBack).Visible = false;
			TimeSpan timeSpan = _gameTiming.CurTime.Subtract(_gameTicker.RoundStartTimeSpan);
			Lobby.StationTime.Text = Loc.GetString("lobby-state-player-status-round-time", new(string, object)[2]
			{
				("hours", timeSpan.Hours),
				("minutes", timeSpan.Minutes)
			});
			return;
		}
		Lobby.StationTime.Text = Loc.GetString("lobby-state-player-status-round-not-started");
		string item;
		if (_gameTicker.Paused)
		{
			item = Loc.GetString("lobby-state-paused");
		}
		else
		{
			if (_gameTicker.StartTime < _gameTiming.CurTime)
			{
				Lobby.StartTime.Text = Loc.GetString("lobby-state-soon");
				return;
			}
			TimeSpan timeSpan2 = _gameTicker.StartTime - _gameTiming.CurTime;
			double totalSeconds = timeSpan2.TotalSeconds;
			item = ((totalSeconds < 0.0) ? Loc.GetString((totalSeconds < -5.0) ? "lobby-state-right-now-question" : "lobby-state-right-now-confirmation") : ((!(timeSpan2.TotalHours >= 1.0)) ? $"{timeSpan2.Minutes}:{timeSpan2.Seconds:D2}" : $"{Math.Floor(timeSpan2.TotalHours)}:{timeSpan2.Minutes:D2}:{timeSpan2.Seconds:D2}"));
		}
		Lobby.StartTime.Text = Loc.GetString("lobby-state-round-start-countdown-text", new(string, object)[1] { ("timeLeft", item) });
		((Control)Lobby.StripeBack).Visible = true;
	}

	private void LobbyStatusUpdated()
	{
		UpdateLobbyBackground();
		UpdateLobbyUi();
	}

	private void LobbyLateJoinStatusUpdated()
	{
	}

	private void UpdateLobbyUi()
	{
		Lobby.StartTime.Text = string.Empty;
		((BaseButton)Lobby.ObserveButton).Disabled = _gameTicker.IsGameStarted;
		((Control)Lobby.LeaveButton).Visible = !_gameTicker.IsGameStarted;
		if (_gameTicker.ServerInfoBlob != null)
		{
			Lobby.ServerInfo.SetInfoBlob(_gameTicker.ServerInfoBlob);
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
		UpdatePlayerLabels();
	}

	private void UpdateLobbyBackground()
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

	private void RequestJoinMode(PubgMatchMode matchMode)
	{
		_entityManager.System<PubgLobbyClientSystem>().RequestJoinMode(matchMode, _preferFullSquad);
	}

	private void OnSoloPressed(ButtonEventArgs args)
	{
		SetSelectedMode(PubgMatchMode.Solo);
	}

	private void OnDuoPressed(ButtonEventArgs args)
	{
		SetSelectedMode(PubgMatchMode.Duo);
	}

	private void OnSquadPressed(ButtonEventArgs args)
	{
		SetSelectedMode(PubgMatchMode.Squad);
	}

	private void OnFiftyFiftyPressed(ButtonEventArgs args)
	{
		SetSelectedMode(PubgMatchMode.FiftyFifty);
	}

	private void OnFullSquadToggled(ButtonToggledEventArgs args)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		PubgPreLobbyPartyClientSystem pubgPreLobbyPartyClientSystem = _entityManager.System<PubgPreLobbyPartyClientSystem>();
		bool flag = pubgPreLobbyPartyClientSystem.Members.Count > 1;
		int num;
		if (flag)
		{
			NetUserId? leaderId = pubgPreLobbyPartyClientSystem.LeaderId;
			ICommonSession localSession = ((ISharedPlayerManager)_playerManager).LocalSession;
			NetUserId? val = ((localSession != null) ? new NetUserId?(localSession.UserId) : ((NetUserId?)null));
			num = ((leaderId.HasValue == val.HasValue && (!leaderId.HasValue || leaderId.GetValueOrDefault() == val.GetValueOrDefault())) ? 1 : 0);
		}
		else
		{
			num = 1;
		}
		bool flag2 = _entityManager.System<PubgReputationClientSystem>().Reputation < 80;
		if (num == 0)
		{
			UpdateSelectedModeUi();
		}
		else if (flag2)
		{
			_preferFullSquad = false;
			UpdateSelectedModeUi();
			if (flag)
			{
				pubgPreLobbyPartyClientSystem.SendModeSelection(_selectedMode, _preferFullSquad);
			}
		}
		else
		{
			_preferFullSquad = args.Pressed;
			UpdateSelectedModeUi();
			if (flag)
			{
				pubgPreLobbyPartyClientSystem.SendModeSelection(_selectedMode, _preferFullSquad);
			}
		}
	}

	private void OnStartModePressed(ButtonEventArgs args)
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		if (!(_gameTiming.CurTime < _nextJoinRequestAllowed))
		{
			PubgPreLobbyPartyClientSystem pubgPreLobbyPartyClientSystem = _entityManager.System<PubgPreLobbyPartyClientSystem>();
			bool num = pubgPreLobbyPartyClientSystem.Members.Count > 1;
			int num2;
			if (num)
			{
				NetUserId? leaderId = pubgPreLobbyPartyClientSystem.LeaderId;
				ICommonSession localSession = ((ISharedPlayerManager)_playerManager).LocalSession;
				NetUserId? val = ((localSession != null) ? new NetUserId?(localSession.UserId) : ((NetUserId?)null));
				num2 = ((leaderId.HasValue == val.HasValue && (!leaderId.HasValue || leaderId.GetValueOrDefault() == val.GetValueOrDefault())) ? 1 : 0);
			}
			else
			{
				num2 = 0;
			}
			bool flag = (byte)num2 != 0;
			if (num && !flag)
			{
				bool localReady = GetLocalReady(pubgPreLobbyPartyClientSystem);
				pubgPreLobbyPartyClientSystem.SendReadyToggle(!localReady);
			}
			else if (_selectedMode.HasValue)
			{
				_nextJoinRequestAllowed = _gameTiming.CurTime + JoinRequestCooldown;
				RequestJoinMode(_selectedMode.Value);
			}
		}
	}

	private void OnLeavePartyPressed(ButtonEventArgs args)
	{
		_entityManager.System<PubgPreLobbyPartyClientSystem>().SendLeaveRequest();
	}

	private void OnPreLobbyPartyUpdated()
	{
		SyncSelectedModeFromParty();
		UpdateModeButtonsForParty();
		UpdatePlayerLabels();
		UpdatePartyPreviews();
	}

	private void OnModeOverviewUpdated()
	{
		UpdateModeOverviewPanel();
	}

	private void OnReputationUpdated(int reputation)
	{
		UpdateSelectedModeUi();
	}

	private void UpdateModeButtonsForParty()
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		PubgPreLobbyPartyClientSystem pubgPreLobbyPartyClientSystem = _entityManager.System<PubgPreLobbyPartyClientSystem>();
		bool flag = pubgPreLobbyPartyClientSystem.Members.Count > 1;
		int num;
		if (flag)
		{
			NetUserId? leaderId = pubgPreLobbyPartyClientSystem.LeaderId;
			ICommonSession localSession = ((ISharedPlayerManager)_playerManager).LocalSession;
			NetUserId? val = ((localSession != null) ? new NetUserId?(localSession.UserId) : ((NetUserId?)null));
			num = ((leaderId.HasValue == val.HasValue && (!leaderId.HasValue || leaderId.GetValueOrDefault() == val.GetValueOrDefault())) ? 1 : 0);
		}
		else
		{
			num = 0;
		}
		bool flag2 = (byte)num != 0;
		((BaseButton)Lobby.SoloButton).Disabled = flag;
		((Control)Lobby.DuoButton).MouseFilter = (MouseFilterMode)((!flag || flag2) ? 1 : 2);
		((Control)Lobby.SquadButton).MouseFilter = (MouseFilterMode)((!flag || flag2) ? 1 : 2);
		((Control)Lobby.FiftyFiftyButton).MouseFilter = (MouseFilterMode)((!flag || flag2) ? 1 : 2);
		Lobby.StartModeButton.Text = ((flag && !flag2) ? Loc.GetString("pubg-lobby-ready-button") : Loc.GetString("pubg-lobby-start-button"));
		((Control)Lobby.LeavePartyButton).Visible = flag;
		if (flag && _selectedMode == PubgMatchMode.Solo)
		{
			_selectedMode = null;
		}
		UpdateSelectedModeUi();
		UpdatePlayerLabels();
	}

	private void SetSelectedMode(PubgMatchMode mode)
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		PubgPreLobbyPartyClientSystem pubgPreLobbyPartyClientSystem = _entityManager.System<PubgPreLobbyPartyClientSystem>();
		if (pubgPreLobbyPartyClientSystem.Members.Count > 1 && mode == PubgMatchMode.Solo)
		{
			return;
		}
		if (pubgPreLobbyPartyClientSystem.Members.Count > 1)
		{
			NetUserId? leaderId = pubgPreLobbyPartyClientSystem.LeaderId;
			ICommonSession localSession = ((ISharedPlayerManager)_playerManager).LocalSession;
			NetUserId? val = ((localSession != null) ? new NetUserId?(localSession.UserId) : ((NetUserId?)null));
			if (leaderId.HasValue != val.HasValue || (leaderId.HasValue && !(leaderId.GetValueOrDefault() == val.GetValueOrDefault())))
			{
				return;
			}
		}
		_selectedMode = mode;
		UpdateSelectedModeUi();
		pubgPreLobbyPartyClientSystem.SendModeSelection(_selectedMode, _preferFullSquad);
	}

	private void UpdateSelectedModeUi()
	{
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		PubgPreLobbyPartyClientSystem pubgPreLobbyPartyClientSystem = _entityManager.System<PubgPreLobbyPartyClientSystem>();
		bool flag = pubgPreLobbyPartyClientSystem.Members.Count > 1;
		int num = ((!flag) ? 1 : pubgPreLobbyPartyClientSystem.Members.Count);
		int num2 = ((!_selectedMode.HasValue) ? 1 : GetTeamSize(_selectedMode.Value));
		bool flag2 = _selectedMode.HasValue && _selectedMode != PubgMatchMode.FiftyFifty && num2 > 1 && num < num2;
		int reputation = _entityManager.System<PubgReputationClientSystem>().Reputation;
		bool flag3 = reputation < 80;
		if (!flag2 && _selectedMode.HasValue)
		{
			_preferFullSquad = false;
		}
		if (flag3)
		{
			_preferFullSquad = false;
		}
		((BaseButton)Lobby.SoloButton).Pressed = _selectedMode == PubgMatchMode.Solo;
		((BaseButton)Lobby.DuoButton).Pressed = _selectedMode == PubgMatchMode.Duo;
		((BaseButton)Lobby.SquadButton).Pressed = _selectedMode == PubgMatchMode.Squad;
		((BaseButton)Lobby.FiftyFiftyButton).Pressed = _selectedMode == PubgMatchMode.FiftyFifty;
		((BaseButton)Lobby.FullSquadCheckBox).Pressed = _preferFullSquad;
		NetUserId? leaderId = pubgPreLobbyPartyClientSystem.LeaderId;
		ICommonSession localSession = ((ISharedPlayerManager)_playerManager).LocalSession;
		NetUserId? val = ((localSession != null) ? new NetUserId?(localSession.UserId) : ((NetUserId?)null));
		bool flag4 = leaderId.HasValue == val.HasValue && (!leaderId.HasValue || leaderId.GetValueOrDefault() == val.GetValueOrDefault());
		((Control)Lobby.FullSquadCheckBox).Visible = flag2;
		((BaseButton)Lobby.FullSquadCheckBox).Disabled = !flag2 || (flag && !flag4) || flag3;
		((Control)Lobby.FullSquadCheckBox).ToolTip = (flag3 ? Loc.GetString("pubg-reputation-full-squad-locked-tooltip", new(string, object)[2]
		{
			("reputation", reputation),
			("required", 80)
		}) : string.Empty);
		if (pubgPreLobbyPartyClientSystem.Members.Count > 1)
		{
			if (flag4)
			{
				((BaseButton)Lobby.StartModeButton).Disabled = !_selectedMode.HasValue || !pubgPreLobbyPartyClientSystem.AllReady;
			}
			else
			{
				((BaseButton)Lobby.StartModeButton).Disabled = !pubgPreLobbyPartyClientSystem.SelectedMode.HasValue;
			}
		}
		else
		{
			((BaseButton)Lobby.StartModeButton).Disabled = !_selectedMode.HasValue;
		}
		if (flag && !flag4)
		{
			((BaseButton)Lobby.StartModeButton).ToggleMode = true;
			((BaseButton)Lobby.StartModeButton).Pressed = GetLocalReady(pubgPreLobbyPartyClientSystem);
		}
		else
		{
			((BaseButton)Lobby.StartModeButton).ToggleMode = false;
			((BaseButton)Lobby.StartModeButton).Pressed = false;
		}
	}

	private void SyncSelectedModeFromParty()
	{
		PubgPreLobbyPartyClientSystem pubgPreLobbyPartyClientSystem = _entityManager.System<PubgPreLobbyPartyClientSystem>();
		if (pubgPreLobbyPartyClientSystem.Members.Count > 1)
		{
			if (!pubgPreLobbyPartyClientSystem.SelectedMode.HasValue)
			{
				_selectedMode = null;
				_preferFullSquad = true;
			}
			else
			{
				_selectedMode = pubgPreLobbyPartyClientSystem.SelectedMode;
				_preferFullSquad = pubgPreLobbyPartyClientSystem.PreferFullSquad;
			}
		}
	}

	private bool IsFullSquadAvailable()
	{
		if (!_selectedMode.HasValue)
		{
			return false;
		}
		return GetTeamSize(_selectedMode.Value) > 1;
	}

	private int GetTeamSize(PubgMatchMode mode)
	{
		return mode switch
		{
			PubgMatchMode.Duo => 2, 
			PubgMatchMode.Squad => 4, 
			PubgMatchMode.FiftyFifty => 50, 
			_ => 1, 
		};
	}

	private void UpdatePartyPreviews()
	{
		_userInterfaceManager.GetUIController<LobbyUIController>().UpdatePartyPreviews();
	}

	private void UpdatePlayerLabels()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		ICommonSession localSession = ((ISharedPlayerManager)_playerManager).LocalSession;
		NetUserId? localId = ((localSession != null) ? new NetUserId?(localSession.UserId) : ((NetUserId?)null));
		HumanoidCharacterProfile humanoidCharacterProfile = _preferencesManager.Preferences?.SelectedCharacter as HumanoidCharacterProfile;
		PubgPreLobbyPartyClientSystem pubgPreLobbyPartyClientSystem = _entityManager.System<PubgPreLobbyPartyClientSystem>();
		if (pubgPreLobbyPartyClientSystem.Members.Count > 0)
		{
			UpdatePartySlotDefaults(pubgPreLobbyPartyClientSystem, localId, humanoidCharacterProfile);
			return;
		}
		Lobby.PartySlot1CkeyLabel.Text = ((localSession != null) ? localSession.Name : null) ?? Loc.GetString("pubg-lobby-player-ckey");
		Lobby.PartySlot1NameLabel.Text = humanoidCharacterProfile?.Name ?? Loc.GetString("pubg-lobby-player-name");
		UpdatePartySlotDefaults(pubgPreLobbyPartyClientSystem, localId, humanoidCharacterProfile);
	}

	private void UpdatePartySlotDefaults(PubgPreLobbyPartyClientSystem preLobbyParty, NetUserId? localId, HumanoidCharacterProfile? character)
	{
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		Lobby.PartySlot2CkeyLabel.Text = string.Empty;
		Lobby.PartySlot2NameLabel.Text = string.Empty;
		Lobby.PartySlot3CkeyLabel.Text = string.Empty;
		Lobby.PartySlot3NameLabel.Text = string.Empty;
		Lobby.PartySlot4CkeyLabel.Text = string.Empty;
		Lobby.PartySlot4NameLabel.Text = string.Empty;
		if (preLobbyParty.Members.Count != 0)
		{
			Lobby.PartySlot1CkeyLabel.Text = string.Empty;
			Lobby.PartySlot1NameLabel.Text = string.Empty;
			List<PubgPreLobbyPartyMemberState> list = preLobbyParty.Members.Take(4).ToList();
			if (list.Count > 0)
			{
				Lobby.PartySlot1CkeyLabel.Text = FormatCkeyWithReady(list[0]);
				Label partySlot1NameLabel = Lobby.PartySlot1NameLabel;
				NetUserId userId = list[0].UserId;
				NetUserId? val = localId;
				partySlot1NameLabel.Text = ((val.HasValue && userId == val.GetValueOrDefault() && character != null) ? character.Name : (list[0].Profile?.Name ?? list[0].Ckey));
			}
			if (list.Count > 1)
			{
				Lobby.PartySlot2CkeyLabel.Text = FormatCkeyWithReady(list[1]);
				Label partySlot2NameLabel = Lobby.PartySlot2NameLabel;
				NetUserId userId = list[1].UserId;
				NetUserId? val = localId;
				partySlot2NameLabel.Text = ((val.HasValue && userId == val.GetValueOrDefault() && character != null) ? character.Name : (list[1].Profile?.Name ?? list[1].Ckey));
			}
			if (list.Count > 2)
			{
				Lobby.PartySlot3CkeyLabel.Text = FormatCkeyWithReady(list[2]);
				Label partySlot3NameLabel = Lobby.PartySlot3NameLabel;
				NetUserId userId = list[2].UserId;
				NetUserId? val = localId;
				partySlot3NameLabel.Text = ((val.HasValue && userId == val.GetValueOrDefault() && character != null) ? character.Name : (list[2].Profile?.Name ?? list[2].Ckey));
			}
			if (list.Count > 3)
			{
				Lobby.PartySlot4CkeyLabel.Text = FormatCkeyWithReady(list[3]);
				Label partySlot4NameLabel = Lobby.PartySlot4NameLabel;
				NetUserId userId = list[3].UserId;
				NetUserId? val = localId;
				partySlot4NameLabel.Text = ((val.HasValue && userId == val.GetValueOrDefault() && character != null) ? character.Name : (list[3].Profile?.Name ?? list[3].Ckey));
			}
		}
	}

	private bool GetLocalReady(PubgPreLobbyPartyClientSystem preLobbyParty)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		ICommonSession localSession = ((ISharedPlayerManager)_playerManager).LocalSession;
		NetUserId? val = ((localSession != null) ? new NetUserId?(localSession.UserId) : ((NetUserId?)null));
		foreach (PubgPreLobbyPartyMemberState member in preLobbyParty.Members)
		{
			NetUserId userId = member.UserId;
			NetUserId? val2 = val;
			if (val2.HasValue && userId == val2.GetValueOrDefault())
			{
				return member.IsReady;
			}
		}
		return false;
	}

	private string FormatCkeyWithReady(PubgPreLobbyPartyMemberState member)
	{
		if (member.InPreLobby && member.IsReady)
		{
			return Loc.GetString("pubg-lobby-player-ckey-with-level-ready", new(string, object)[2]
			{
				("ckey", member.Ckey),
				("level", member.Level)
			});
		}
		return Loc.GetString("pubg-lobby-player-ckey-with-level", new(string, object)[2]
		{
			("ckey", member.Ckey),
			("level", member.Level)
		});
	}

	private void UpdateModeOverviewPanel()
	{
		PubgPreLobbyPartyClientSystem preLobbyParty = _entityManager.System<PubgPreLobbyPartyClientSystem>();
		SetModeOverviewRow(PubgMatchMode.Solo, Lobby.ModeSoloInGameLabel, Lobby.ModeSoloInLobbyLabel, Lobby.ModeSoloNextStartLabel, preLobbyParty);
		SetModeOverviewRow(PubgMatchMode.Duo, Lobby.ModeDuoInGameLabel, Lobby.ModeDuoInLobbyLabel, Lobby.ModeDuoNextStartLabel, preLobbyParty);
		SetModeOverviewRow(PubgMatchMode.Squad, Lobby.ModeSquadInGameLabel, Lobby.ModeSquadInLobbyLabel, Lobby.ModeSquadNextStartLabel, preLobbyParty);
		SetModeOverviewRow(PubgMatchMode.FiftyFifty, Lobby.ModeFiftyFiftyInGameLabel, Lobby.ModeFiftyFiftyInLobbyLabel, Lobby.ModeFiftyFiftyNextStartLabel, preLobbyParty);
	}

	private void SetModeOverviewRow(PubgMatchMode mode, Label inGameLabel, Label inLobbyLabel, Label nextStartLabel, PubgPreLobbyPartyClientSystem preLobbyParty)
	{
		PubgPreLobbyModeOverviewEntry modeOverview = preLobbyParty.GetModeOverview(mode);
		if (modeOverview == null)
		{
			inGameLabel.Text = "0";
			inLobbyLabel.Text = "0";
			nextStartLabel.Text = Loc.GetString("pubg-lobby-mode-overview-nextstart-none");
		}
		else
		{
			inGameLabel.Text = modeOverview.InGamePlayers.ToString();
			inLobbyLabel.Text = modeOverview.InLobbyPlayers.ToString();
			nextStartLabel.Text = FormatModeOverviewStart(modeOverview.NextStartSeconds);
		}
	}

	private string FormatModeOverviewStart(int? seconds)
	{
		if (!seconds.HasValue)
		{
			return Loc.GetString("pubg-lobby-mode-overview-nextstart-none");
		}
		int num = Math.Max(0, seconds.Value);
		int value = num / 60;
		int value2 = num % 60;
		return $"{value:D2}:{value2:D2}";
	}

	private void LoadLobbyBackgroundMode()
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
