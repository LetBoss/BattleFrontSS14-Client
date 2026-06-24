// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.UserInterface.Systems.Lobby.PubgPreLobbyHubUIController
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._PUBG.BattlePass;
using Content.Client._PUBG.Calendar;
using Content.Client._PUBG.Events;
using Content.Client._PUBG.Inventory;
using Content.Client._PUBG.Lobby;
using Content.Client._PUBG.Lobby.UI;
using Content.Client._PUBG.UserInterface.MainMenu;
using Content.Client.Lobby;
using Content.Client.UserInterface.Systems.EscapeMenu;
using Content.Shared._PUBG.BattlePass;
using Content.Shared._PUBG.Calendar;
using Content.Shared._PUBG.Events;
using Content.Shared._PUBG.Skin;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Client.State;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Content.Client._PUBG.UserInterface.Systems.Lobby;

public sealed class PubgPreLobbyHubUIController : 
  UIController,
  IOnStateEntered<PubgPreLobbyHubState>,
  IOnStateExited<PubgPreLobbyHubState>
{
  private const string DiscordLinkEventKey = "discord_link_once";
  private const string MarsOpeningEventKey = "mars_opening";
  private const string MapperCookieEventKey = "mapper_cookie";
  private static readonly TimeSpan EventCardRefreshInterval = TimeSpan.FromSeconds(1L);
  private static readonly StyleBoxFlat CalendarCardStyle = PubgPreLobbyHubUIController.CreateCardStyle("#1A1C20F0", "#4B5320");
  private static readonly StyleBoxFlat CalendarCardHoverStyle = PubgPreLobbyHubUIController.CreateCardStyle("#23252BF2", "#707C30");
  private static readonly StyleBoxFlat EventsCardStyle = PubgPreLobbyHubUIController.CreateCardStyle("#1F2228ED", "#555C4A");
  private static readonly StyleBoxFlat EventsCardHoverStyle = PubgPreLobbyHubUIController.CreateCardStyle("#282C34F2", "#808B6E");
  private static readonly StyleBoxFlat EventsFeatureCardStyle = PubgPreLobbyHubUIController.CreateCardStyle("#141518", "#646D5C");
  private static readonly StyleBoxFlat EventsFeatureCardHoverStyle = PubgPreLobbyHubUIController.CreateCardStyle("#1C1E23", "#92A086");
  [Dependency]
  private IStateManager _stateManager;
  [Dependency]
  private IPlayerManager _playerManager;
  [Dependency]
  private IUriOpener _uriOpener;
  [Dependency]
  private ILocalizationManager _loc;
  private PubgPreLobbyHubState? _state;
  private bool _systemSubscribed;
  private int _coins;
  private int _scrap;
  private int _premiumCoins;
  private int _playerLevel = 1;
  private SponsorTierInfo? _sponsorDisplayTier;
  private SponsorDisplayMode _sponsorDisplayMode;
  private BattlePassStateMessage? _battlePassState;
  private PubgCalendarStateMessage? _calendarState;
  private PubgEventsHubStateMessage? _eventsHubState;
  private PubgPreLobbyCalendarWindow? _calendarWindow;
  private PubgPreLobbyEventsWindow? _eventsWindow;
  private PubgPreLobbyInventoryWindow? _inventoryWindow;
  private readonly HashSet<string> _preloadedEventKeys = new HashSet<string>();
  private string? _pendingPreferredEventKey;
  private string? _lastSelectedEventKey;
  private DateTime _lastHubRequestAt = DateTime.MinValue;
  private DateTime _lastHubResponseAt = DateTime.MinValue;
  private readonly Dictionary<string, DateTime> _lastEventStateRequestAt = new Dictionary<string, DateTime>();
  private readonly Dictionary<string, DateTime> _lastEventStateResponseAt = new Dictionary<string, DateTime>();
  private DateTime _lastEventCardRefreshAt = DateTime.MinValue;
  private bool _calendarCardHovered;
  private bool _eventsRouletteCardHovered;
  private bool _eventsPromoCardHovered;
  private bool _eventsFeatureCardHovered;
  private PubgPreLobbyHubUIController.PendingEventClaim? _pendingEventClaim;
  private string? _lastFeatureMilestoneSignature;

  private PubgPreLobbyHubGui? Hub => this._state?.Hub;

  public void OnStateEntered(PubgPreLobbyHubState state)
  {
    this._state = state;
    this._pendingEventClaim = (PubgPreLobbyHubUIController.PendingEventClaim) null;
    this._lastFeatureMilestoneSignature = (string) null;
    this.EnsureSystemSubscribed();
    this.SubscribeButtons();
    MainMenuSystem mainMenuSystem = this.EntityManager.System<MainMenuSystem>();
    bool cachedPlayerLevel = mainMenuSystem.TryGetCachedPlayerLevel(out this._playerLevel);
    if (!mainMenuSystem.TryGetCachedBalance(out this._coins, out this._scrap, out this._premiumCoins) || !cachedPlayerLevel)
      mainMenuSystem.RequestSkinState(true);
    BattlePassSystem battlePassSystem = this.EntityManager.System<BattlePassSystem>();
    this._battlePassState = battlePassSystem.LastState;
    battlePassSystem.RequestBattlePass();
    PubgCalendarSystem pubgCalendarSystem = this.EntityManager.System<PubgCalendarSystem>();
    this._calendarState = pubgCalendarSystem.LastState;
    pubgCalendarSystem.RequestCalendarState(this._calendarState == null);
    PubgEventsSystem eventsSystem = this.EntityManager.System<PubgEventsSystem>();
    this._eventsHubState = eventsSystem.LastHubState;
    if (this._eventsHubState != null)
      this._lastHubResponseAt = DateTime.UtcNow;
    this._preloadedEventKeys.Clear();
    if (this._eventsHubState != null)
      this.PreloadEventStates(this._eventsHubState, eventsSystem);
    this.RequestHubIfStale(eventsSystem, this._eventsHubState == null);
    this.RequestEventStateIfStale(eventsSystem, "discord_link_once", !eventsSystem.LastEventStates.ContainsKey("discord_link_once"));
    this.RequestEventStateIfStale(eventsSystem, "mars_opening", !eventsSystem.LastEventStates.ContainsKey("mars_opening"));
    this.RequestEventStateIfStale(eventsSystem, "mapper_cookie", !eventsSystem.LastEventStates.ContainsKey("mapper_cookie"));
    this.UpdateBalanceLabels();
    this.UpdateProfileLabels();
    this.UpdateBattlePassLabels();
    this.UpdateEventCardLabels();
    this.UpdateEventsRedDots();
    this.UpdateInteractiveCardStyles();
  }

  public void OnStateExited(PubgPreLobbyHubState state)
  {
    this.UnsubscribeButtons();
    this.CloseCalendarWindow();
    this.CloseEventsWindow();
    this.CloseInventoryWindow();
    this.UnsubscribeSystem();
    this._preloadedEventKeys.Clear();
    this._lastEventStateRequestAt.Clear();
    this._lastEventStateResponseAt.Clear();
    this._pendingPreferredEventKey = (string) null;
    this._pendingEventClaim = (PubgPreLobbyHubUIController.PendingEventClaim) null;
    this._lastFeatureMilestoneSignature = (string) null;
    this._calendarCardHovered = false;
    this._eventsRouletteCardHovered = false;
    this._eventsPromoCardHovered = false;
    this._eventsFeatureCardHovered = false;
    this._state = (PubgPreLobbyHubState) null;
  }

  public virtual void FrameUpdate(FrameEventArgs args)
  {
    base.FrameUpdate(args);
    if (this._state == null || this._eventsHubState == null)
      return;
    DateTime utcNow = DateTime.UtcNow;
    if (utcNow - this._lastEventCardRefreshAt < PubgPreLobbyHubUIController.EventCardRefreshInterval)
      return;
    this._lastEventCardRefreshAt = utcNow;
    this.UpdateEventCardLabels();
  }

  private void EnsureSystemSubscribed()
  {
    if (this._systemSubscribed)
      return;
    MainMenuSystem mainMenuSystem = this.EntityManager.System<MainMenuSystem>();
    mainMenuSystem.OnSkinStateReceived += new Action<SkinStateMessage>(this.OnSkinState);
    mainMenuSystem.OnBalanceUpdateReceived += new Action<BalanceUpdateMessage>(this.OnBalanceUpdate);
    this.EntityManager.System<BattlePassSystem>().OnStateReceived += new Action<BattlePassStateMessage>(this.OnBattlePassState);
    PubgCalendarSystem pubgCalendarSystem = this.EntityManager.System<PubgCalendarSystem>();
    pubgCalendarSystem.OnStateReceived += new Action<PubgCalendarStateMessage>(this.OnCalendarState);
    pubgCalendarSystem.OnClaimResultReceived += new Action<PubgCalendarClaimResultMessage>(this.OnCalendarClaimResult);
    PubgEventsSystem pubgEventsSystem = this.EntityManager.System<PubgEventsSystem>();
    pubgEventsSystem.OnHubStateReceived += new Action<PubgEventsHubStateMessage>(this.OnEventsHubState);
    pubgEventsSystem.OnEventStateReceived += new Action<PubgEventStateMessage>(this.OnEventState);
    pubgEventsSystem.OnClaimResultReceived += new Action<PubgEventClaimResultMessage>(this.OnEventClaimResult);
    this.EntityManager.System<PubgEventInventorySystem>().OnInventoryStateReceived += new Action<PubgEventInventoryStateMessage>(this.OnInventoryState);
    this._systemSubscribed = true;
  }

  private void UnsubscribeSystem()
  {
    if (!this._systemSubscribed)
      return;
    MainMenuSystem mainMenuSystem = this.EntityManager.SystemOrNull<MainMenuSystem>();
    if (mainMenuSystem != null)
    {
      mainMenuSystem.OnSkinStateReceived -= new Action<SkinStateMessage>(this.OnSkinState);
      mainMenuSystem.OnBalanceUpdateReceived -= new Action<BalanceUpdateMessage>(this.OnBalanceUpdate);
    }
    BattlePassSystem battlePassSystem = this.EntityManager.SystemOrNull<BattlePassSystem>();
    if (battlePassSystem != null)
      battlePassSystem.OnStateReceived -= new Action<BattlePassStateMessage>(this.OnBattlePassState);
    PubgCalendarSystem pubgCalendarSystem = this.EntityManager.SystemOrNull<PubgCalendarSystem>();
    if (pubgCalendarSystem != null)
    {
      pubgCalendarSystem.OnStateReceived -= new Action<PubgCalendarStateMessage>(this.OnCalendarState);
      pubgCalendarSystem.OnClaimResultReceived -= new Action<PubgCalendarClaimResultMessage>(this.OnCalendarClaimResult);
    }
    PubgEventsSystem pubgEventsSystem = this.EntityManager.SystemOrNull<PubgEventsSystem>();
    if (pubgEventsSystem != null)
    {
      pubgEventsSystem.OnHubStateReceived -= new Action<PubgEventsHubStateMessage>(this.OnEventsHubState);
      pubgEventsSystem.OnEventStateReceived -= new Action<PubgEventStateMessage>(this.OnEventState);
      pubgEventsSystem.OnClaimResultReceived -= new Action<PubgEventClaimResultMessage>(this.OnEventClaimResult);
    }
    PubgEventInventorySystem eventInventorySystem = this.EntityManager.SystemOrNull<PubgEventInventorySystem>();
    if (eventInventorySystem != null)
      eventInventorySystem.OnInventoryStateReceived -= new Action<PubgEventInventoryStateMessage>(this.OnInventoryState);
    this._systemSubscribed = false;
  }

  private void SubscribeButtons()
  {
    if (this.Hub == null)
      return;
    ((BaseButton) this.Hub.PlayButton).OnPressed -= new Action<BaseButton.ButtonEventArgs>(this.OnPlayPressed);
    ((BaseButton) this.Hub.PlayButton).OnPressed += new Action<BaseButton.ButtonEventArgs>(this.OnPlayPressed);
    ((BaseButton) this.Hub.DiscordButton).OnPressed -= new Action<BaseButton.ButtonEventArgs>(this.OnDiscordPressed);
    ((BaseButton) this.Hub.DiscordButton).OnPressed += new Action<BaseButton.ButtonEventArgs>(this.OnDiscordPressed);
    ((BaseButton) this.Hub.SettingsButton).OnPressed -= new Action<BaseButton.ButtonEventArgs>(this.OnSettingsPressed);
    ((BaseButton) this.Hub.SettingsButton).OnPressed += new Action<BaseButton.ButtonEventArgs>(this.OnSettingsPressed);
    ((BaseButton) this.Hub.CalendarOpenButton).OnPressed -= new Action<BaseButton.ButtonEventArgs>(this.OnCalendarOpenPressed);
    ((BaseButton) this.Hub.CalendarOpenButton).OnPressed += new Action<BaseButton.ButtonEventArgs>(this.OnCalendarOpenPressed);
    ((Control) this.Hub.CalendarOpenButton).OnMouseEntered -= new Action<GUIMouseHoverEventArgs>(this.OnCalendarCardMouseEntered);
    ((Control) this.Hub.CalendarOpenButton).OnMouseEntered += new Action<GUIMouseHoverEventArgs>(this.OnCalendarCardMouseEntered);
    ((Control) this.Hub.CalendarOpenButton).OnMouseExited -= new Action<GUIMouseHoverEventArgs>(this.OnCalendarCardMouseExited);
    ((Control) this.Hub.CalendarOpenButton).OnMouseExited += new Action<GUIMouseHoverEventArgs>(this.OnCalendarCardMouseExited);
    ((BaseButton) this.Hub.InventoryButton).OnPressed -= new Action<BaseButton.ButtonEventArgs>(this.OnInventoryPressed);
    ((BaseButton) this.Hub.InventoryButton).OnPressed += new Action<BaseButton.ButtonEventArgs>(this.OnInventoryPressed);
    ((BaseButton) this.Hub.SponsorsButton).OnPressed -= new Action<BaseButton.ButtonEventArgs>(this.OnSponsorsPressed);
    ((BaseButton) this.Hub.SponsorsButton).OnPressed += new Action<BaseButton.ButtonEventArgs>(this.OnSponsorsPressed);
    ((BaseButton) this.Hub.EventsRouletteButton).OnPressed -= new Action<BaseButton.ButtonEventArgs>(this.OnEventsRoulettePressed);
    ((BaseButton) this.Hub.EventsRouletteButton).OnPressed += new Action<BaseButton.ButtonEventArgs>(this.OnEventsRoulettePressed);
    ((Control) this.Hub.EventsRouletteButton).OnMouseEntered -= new Action<GUIMouseHoverEventArgs>(this.OnEventsRouletteMouseEntered);
    ((Control) this.Hub.EventsRouletteButton).OnMouseEntered += new Action<GUIMouseHoverEventArgs>(this.OnEventsRouletteMouseEntered);
    ((Control) this.Hub.EventsRouletteButton).OnMouseExited -= new Action<GUIMouseHoverEventArgs>(this.OnEventsRouletteMouseExited);
    ((Control) this.Hub.EventsRouletteButton).OnMouseExited += new Action<GUIMouseHoverEventArgs>(this.OnEventsRouletteMouseExited);
    ((BaseButton) this.Hub.EventsPromoButton).OnPressed -= new Action<BaseButton.ButtonEventArgs>(this.OnEventsPromoPressed);
    ((BaseButton) this.Hub.EventsPromoButton).OnPressed += new Action<BaseButton.ButtonEventArgs>(this.OnEventsPromoPressed);
    ((Control) this.Hub.EventsPromoButton).OnMouseEntered -= new Action<GUIMouseHoverEventArgs>(this.OnEventsPromoMouseEntered);
    ((Control) this.Hub.EventsPromoButton).OnMouseEntered += new Action<GUIMouseHoverEventArgs>(this.OnEventsPromoMouseEntered);
    ((Control) this.Hub.EventsPromoButton).OnMouseExited -= new Action<GUIMouseHoverEventArgs>(this.OnEventsPromoMouseExited);
    ((Control) this.Hub.EventsPromoButton).OnMouseExited += new Action<GUIMouseHoverEventArgs>(this.OnEventsPromoMouseExited);
    ((BaseButton) this.Hub.EventsFeatureButton).OnPressed -= new Action<BaseButton.ButtonEventArgs>(this.OnEventsFeaturePressed);
    ((BaseButton) this.Hub.EventsFeatureButton).OnPressed += new Action<BaseButton.ButtonEventArgs>(this.OnEventsFeaturePressed);
    ((Control) this.Hub.EventsFeatureButton).OnMouseEntered -= new Action<GUIMouseHoverEventArgs>(this.OnEventsFeatureMouseEntered);
    ((Control) this.Hub.EventsFeatureButton).OnMouseEntered += new Action<GUIMouseHoverEventArgs>(this.OnEventsFeatureMouseEntered);
    ((Control) this.Hub.EventsFeatureButton).OnMouseExited -= new Action<GUIMouseHoverEventArgs>(this.OnEventsFeatureMouseExited);
    ((Control) this.Hub.EventsFeatureButton).OnMouseExited += new Action<GUIMouseHoverEventArgs>(this.OnEventsFeatureMouseExited);
  }

  private void UnsubscribeButtons()
  {
    if (this.Hub == null)
      return;
    ((BaseButton) this.Hub.PlayButton).OnPressed -= new Action<BaseButton.ButtonEventArgs>(this.OnPlayPressed);
    ((BaseButton) this.Hub.DiscordButton).OnPressed -= new Action<BaseButton.ButtonEventArgs>(this.OnDiscordPressed);
    ((BaseButton) this.Hub.SettingsButton).OnPressed -= new Action<BaseButton.ButtonEventArgs>(this.OnSettingsPressed);
    ((BaseButton) this.Hub.CalendarOpenButton).OnPressed -= new Action<BaseButton.ButtonEventArgs>(this.OnCalendarOpenPressed);
    ((Control) this.Hub.CalendarOpenButton).OnMouseEntered -= new Action<GUIMouseHoverEventArgs>(this.OnCalendarCardMouseEntered);
    ((Control) this.Hub.CalendarOpenButton).OnMouseExited -= new Action<GUIMouseHoverEventArgs>(this.OnCalendarCardMouseExited);
    ((BaseButton) this.Hub.InventoryButton).OnPressed -= new Action<BaseButton.ButtonEventArgs>(this.OnInventoryPressed);
    ((BaseButton) this.Hub.SponsorsButton).OnPressed -= new Action<BaseButton.ButtonEventArgs>(this.OnSponsorsPressed);
    ((BaseButton) this.Hub.EventsRouletteButton).OnPressed -= new Action<BaseButton.ButtonEventArgs>(this.OnEventsRoulettePressed);
    ((Control) this.Hub.EventsRouletteButton).OnMouseEntered -= new Action<GUIMouseHoverEventArgs>(this.OnEventsRouletteMouseEntered);
    ((Control) this.Hub.EventsRouletteButton).OnMouseExited -= new Action<GUIMouseHoverEventArgs>(this.OnEventsRouletteMouseExited);
    ((BaseButton) this.Hub.EventsPromoButton).OnPressed -= new Action<BaseButton.ButtonEventArgs>(this.OnEventsPromoPressed);
    ((Control) this.Hub.EventsPromoButton).OnMouseEntered -= new Action<GUIMouseHoverEventArgs>(this.OnEventsPromoMouseEntered);
    ((Control) this.Hub.EventsPromoButton).OnMouseExited -= new Action<GUIMouseHoverEventArgs>(this.OnEventsPromoMouseExited);
    ((BaseButton) this.Hub.EventsFeatureButton).OnPressed -= new Action<BaseButton.ButtonEventArgs>(this.OnEventsFeaturePressed);
    ((Control) this.Hub.EventsFeatureButton).OnMouseEntered -= new Action<GUIMouseHoverEventArgs>(this.OnEventsFeatureMouseEntered);
    ((Control) this.Hub.EventsFeatureButton).OnMouseExited -= new Action<GUIMouseHoverEventArgs>(this.OnEventsFeatureMouseExited);
  }

  private void OnPlayPressed(BaseButton.ButtonEventArgs args)
  {
    this._stateManager.RequestStateChange<LobbyState>();
  }

  private void OnDiscordPressed(BaseButton.ButtonEventArgs args)
  {
    this._uriOpener.OpenUri("https://discord.gg/xdQ4vSKRB8");
  }

  private void OnSettingsPressed(BaseButton.ButtonEventArgs args)
  {
    this.UIManager.GetUIController<OptionsUIController>().ToggleWindow();
  }

  private void OnCalendarOpenPressed(BaseButton.ButtonEventArgs args)
  {
    this.EnsureCalendarWindow();
    ((BaseWindow) this._calendarWindow)?.OpenCentered();
    if (this._calendarState != null)
      this._calendarWindow?.UpdateState(this._calendarState);
    this.EntityManager.System<PubgCalendarSystem>().RequestCalendarState();
  }

  private void OnEventsRoulettePressed(BaseButton.ButtonEventArgs args)
  {
    this.OpenEventsWindow(this.ResolveEventKeyForCard("discord_link_once", 0));
  }

  private void OnEventsPromoPressed(BaseButton.ButtonEventArgs args)
  {
    this.OpenEventsWindow(this.ResolveEventKeyForCard("mars_opening", 1));
  }

  private void OnEventsFeaturePressed(BaseButton.ButtonEventArgs args)
  {
    this.OpenEventsWindow(this.ResolveEventKeyForCard("mapper_cookie", 2));
  }

  private void OnCalendarCardMouseEntered(GUIMouseHoverEventArgs args)
  {
    this._calendarCardHovered = true;
    this.UpdateInteractiveCardStyles();
  }

  private void OnCalendarCardMouseExited(GUIMouseHoverEventArgs args)
  {
    this._calendarCardHovered = false;
    this.UpdateInteractiveCardStyles();
  }

  private void OnEventsRouletteMouseEntered(GUIMouseHoverEventArgs args)
  {
    this._eventsRouletteCardHovered = true;
    this.UpdateInteractiveCardStyles();
  }

  private void OnEventsRouletteMouseExited(GUIMouseHoverEventArgs args)
  {
    this._eventsRouletteCardHovered = false;
    this.UpdateInteractiveCardStyles();
  }

  private void OnEventsPromoMouseEntered(GUIMouseHoverEventArgs args)
  {
    this._eventsPromoCardHovered = true;
    this.UpdateInteractiveCardStyles();
  }

  private void OnEventsPromoMouseExited(GUIMouseHoverEventArgs args)
  {
    this._eventsPromoCardHovered = false;
    this.UpdateInteractiveCardStyles();
  }

  private void OnEventsFeatureMouseEntered(GUIMouseHoverEventArgs args)
  {
    this._eventsFeatureCardHovered = true;
    this.UpdateInteractiveCardStyles();
  }

  private void OnEventsFeatureMouseExited(GUIMouseHoverEventArgs args)
  {
    this._eventsFeatureCardHovered = false;
    this.UpdateInteractiveCardStyles();
  }

  private void OpenEventsWindow(string? preferredEventKey)
  {
    this.EnsureEventsWindow();
    ((BaseWindow) this._eventsWindow)?.OpenCentered();
    this._eventsWindow?.SetError(string.Empty);
    this._pendingPreferredEventKey = string.IsNullOrWhiteSpace(preferredEventKey) ? this._lastSelectedEventKey : preferredEventKey;
    PubgEventsSystem eventsSystem = this.EntityManager.System<PubgEventsSystem>();
    if (this._eventsHubState != null)
      this._eventsWindow?.UpdateHub(this._eventsHubState, this._pendingPreferredEventKey ?? this._eventsWindow?.SelectedEventKey);
    if (this._eventsHubState == null && this._pendingPreferredEventKey != null)
      this.RequestEventStateIfStale(eventsSystem, this._pendingPreferredEventKey, true);
    this.RequestHubIfStale(eventsSystem, this._eventsHubState == null);
  }

  private void OnInventoryPressed(BaseButton.ButtonEventArgs args)
  {
    this.EnsureInventoryWindow();
    ((BaseWindow) this._inventoryWindow)?.OpenCentered();
    PubgEventInventorySystem eventInventorySystem = this.EntityManager.System<PubgEventInventorySystem>();
    if (eventInventorySystem.LastInventoryState != null)
      this._inventoryWindow?.UpdateInventory(eventInventorySystem.LastInventoryState);
    eventInventorySystem.RequestInventory();
  }

  private void OnSponsorsPressed(BaseButton.ButtonEventArgs args)
  {
    this.UIManager.GetUIController<MainMenuUIController>().OpenSponsorDisplayWindow();
  }

  private void OnCalendarClaimRequested()
  {
    this.EntityManager.System<PubgCalendarSystem>().ClaimNextDay();
  }

  private void OnEventsWindowSelected(string eventKey)
  {
    this._lastSelectedEventKey = eventKey;
    PubgEventsSystem eventsSystem = this.EntityManager.System<PubgEventsSystem>();
    PubgEventDetailInfo state;
    if (eventsSystem.LastEventStates.TryGetValue(eventKey, out state))
      this._eventsWindow?.UpdateEventState(state);
    else
      this._eventsWindow?.ShowLoadingState(eventKey);
    this.RequestEventStateIfStale(eventsSystem, eventKey);
  }

  private void OnEventsWindowClaimRequested(string eventKey, string claimType, string claimId)
  {
    if (string.IsNullOrWhiteSpace(eventKey))
      return;
    PubgEventsSystem eventsSystem = this.EntityManager.System<PubgEventsSystem>();
    PubgEventDetailInfo state;
    if (eventsSystem.LastEventStates.TryGetValue(eventKey, out state) && !PubgPreLobbyHubUIController.IsEventActiveNow(state, DateTime.UtcNow))
    {
      this._eventsWindow?.SetError(Loc.GetString("pubg-events-error-event-not-active"));
    }
    else
    {
      if (!eventsSystem.TryClaim(eventKey, claimType, claimId))
        return;
      this._eventsWindow?.SetError(string.Empty);
      this.ApplyOptimisticClaim(eventKey, claimType, claimId, eventsSystem);
    }
  }

  private void OnEventsWindowRefreshRequested(string eventKey)
  {
    if (string.IsNullOrWhiteSpace(eventKey))
      return;
    PubgEventsSystem eventsSystem = this.EntityManager.System<PubgEventsSystem>();
    this.RequestHubIfStale(eventsSystem, true);
    this.RequestEventStateIfStale(eventsSystem, eventKey, true);
  }

  private void OnInventoryWindowRefreshRequested()
  {
    this.EntityManager.System<PubgEventInventorySystem>().RequestInventory();
  }

  private void EnsureCalendarWindow()
  {
    if (this._calendarWindow != null)
      return;
    this._calendarWindow = this.UIManager.CreateWindow<PubgPreLobbyCalendarWindow>();
    ((BaseWindow) this._calendarWindow).OnClose += new Action(this.OnCalendarWindowClosed);
    this._calendarWindow.ClaimRequested += new Action(this.OnCalendarClaimRequested);
  }

  private void EnsureEventsWindow()
  {
    if (this._eventsWindow != null)
      return;
    this._eventsWindow = this.UIManager.CreateWindow<PubgPreLobbyEventsWindow>();
    ((BaseWindow) this._eventsWindow).OnClose += new Action(this.OnEventsWindowClosed);
    this._eventsWindow.EventSelected += new Action<string>(this.OnEventsWindowSelected);
    this._eventsWindow.ClaimRequested += new Action<string, string, string>(this.OnEventsWindowClaimRequested);
    this._eventsWindow.RefreshRequested += new Action<string>(this.OnEventsWindowRefreshRequested);
  }

  private void EnsureInventoryWindow()
  {
    if (this._inventoryWindow != null)
      return;
    this._inventoryWindow = this.UIManager.CreateWindow<PubgPreLobbyInventoryWindow>();
    ((BaseWindow) this._inventoryWindow).OnClose += new Action(this.OnInventoryWindowClosed);
    this._inventoryWindow.RefreshRequested += new Action(this.OnInventoryWindowRefreshRequested);
  }

  private void OnCalendarWindowClosed() => this.CloseCalendarWindow();

  private void OnEventsWindowClosed() => this.CloseEventsWindow();

  private void OnInventoryWindowClosed() => this.CloseInventoryWindow();

  private void CloseCalendarWindow()
  {
    if (this._calendarWindow == null)
      return;
    ((BaseWindow) this._calendarWindow).OnClose -= new Action(this.OnCalendarWindowClosed);
    this._calendarWindow.ClaimRequested -= new Action(this.OnCalendarClaimRequested);
    ((BaseWindow) this._calendarWindow).Close();
    this._calendarWindow = (PubgPreLobbyCalendarWindow) null;
  }

  private void CloseEventsWindow()
  {
    if (this._eventsWindow == null)
      return;
    ((BaseWindow) this._eventsWindow).OnClose -= new Action(this.OnEventsWindowClosed);
    this._eventsWindow.EventSelected -= new Action<string>(this.OnEventsWindowSelected);
    this._eventsWindow.ClaimRequested -= new Action<string, string, string>(this.OnEventsWindowClaimRequested);
    this._eventsWindow.RefreshRequested -= new Action<string>(this.OnEventsWindowRefreshRequested);
    ((BaseWindow) this._eventsWindow).Close();
    this._eventsWindow = (PubgPreLobbyEventsWindow) null;
  }

  private void CloseInventoryWindow()
  {
    if (this._inventoryWindow == null)
      return;
    ((BaseWindow) this._inventoryWindow).OnClose -= new Action(this.OnInventoryWindowClosed);
    this._inventoryWindow.RefreshRequested -= new Action(this.OnInventoryWindowRefreshRequested);
    ((BaseWindow) this._inventoryWindow).Close();
    this._inventoryWindow = (PubgPreLobbyInventoryWindow) null;
  }

  private void OnSkinState(SkinStateMessage msg)
  {
    this._coins = msg.PlayerCoins;
    this._scrap = msg.PlayerScrap;
    this._premiumCoins = msg.PlayerPremiumCoins;
    this._playerLevel = msg.PlayerLevel;
    this._sponsorDisplayTier = msg.SponsorDisplayTier;
    this._sponsorDisplayMode = msg.SponsorDisplayMode;
    this.UpdateBalanceLabels();
    this.UpdateProfileLabels();
  }

  private void OnBalanceUpdate(BalanceUpdateMessage msg)
  {
    this._coins = msg.Coins;
    this._scrap = msg.Scrap;
    this._premiumCoins = msg.PremiumCoins;
    this.UpdateBalanceLabels();
  }

  private void OnBattlePassState(BattlePassStateMessage msg)
  {
    this._battlePassState = msg;
    this.UpdateBattlePassLabels();
  }

  private void OnCalendarState(PubgCalendarStateMessage msg)
  {
    this._calendarState = msg;
    this._calendarWindow?.UpdateState(msg);
  }

  private void OnCalendarClaimResult(PubgCalendarClaimResultMessage msg)
  {
    if (this._calendarWindow == null)
      return;
    if (msg.Success)
      this._calendarWindow.SetError(string.Empty);
    else
      this._calendarWindow.SetError(this.ResolveCalendarErrorText(msg.Error));
  }

  private void OnEventsHubState(PubgEventsHubStateMessage msg)
  {
    this._eventsHubState = msg;
    this._lastHubResponseAt = DateTime.UtcNow;
    if (string.IsNullOrWhiteSpace(this._lastSelectedEventKey) && msg.Events.Count > 0)
      this._lastSelectedEventKey = msg.Events.OrderBy<PubgEventCardInfo, int>((Func<PubgEventCardInfo, int>) (card => card.SortOrder)).First<PubgEventCardInfo>().EventKey;
    PubgEventsSystem eventsSystem = this.EntityManager.System<PubgEventsSystem>();
    this.PreloadEventStates(msg, eventsSystem);
    this.UpdateEventCardLabels();
    this.UpdateEventsRedDots();
    this._eventsWindow?.UpdateHub(msg, this._pendingPreferredEventKey ?? this._eventsWindow?.SelectedEventKey);
    this._pendingPreferredEventKey = (string) null;
  }

  private void RequestHubIfStale(PubgEventsSystem eventsSystem, bool force = false)
  {
    DateTime utcNow = DateTime.UtcNow;
    bool flag1 = utcNow - this._lastHubRequestAt < TimeSpan.FromSeconds(3L);
    bool flag2 = this._eventsHubState != null && utcNow - this._lastHubResponseAt < TimeSpan.FromSeconds(8L);
    if (!force && flag1 | flag2)
      return;
    this._lastHubRequestAt = utcNow;
    eventsSystem.RequestHub(force);
  }

  private void OnEventState(PubgEventStateMessage msg)
  {
    this._lastEventStateResponseAt[msg.State.EventKey] = DateTime.UtcNow;
    this.UpdateHubCardFromEventState(msg.State);
    this._eventsWindow?.UpdateEventState(msg.State);
    this.UpdateEventCardLabels();
    this.UpdateEventsRedDots();
  }

  private void OnEventClaimResult(PubgEventClaimResultMessage msg)
  {
    PubgEventsSystem eventsSystem = this.EntityManager.System<PubgEventsSystem>();
    if (msg.HasBalances)
    {
      this._coins = msg.Coins;
      this._scrap = msg.Scrap;
      this._premiumCoins = msg.PremiumCoins;
      this.UpdateBalanceLabels();
    }
    if (msg.Success)
    {
      this.ApplyClaimResultToLocalState(msg, eventsSystem);
      this._pendingEventClaim = (PubgPreLobbyHubUIController.PendingEventClaim) null;
      this._eventsWindow?.SetError(string.Empty);
    }
    else
    {
      string eventKey = this._pendingEventClaim?.EventKey ?? this._lastSelectedEventKey ?? this._eventsWindow?.SelectedEventKey;
      this.RollbackOptimisticClaim(eventsSystem);
      this._pendingEventClaim = (PubgPreLobbyHubUIController.PendingEventClaim) null;
      this._eventsWindow?.SetError(this.ResolveEventErrorText(msg.Error));
      this.RequestHubIfStale(eventsSystem, true);
      if (string.IsNullOrWhiteSpace(eventKey))
        return;
      this.RequestEventStateIfStale(eventsSystem, eventKey, true);
    }
  }

  private void ApplyOptimisticClaim(
    string eventKey,
    string claimType,
    string claimId,
    PubgEventsSystem eventsSystem)
  {
    PubgEventDetailInfo pubgEventDetailInfo1;
    if (!eventsSystem.LastEventStates.TryGetValue(eventKey, out pubgEventDetailInfo1))
      return;
    int coins = this._coins;
    int scrap = this._scrap;
    int premiumCoins = this._premiumCoins;
    PubgEventDetailInfo pubgEventDetailInfo2 = PubgPreLobbyHubUIController.CloneEventState(pubgEventDetailInfo1);
    if (!this.TryApplyClaimToState(pubgEventDetailInfo1, claimType, claimId))
      return;
    this._pendingEventClaim = new PubgPreLobbyHubUIController.PendingEventClaim()
    {
      EventKey = eventKey,
      CoinsBefore = coins,
      ScrapBefore = scrap,
      PremiumCoinsBefore = premiumCoins,
      EventStateBefore = pubgEventDetailInfo2
    };
    this.UpdateHubCardFromEventState(pubgEventDetailInfo1);
    this._eventsWindow?.UpdateEventState(pubgEventDetailInfo1);
    this.UpdateEventCardLabels();
    this.UpdateEventsRedDots();
    this.UpdateBalanceLabels();
  }

  private void ApplyClaimResultToLocalState(
    PubgEventClaimResultMessage msg,
    PubgEventsSystem eventsSystem)
  {
    string key = this._pendingEventClaim?.EventKey ?? this._lastSelectedEventKey ?? this._eventsWindow?.SelectedEventKey;
    PubgEventDetailInfo state;
    if (string.IsNullOrWhiteSpace(key) || !eventsSystem.LastEventStates.TryGetValue(key, out state))
      return;
    if (msg.ClaimResult != null)
      this.TryApplyClaimToState(state, msg.ClaimResult.ClaimType, msg.ClaimResult.ClaimId, !msg.HasBalances);
    if (state.MarsState != null)
    {
      foreach (PubgEventWalletDeltaInfo eventWalletDeltaInfo in msg.WalletsDelta)
      {
        if (string.Equals(eventWalletDeltaInfo.WalletKey, state.MarsState.Wallet.WalletKey, StringComparison.Ordinal))
        {
          state.MarsState.Wallet.Balance = eventWalletDeltaInfo.Balance;
          state.MarsState.Wallet.TotalEarned = eventWalletDeltaInfo.TotalEarned;
        }
      }
    }
    this.RefreshClaimableFlags(state);
    this.UpdateHubCardFromEventState(state);
    this._eventsWindow?.UpdateEventState(state);
    this.UpdateEventCardLabels();
    this.UpdateEventsRedDots();
  }

  private void RollbackOptimisticClaim(PubgEventsSystem eventsSystem)
  {
    if (this._pendingEventClaim == null)
      return;
    this._coins = this._pendingEventClaim.CoinsBefore;
    this._scrap = this._pendingEventClaim.ScrapBefore;
    this._premiumCoins = this._pendingEventClaim.PremiumCoinsBefore;
    this.UpdateBalanceLabels();
    eventsSystem.LastEventStates[this._pendingEventClaim.EventKey] = PubgPreLobbyHubUIController.CloneEventState(this._pendingEventClaim.EventStateBefore);
    PubgEventDetailInfo lastEventState = eventsSystem.LastEventStates[this._pendingEventClaim.EventKey];
    this.UpdateHubCardFromEventState(lastEventState);
    this._eventsWindow?.UpdateEventState(lastEventState);
    this.UpdateEventCardLabels();
    this.UpdateEventsRedDots();
  }

  private bool TryApplyClaimToState(
    PubgEventDetailInfo state,
    string claimType,
    string claimId,
    bool applyBalanceReward = true)
  {
    if (string.IsNullOrWhiteSpace(claimType) || string.IsNullOrWhiteSpace(claimId))
      return false;
    bool flag = false;
    if (string.Equals(claimType, "one_time", StringComparison.OrdinalIgnoreCase))
    {
      PubgDiscordEventStateInfo discordState = state.DiscordState;
      if (discordState == null || !string.Equals(discordState.ClaimKey, claimId, StringComparison.Ordinal) || discordState.AlreadyClaimed)
        return false;
      discordState.AlreadyClaimed = true;
      discordState.CanClaim = false;
      if (applyBalanceReward)
        this.ApplyBalanceReward(discordState.RewardType, discordState.RewardValue);
      flag = true;
    }
    else if (string.Equals(claimType, "task", StringComparison.OrdinalIgnoreCase))
    {
      PubgMarsEventStateInfo marsState = state.MarsState;
      if (marsState == null)
        return false;
      PubgMarsTaskInfo pubgMarsTaskInfo = marsState.LoginTasks.FirstOrDefault<PubgMarsTaskInfo>((Func<PubgMarsTaskInfo, bool>) (item => string.Equals(item.TaskKey, claimId, StringComparison.Ordinal))) ?? marsState.ChallengeTasks.FirstOrDefault<PubgMarsTaskInfo>((Func<PubgMarsTaskInfo, bool>) (item => string.Equals(item.TaskKey, claimId, StringComparison.Ordinal)));
      if (pubgMarsTaskInfo == null || pubgMarsTaskInfo.IsClaimed || !pubgMarsTaskInfo.IsClaimable)
        return false;
      pubgMarsTaskInfo.IsClaimed = true;
      pubgMarsTaskInfo.IsClaimable = false;
      marsState.Points += pubgMarsTaskInfo.TokenReward;
      marsState.Wallet.Balance += pubgMarsTaskInfo.TokenReward;
      marsState.Wallet.TotalEarned += pubgMarsTaskInfo.TokenReward;
      flag = true;
    }
    else if (string.Equals(claimType, "milestone", StringComparison.OrdinalIgnoreCase))
    {
      PubgMarsEventStateInfo marsState = state.MarsState;
      if (marsState == null)
        return false;
      PubgMarsMilestoneInfo marsMilestoneInfo = marsState.Milestones.FirstOrDefault<PubgMarsMilestoneInfo>((Func<PubgMarsMilestoneInfo, bool>) (item => string.Equals(item.MilestoneId, claimId, StringComparison.Ordinal)));
      if (marsMilestoneInfo == null || marsMilestoneInfo.IsClaimed || !marsMilestoneInfo.IsClaimable)
        return false;
      marsMilestoneInfo.IsClaimed = true;
      marsMilestoneInfo.IsClaimable = false;
      if (applyBalanceReward)
        this.ApplyBalanceReward(marsMilestoneInfo.RewardType, marsMilestoneInfo.RewardValue);
      flag = true;
    }
    if (!flag)
      return false;
    this.RefreshClaimableFlags(state);
    return true;
  }

  private void RefreshClaimableFlags(PubgEventDetailInfo state)
  {
    PubgEventDetailInfo pubgEventDetailInfo = state;
    PubgDiscordEventStateInfo discordState = state.DiscordState;
    int num = discordState != null ? (discordState.CanClaim ? 1 : 0) : 0;
    pubgEventDetailInfo.RedDotOneTime = num != 0;
    if (state.MarsState == null)
    {
      state.RedDotTasks = false;
      state.RedDotMilestones = false;
    }
    else
    {
      state.RedDotTasks = state.MarsState.LoginTasks.Any<PubgMarsTaskInfo>((Func<PubgMarsTaskInfo, bool>) (task => task.IsClaimable && !task.IsClaimed)) || state.MarsState.ChallengeTasks.Any<PubgMarsTaskInfo>((Func<PubgMarsTaskInfo, bool>) (task => task.IsClaimable && !task.IsClaimed));
      state.RedDotMilestones = state.MarsState.Milestones.Any<PubgMarsMilestoneInfo>((Func<PubgMarsMilestoneInfo, bool>) (milestone => milestone.IsClaimable && !milestone.IsClaimed));
    }
    state.HasClaimable = state.RedDotOneTime || state.RedDotTasks || state.RedDotMilestones;
  }

  private void ApplyBalanceReward(string rewardType, string rewardValue)
  {
    int result;
    if (!int.TryParse(rewardValue, out result) || result <= 0)
      return;
    if (string.Equals(rewardType, "coins", StringComparison.OrdinalIgnoreCase))
      this._coins += result;
    else if (string.Equals(rewardType, "scrap", StringComparison.OrdinalIgnoreCase))
    {
      this._scrap += result;
    }
    else
    {
      if (!string.Equals(rewardType, "premiumCoins", StringComparison.OrdinalIgnoreCase))
        return;
      this._premiumCoins += result;
    }
  }

  private void UpdateHubCardFromEventState(PubgEventDetailInfo state)
  {
    if (this._eventsHubState == null)
      return;
    PubgEventCardInfo pubgEventCardInfo = this._eventsHubState.Events.FirstOrDefault<PubgEventCardInfo>((Func<PubgEventCardInfo, bool>) (item => string.Equals(item.EventKey, state.EventKey, StringComparison.Ordinal)));
    if (pubgEventCardInfo == null)
      return;
    pubgEventCardInfo.TitleKey = state.TitleKey;
    pubgEventCardInfo.DescriptionKey = state.DescriptionKey;
    pubgEventCardInfo.StartAt = state.StartAt;
    pubgEventCardInfo.EndAt = state.EndAt;
    pubgEventCardInfo.SortOrder = state.SortOrder;
    pubgEventCardInfo.IsActive = state.IsActive;
    pubgEventCardInfo.HasClaimable = state.HasClaimable;
    pubgEventCardInfo.RedDotOneTime = state.RedDotOneTime;
    pubgEventCardInfo.RedDotTasks = state.RedDotTasks;
    pubgEventCardInfo.RedDotMilestones = state.RedDotMilestones;
    pubgEventCardInfo.HubSummary = PubgPreLobbyHubUIController.BuildHubSummaryFromEventState(state);
    if (!string.IsNullOrWhiteSpace(state.SelectorIconPath))
      pubgEventCardInfo.SelectorIconPath = state.SelectorIconPath;
    if (!string.IsNullOrWhiteSpace(state.SelectorBannerPath))
      pubgEventCardInfo.SelectorBannerPath = state.SelectorBannerPath;
    if (string.IsNullOrWhiteSpace(state.SelectorAccentHex))
      return;
    pubgEventCardInfo.SelectorAccentHex = state.SelectorAccentHex;
  }

  private static PubgEventDetailInfo CloneEventState(PubgEventDetailInfo source)
  {
    PubgEventDetailInfo pubgEventDetailInfo = new PubgEventDetailInfo();
    pubgEventDetailInfo.EventKey = source.EventKey;
    pubgEventDetailInfo.Kind = source.Kind;
    pubgEventDetailInfo.TitleKey = source.TitleKey;
    pubgEventDetailInfo.DescriptionKey = source.DescriptionKey;
    pubgEventDetailInfo.SelectorIconPath = source.SelectorIconPath;
    pubgEventDetailInfo.SelectorBannerPath = source.SelectorBannerPath;
    pubgEventDetailInfo.SelectorAccentHex = source.SelectorAccentHex;
    pubgEventDetailInfo.StartAt = source.StartAt;
    pubgEventDetailInfo.EndAt = source.EndAt;
    pubgEventDetailInfo.SortOrder = source.SortOrder;
    pubgEventDetailInfo.IsActive = source.IsActive;
    pubgEventDetailInfo.HasClaimable = source.HasClaimable;
    pubgEventDetailInfo.RedDotOneTime = source.RedDotOneTime;
    pubgEventDetailInfo.RedDotTasks = source.RedDotTasks;
    pubgEventDetailInfo.RedDotMilestones = source.RedDotMilestones;
    PubgDiscordEventStateInfo discordEventStateInfo;
    if (source.DiscordState != null)
      discordEventStateInfo = new PubgDiscordEventStateInfo()
      {
        ClaimKey = source.DiscordState.ClaimKey,
        RewardType = source.DiscordState.RewardType,
        RewardValue = source.DiscordState.RewardValue,
        Linked = source.DiscordState.Linked,
        AlreadyClaimed = source.DiscordState.AlreadyClaimed,
        ClaimedAt = source.DiscordState.ClaimedAt,
        CanClaim = source.DiscordState.CanClaim
      };
    else
      discordEventStateInfo = (PubgDiscordEventStateInfo) null;
    pubgEventDetailInfo.DiscordState = discordEventStateInfo;
    PubgMarsEventStateInfo marsEventStateInfo;
    if (source.MarsState != null)
      marsEventStateInfo = new PubgMarsEventStateInfo()
      {
        Points = source.MarsState.Points,
        BalanceLabelKey = source.MarsState.BalanceLabelKey,
        TaskRewardLabelKey = source.MarsState.TaskRewardLabelKey,
        Wallet = new PubgEventWalletInfo()
        {
          WalletKey = source.MarsState.Wallet.WalletKey,
          Balance = source.MarsState.Wallet.Balance,
          TotalEarned = source.MarsState.Wallet.TotalEarned
        },
        WeekKey = source.MarsState.WeekKey,
        WeekStartsAt = source.MarsState.WeekStartsAt,
        WeekEndsAt = source.MarsState.WeekEndsAt,
        Milestones = source.MarsState.Milestones.Select<PubgMarsMilestoneInfo, PubgMarsMilestoneInfo>((Func<PubgMarsMilestoneInfo, PubgMarsMilestoneInfo>) (milestone => new PubgMarsMilestoneInfo()
        {
          MilestoneId = milestone.MilestoneId,
          Threshold = milestone.Threshold,
          RewardType = milestone.RewardType,
          RewardValue = milestone.RewardValue,
          DurationDays = milestone.DurationDays,
          IsClaimed = milestone.IsClaimed,
          ClaimedAt = milestone.ClaimedAt,
          IsClaimable = milestone.IsClaimable
        })).ToList<PubgMarsMilestoneInfo>(),
        LoginTasks = source.MarsState.LoginTasks.Select<PubgMarsTaskInfo, PubgMarsTaskInfo>((Func<PubgMarsTaskInfo, PubgMarsTaskInfo>) (task => new PubgMarsTaskInfo()
        {
          TemplateId = task.TemplateId,
          TaskKey = task.TaskKey,
          Category = task.Category,
          PeriodType = task.PeriodType,
          ObjectiveType = task.ObjectiveType,
          TargetValue = task.TargetValue,
          TokenReward = task.TokenReward,
          CoinsReward = task.CoinsReward,
          MinSurvivalSeconds = task.MinSurvivalSeconds,
          Progress = task.Progress,
          IsCompleted = task.IsCompleted,
          IsClaimed = task.IsClaimed,
          PeriodKey = task.PeriodKey,
          IsClaimable = task.IsClaimable
        })).ToList<PubgMarsTaskInfo>(),
        ChallengeTasks = source.MarsState.ChallengeTasks.Select<PubgMarsTaskInfo, PubgMarsTaskInfo>((Func<PubgMarsTaskInfo, PubgMarsTaskInfo>) (task => new PubgMarsTaskInfo()
        {
          TemplateId = task.TemplateId,
          TaskKey = task.TaskKey,
          Category = task.Category,
          PeriodType = task.PeriodType,
          ObjectiveType = task.ObjectiveType,
          TargetValue = task.TargetValue,
          TokenReward = task.TokenReward,
          CoinsReward = task.CoinsReward,
          MinSurvivalSeconds = task.MinSurvivalSeconds,
          Progress = task.Progress,
          IsCompleted = task.IsCompleted,
          IsClaimed = task.IsClaimed,
          PeriodKey = task.PeriodKey,
          IsClaimable = task.IsClaimable
        })).ToList<PubgMarsTaskInfo>()
      };
    else
      marsEventStateInfo = (PubgMarsEventStateInfo) null;
    pubgEventDetailInfo.MarsState = marsEventStateInfo;
    return pubgEventDetailInfo;
  }

  private static PubgEventCardHubSummaryInfo? BuildHubSummaryFromEventState(
    PubgEventDetailInfo state)
  {
    if (!string.Equals(state.EventKey, "mapper_cookie", StringComparison.OrdinalIgnoreCase))
      return (PubgEventCardHubSummaryInfo) null;
    if (state.MarsState == null)
      return (PubgEventCardHubSummaryInfo) null;
    int val1 = Math.Max(0, state.MarsState.Points);
    List<int> list1 = state.MarsState.Milestones.Select<PubgMarsMilestoneInfo, int>((Func<PubgMarsMilestoneInfo, int>) (milestone => milestone.Threshold)).Where<int>((Func<int, bool>) (threshold => threshold > 0)).Distinct<int>().OrderBy<int, int>((Func<int, int>) (threshold => threshold)).ToList<int>();
    int num1 = Math.Max(val1, list1.LastOrDefault<int>());
    int? nullable = new int?();
    foreach (int num2 in list1)
    {
      if (num2 > val1)
      {
        nullable = new int?(num2);
        break;
      }
    }
    List<PubgMarsTaskInfo> list2 = state.MarsState.ChallengeTasks.Where<PubgMarsTaskInfo>((Func<PubgMarsTaskInfo, bool>) (task => string.Equals(task.PeriodType, "daily", StringComparison.OrdinalIgnoreCase))).ToList<PubgMarsTaskInfo>();
    return new PubgEventCardHubSummaryInfo()
    {
      ProgressCurrent = val1,
      ProgressTarget = num1,
      NextRewardThreshold = nullable,
      NextRewardIn = !nullable.HasValue ? 0 : Math.Max(0, nullable.Value - val1),
      DailyCompleted = list2.Count<PubgMarsTaskInfo>((Func<PubgMarsTaskInfo, bool>) (task => task.IsCompleted || task.IsClaimed)),
      DailyTotal = list2.Count,
      MilestoneThresholds = list1
    };
  }

  private void OnInventoryState(PubgEventInventoryStateMessage msg)
  {
    this._inventoryWindow?.UpdateInventory(msg);
  }

  private string ResolveCalendarErrorText(string? error)
  {
    if (string.IsNullOrWhiteSpace(error))
      return Loc.GetString("pubg-prelobby-calendar-error-generic");
    string str1;
    if (error.StartsWith("pubg-prelobby-calendar-error-") && this._loc.TryGetString(error, ref str1))
      return str1;
    string str2;
    return this._loc.TryGetString("pubg-prelobby-calendar-error-" + error.Trim().Replace('_', '-'), ref str2) ? str2 : Loc.GetString("pubg-prelobby-calendar-error-generic");
  }

  private string ResolveEventErrorText(string? error)
  {
    if (string.IsNullOrWhiteSpace(error))
      return Loc.GetString("pubg-events-error-generic");
    string str1;
    if (error.StartsWith("pubg-events-error-") && this._loc.TryGetString(error, ref str1))
      return str1;
    string str2;
    return this._loc.TryGetString("pubg-events-error-" + error.Trim().Replace('_', '-'), ref str2) ? str2 : Loc.GetString("pubg-events-error-generic");
  }

  private void UpdateBalanceLabels()
  {
    if (this.Hub == null)
      return;
    this.Hub.CoinsLabel.Text = this._coins.ToString("N0");
    this.Hub.ScrapLabel.Text = this._scrap.ToString("N0");
    this.Hub.PremiumCoinsLabel.Text = this._premiumCoins.ToString("N0");
  }

  private void UpdateProfileLabels()
  {
    if (this.Hub == null)
      return;
    this.Hub.PlayerCkeyLabel.Text = Loc.GetString("pubg-prelobby-hub-profile-ckey", new (string, object)[1]
    {
      ("ckey", (object) (((ISharedPlayerManager) this._playerManager).LocalSession?.Name ?? Loc.GetString("pubg-prelobby-hub-player-unknown")))
    });
    this.Hub.PlayerLevelLabel.Text = Loc.GetString("pubg-prelobby-hub-profile-level", new (string, object)[1]
    {
      ("level", (object) this._playerLevel)
    });
    this.Hub.PlayerSponsorLabel.Text = Loc.GetString("pubg-prelobby-hub-profile-sponsor", new (string, object)[1]
    {
      ("tier", (object) (this._sponsorDisplayMode == SponsorDisplayMode.Hidden ? Loc.GetString("mainmenu-sponsor-display-current-hidden") : this._sponsorDisplayTier?.TierName ?? Loc.GetString("mainmenu-sponsor-display-current-none")))
    });
  }

  private void UpdateBattlePassLabels()
  {
    if (this.Hub == null)
      return;
    if (this._battlePassState == null)
    {
      this.Hub.BattlePassSeasonLabel.Text = Loc.GetString("pubg-prelobby-hub-battlepass-loading");
      this.Hub.BattlePassLevelLabel.Text = string.Empty;
      this.Hub.BattlePassProgressLabel.Text = string.Empty;
      this.Hub.BattlePassStatusLabel.Text = string.Empty;
      ((Range) this.Hub.BattlePassProgressBar).MinValue = 0.0f;
      ((Range) this.Hub.BattlePassProgressBar).MaxValue = 100f;
      ((Range) this.Hub.BattlePassProgressBar).Value = 0.0f;
      this.RenderBattlePassTasks();
    }
    else
    {
      (int level, int xpInLevel, int xpRequired) tuple = PubgPreLobbyHubUIController.ResolveBattlePassProgress(this._battlePassState, this._battlePassState.CurrentXp);
      int num = this._battlePassState.Tasks.Count<BattlePassTaskInfo>((Func<BattlePassTaskInfo, bool>) (task => task.IsCompleted && !task.XpClaimed));
      this.Hub.BattlePassSeasonLabel.Text = Loc.GetString("pubg-prelobby-hub-battlepass-season", new (string, object)[1]
      {
        ("name", (object) this._battlePassState.SeasonName)
      });
      this.Hub.BattlePassLevelLabel.Text = Loc.GetString("pubg-bp-level", new (string, object)[1]
      {
        ("level", (object) tuple.level)
      });
      this.Hub.BattlePassProgressLabel.Text = Loc.GetString("pubg-bp-xp", new (string, object)[2]
      {
        ("current", (object) tuple.xpInLevel),
        ("required", (object) tuple.xpRequired)
      });
      this.Hub.BattlePassStatusLabel.Text = Loc.GetString("pubg-prelobby-hub-battlepass-status", new (string, object)[1]
      {
        ("count", (object) num)
      });
      ((Range) this.Hub.BattlePassProgressBar).MinValue = 0.0f;
      ((Range) this.Hub.BattlePassProgressBar).MaxValue = (float) tuple.xpRequired;
      ((Range) this.Hub.BattlePassProgressBar).Value = (float) Math.Min(tuple.xpInLevel, tuple.xpRequired);
      this.RenderBattlePassTasks();
    }
  }

  private void RenderBattlePassTasks()
  {
    if (this.Hub == null)
      return;
    ((Control) this.Hub.BattlePassTasksList).RemoveAllChildren();
    if (this._battlePassState == null)
    {
      ((Control) this.Hub.BattlePassTasksList).AddChild((Control) PubgPreLobbyHubUIController.CreateBattlePassTasksPlaceholder("pubg-prelobby-hub-battlepass-loading", Color.FromHex((ReadOnlySpan<char>) "#9FB2CE", new Color?())));
    }
    else
    {
      List<BattlePassTaskInfo> list = this._battlePassState.Tasks.Where<BattlePassTaskInfo>((Func<BattlePassTaskInfo, bool>) (task => !task.IsSkipped)).OrderBy<BattlePassTaskInfo, int>((Func<BattlePassTaskInfo, int>) (task => task.Slot)).ToList<BattlePassTaskInfo>();
      if (list.Count == 0)
      {
        ((Control) this.Hub.BattlePassTasksList).AddChild((Control) PubgPreLobbyHubUIController.CreateBattlePassTasksPlaceholder("pubg-prelobby-hub-bp-tasks-empty", Color.FromHex((ReadOnlySpan<char>) "#9FB2CE", new Color?())));
      }
      else
      {
        foreach (BattlePassTaskInfo task in list)
          ((Control) this.Hub.BattlePassTasksList).AddChild(PubgPreLobbyHubUIController.CreateBattlePassTaskRow(task));
      }
    }
  }

  private static Label CreateBattlePassTasksPlaceholder(string locKey, Color textColor)
  {
    Label tasksPlaceholder = new Label();
    tasksPlaceholder.Text = Loc.GetString(locKey);
    ((Control) tasksPlaceholder).HorizontalExpand = true;
    tasksPlaceholder.FontColorOverride = new Color?(textColor);
    return tasksPlaceholder;
  }

  private static Control CreateBattlePassTaskRow(BattlePassTaskInfo task)
  {
    bool xpClaimed = task.XpClaimed;
    bool flag = task.IsCompleted && !task.XpClaimed;
    Color color1 = flag ? Color.FromHex((ReadOnlySpan<char>) "#3A3A22", new Color?()) : (xpClaimed ? Color.FromHex((ReadOnlySpan<char>) "#242E24", new Color?()) : Color.FromHex((ReadOnlySpan<char>) "#22242B", new Color?()));
    Color color2 = flag ? Color.FromHex((ReadOnlySpan<char>) "#DDA02B", new Color?()) : (xpClaimed ? Color.FromHex((ReadOnlySpan<char>) "#507650", new Color?()) : Color.FromHex((ReadOnlySpan<char>) "#575C66", new Color?()));
    Color color3 = flag ? Color.FromHex((ReadOnlySpan<char>) "#5A4520", new Color?()) : (xpClaimed ? Color.FromHex((ReadOnlySpan<char>) "#2A442A", new Color?()) : Color.FromHex((ReadOnlySpan<char>) "#2A2C34", new Color?()));
    Color color4 = flag ? Color.FromHex((ReadOnlySpan<char>) "#F2A900", new Color?()) : (xpClaimed ? Color.FromHex((ReadOnlySpan<char>) "#669766", new Color?()) : Color.FromHex((ReadOnlySpan<char>) "#707785", new Color?()));
    string str1 = xpClaimed ? "pubg-prelobby-hub-bp-task-claimed" : (flag ? "pubg-prelobby-hub-bp-task-ready" : "pubg-prelobby-hub-bp-task-in-progress");
    PanelContainer panelContainer1 = new PanelContainer();
    ((Control) panelContainer1).HorizontalExpand = true;
    StyleBoxFlat styleBoxFlat1 = new StyleBoxFlat();
    styleBoxFlat1.BackgroundColor = color1;
    styleBoxFlat1.BorderColor = color2;
    styleBoxFlat1.BorderThickness = new Thickness(1f);
    ((StyleBox) styleBoxFlat1).ContentMarginLeftOverride = new float?(8f);
    ((StyleBox) styleBoxFlat1).ContentMarginRightOverride = new float?(8f);
    ((StyleBox) styleBoxFlat1).ContentMarginTopOverride = new float?(6f);
    ((StyleBox) styleBoxFlat1).ContentMarginBottomOverride = new float?(6f);
    panelContainer1.PanelOverride = (StyleBox) styleBoxFlat1;
    PanelContainer battlePassTaskRow = panelContainer1;
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 1;
    ((Control) boxContainer1).HorizontalExpand = true;
    boxContainer1.SeparationOverride = new int?(4);
    BoxContainer boxContainer2 = boxContainer1;
    BoxContainer boxContainer3 = new BoxContainer();
    boxContainer3.Orientation = (BoxContainer.LayoutOrientation) 0;
    ((Control) boxContainer3).HorizontalExpand = true;
    boxContainer3.SeparationOverride = new int?(6);
    BoxContainer boxContainer4 = boxContainer3;
    PanelContainer panelContainer2 = new PanelContainer();
    ((Control) panelContainer2).MinSize = new Vector2(40f, 22f);
    ((Control) panelContainer2).VerticalAlignment = (Control.VAlignment) 2;
    StyleBoxFlat styleBoxFlat2 = new StyleBoxFlat();
    styleBoxFlat2.BackgroundColor = Color.FromHex((ReadOnlySpan<char>) "#11192A", new Color?());
    styleBoxFlat2.BorderColor = Color.FromHex((ReadOnlySpan<char>) "#4D6E9F", new Color?());
    styleBoxFlat2.BorderThickness = new Thickness(1f);
    ((StyleBox) styleBoxFlat2).ContentMarginLeftOverride = new float?(4f);
    ((StyleBox) styleBoxFlat2).ContentMarginRightOverride = new float?(4f);
    ((StyleBox) styleBoxFlat2).ContentMarginTopOverride = new float?(1f);
    ((StyleBox) styleBoxFlat2).ContentMarginBottomOverride = new float?(1f);
    panelContainer2.PanelOverride = (StyleBox) styleBoxFlat2;
    PanelContainer panelContainer3 = panelContainer2;
    PanelContainer panelContainer4 = panelContainer3;
    Label label1 = new Label();
    label1.Text = $"#{task.Slot}";
    ((Control) label1).HorizontalAlignment = (Control.HAlignment) 2;
    label1.FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#CFE1FF", new Color?()));
    ((Control) panelContainer4).AddChild((Control) label1);
    string str2 = PubgBattlePassTaskFormatter.GetTaskDisplayText(task);
    if (string.IsNullOrWhiteSpace(str2))
      str2 = task.NameKey;
    Label label2 = new Label();
    label2.Text = str2;
    ((Control) label2).HorizontalExpand = true;
    label2.ClipText = true;
    label2.FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#E9F2FF", new Color?()));
    Label label3 = label2;
    PanelContainer panelContainer5 = new PanelContainer();
    ((Control) panelContainer5).VerticalAlignment = (Control.VAlignment) 2;
    StyleBoxFlat styleBoxFlat3 = new StyleBoxFlat();
    styleBoxFlat3.BackgroundColor = color3;
    styleBoxFlat3.BorderColor = color4;
    styleBoxFlat3.BorderThickness = new Thickness(1f);
    ((StyleBox) styleBoxFlat3).ContentMarginLeftOverride = new float?(6f);
    ((StyleBox) styleBoxFlat3).ContentMarginRightOverride = new float?(6f);
    ((StyleBox) styleBoxFlat3).ContentMarginTopOverride = new float?(1f);
    ((StyleBox) styleBoxFlat3).ContentMarginBottomOverride = new float?(1f);
    panelContainer5.PanelOverride = (StyleBox) styleBoxFlat3;
    PanelContainer panelContainer6 = panelContainer5;
    ((Control) panelContainer6).AddChild((Control) new Label()
    {
      Text = Loc.GetString(str1),
      FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#EAF4FF", new Color?()))
    });
    ((Control) boxContainer4).AddChild((Control) panelContainer3);
    ((Control) boxContainer4).AddChild((Control) label3);
    ((Control) boxContainer4).AddChild((Control) panelContainer6);
    ((Control) boxContainer2).AddChild((Control) boxContainer4);
    int num = Math.Clamp(task.Progress, 0, Math.Max(1, task.TargetValue));
    BoxContainer boxContainer5 = new BoxContainer();
    boxContainer5.Orientation = (BoxContainer.LayoutOrientation) 0;
    ((Control) boxContainer5).HorizontalExpand = true;
    BoxContainer boxContainer6 = boxContainer5;
    BoxContainer boxContainer7 = boxContainer6;
    Label label4 = new Label();
    label4.Text = Loc.GetString("pubg-bp-task-progress", new (string, object)[2]
    {
      ("current", (object) num),
      ("target", (object) task.TargetValue)
    });
    ((Control) label4).HorizontalExpand = true;
    label4.FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#9FB6D9", new Color?()));
    Label label5 = label4;
    ((Control) boxContainer7).AddChild((Control) label5);
    ((Control) boxContainer6).AddChild((Control) new Label()
    {
      Text = Loc.GetString("pubg-bp-task-xp", new (string, object)[1]
      {
        ("xp", (object) task.XpReward)
      }),
      FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#FFD57E", new Color?()))
    });
    ((Control) boxContainer2).AddChild((Control) boxContainer6);
    ((Control) battlePassTaskRow).AddChild((Control) boxContainer2);
    return (Control) battlePassTaskRow;
  }

  private void UpdateEventsRedDots()
  {
    if (this.Hub == null)
      return;
    PanelContainer eventsRouletteRedDot = this.Hub.EventsRouletteRedDot;
    PubgEventCardInfo pubgEventCardInfo1 = this.ResolveEventCard("discord_link_once", 0);
    int num1 = pubgEventCardInfo1 != null ? (pubgEventCardInfo1.HasClaimable ? 1 : 0) : 0;
    ((Control) eventsRouletteRedDot).Visible = num1 != 0;
    PanelContainer eventsPromoRedDot = this.Hub.EventsPromoRedDot;
    PubgEventCardInfo pubgEventCardInfo2 = this.ResolveEventCard("mars_opening", 1);
    int num2 = pubgEventCardInfo2 != null ? (pubgEventCardInfo2.HasClaimable ? 1 : 0) : 0;
    ((Control) eventsPromoRedDot).Visible = num2 != 0;
    PanelContainer eventsFeatureRedDot = this.Hub.EventsFeatureRedDot;
    PubgEventCardInfo pubgEventCardInfo3 = this.ResolveEventCard("mapper_cookie", 2);
    int num3 = pubgEventCardInfo3 != null ? (pubgEventCardInfo3.HasClaimable ? 1 : 0) : 0;
    ((Control) eventsFeatureRedDot).Visible = num3 != 0;
  }

  private void PreloadEventStates(PubgEventsHubStateMessage hubState, PubgEventsSystem eventsSystem)
  {
    foreach (PubgEventCardInfo pubgEventCardInfo in (IEnumerable<PubgEventCardInfo>) hubState.Events.OrderBy<PubgEventCardInfo, int>((Func<PubgEventCardInfo, int>) (card => card.SortOrder)))
    {
      if (!string.IsNullOrWhiteSpace(pubgEventCardInfo.EventKey) && this._preloadedEventKeys.Add(pubgEventCardInfo.EventKey))
        this.RequestEventStateIfStale(eventsSystem, pubgEventCardInfo.EventKey);
    }
  }

  private void RequestEventStateIfStale(PubgEventsSystem eventsSystem, string eventKey, bool force = false)
  {
    if (string.IsNullOrWhiteSpace(eventKey))
      return;
    DateTime utcNow = DateTime.UtcNow;
    DateTime dateTime1;
    bool flag1 = this._lastEventStateRequestAt.TryGetValue(eventKey, out dateTime1) && utcNow - dateTime1 < TimeSpan.FromSeconds(3L);
    DateTime dateTime2;
    bool flag2 = eventsSystem.LastEventStates.TryGetValue(eventKey, out PubgEventDetailInfo _) && this._lastEventStateResponseAt.TryGetValue(eventKey, out dateTime2) && utcNow - dateTime2 < TimeSpan.FromSeconds(12L);
    if (!force && flag1 | flag2)
      return;
    this._lastEventStateRequestAt[eventKey] = utcNow;
    eventsSystem.RequestEventState(eventKey, force);
  }

  private void UpdateEventCardLabels()
  {
    if (this.Hub == null)
      return;
    PubgEventCardInfo card1 = this.ResolveEventCard("discord_link_once", 0);
    PubgEventCardInfo card2 = this.ResolveEventCard("mars_opening", 1);
    PubgEventCardInfo pubgEventCardInfo = this.ResolveEventCard("mapper_cookie", 2);
    DateTime nowUtc = this.ResolveEventsNowUtc();
    this.Hub.EventsRouletteTitleLabel.Text = PubgPreLobbyHubUIController.ResolveLocalizedCardText(card1?.TitleKey, "pubg-prelobby-hub-events-roulette-title");
    this.Hub.EventsRouletteSubtitleLabel.Text = this.BuildEventCardSubtitle(card1, nowUtc, "pubg-prelobby-hub-events-roulette-subtitle");
    this.Hub.EventsPromoTitleLabel.Text = PubgPreLobbyHubUIController.ResolveLocalizedCardText(card2?.TitleKey, "pubg-prelobby-hub-events-promo-title");
    this.Hub.EventsPromoSubtitleLabel.Text = this.BuildEventCardSubtitle(card2, nowUtc, "pubg-prelobby-hub-events-promo-subtitle");
    this.Hub.EventsFeatureTitleLabel.Text = PubgPreLobbyHubUIController.ResolveLocalizedCardText(pubgEventCardInfo?.TitleKey, "pubg-prelobby-hub-events-feature-title");
    this.Hub.EventsFeatureSubtitleLabel.Text = this.BuildEventCardSubtitle(pubgEventCardInfo, nowUtc, "pubg-prelobby-hub-events-feature-caption");
    this.UpdateFeatureCardSummary(pubgEventCardInfo);
  }

  private void UpdateInteractiveCardStyles()
  {
    if (this.Hub == null)
      return;
    this.Hub.CalendarCardPanel.PanelOverride = this._calendarCardHovered ? (StyleBox) PubgPreLobbyHubUIController.CalendarCardHoverStyle : (StyleBox) PubgPreLobbyHubUIController.CalendarCardStyle;
    this.Hub.EventsRouletteCardPanel.PanelOverride = this._eventsRouletteCardHovered ? (StyleBox) PubgPreLobbyHubUIController.EventsCardHoverStyle : (StyleBox) PubgPreLobbyHubUIController.EventsCardStyle;
    this.Hub.EventsPromoCardPanel.PanelOverride = this._eventsPromoCardHovered ? (StyleBox) PubgPreLobbyHubUIController.EventsCardHoverStyle : (StyleBox) PubgPreLobbyHubUIController.EventsCardStyle;
    this.Hub.EventsFeatureCardPanel.PanelOverride = this._eventsFeatureCardHovered ? (StyleBox) PubgPreLobbyHubUIController.EventsFeatureCardHoverStyle : (StyleBox) PubgPreLobbyHubUIController.EventsFeatureCardStyle;
  }

  private PubgEventCardInfo? ResolveEventCard(string preferredKey, int fallbackIndex)
  {
    if (this._eventsHubState == null || this._eventsHubState.Events.Count == 0)
      return (PubgEventCardInfo) null;
    PubgEventCardInfo pubgEventCardInfo = this._eventsHubState.Events.FirstOrDefault<PubgEventCardInfo>((Func<PubgEventCardInfo, bool>) (card => card.EventKey == preferredKey));
    if (pubgEventCardInfo != null)
      return pubgEventCardInfo;
    List<PubgEventCardInfo> list = this._eventsHubState.Events.OrderBy<PubgEventCardInfo, int>((Func<PubgEventCardInfo, int>) (card => card.SortOrder)).ToList<PubgEventCardInfo>();
    return fallbackIndex < list.Count ? list[fallbackIndex] : list[0];
  }

  private string ResolveEventKeyForCard(string preferredKey, int fallbackIndex)
  {
    return this.ResolveEventCard(preferredKey, fallbackIndex)?.EventKey ?? preferredKey;
  }

  private static string ResolveLocalizedCardText(string? key, string fallbackKey)
  {
    string str;
    return !string.IsNullOrWhiteSpace(key) && IoCManager.Resolve<ILocalizationManager>().TryGetString(key, ref str) ? str : Loc.GetString(fallbackKey);
  }

  private string BuildEventCardSubtitle(
    PubgEventCardInfo? card,
    DateTime nowUtc,
    string fallbackKey)
  {
    if (card == null)
      return Loc.GetString(fallbackKey);
    List<string> source = new List<string>()
    {
      PubgPreLobbyHubUIController.ResolveLocalizedCardText(card.DescriptionKey, fallbackKey)
    };
    source.Add(PubgPreLobbyHubUIController.BuildEventCardTimeText(card, nowUtc));
    return string.Join(" | ", source.Where<string>((Func<string, bool>) (text => !string.IsNullOrWhiteSpace(text))));
  }

  private void UpdateFeatureCardSummary(PubgEventCardInfo? featureCard)
  {
    if (this.Hub == null)
      return;
    PubgEventCardHubSummaryInfo hubSummary = featureCard?.HubSummary;
    if (hubSummary == null)
    {
      ((Control) this.Hub.EventsFeatureSummaryPanel).Visible = false;
      ((Control) this.Hub.EventsFeatureMilestonesRow).RemoveAllChildren();
      this._lastFeatureMilestoneSignature = (string) null;
    }
    else
    {
      int num1 = Math.Max(0, hubSummary.ProgressCurrent);
      int max = Math.Max(1, Math.Max(hubSummary.ProgressTarget, num1));
      int? nextRewardThreshold = hubSummary.NextRewardThreshold;
      int num2 = Math.Max(0, hubSummary.NextRewardIn);
      int num3 = Math.Max(0, hubSummary.DailyCompleted);
      int num4 = Math.Max(0, hubSummary.DailyTotal);
      ((Control) this.Hub.EventsFeatureSummaryPanel).Visible = true;
      this.Hub.EventsFeatureProgressStatLabel.Text = Loc.GetString("pubg-prelobby-hub-events-feature-progress", new (string, object)[2]
      {
        ("current", (object) num1.ToString("N0", (IFormatProvider) CultureInfo.InvariantCulture)),
        ("target", (object) max.ToString("N0", (IFormatProvider) CultureInfo.InvariantCulture))
      });
      Label nextRewardStatLabel = this.Hub.EventsFeatureNextRewardStatLabel;
      string str1;
      if (!nextRewardThreshold.HasValue)
        str1 = Loc.GetString("pubg-prelobby-hub-events-feature-next-complete");
      else if (num2 <= 0)
        str1 = Loc.GetString("pubg-prelobby-hub-events-feature-next-ready");
      else
        str1 = Loc.GetString("pubg-prelobby-hub-events-feature-next-threshold", new (string, object)[2]
        {
          ("threshold", (object) nextRewardThreshold.Value.ToString("N0", (IFormatProvider) CultureInfo.InvariantCulture)),
          ("amount", (object) num2.ToString("N0", (IFormatProvider) CultureInfo.InvariantCulture))
        });
      nextRewardStatLabel.Text = str1;
      Label featureDailyStatLabel = this.Hub.EventsFeatureDailyStatLabel;
      string str2;
      if (num4 <= 0)
        str2 = Loc.GetString("pubg-prelobby-hub-events-feature-daily-empty");
      else
        str2 = Loc.GetString("pubg-prelobby-hub-events-feature-daily", new (string, object)[2]
        {
          ("current", (object) num3),
          ("target", (object) num4)
        });
      featureDailyStatLabel.Text = str2;
      ((Range) this.Hub.EventsFeatureProgressBar).MinValue = 0.0f;
      ((Range) this.Hub.EventsFeatureProgressBar).MaxValue = (float) max;
      ((Range) this.Hub.EventsFeatureProgressBar).Value = (float) Math.Clamp(num1, 0, max);
      string b = PubgPreLobbyHubUIController.BuildFeatureMilestoneSignature(hubSummary.MilestoneThresholds, num1);
      if (string.Equals(this._lastFeatureMilestoneSignature, b, StringComparison.Ordinal))
        return;
      this.RenderFeatureMilestoneBadges(hubSummary.MilestoneThresholds, num1);
      this._lastFeatureMilestoneSignature = b;
    }
  }

  private void RenderFeatureMilestoneBadges(List<int> thresholds, int currentPoints)
  {
    if (this.Hub == null)
      return;
    ((Control) this.Hub.EventsFeatureMilestonesRow).RemoveAllChildren();
    List<int> list = thresholds.Where<int>((Func<int, bool>) (threshold => threshold > 0)).Distinct<int>().OrderBy<int, int>((Func<int, int>) (threshold => threshold)).ToList<int>();
    if (list.Count == 0)
    {
      ((Control) this.Hub.EventsFeatureMilestonesRow).AddChild((Control) new Label()
      {
        Text = Loc.GetString("pubg-prelobby-hub-events-feature-milestones-empty"),
        FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#97AFCD", new Color?()))
      });
    }
    else
    {
      foreach (int num in list)
      {
        bool flag = currentPoints >= num;
        PanelContainer panelContainer1 = new PanelContainer();
        StyleBoxFlat styleBoxFlat = new StyleBoxFlat();
        styleBoxFlat.BackgroundColor = flag ? Color.FromHex((ReadOnlySpan<char>) "#2A3D2F", new Color?()) : Color.FromHex((ReadOnlySpan<char>) "#1A2840", new Color?());
        styleBoxFlat.BorderColor = flag ? Color.FromHex((ReadOnlySpan<char>) "#7CB684", new Color?()) : Color.FromHex((ReadOnlySpan<char>) "#6082B0", new Color?());
        styleBoxFlat.BorderThickness = new Thickness(1f);
        ((StyleBox) styleBoxFlat).ContentMarginLeftOverride = new float?(6f);
        ((StyleBox) styleBoxFlat).ContentMarginRightOverride = new float?(6f);
        ((StyleBox) styleBoxFlat).ContentMarginTopOverride = new float?(2f);
        ((StyleBox) styleBoxFlat).ContentMarginBottomOverride = new float?(2f);
        panelContainer1.PanelOverride = (StyleBox) styleBoxFlat;
        PanelContainer panelContainer2 = panelContainer1;
        ((Control) panelContainer2).AddChild((Control) new Label()
        {
          Text = num.ToString("N0", (IFormatProvider) CultureInfo.InvariantCulture),
          FontColorOverride = new Color?(flag ? Color.FromHex((ReadOnlySpan<char>) "#D7F2DF", new Color?()) : Color.FromHex((ReadOnlySpan<char>) "#C8DCF8", new Color?()))
        });
        ((Control) this.Hub.EventsFeatureMilestonesRow).AddChild((Control) panelContainer2);
      }
    }
  }

  private static string BuildFeatureMilestoneSignature(List<int> thresholds, int currentPoints)
  {
    int[] array = thresholds.Where<int>((Func<int, bool>) (threshold => threshold > 0)).Distinct<int>().OrderBy<int, int>((Func<int, int>) (threshold => threshold)).ToArray<int>();
    return $"{currentPoints}:{string.Join<int>(",", (IEnumerable<int>) array)}";
  }

  private static StyleBoxFlat CreateCardStyle(string backgroundColorHex, string borderColorHex)
  {
    return new StyleBoxFlat()
    {
      BackgroundColor = Color.FromHex((ReadOnlySpan<char>) backgroundColorHex, new Color?()),
      BorderColor = Color.FromHex((ReadOnlySpan<char>) borderColorHex, new Color?()),
      BorderThickness = new Thickness(1f)
    };
  }

  private static string BuildEventCardTimeText(PubgEventCardInfo card, DateTime nowUtc)
  {
    DateTime dateTime = PubgPreLobbyHubUIController.EnsureUtc(card.StartAt);
    if (dateTime > nowUtc)
    {
      TimeSpan timeSpan = dateTime - nowUtc;
      return timeSpan.TotalDays >= 1.0 ? Loc.GetString("pubg-prelobby-hub-event-card-starts-in-days", new (string, object)[2]
      {
        ("days", (object) (int) timeSpan.TotalDays),
        ("hours", (object) timeSpan.Hours)
      }) : Loc.GetString("pubg-prelobby-hub-event-card-starts-in-hours", new (string, object)[2]
      {
        ("hours", (object) Math.Max(1, (int) timeSpan.TotalHours)),
        ("minutes", (object) Math.Max(0, timeSpan.Minutes))
      });
    }
    DateTime? endAt = card.EndAt;
    if (!endAt.HasValue)
      return Loc.GetString("pubg-prelobby-hub-event-card-open");
    endAt = card.EndAt;
    TimeSpan timeSpan1 = PubgPreLobbyHubUIController.EnsureUtc(endAt.Value) - nowUtc;
    if (timeSpan1 <= TimeSpan.Zero)
      return Loc.GetString("pubg-prelobby-hub-event-card-ended");
    return timeSpan1.TotalDays >= 1.0 ? Loc.GetString("pubg-prelobby-hub-event-card-time-left-days", new (string, object)[2]
    {
      ("days", (object) (int) timeSpan1.TotalDays),
      ("hours", (object) timeSpan1.Hours)
    }) : Loc.GetString("pubg-prelobby-hub-event-card-time-left-hours", new (string, object)[2]
    {
      ("hours", (object) Math.Max(1, (int) timeSpan1.TotalHours)),
      ("minutes", (object) Math.Max(0, timeSpan1.Minutes))
    });
  }

  private DateTime ResolveEventsNowUtc()
  {
    if (this._eventsHubState == null)
      return DateTime.UtcNow;
    if (this._lastHubResponseAt == DateTime.MinValue)
      return PubgPreLobbyHubUIController.EnsureUtc(this._eventsHubState.ServerNowUtc);
    TimeSpan timeSpan = DateTime.UtcNow - this._lastHubResponseAt;
    return PubgPreLobbyHubUIController.EnsureUtc(this._eventsHubState.ServerNowUtc) + timeSpan;
  }

  private static DateTime EnsureUtc(DateTime value)
  {
    return value.Kind == DateTimeKind.Utc ? value : DateTime.SpecifyKind(value, DateTimeKind.Utc);
  }

  private static bool IsEventActiveNow(PubgEventDetailInfo state, DateTime nowUtc)
  {
    return state.IsActive && !(PubgPreLobbyHubUIController.EnsureUtc(state.StartAt) > nowUtc) && (!state.EndAt.HasValue || !(PubgPreLobbyHubUIController.EnsureUtc(state.EndAt.Value) <= nowUtc));
  }

  private static (int level, int xpInLevel, int xpRequired) ResolveBattlePassProgress(
    BattlePassStateMessage state,
    int totalXp)
  {
    List<BattlePassLevelInfo> list = state.Levels.OrderBy<BattlePassLevelInfo, int>((Func<BattlePassLevelInfo, int>) (level => level.Level)).ToList<BattlePassLevelInfo>();
    if (list.Count == 0)
      return (Math.Max(1, state.CurrentLevel), 0, 100);
    int num1 = Math.Max(0, totalXp);
    foreach (BattlePassLevelInfo battlePassLevelInfo in list)
    {
      int num2 = Math.Max(1, battlePassLevelInfo.XpRequired);
      if (num1 < num2)
        return (battlePassLevelInfo.Level, num1, num2);
      num1 -= num2;
    }
    List<BattlePassLevelInfo> battlePassLevelInfoList = list;
    BattlePassLevelInfo battlePassLevelInfo1 = battlePassLevelInfoList[battlePassLevelInfoList.Count - 1];
    int num3 = Math.Max(1, battlePassLevelInfo1.XpRequired);
    return (Math.Max(battlePassLevelInfo1.Level, state.CurrentLevel), num3, num3);
  }

  private sealed class PendingEventClaim
  {
    public string EventKey = string.Empty;
    public int CoinsBefore;
    public int ScrapBefore;
    public int PremiumCoinsBefore;
    public PubgEventDetailInfo EventStateBefore;
  }
}
