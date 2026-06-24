// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.Lobby.CivLobbyState
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

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
using Robust.Client.Console;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Configuration;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Timing;
using System;
using System.Linq;

#nullable enable
namespace Content.Client._CIV14merka.Lobby;

public sealed class CivLobbyState : Robust.Client.State.State
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

  protected virtual Type? LinkedScreenType { get; } = typeof (CivLobbyGui);

  public CivLobbyGui? Lobby { get; private set; }

  protected virtual void Startup()
  {
    if (this._userInterfaceManager.ActiveScreen == null)
      return;
    this.Lobby = (CivLobbyGui) this._userInterfaceManager.ActiveScreen;
    ChatUIController uiController = this._userInterfaceManager.GetUIController<ChatUIController>();
    this._gameTicker = this._entityManager.System<ClientGameTicker>();
    this._rosterSystem = this._entityManager.System<CivRosterSystem>();
    this._hudEvents = this._entityManager.System<CivHudEventsSystem>();
    uiController.SetMainChat(true);
    this._voteManager.SetPopupContainer((Control) this.Lobby.VoteContainer);
    LayoutContainer.SetAnchorPreset((Control) this.Lobby, (LayoutContainer.LayoutPreset) 15, false);
    this.UpdateLobbyUi();
    this.UpdateLobbyBackground();
    this.LoadMainScreen();
    this._nextBackgroundChange = this._gameTiming.CurTime + TimeSpan.FromSeconds(300.0);
    ((BaseButton) this.Lobby.CharacterSetup).OnPressed += new Action<BaseButton.ButtonEventArgs>(this.OnSetupPressed);
    ((BaseButton) this.Lobby.ReadyButton).OnPressed += new Action<BaseButton.ButtonEventArgs>(this.OnReadyPressed);
    ((BaseButton) this.Lobby.ReadyButton).OnToggled += new Action<BaseButton.ButtonToggledEventArgs>(this.OnReadyToggled);
    ((BaseButton) this.Lobby.ModeMenuButton).OnPressed += new Action<BaseButton.ButtonEventArgs>(this.OnModeMenuPressed);
    ((BaseButton) this.Lobby.AllowAutoLeaderCheck).OnToggled += new Action<BaseButton.ButtonToggledEventArgs>(this.OnAllowAutoLeaderToggled);
    CivRosterControl civRosterControl = new CivRosterControl();
    ((Control) civRosterControl).HorizontalExpand = true;
    ((Control) civRosterControl).VerticalExpand = true;
    this._rosterControl = civRosterControl;
    ((Control) this.Lobby.RosterHost).AddChild((Control) this._rosterControl);
    this._rosterSystem.AttachInlineControl(this._rosterControl);
    this._gameTicker.InfoBlobUpdated += new Action(this.UpdateLobbyUi);
    this._gameTicker.LobbyStatusUpdated += new Action(this.LobbyStatusUpdated);
    this._gameTicker.Civ14LobbyStatusUpdated += new Action(this.UpdateLobbyUi);
    this._rosterSystem.StateUpdated += new Action<CivRosterStateEvent>(this.OnRosterStateUpdated);
    this.Lobby.UpdateAssignment(this._rosterSystem.GetState());
    this._rosterSystem.RequestState();
    this._cfg.OnValueChanged<string>(Content.Shared.DeadSpace.CCCCVars.CCCCVars.Background, new Action<string>(this.OnBackgroundChanged), true);
    this.TryShowSettingNotices();
  }

  private void TryShowSettingNotices()
  {
    if (this._settingNotice != null || this._cfg.GetCVar<bool>(CCVars.Civ14ForeignNamesPromptSeen))
      return;
    this._settingNotice = new CivSettingNoticeWindow(Loc.GetString("civ-setting-notice-foreign-names-title"), Loc.GetString("civ-setting-notice-foreign-names-message"), Loc.GetString("civ-setting-notice-foreign-names-enable"), Loc.GetString("civ-setting-notice-foreign-names-disable"));
    this._settingNotice.ChoiceMade += (Action<bool>) (enabled =>
    {
      this._cfg.SetCVar<bool>(CCVars.Civ14ShowForeignNames, enabled, false);
      this._cfg.SetCVar<bool>(CCVars.Civ14ForeignNamesPromptSeen, true, false);
      this._cfg.SaveToFile();
    });
    ((BaseWindow) this._settingNotice).OnClose += (Action) (() => this._settingNotice = (CivSettingNoticeWindow) null);
    ((BaseWindow) this._settingNotice).OpenCentered();
  }

  protected virtual void Shutdown()
  {
    this._userInterfaceManager.GetUIController<ChatUIController>().SetMainChat(false);
    this._gameTicker.InfoBlobUpdated -= new Action(this.UpdateLobbyUi);
    this._gameTicker.LobbyStatusUpdated -= new Action(this.LobbyStatusUpdated);
    this._gameTicker.Civ14LobbyStatusUpdated -= new Action(this.UpdateLobbyUi);
    this._rosterSystem.StateUpdated -= new Action<CivRosterStateEvent>(this.OnRosterStateUpdated);
    if (this._settingNotice != null)
    {
      ((BaseWindow) this._settingNotice).Close();
      this._settingNotice = (CivSettingNoticeWindow) null;
    }
    this._voteManager.ClearPopupContainer();
    this._rosterSystem.DetachInlineControl();
    if (this._rosterControl != null)
    {
      ((Control) this.Lobby?.RosterHost).RemoveChild((Control) this._rosterControl);
      this._rosterControl = (CivRosterControl) null;
    }
    if (this.Lobby != null)
    {
      ((BaseButton) this.Lobby.CharacterSetup).OnPressed -= new Action<BaseButton.ButtonEventArgs>(this.OnSetupPressed);
      ((BaseButton) this.Lobby.ReadyButton).OnPressed -= new Action<BaseButton.ButtonEventArgs>(this.OnReadyPressed);
      ((BaseButton) this.Lobby.ReadyButton).OnToggled -= new Action<BaseButton.ButtonToggledEventArgs>(this.OnReadyToggled);
      ((BaseButton) this.Lobby.ModeMenuButton).OnPressed -= new Action<BaseButton.ButtonEventArgs>(this.OnModeMenuPressed);
      ((BaseButton) this.Lobby.AllowAutoLeaderCheck).OnToggled -= new Action<BaseButton.ButtonToggledEventArgs>(this.OnAllowAutoLeaderToggled);
    }
    this.Lobby = (CivLobbyGui) null;
  }

  public void SwitchState(CivLobbyGui.LobbyGuiState state) => this.Lobby?.SwitchState(state);

  public virtual void FrameUpdate(FrameEventArgs e)
  {
    if (this.Lobby == null)
      return;
    if (this._gameTiming.CurTime >= this._nextBackgroundChange)
    {
      this.RequestBackgroundChange();
      this._nextBackgroundChange = this._gameTiming.CurTime + TimeSpan.FromSeconds(300.0);
    }
    CivRosterStateEvent state = this._rosterSystem.GetState();
    if (CivLobbyState.IsCivRoundActive(state))
    {
      TimeSpan timeSpan = this._gameTiming.CurTime.Subtract(this._gameTicker.RoundStartTimeSpan);
      this.Lobby.StationTime.Text = Loc.GetString("lobby-state-player-status-round-time", new (string, object)[2]
      {
        ("hours", (object) timeSpan.Hours),
        ("minutes", (object) timeSpan.Minutes)
      });
      CivHudStatusEvent lastStatus = this._hudEvents.LastStatus;
      if (lastStatus != null && (state.RoundMode == Civ14RoundMode.PointCapture || state.RoundMode == Civ14RoundMode.Front))
      {
        CivRosterTeamEntry civRosterTeamEntry1 = state.Teams.FirstOrDefault<CivRosterTeamEntry>((Func<CivRosterTeamEntry, bool>) (t => t.TeamId == 1));
        CivRosterTeamEntry civRosterTeamEntry2 = state.Teams.FirstOrDefault<CivRosterTeamEntry>((Func<CivRosterTeamEntry, bool>) (t => t.TeamId == 2));
        this.Lobby.StartTime.Text = Loc.GetString("civ-lobby-score", new (string, object)[4]
        {
          ("team1", (object) (civRosterTeamEntry1?.TeamName ?? "1")),
          ("s1", (object) lastStatus.Team1Score),
          ("s2", (object) lastStatus.Team2Score),
          ("team2", (object) (civRosterTeamEntry2?.TeamName ?? "2"))
        });
        ((Control) this.Lobby.StripeBack).Visible = true;
      }
      else
      {
        this.Lobby.StartTime.Text = string.Empty;
        ((Control) this.Lobby.StripeBack).Visible = false;
      }
    }
    else
    {
      this.Lobby.StationTime.Text = Loc.GetString("lobby-state-player-status-round-not-started");
      this.UpdateCivLobbyInfo();
      if (!this._gameTicker.IsCiv14CountdownActive)
      {
        this.Lobby.StartTime.Text = string.Empty;
        ((Control) this.Lobby.StripeBack).Visible = false;
      }
      else
      {
        string str;
        if (this._gameTicker.Paused)
        {
          str = Loc.GetString("lobby-state-paused");
        }
        else
        {
          if (this._gameTicker.Civ14StartTime < this._gameTiming.CurTime)
          {
            this.Lobby.StartTime.Text = Loc.GetString("lobby-state-soon");
            return;
          }
          TimeSpan timeSpan = this._gameTicker.Civ14StartTime - this._gameTiming.CurTime;
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
  }

  private void OnModeMenuPressed(BaseButton.ButtonEventArgs args)
  {
    this._gameTicker.OpenModeMenu();
  }

  private void OnSetupPressed(BaseButton.ButtonEventArgs args)
  {
    if (!CivLobbyState.IsReadyLockedBySquad(this._rosterSystem.GetState()))
      this.SetReady(false);
    this.Lobby?.SwitchState(CivLobbyGui.LobbyGuiState.CharacterSetup);
  }

  private void OnReadyPressed(BaseButton.ButtonEventArgs args)
  {
  }

  private void OnReadyToggled(BaseButton.ButtonToggledEventArgs args)
  {
    this.SetReady(args.Pressed);
  }

  private void OnBackgroundChanged(string background) => this.LoadMainScreen();

  private void LobbyStatusUpdated()
  {
    this.UpdateLobbyBackground();
    this.UpdateLobbyUi();
  }

  private void UpdateLobbyUi()
  {
    if (this.Lobby == null)
      return;
    CivRosterStateEvent state = this._rosterSystem.GetState();
    bool flag = CivLobbyState.IsReadyLockedBySquad(state);
    if (CivLobbyState.IsCivRoundActive(state))
    {
      this.Lobby.ReadyButton.Text = state.RejoinBlockedForCurrentRound ? Loc.GetString("civ-lobby-ready-rejoin-closed") : Loc.GetString("lobby-state-ready-button-join-state");
      ((BaseButton) this.Lobby.ReadyButton).ToggleMode = false;
      ((BaseButton) this.Lobby.ReadyButton).Pressed = false;
      ((BaseButton) this.Lobby.ReadyButton).Disabled = state.RejoinBlockedForCurrentRound;
      ((BaseButton) this.Lobby.ObserveButton).Disabled = false;
      this.Lobby.SetReadyPulse(false);
    }
    else
    {
      this.Lobby.StartTime.Text = string.Empty;
      ((BaseButton) this.Lobby.ReadyButton).Pressed = flag || this._gameTicker.AreWeReady;
      this.Lobby.ReadyButton.Text = Loc.GetString(((BaseButton) this.Lobby.ReadyButton).Pressed ? "lobby-state-player-status-ready" : "lobby-state-player-status-not-ready");
      ((BaseButton) this.Lobby.ReadyButton).ToggleMode = true;
      ((BaseButton) this.Lobby.ReadyButton).Disabled = flag;
      ((BaseButton) this.Lobby.ObserveButton).Disabled = true;
      this.Lobby.SetReadyPulse(((BaseButton) this.Lobby.ReadyButton).Pressed);
    }
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
    this.UpdateCivLobbyInfo();
  }

  private void UpdateCivLobbyInfo()
  {
    if (this.Lobby == null)
      return;
    this.Lobby.TdmReadyLabel.Text = Loc.GetString("civ-lobby-info-mode", new (string, object)[2]
    {
      ("mode", (object) this._gameTicker.Civ14ModeName),
      ("ready", (object) this._gameTicker.Civ14ReadyCount)
    });
    Label tdmMapLabel = this.Lobby.TdmMapLabel;
    string str;
    if (!this._gameTicker.IsCiv14MapRandom)
      str = Loc.GetString("civ-lobby-info-map", new (string, object)[1]
      {
        ("map", (object) this._gameTicker.Civ14MapName)
      });
    else
      str = Loc.GetString("civ-lobby-info-map-random");
    tdmMapLabel.Text = str;
  }

  private void UpdateLobbyBackground()
  {
    if (this.Lobby == null)
      return;
    if (this._gameTicker.LobbyBackground != null)
      this.Lobby.Background.Texture = TextureResource.op_Implicit(this._resourceCache.GetResource<TextureResource>(this._gameTicker.LobbyBackground, true));
    else
      this.Lobby.Background.Texture = (Texture) null;
  }

  private void RequestBackgroundChange()
  {
    ((IConsoleHost) this._consoleHost).ExecuteCommand("lobbybackground");
  }

  private void SetReady(bool newReady)
  {
    if (CivLobbyState.IsCivRoundActive(this._rosterSystem.GetState()) || !newReady && CivLobbyState.IsReadyLockedBySquad(this._rosterSystem.GetState()))
      return;
    ((IConsoleHost) this._consoleHost).ExecuteCommand($"toggleready {newReady}");
  }

  private static bool IsReadyLockedBySquad(CivRosterStateEvent state)
  {
    return state.Enabled && !state.RoundInProgress && state.SelectedTeamId.HasValue && state.SelectedSquadId.HasValue;
  }

  private static bool IsCivRoundActive(CivRosterStateEvent state) => state.LateJoinActive;

  private void LoadMainScreen()
  {
    if (this.Lobby == null)
      return;
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

  private void OnRosterStateUpdated(CivRosterStateEvent state)
  {
    this.Lobby?.UpdateAssignment(state);
    if (this.Lobby != null)
      ((BaseButton) this.Lobby.AllowAutoLeaderCheck).Pressed = state.AllowAutoLeader;
    this.UpdateLobbyUi();
  }

  private void OnAllowAutoLeaderToggled(BaseButton.ButtonToggledEventArgs args)
  {
    this._rosterSystem.SetAllowAutoLeader(args.Pressed);
  }
}
