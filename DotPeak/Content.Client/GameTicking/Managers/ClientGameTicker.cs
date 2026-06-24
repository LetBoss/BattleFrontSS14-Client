// Decompiled with JetBrains decompiler
// Type: Content.Client.GameTicking.Managers.ClientGameTicker
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

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
using System;
using System.Collections.Generic;

#nullable enable
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

  [Robust.Shared.ViewVariables.ViewVariables]
  public bool AreWeReady { get; private set; }

  [Robust.Shared.ViewVariables.ViewVariables]
  public bool IsGameStarted { get; private set; }

  [Robust.Shared.ViewVariables.ViewVariables]
  public ResolvedSoundSpecifier? RestartSound { get; private set; }

  [Robust.Shared.ViewVariables.ViewVariables]
  public string? LobbyBackground { get; private set; }

  [Robust.Shared.ViewVariables.ViewVariables]
  public bool DisallowedLateJoin { get; private set; }

  [Robust.Shared.ViewVariables.ViewVariables]
  public string? ServerInfoBlob { get; private set; }

  [Robust.Shared.ViewVariables.ViewVariables]
  public TimeSpan StartTime { get; private set; }

  [Robust.Shared.ViewVariables.ViewVariables]
  public bool Paused { get; private set; }

  [Robust.Shared.ViewVariables.ViewVariables]
  public int ServerOnlineCount { get; private set; }

  [Robust.Shared.ViewVariables.ViewVariables]
  public int PubgModeOnlineCount { get; private set; }

  [Robust.Shared.ViewVariables.ViewVariables]
  public int Civ14ModeOnlineCount { get; private set; }

  [Robust.Shared.ViewVariables.ViewVariables]
  public bool IsPubgModeSelectable { get; private set; } = true;

  [Robust.Shared.ViewVariables.ViewVariables]
  public bool IsCiv14ModeSelectable { get; private set; } = true;

  [Robust.Shared.ViewVariables.ViewVariables]
  public TimeSpan Civ14StartTime { get; private set; }

  [Robust.Shared.ViewVariables.ViewVariables]
  public bool IsCiv14CountdownActive { get; private set; }

  [Robust.Shared.ViewVariables.ViewVariables]
  public int Civ14ReadyCount { get; private set; }

  [Robust.Shared.ViewVariables.ViewVariables]
  public string Civ14ModeName { get; private set; } = "Захват базы";

  [Robust.Shared.ViewVariables.ViewVariables]
  public string Civ14MapName { get; private set; } = string.Empty;

  [Robust.Shared.ViewVariables.ViewVariables]
  public bool IsCiv14MapRandom { get; private set; } = true;

  [Robust.Shared.ViewVariables.ViewVariables]
  public IReadOnlyDictionary<NetEntity, Dictionary<ProtoId<JobPrototype>, int?>> JobsAvailable
  {
    get
    {
      return (IReadOnlyDictionary<NetEntity, Dictionary<ProtoId<JobPrototype>, int?>>) this._jobsAvailable;
    }
  }

  [Robust.Shared.ViewVariables.ViewVariables]
  public IReadOnlyDictionary<NetEntity, string> StationNames
  {
    get => (IReadOnlyDictionary<NetEntity, string>) this._stationNames;
  }

  public event Action? InfoBlobUpdated;

  public event Action? LobbyStatusUpdated;

  public event Action? LobbyLateJoinStatusUpdated;

  public event Action? ModeMenuStatusUpdated;

  public event Action? Civ14LobbyStatusUpdated;

  public event Action<IReadOnlyDictionary<NetEntity, Dictionary<ProtoId<JobPrototype>, int?>>>? LobbyJobsAvailableUpdated;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeNetworkEvent<TickerJoinLobbyEvent>(new EntityEventHandler<TickerJoinLobbyEvent>(this.JoinLobby), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<TickerJoinGameEvent>(new EntityEventHandler<TickerJoinGameEvent>(this.JoinGame), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<TickerConnectionStatusEvent>(new EntityEventHandler<TickerConnectionStatusEvent>(this.ConnectionStatus), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<TickerLobbyStatusEvent>(new EntityEventHandler<TickerLobbyStatusEvent>(this.LobbyStatus), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<TickerLobbyInfoEvent>(new EntityEventHandler<TickerLobbyInfoEvent>(this.LobbyInfo), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<TickerLobbyCountdownEvent>(new EntityEventHandler<TickerLobbyCountdownEvent>(this.LobbyCountdown), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<RoundEndMessageEvent>(new EntityEventHandler<RoundEndMessageEvent>(this.RoundEnd), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<RequestWindowAttentionEvent>(new EntityEventHandler<RequestWindowAttentionEvent>(this.OnAttentionRequest), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<TickerLateJoinStatusEvent>(new EntityEventHandler<TickerLateJoinStatusEvent>(this.LateJoinStatus), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<TickerJobsAvailableEvent>(new EntityEventHandler<TickerJobsAvailableEvent>(this.UpdateJobsAvailable), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<ModeMenuStatusEvent>(new EntityEventHandler<ModeMenuStatusEvent>(this.OnModeMenuStatus), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<CivLobbyStatusEvent>(new EntityEventHandler<CivLobbyStatusEvent>(this.OnCiv14LobbyStatus), (Type[]) null, (Type[]) null);
    this._admin.AdminStatusUpdated += new Action(this.OnAdminUpdated);
    this.OnAdminUpdated();
  }

  public override void Shutdown()
  {
    this._admin.AdminStatusUpdated -= new Action(this.OnAdminUpdated);
    base.Shutdown();
  }

  private void OnAdminUpdated()
  {
    ((EntitySystem) this.EntityManager.System<SharedMapSystem>()).Log.Level = new LogLevel?(this._admin.IsAdmin() ? (LogLevel) 2 : (LogLevel) 3);
  }

  private void OnAttentionRequest(RequestWindowAttentionEvent ev)
  {
    this._clyde.RequestWindowAttention();
  }

  private void LateJoinStatus(TickerLateJoinStatusEvent message)
  {
    this.DisallowedLateJoin = message.Disallowed;
    Action joinStatusUpdated = this.LobbyLateJoinStatusUpdated;
    if (joinStatusUpdated == null)
      return;
    joinStatusUpdated();
  }

  private void UpdateJobsAvailable(TickerJobsAvailableEvent message)
  {
    this._jobsAvailable.Clear();
    foreach (KeyValuePair<NetEntity, Dictionary<ProtoId<JobPrototype>, int?>> keyValuePair in message.JobsAvailableByStation)
    {
      NetEntity key;
      (key, this._jobsAvailable[key]) = keyValuePair;
    }
    this._stationNames.Clear();
    foreach (KeyValuePair<NetEntity, string> stationName in message.StationNames)
      this._stationNames[stationName.Key] = stationName.Value;
    Action<IReadOnlyDictionary<NetEntity, Dictionary<ProtoId<JobPrototype>, int?>>> availableUpdated = this.LobbyJobsAvailableUpdated;
    if (availableUpdated == null)
      return;
    availableUpdated(this.JobsAvailable);
  }

  private void JoinLobby(TickerJoinLobbyEvent message) => this.OpenModeMenu();

  public void OpenModeMenu() => this._stateManager.RequestStateChange<ModeSelectState>();

  public void ClearSelectedModeAndOpenModeMenu()
  {
    this.RaiseNetworkEvent((EntityEventArgs) new ModeMenuSelectRequestEvent(ModeMenuMode.None));
    this.OpenModeMenu();
  }

  public void SelectPubgMode()
  {
    if (!this.IsPubgModeSelectable)
      return;
    this.RaiseNetworkEvent((EntityEventArgs) new ModeMenuSelectRequestEvent(ModeMenuMode.Pubg));
    this.OpenPubgLobby();
  }

  public void SelectCiv14Mode()
  {
    if (!this.IsCiv14ModeSelectable)
      return;
    this.RaiseNetworkEvent((EntityEventArgs) new ModeMenuSelectRequestEvent(ModeMenuMode.Civ14));
    this.OpenCivLobby();
  }

  public void OpenPubgLobby()
  {
    if (this._stateManager.CurrentState is GameplayState)
    {
      if (this.ShouldOpenPreLobbyHubForCompletedTasks())
        this._stateManager.RequestStateChange<PubgPreLobbyHubState>();
      else
        this._stateManager.RequestStateChange<LobbyState>();
    }
    else
      this._stateManager.RequestStateChange<PubgPreLobbyHubState>();
  }

  public void OpenCivLobby() => this._stateManager.RequestStateChange<CivLobbyState>();

  private bool ShouldOpenPreLobbyHubForCompletedTasks()
  {
    PubgEventsSystem pubgEventsSystem = this.EntityManager.SystemOrNull<PubgEventsSystem>();
    return (pubgEventsSystem != null ? (pubgEventsSystem.HasClaimableTasksInCache() ? 1 : 0) : 0) != 0;
  }

  private void ConnectionStatus(TickerConnectionStatusEvent message)
  {
    this.RoundStartTimeSpan = message.RoundStartTimeSpan;
  }

  private void LobbyStatus(TickerLobbyStatusEvent message)
  {
    this.StartTime = message.StartTime;
    this.RoundStartTimeSpan = message.RoundStartTimeSpan;
    this.IsGameStarted = message.IsRoundStarted;
    this.AreWeReady = message.YouAreReady;
    this.LobbyBackground = message.LobbyBackground;
    this.Paused = message.Paused;
    Action lobbyStatusUpdated = this.LobbyStatusUpdated;
    if (lobbyStatusUpdated == null)
      return;
    lobbyStatusUpdated();
  }

  private void LobbyInfo(TickerLobbyInfoEvent message)
  {
    this.ServerInfoBlob = message.TextBlob;
    Action infoBlobUpdated = this.InfoBlobUpdated;
    if (infoBlobUpdated == null)
      return;
    infoBlobUpdated();
  }

  private void OnModeMenuStatus(ModeMenuStatusEvent message)
  {
    this.ServerOnlineCount = message.ServerOnlineCount;
    this.PubgModeOnlineCount = message.PubgOnlineCount;
    this.Civ14ModeOnlineCount = message.Civ14OnlineCount;
    this.IsPubgModeSelectable = message.PubgEnabled;
    this.IsCiv14ModeSelectable = message.Civ14Enabled;
    Action menuStatusUpdated = this.ModeMenuStatusUpdated;
    if (menuStatusUpdated == null)
      return;
    menuStatusUpdated();
  }

  private void OnCiv14LobbyStatus(CivLobbyStatusEvent message)
  {
    this.Civ14StartTime = message.StartTime;
    this.IsCiv14CountdownActive = message.CountdownActive;
    this.Civ14ReadyCount = message.ReadyCount;
    this.Civ14ModeName = message.RoundModeName;
    this.Civ14MapName = message.MapName;
    this.IsCiv14MapRandom = message.RandomMapSelection;
    Action lobbyStatusUpdated = this.Civ14LobbyStatusUpdated;
    if (lobbyStatusUpdated == null)
      return;
    lobbyStatusUpdated();
  }

  private void JoinGame(TickerJoinGameEvent message)
  {
    this._stateManager.RequestStateChange<GameplayState>();
  }

  private void LobbyCountdown(TickerLobbyCountdownEvent message)
  {
    this.StartTime = message.StartTime;
    this.Paused = message.Paused;
  }

  private void RoundEnd(RoundEndMessageEvent message)
  {
    this.RestartSound = message.RestartSound;
    this._userInterfaceManager.GetUIController<RoundEndSummaryUIController>().OpenRoundEndSummaryWindow(message);
  }
}
