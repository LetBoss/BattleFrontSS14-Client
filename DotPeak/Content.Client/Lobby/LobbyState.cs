// Decompiled with JetBrains decompiler
// Type: Content.Client.Lobby.LobbyState
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

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
using Content.Shared.Preferences;
using Robust.Client.Graphics;
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
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Client.Lobby;

public sealed class LobbyState : Robust.Client.State.State
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

  protected virtual Type? LinkedScreenType { get; } = typeof (LobbyGui);

  protected virtual void Startup()
  {
    if (this._userInterfaceManager.ActiveScreen == null)
      return;
    this.Lobby = (LobbyGui) this._userInterfaceManager.ActiveScreen;
    ChatUIController uiController = this._userInterfaceManager.GetUIController<ChatUIController>();
    this._gameTicker = this._entityManager.System<ClientGameTicker>();
    uiController.SetMainChat(true);
    uiController.SetPreferredChannel(ChatSelectChannel.Lobby);
    this._voteManager.SetPopupContainer((Control) this.Lobby.VoteContainer);
    LayoutContainer.SetAnchorPreset((Control) this.Lobby, (LayoutContainer.LayoutPreset) 15, false);
    this.UpdateLobbyUi();
    this.LoadLobbyBackgroundMode();
    ((BaseButton) this.Lobby.CharacterPreview.CharacterSetupButton).OnPressed += new Action<BaseButton.ButtonEventArgs>(this.OnSetupPressed);
    ((BaseButton) this.Lobby.CustomizeButton).OnPressed += new Action<BaseButton.ButtonEventArgs>(this.OnSetupPressed);
    ((BaseButton) this.Lobby.SoloButton).OnPressed += new Action<BaseButton.ButtonEventArgs>(this.OnSoloPressed);
    ((BaseButton) this.Lobby.DuoButton).OnPressed += new Action<BaseButton.ButtonEventArgs>(this.OnDuoPressed);
    ((BaseButton) this.Lobby.SquadButton).OnPressed += new Action<BaseButton.ButtonEventArgs>(this.OnSquadPressed);
    ((BaseButton) this.Lobby.FiftyFiftyButton).OnPressed += new Action<BaseButton.ButtonEventArgs>(this.OnFiftyFiftyPressed);
    ((BaseButton) this.Lobby.FullSquadCheckBox).OnToggled += new Action<BaseButton.ButtonToggledEventArgs>(this.OnFullSquadToggled);
    ((BaseButton) this.Lobby.StartModeButton).OnPressed += new Action<BaseButton.ButtonEventArgs>(this.OnStartModePressed);
    ((BaseButton) this.Lobby.LeavePartyButton).OnPressed += new Action<BaseButton.ButtonEventArgs>(this.OnLeavePartyPressed);
    ((BaseButton) this.Lobby.FriendsButton).OnPressed += new Action<BaseButton.ButtonEventArgs>(this.OnFriendsPressed);
    ((BaseButton) this.Lobby.BackButton).OnPressed += new Action<BaseButton.ButtonEventArgs>(this.OnBackPressed);
    PubgPreLobbyPartyClientSystem partyClientSystem = this._entityManager.System<PubgPreLobbyPartyClientSystem>();
    partyClientSystem.PartyStateUpdated += new Action(this.OnPreLobbyPartyUpdated);
    partyClientSystem.RequestState();
    partyClientSystem.ModeOverviewUpdated += new Action(this.OnModeOverviewUpdated);
    partyClientSystem.RequestModeOverview();
    this.UpdateModeButtonsForParty();
    this.UpdateSelectedModeUi();
    this.UpdatePlayerLabels();
    this.UpdatePartyPreviews();
    this.UpdateModeOverviewPanel();
    this._entityManager.System<PubgFriendsClientSystem>().PendingRequestsUpdated += new Action<int>(this.OnFriendsPendingUpdated);
    this.UpdateFriendsButton(0);
    PubgReputationClientSystem reputationClientSystem = this._entityManager.System<PubgReputationClientSystem>();
    reputationClientSystem.OnReputationUpdated += new Action<int>(this.OnReputationUpdated);
    reputationClientSystem.RequestState();
    this._gameTicker.InfoBlobUpdated += new Action(this.UpdateLobbyUi);
    this._gameTicker.LobbyStatusUpdated += new Action(this.LobbyStatusUpdated);
    this._gameTicker.LobbyLateJoinStatusUpdated += new Action(this.LobbyLateJoinStatusUpdated);
    this._cfg.OnValueChanged<string>(Content.Shared.DeadSpace.CCCCVars.CCCCVars.Background, new Action<string>(this.OnBackgroundChanged), true);
  }

  private void OnBackgroundChanged(string obj) => this.LoadLobbyBackgroundMode();

  protected virtual void Shutdown()
  {
    this._userInterfaceManager.GetUIController<ChatUIController>().SetMainChat(false);
    this._gameTicker.InfoBlobUpdated -= new Action(this.UpdateLobbyUi);
    this._gameTicker.LobbyStatusUpdated -= new Action(this.LobbyStatusUpdated);
    this._gameTicker.LobbyLateJoinStatusUpdated -= new Action(this.LobbyLateJoinStatusUpdated);
    this._voteManager.ClearPopupContainer();
    ((BaseButton) this.Lobby.CharacterPreview.CharacterSetupButton).OnPressed -= new Action<BaseButton.ButtonEventArgs>(this.OnSetupPressed);
    ((BaseButton) this.Lobby.CustomizeButton).OnPressed -= new Action<BaseButton.ButtonEventArgs>(this.OnSetupPressed);
    ((BaseButton) this.Lobby.SoloButton).OnPressed -= new Action<BaseButton.ButtonEventArgs>(this.OnSoloPressed);
    ((BaseButton) this.Lobby.DuoButton).OnPressed -= new Action<BaseButton.ButtonEventArgs>(this.OnDuoPressed);
    ((BaseButton) this.Lobby.SquadButton).OnPressed -= new Action<BaseButton.ButtonEventArgs>(this.OnSquadPressed);
    ((BaseButton) this.Lobby.FiftyFiftyButton).OnPressed -= new Action<BaseButton.ButtonEventArgs>(this.OnFiftyFiftyPressed);
    ((BaseButton) this.Lobby.FullSquadCheckBox).OnToggled -= new Action<BaseButton.ButtonToggledEventArgs>(this.OnFullSquadToggled);
    ((BaseButton) this.Lobby.StartModeButton).OnPressed -= new Action<BaseButton.ButtonEventArgs>(this.OnStartModePressed);
    ((BaseButton) this.Lobby.LeavePartyButton).OnPressed -= new Action<BaseButton.ButtonEventArgs>(this.OnLeavePartyPressed);
    ((BaseButton) this.Lobby.FriendsButton).OnPressed -= new Action<BaseButton.ButtonEventArgs>(this.OnFriendsPressed);
    ((BaseButton) this.Lobby.BackButton).OnPressed -= new Action<BaseButton.ButtonEventArgs>(this.OnBackPressed);
    PubgPreLobbyPartyClientSystem partyClientSystem;
    if (this._entityManager.EntitySysManager.TryGetEntitySystem<PubgPreLobbyPartyClientSystem>(ref partyClientSystem))
    {
      partyClientSystem.PartyStateUpdated -= new Action(this.OnPreLobbyPartyUpdated);
      partyClientSystem.ModeOverviewUpdated -= new Action(this.OnModeOverviewUpdated);
    }
    PubgFriendsClientSystem friendsClientSystem;
    if (this._entityManager.EntitySysManager.TryGetEntitySystem<PubgFriendsClientSystem>(ref friendsClientSystem))
      friendsClientSystem.PendingRequestsUpdated -= new Action<int>(this.OnFriendsPendingUpdated);
    PubgReputationClientSystem reputationClientSystem;
    if (this._entityManager.EntitySysManager.TryGetEntitySystem<PubgReputationClientSystem>(ref reputationClientSystem))
      reputationClientSystem.OnReputationUpdated -= new Action<int>(this.OnReputationUpdated);
    this.Lobby = (LobbyGui) null;
  }

  public void SwitchState(LobbyGui.LobbyGuiState state) => this.Lobby?.SwitchState(state);

  private void OnSetupPressed(BaseButton.ButtonEventArgs args)
  {
    this.Lobby?.SwitchState(LobbyGui.LobbyGuiState.CharacterSetup);
  }

  private void OnFriendsPressed(BaseButton.ButtonEventArgs args)
  {
    this._entityManager.System<PubgFriendsClientSystem>().ToggleWindow();
  }

  private void OnBackPressed(BaseButton.ButtonEventArgs args)
  {
    this._gameTicker.ClearSelectedModeAndOpenModeMenu();
  }

  private void OnFriendsPendingUpdated(int count) => this.UpdateFriendsButton(count);

  private void UpdateFriendsButton(int count)
  {
    if (this.Lobby == null)
      return;
    Button friendsButton = this.Lobby.FriendsButton;
    string str;
    if (count <= 0)
      str = Loc.GetString("pubg-lobby-friends-button");
    else
      str = Loc.GetString("pubg-lobby-friends-button-count", new (string, object)[1]
      {
        (nameof (count), (object) count)
      });
    friendsButton.Text = str;
  }

  public virtual void FrameUpdate(FrameEventArgs e)
  {
    if (this._gameTicker.IsGameStarted)
    {
      this.Lobby.StartTime.Text = string.Empty;
      ((Control) this.Lobby.StripeBack).Visible = false;
      TimeSpan timeSpan = this._gameTiming.CurTime.Subtract(this._gameTicker.RoundStartTimeSpan);
      this.Lobby.StationTime.Text = Loc.GetString("lobby-state-player-status-round-time", new (string, object)[2]
      {
        ("hours", (object) timeSpan.Hours),
        ("minutes", (object) timeSpan.Minutes)
      });
    }
    else
    {
      this.Lobby.StationTime.Text = Loc.GetString("lobby-state-player-status-round-not-started");
      string str;
      if (this._gameTicker.Paused)
      {
        str = Loc.GetString("lobby-state-paused");
      }
      else
      {
        if (this._gameTicker.StartTime < this._gameTiming.CurTime)
        {
          this.Lobby.StartTime.Text = Loc.GetString("lobby-state-soon");
          return;
        }
        TimeSpan timeSpan = this._gameTicker.StartTime - this._gameTiming.CurTime;
        double totalSeconds = timeSpan.TotalSeconds;
        if (totalSeconds < 0.0)
          str = Loc.GetString(totalSeconds < -5.0 ? "lobby-state-right-now-question" : "lobby-state-right-now-confirmation");
        else if (timeSpan.TotalHours >= 1.0)
          str = $"{Math.Floor(timeSpan.TotalHours)}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
        else
          str = $"{timeSpan.Minutes}:{timeSpan.Seconds:D2}";
      }
      this.Lobby.StartTime.Text = Loc.GetString("lobby-state-round-start-countdown-text", new (string, object)[1]
      {
        ("timeLeft", (object) str)
      });
      ((Control) this.Lobby.StripeBack).Visible = true;
    }
  }

  private void LobbyStatusUpdated()
  {
    this.UpdateLobbyBackground();
    this.UpdateLobbyUi();
  }

  private void LobbyLateJoinStatusUpdated()
  {
  }

  private void UpdateLobbyUi()
  {
    this.Lobby.StartTime.Text = string.Empty;
    ((BaseButton) this.Lobby.ObserveButton).Disabled = this._gameTicker.IsGameStarted;
    ((Control) this.Lobby.LeaveButton).Visible = !this._gameTicker.IsGameStarted;
    if (this._gameTicker.ServerInfoBlob != null)
      this.Lobby.ServerInfo.SetInfoBlob(this._gameTicker.ServerInfoBlob);
    float playtimeMinutesToday = this._playtimeTracking.PlaytimeMinutesToday;
    if ((double) playtimeMinutesToday > 60.0)
    {
      ((Control) this.Lobby.PlaytimeComment).Visible = true;
      double num = Math.Round((double) playtimeMinutesToday / 60.0, 1);
      this.Lobby.PlaytimeComment.SetMarkup(Loc.GetString((double) playtimeMinutesToday < 180.0 ? "lobby-state-playtime-comment-normal" : ((double) playtimeMinutesToday < 360.0 ? "lobby-state-playtime-comment-concerning" : ((double) playtimeMinutesToday < 720.0 ? "lobby-state-playtime-comment-grasstouchless" : "lobby-state-playtime-comment-selfdestructive")), new (string, object)[1]
      {
        ("hours", (object) num)
      }));
    }
    else
      ((Control) this.Lobby.PlaytimeComment).Visible = false;
    this.UpdatePlayerLabels();
  }

  private void UpdateLobbyBackground()
  {
    if (this._gameTicker.LobbyBackground != null)
      this.Lobby.Background.Texture = TextureResource.op_Implicit(this._resourceCache.GetResource<TextureResource>(this._gameTicker.LobbyBackground, true));
    else
      this.Lobby.Background.Texture = (Texture) null;
  }

  private void RequestJoinMode(PubgMatchMode matchMode)
  {
    this._entityManager.System<PubgLobbyClientSystem>().RequestJoinMode(matchMode, this._preferFullSquad);
  }

  private void OnSoloPressed(BaseButton.ButtonEventArgs args)
  {
    this.SetSelectedMode(PubgMatchMode.Solo);
  }

  private void OnDuoPressed(BaseButton.ButtonEventArgs args)
  {
    this.SetSelectedMode(PubgMatchMode.Duo);
  }

  private void OnSquadPressed(BaseButton.ButtonEventArgs args)
  {
    this.SetSelectedMode(PubgMatchMode.Squad);
  }

  private void OnFiftyFiftyPressed(BaseButton.ButtonEventArgs args)
  {
    this.SetSelectedMode(PubgMatchMode.FiftyFifty);
  }

  private void OnFullSquadToggled(BaseButton.ButtonToggledEventArgs args)
  {
    PubgPreLobbyPartyClientSystem partyClientSystem = this._entityManager.System<PubgPreLobbyPartyClientSystem>();
    bool flag1 = partyClientSystem.Members.Count > 1;
    int num;
    if (flag1)
    {
      NetUserId? leaderId = partyClientSystem.LeaderId;
      NetUserId? userId = ((ISharedPlayerManager) this._playerManager).LocalSession?.UserId;
      num = leaderId.HasValue == userId.HasValue ? (leaderId.HasValue ? (NetUserId.op_Equality(leaderId.GetValueOrDefault(), userId.GetValueOrDefault()) ? 1 : 0) : 1) : 0;
    }
    else
      num = 1;
    bool flag2 = this._entityManager.System<PubgReputationClientSystem>().Reputation < 80 /*0x50*/;
    if (num == 0)
      this.UpdateSelectedModeUi();
    else if (flag2)
    {
      this._preferFullSquad = false;
      this.UpdateSelectedModeUi();
      if (!flag1)
        return;
      partyClientSystem.SendModeSelection(this._selectedMode, this._preferFullSquad);
    }
    else
    {
      this._preferFullSquad = args.Pressed;
      this.UpdateSelectedModeUi();
      if (!flag1)
        return;
      partyClientSystem.SendModeSelection(this._selectedMode, this._preferFullSquad);
    }
  }

  private void OnStartModePressed(BaseButton.ButtonEventArgs args)
  {
    if (this._gameTiming.CurTime < this._nextJoinRequestAllowed)
      return;
    PubgPreLobbyPartyClientSystem preLobbyParty = this._entityManager.System<PubgPreLobbyPartyClientSystem>();
    int num1 = preLobbyParty.Members.Count > 1 ? 1 : 0;
    int num2;
    if (num1 != 0)
    {
      NetUserId? leaderId = preLobbyParty.LeaderId;
      NetUserId? userId = ((ISharedPlayerManager) this._playerManager).LocalSession?.UserId;
      num2 = leaderId.HasValue == userId.HasValue ? (leaderId.HasValue ? (NetUserId.op_Equality(leaderId.GetValueOrDefault(), userId.GetValueOrDefault()) ? 1 : 0) : 1) : 0;
    }
    else
      num2 = 0;
    bool flag = num2 != 0;
    if (num1 != 0 && !flag)
    {
      bool localReady = this.GetLocalReady(preLobbyParty);
      preLobbyParty.SendReadyToggle(!localReady);
    }
    else
    {
      if (!this._selectedMode.HasValue)
        return;
      this._nextJoinRequestAllowed = this._gameTiming.CurTime + LobbyState.JoinRequestCooldown;
      this.RequestJoinMode(this._selectedMode.Value);
    }
  }

  private void OnLeavePartyPressed(BaseButton.ButtonEventArgs args)
  {
    this._entityManager.System<PubgPreLobbyPartyClientSystem>().SendLeaveRequest();
  }

  private void OnPreLobbyPartyUpdated()
  {
    this.SyncSelectedModeFromParty();
    this.UpdateModeButtonsForParty();
    this.UpdatePlayerLabels();
    this.UpdatePartyPreviews();
  }

  private void OnModeOverviewUpdated() => this.UpdateModeOverviewPanel();

  private void OnReputationUpdated(int reputation) => this.UpdateSelectedModeUi();

  private void UpdateModeButtonsForParty()
  {
    PubgPreLobbyPartyClientSystem partyClientSystem = this._entityManager.System<PubgPreLobbyPartyClientSystem>();
    bool flag1 = partyClientSystem.Members.Count > 1;
    int num;
    if (flag1)
    {
      NetUserId? leaderId = partyClientSystem.LeaderId;
      NetUserId? userId = ((ISharedPlayerManager) this._playerManager).LocalSession?.UserId;
      num = leaderId.HasValue == userId.HasValue ? (leaderId.HasValue ? (NetUserId.op_Equality(leaderId.GetValueOrDefault(), userId.GetValueOrDefault()) ? 1 : 0) : 1) : 0;
    }
    else
      num = 0;
    bool flag2 = num != 0;
    ((BaseButton) this.Lobby.SoloButton).Disabled = flag1;
    ((Control) this.Lobby.DuoButton).MouseFilter = !flag1 || flag2 ? (Control.MouseFilterMode) 1 : (Control.MouseFilterMode) 2;
    ((Control) this.Lobby.SquadButton).MouseFilter = !flag1 || flag2 ? (Control.MouseFilterMode) 1 : (Control.MouseFilterMode) 2;
    ((Control) this.Lobby.FiftyFiftyButton).MouseFilter = !flag1 || flag2 ? (Control.MouseFilterMode) 1 : (Control.MouseFilterMode) 2;
    this.Lobby.StartModeButton.Text = !flag1 || flag2 ? Loc.GetString("pubg-lobby-start-button") : Loc.GetString("pubg-lobby-ready-button");
    ((Control) this.Lobby.LeavePartyButton).Visible = flag1;
    if (flag1)
    {
      PubgMatchMode? selectedMode = this._selectedMode;
      PubgMatchMode pubgMatchMode = PubgMatchMode.Solo;
      if (selectedMode.GetValueOrDefault() == pubgMatchMode & selectedMode.HasValue)
        this._selectedMode = new PubgMatchMode?();
    }
    this.UpdateSelectedModeUi();
    this.UpdatePlayerLabels();
  }

  private void SetSelectedMode(PubgMatchMode mode)
  {
    PubgPreLobbyPartyClientSystem partyClientSystem = this._entityManager.System<PubgPreLobbyPartyClientSystem>();
    if (partyClientSystem.Members.Count > 1 && mode == PubgMatchMode.Solo)
      return;
    int num;
    if (partyClientSystem.Members.Count > 1)
    {
      NetUserId? leaderId = partyClientSystem.LeaderId;
      NetUserId? userId = ((ISharedPlayerManager) this._playerManager).LocalSession?.UserId;
      num = leaderId.HasValue == userId.HasValue ? (leaderId.HasValue ? (NetUserId.op_Equality(leaderId.GetValueOrDefault(), userId.GetValueOrDefault()) ? 1 : 0) : 1) : 0;
    }
    else
      num = 1;
    if (num == 0)
      return;
    this._selectedMode = new PubgMatchMode?(mode);
    this.UpdateSelectedModeUi();
    partyClientSystem.SendModeSelection(this._selectedMode, this._preferFullSquad);
  }

  private void UpdateSelectedModeUi()
  {
    PubgPreLobbyPartyClientSystem preLobbyParty = this._entityManager.System<PubgPreLobbyPartyClientSystem>();
    bool flag1 = preLobbyParty.Members.Count > 1;
    int num1 = flag1 ? preLobbyParty.Members.Count : 1;
    int num2 = this._selectedMode.HasValue ? this.GetTeamSize(this._selectedMode.Value) : 1;
    bool flag2 = this._selectedMode.HasValue && this._selectedMode.GetValueOrDefault() != PubgMatchMode.FiftyFifty && num2 > 1 && num1 < num2;
    int reputation = this._entityManager.System<PubgReputationClientSystem>().Reputation;
    bool flag3 = reputation < 80 /*0x50*/;
    if (!flag2 && this._selectedMode.HasValue)
      this._preferFullSquad = false;
    if (flag3)
      this._preferFullSquad = false;
    Button soloButton = this.Lobby.SoloButton;
    PubgMatchMode? selectedMode = this._selectedMode;
    PubgMatchMode pubgMatchMode = PubgMatchMode.Solo;
    int num3 = selectedMode.GetValueOrDefault() == pubgMatchMode & selectedMode.HasValue ? 1 : 0;
    ((BaseButton) soloButton).Pressed = num3 != 0;
    ((BaseButton) this.Lobby.DuoButton).Pressed = this._selectedMode.GetValueOrDefault() == PubgMatchMode.Duo;
    ((BaseButton) this.Lobby.SquadButton).Pressed = this._selectedMode.GetValueOrDefault() == PubgMatchMode.Squad;
    ((BaseButton) this.Lobby.FiftyFiftyButton).Pressed = this._selectedMode.GetValueOrDefault() == PubgMatchMode.FiftyFifty;
    ((BaseButton) this.Lobby.FullSquadCheckBox).Pressed = this._preferFullSquad;
    NetUserId? leaderId = preLobbyParty.LeaderId;
    NetUserId? userId = ((ISharedPlayerManager) this._playerManager).LocalSession?.UserId;
    bool flag4 = leaderId.HasValue == userId.HasValue && (!leaderId.HasValue || NetUserId.op_Equality(leaderId.GetValueOrDefault(), userId.GetValueOrDefault()));
    ((Control) this.Lobby.FullSquadCheckBox).Visible = flag2;
    ((BaseButton) this.Lobby.FullSquadCheckBox).Disabled = ((!flag2 ? 1 : (!flag1 ? 0 : (!flag4 ? 1 : 0))) | (flag3 ? 1 : 0)) != 0;
    CheckBox fullSquadCheckBox = this.Lobby.FullSquadCheckBox;
    string empty;
    if (!flag3)
      empty = string.Empty;
    else
      empty = Loc.GetString("pubg-reputation-full-squad-locked-tooltip", new (string, object)[2]
      {
        ("reputation", (object) reputation),
        ("required", (object) 80 /*0x50*/)
      });
    ((Control) fullSquadCheckBox).ToolTip = empty;
    if (preLobbyParty.Members.Count > 1)
    {
      if (flag4)
      {
        ((BaseButton) this.Lobby.StartModeButton).Disabled = !this._selectedMode.HasValue || !preLobbyParty.AllReady;
      }
      else
      {
        Button startModeButton = this.Lobby.StartModeButton;
        selectedMode = preLobbyParty.SelectedMode;
        int num4 = !selectedMode.HasValue ? 1 : 0;
        ((BaseButton) startModeButton).Disabled = num4 != 0;
      }
    }
    else
      ((BaseButton) this.Lobby.StartModeButton).Disabled = !this._selectedMode.HasValue;
    if (flag1 && !flag4)
    {
      ((BaseButton) this.Lobby.StartModeButton).ToggleMode = true;
      ((BaseButton) this.Lobby.StartModeButton).Pressed = this.GetLocalReady(preLobbyParty);
    }
    else
    {
      ((BaseButton) this.Lobby.StartModeButton).ToggleMode = false;
      ((BaseButton) this.Lobby.StartModeButton).Pressed = false;
    }
  }

  private void SyncSelectedModeFromParty()
  {
    PubgPreLobbyPartyClientSystem partyClientSystem = this._entityManager.System<PubgPreLobbyPartyClientSystem>();
    if (partyClientSystem.Members.Count <= 1)
      return;
    if (!partyClientSystem.SelectedMode.HasValue)
    {
      this._selectedMode = new PubgMatchMode?();
      this._preferFullSquad = true;
    }
    else
    {
      this._selectedMode = partyClientSystem.SelectedMode;
      this._preferFullSquad = partyClientSystem.PreferFullSquad;
    }
  }

  private bool IsFullSquadAvailable()
  {
    return this._selectedMode.HasValue && this.GetTeamSize(this._selectedMode.Value) > 1;
  }

  private int GetTeamSize(PubgMatchMode mode)
  {
    int teamSize;
    switch (mode)
    {
      case PubgMatchMode.Duo:
        teamSize = 2;
        break;
      case PubgMatchMode.Squad:
        teamSize = 4;
        break;
      case PubgMatchMode.FiftyFifty:
        teamSize = 50;
        break;
      default:
        teamSize = 1;
        break;
    }
    return teamSize;
  }

  private void UpdatePartyPreviews()
  {
    this._userInterfaceManager.GetUIController<LobbyUIController>().UpdatePartyPreviews();
  }

  private void UpdatePlayerLabels()
  {
    ICommonSession localSession = ((ISharedPlayerManager) this._playerManager).LocalSession;
    NetUserId? userId = localSession?.UserId;
    HumanoidCharacterProfile selectedCharacter = this._preferencesManager.Preferences?.SelectedCharacter as HumanoidCharacterProfile;
    PubgPreLobbyPartyClientSystem preLobbyParty = this._entityManager.System<PubgPreLobbyPartyClientSystem>();
    if (preLobbyParty.Members.Count > 0)
    {
      this.UpdatePartySlotDefaults(preLobbyParty, userId, selectedCharacter);
    }
    else
    {
      this.Lobby.PartySlot1CkeyLabel.Text = localSession?.Name ?? Loc.GetString("pubg-lobby-player-ckey");
      this.Lobby.PartySlot1NameLabel.Text = selectedCharacter?.Name ?? Loc.GetString("pubg-lobby-player-name");
      this.UpdatePartySlotDefaults(preLobbyParty, userId, selectedCharacter);
    }
  }

  private void UpdatePartySlotDefaults(
    PubgPreLobbyPartyClientSystem preLobbyParty,
    NetUserId? localId,
    HumanoidCharacterProfile? character)
  {
    this.Lobby.PartySlot2CkeyLabel.Text = string.Empty;
    this.Lobby.PartySlot2NameLabel.Text = string.Empty;
    this.Lobby.PartySlot3CkeyLabel.Text = string.Empty;
    this.Lobby.PartySlot3NameLabel.Text = string.Empty;
    this.Lobby.PartySlot4CkeyLabel.Text = string.Empty;
    this.Lobby.PartySlot4NameLabel.Text = string.Empty;
    if (preLobbyParty.Members.Count == 0)
      return;
    this.Lobby.PartySlot1CkeyLabel.Text = string.Empty;
    this.Lobby.PartySlot1NameLabel.Text = string.Empty;
    List<PubgPreLobbyPartyMemberState> list = preLobbyParty.Members.Take<PubgPreLobbyPartyMemberState>(4).ToList<PubgPreLobbyPartyMemberState>();
    NetUserId? nullable;
    if (list.Count > 0)
    {
      this.Lobby.PartySlot1CkeyLabel.Text = this.FormatCkeyWithReady(list[0]);
      Label partySlot1NameLabel = this.Lobby.PartySlot1NameLabel;
      NetUserId userId = list[0].UserId;
      nullable = localId;
      string str = (nullable.HasValue ? (NetUserId.op_Equality(userId, nullable.GetValueOrDefault()) ? 1 : 0) : 0) == 0 || character == null ? list[0].Profile?.Name ?? list[0].Ckey : character.Name;
      partySlot1NameLabel.Text = str;
    }
    if (list.Count > 1)
    {
      this.Lobby.PartySlot2CkeyLabel.Text = this.FormatCkeyWithReady(list[1]);
      Label partySlot2NameLabel = this.Lobby.PartySlot2NameLabel;
      NetUserId userId = list[1].UserId;
      nullable = localId;
      string str = (nullable.HasValue ? (NetUserId.op_Equality(userId, nullable.GetValueOrDefault()) ? 1 : 0) : 0) == 0 || character == null ? list[1].Profile?.Name ?? list[1].Ckey : character.Name;
      partySlot2NameLabel.Text = str;
    }
    if (list.Count > 2)
    {
      this.Lobby.PartySlot3CkeyLabel.Text = this.FormatCkeyWithReady(list[2]);
      Label partySlot3NameLabel = this.Lobby.PartySlot3NameLabel;
      NetUserId userId = list[2].UserId;
      nullable = localId;
      string str = (nullable.HasValue ? (NetUserId.op_Equality(userId, nullable.GetValueOrDefault()) ? 1 : 0) : 0) == 0 || character == null ? list[2].Profile?.Name ?? list[2].Ckey : character.Name;
      partySlot3NameLabel.Text = str;
    }
    if (list.Count <= 3)
      return;
    this.Lobby.PartySlot4CkeyLabel.Text = this.FormatCkeyWithReady(list[3]);
    Label partySlot4NameLabel = this.Lobby.PartySlot4NameLabel;
    NetUserId userId1 = list[3].UserId;
    nullable = localId;
    string str1 = (nullable.HasValue ? (NetUserId.op_Equality(userId1, nullable.GetValueOrDefault()) ? 1 : 0) : 0) == 0 || character == null ? list[3].Profile?.Name ?? list[3].Ckey : character.Name;
    partySlot4NameLabel.Text = str1;
  }

  private bool GetLocalReady(PubgPreLobbyPartyClientSystem preLobbyParty)
  {
    NetUserId? userId1 = ((ISharedPlayerManager) this._playerManager).LocalSession?.UserId;
    foreach (PubgPreLobbyPartyMemberState member in (IEnumerable<PubgPreLobbyPartyMemberState>) preLobbyParty.Members)
    {
      NetUserId userId2 = member.UserId;
      NetUserId? nullable = userId1;
      if ((nullable.HasValue ? (NetUserId.op_Equality(userId2, nullable.GetValueOrDefault()) ? 1 : 0) : 0) != 0)
        return member.IsReady;
    }
    return false;
  }

  private string FormatCkeyWithReady(PubgPreLobbyPartyMemberState member)
  {
    return member.InPreLobby && member.IsReady ? Loc.GetString("pubg-lobby-player-ckey-with-level-ready", new (string, object)[2]
    {
      ("ckey", (object) member.Ckey),
      ("level", (object) member.Level)
    }) : Loc.GetString("pubg-lobby-player-ckey-with-level", new (string, object)[2]
    {
      ("ckey", (object) member.Ckey),
      ("level", (object) member.Level)
    });
  }

  private void UpdateModeOverviewPanel()
  {
    PubgPreLobbyPartyClientSystem preLobbyParty = this._entityManager.System<PubgPreLobbyPartyClientSystem>();
    this.SetModeOverviewRow(PubgMatchMode.Solo, this.Lobby.ModeSoloInGameLabel, this.Lobby.ModeSoloInLobbyLabel, this.Lobby.ModeSoloNextStartLabel, preLobbyParty);
    this.SetModeOverviewRow(PubgMatchMode.Duo, this.Lobby.ModeDuoInGameLabel, this.Lobby.ModeDuoInLobbyLabel, this.Lobby.ModeDuoNextStartLabel, preLobbyParty);
    this.SetModeOverviewRow(PubgMatchMode.Squad, this.Lobby.ModeSquadInGameLabel, this.Lobby.ModeSquadInLobbyLabel, this.Lobby.ModeSquadNextStartLabel, preLobbyParty);
    this.SetModeOverviewRow(PubgMatchMode.FiftyFifty, this.Lobby.ModeFiftyFiftyInGameLabel, this.Lobby.ModeFiftyFiftyInLobbyLabel, this.Lobby.ModeFiftyFiftyNextStartLabel, preLobbyParty);
  }

  private void SetModeOverviewRow(
    PubgMatchMode mode,
    Label inGameLabel,
    Label inLobbyLabel,
    Label nextStartLabel,
    PubgPreLobbyPartyClientSystem preLobbyParty)
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
      nextStartLabel.Text = this.FormatModeOverviewStart(modeOverview.NextStartSeconds);
    }
  }

  private string FormatModeOverviewStart(int? seconds)
  {
    if (!seconds.HasValue)
      return Loc.GetString("pubg-lobby-mode-overview-nextstart-none");
    int num = Math.Max(0, seconds.Value);
    return $"{num / 60:D2}:{num % 60:D2}";
  }

  private void LoadLobbyBackgroundMode()
  {
    BackgroundType result;
    if (!Enum.TryParse<BackgroundType>(this._cfg.GetCVar<string>(Content.Shared.DeadSpace.CCCCVars.CCCCVars.Background), out result))
      result = BackgroundType.Image;
    if (result != BackgroundType.Image)
    {
      if (result != BackgroundType.Parallax)
        return;
      this.Lobby.ParallaxControl.Visible = true;
      ((Control) this.Lobby.Background).Visible = false;
    }
    else
    {
      this.Lobby.ParallaxControl.Visible = false;
      ((Control) this.Lobby.Background).Visible = true;
    }
  }
}
