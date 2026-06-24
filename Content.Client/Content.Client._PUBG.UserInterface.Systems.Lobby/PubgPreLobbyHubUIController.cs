using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
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

namespace Content.Client._PUBG.UserInterface.Systems.Lobby;

public sealed class PubgPreLobbyHubUIController : UIController, IOnStateEntered<PubgPreLobbyHubState>, IOnStateExited<PubgPreLobbyHubState>
{
	private sealed class PendingEventClaim
	{
		public string EventKey = string.Empty;

		public int CoinsBefore;

		public int ScrapBefore;

		public int PremiumCoinsBefore;

		public PubgEventDetailInfo EventStateBefore;
	}

	private const string DiscordLinkEventKey = "discord_link_once";

	private const string MarsOpeningEventKey = "mars_opening";

	private const string MapperCookieEventKey = "mapper_cookie";

	private static readonly TimeSpan EventCardRefreshInterval = TimeSpan.FromSeconds(1L);

	private static readonly StyleBoxFlat CalendarCardStyle = CreateCardStyle("#1A1C20F0", "#4B5320");

	private static readonly StyleBoxFlat CalendarCardHoverStyle = CreateCardStyle("#23252BF2", "#707C30");

	private static readonly StyleBoxFlat EventsCardStyle = CreateCardStyle("#1F2228ED", "#555C4A");

	private static readonly StyleBoxFlat EventsCardHoverStyle = CreateCardStyle("#282C34F2", "#808B6E");

	private static readonly StyleBoxFlat EventsFeatureCardStyle = CreateCardStyle("#141518", "#646D5C");

	private static readonly StyleBoxFlat EventsFeatureCardHoverStyle = CreateCardStyle("#1C1E23", "#92A086");

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

	private PendingEventClaim? _pendingEventClaim;

	private string? _lastFeatureMilestoneSignature;

	private PubgPreLobbyHubGui? Hub => _state?.Hub;

	public void OnStateEntered(PubgPreLobbyHubState state)
	{
		_state = state;
		_pendingEventClaim = null;
		_lastFeatureMilestoneSignature = null;
		EnsureSystemSubscribed();
		SubscribeButtons();
		MainMenuSystem mainMenuSystem = base.EntityManager.System<MainMenuSystem>();
		bool flag = mainMenuSystem.TryGetCachedPlayerLevel(out _playerLevel);
		if (!mainMenuSystem.TryGetCachedBalance(out _coins, out _scrap, out _premiumCoins) || !flag)
		{
			mainMenuSystem.RequestSkinState(force: true);
		}
		BattlePassSystem battlePassSystem = base.EntityManager.System<BattlePassSystem>();
		_battlePassState = battlePassSystem.LastState;
		battlePassSystem.RequestBattlePass();
		PubgCalendarSystem pubgCalendarSystem = base.EntityManager.System<PubgCalendarSystem>();
		_calendarState = pubgCalendarSystem.LastState;
		pubgCalendarSystem.RequestCalendarState(_calendarState == null);
		PubgEventsSystem pubgEventsSystem = base.EntityManager.System<PubgEventsSystem>();
		_eventsHubState = pubgEventsSystem.LastHubState;
		if (_eventsHubState != null)
		{
			_lastHubResponseAt = DateTime.UtcNow;
		}
		_preloadedEventKeys.Clear();
		if (_eventsHubState != null)
		{
			PreloadEventStates(_eventsHubState, pubgEventsSystem);
		}
		RequestHubIfStale(pubgEventsSystem, _eventsHubState == null);
		RequestEventStateIfStale(pubgEventsSystem, "discord_link_once", !pubgEventsSystem.LastEventStates.ContainsKey("discord_link_once"));
		RequestEventStateIfStale(pubgEventsSystem, "mars_opening", !pubgEventsSystem.LastEventStates.ContainsKey("mars_opening"));
		RequestEventStateIfStale(pubgEventsSystem, "mapper_cookie", !pubgEventsSystem.LastEventStates.ContainsKey("mapper_cookie"));
		UpdateBalanceLabels();
		UpdateProfileLabels();
		UpdateBattlePassLabels();
		UpdateEventCardLabels();
		UpdateEventsRedDots();
		UpdateInteractiveCardStyles();
	}

	public void OnStateExited(PubgPreLobbyHubState state)
	{
		UnsubscribeButtons();
		CloseCalendarWindow();
		CloseEventsWindow();
		CloseInventoryWindow();
		UnsubscribeSystem();
		_preloadedEventKeys.Clear();
		_lastEventStateRequestAt.Clear();
		_lastEventStateResponseAt.Clear();
		_pendingPreferredEventKey = null;
		_pendingEventClaim = null;
		_lastFeatureMilestoneSignature = null;
		_calendarCardHovered = false;
		_eventsRouletteCardHovered = false;
		_eventsPromoCardHovered = false;
		_eventsFeatureCardHovered = false;
		_state = null;
	}

	public override void FrameUpdate(FrameEventArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((UIController)this).FrameUpdate(args);
		if (_state != null && _eventsHubState != null)
		{
			DateTime utcNow = DateTime.UtcNow;
			if (!(utcNow - _lastEventCardRefreshAt < EventCardRefreshInterval))
			{
				_lastEventCardRefreshAt = utcNow;
				UpdateEventCardLabels();
			}
		}
	}

	private void EnsureSystemSubscribed()
	{
		if (!_systemSubscribed)
		{
			MainMenuSystem mainMenuSystem = base.EntityManager.System<MainMenuSystem>();
			mainMenuSystem.OnSkinStateReceived += OnSkinState;
			mainMenuSystem.OnBalanceUpdateReceived += OnBalanceUpdate;
			base.EntityManager.System<BattlePassSystem>().OnStateReceived += OnBattlePassState;
			PubgCalendarSystem pubgCalendarSystem = base.EntityManager.System<PubgCalendarSystem>();
			pubgCalendarSystem.OnStateReceived += OnCalendarState;
			pubgCalendarSystem.OnClaimResultReceived += OnCalendarClaimResult;
			PubgEventsSystem pubgEventsSystem = base.EntityManager.System<PubgEventsSystem>();
			pubgEventsSystem.OnHubStateReceived += OnEventsHubState;
			pubgEventsSystem.OnEventStateReceived += OnEventState;
			pubgEventsSystem.OnClaimResultReceived += OnEventClaimResult;
			base.EntityManager.System<PubgEventInventorySystem>().OnInventoryStateReceived += OnInventoryState;
			_systemSubscribed = true;
		}
	}

	private void UnsubscribeSystem()
	{
		if (_systemSubscribed)
		{
			MainMenuSystem mainMenuSystem = base.EntityManager.SystemOrNull<MainMenuSystem>();
			if (mainMenuSystem != null)
			{
				mainMenuSystem.OnSkinStateReceived -= OnSkinState;
				mainMenuSystem.OnBalanceUpdateReceived -= OnBalanceUpdate;
			}
			BattlePassSystem battlePassSystem = base.EntityManager.SystemOrNull<BattlePassSystem>();
			if (battlePassSystem != null)
			{
				battlePassSystem.OnStateReceived -= OnBattlePassState;
			}
			PubgCalendarSystem pubgCalendarSystem = base.EntityManager.SystemOrNull<PubgCalendarSystem>();
			if (pubgCalendarSystem != null)
			{
				pubgCalendarSystem.OnStateReceived -= OnCalendarState;
				pubgCalendarSystem.OnClaimResultReceived -= OnCalendarClaimResult;
			}
			PubgEventsSystem pubgEventsSystem = base.EntityManager.SystemOrNull<PubgEventsSystem>();
			if (pubgEventsSystem != null)
			{
				pubgEventsSystem.OnHubStateReceived -= OnEventsHubState;
				pubgEventsSystem.OnEventStateReceived -= OnEventState;
				pubgEventsSystem.OnClaimResultReceived -= OnEventClaimResult;
			}
			PubgEventInventorySystem pubgEventInventorySystem = base.EntityManager.SystemOrNull<PubgEventInventorySystem>();
			if (pubgEventInventorySystem != null)
			{
				pubgEventInventorySystem.OnInventoryStateReceived -= OnInventoryState;
			}
			_systemSubscribed = false;
		}
	}

	private void SubscribeButtons()
	{
		if (Hub != null)
		{
			((BaseButton)Hub.PlayButton).OnPressed -= OnPlayPressed;
			((BaseButton)Hub.PlayButton).OnPressed += OnPlayPressed;
			((BaseButton)Hub.DiscordButton).OnPressed -= OnDiscordPressed;
			((BaseButton)Hub.DiscordButton).OnPressed += OnDiscordPressed;
			((BaseButton)Hub.SettingsButton).OnPressed -= OnSettingsPressed;
			((BaseButton)Hub.SettingsButton).OnPressed += OnSettingsPressed;
			((BaseButton)Hub.CalendarOpenButton).OnPressed -= OnCalendarOpenPressed;
			((BaseButton)Hub.CalendarOpenButton).OnPressed += OnCalendarOpenPressed;
			((Control)Hub.CalendarOpenButton).OnMouseEntered -= OnCalendarCardMouseEntered;
			((Control)Hub.CalendarOpenButton).OnMouseEntered += OnCalendarCardMouseEntered;
			((Control)Hub.CalendarOpenButton).OnMouseExited -= OnCalendarCardMouseExited;
			((Control)Hub.CalendarOpenButton).OnMouseExited += OnCalendarCardMouseExited;
			((BaseButton)Hub.InventoryButton).OnPressed -= OnInventoryPressed;
			((BaseButton)Hub.InventoryButton).OnPressed += OnInventoryPressed;
			((BaseButton)Hub.SponsorsButton).OnPressed -= OnSponsorsPressed;
			((BaseButton)Hub.SponsorsButton).OnPressed += OnSponsorsPressed;
			((BaseButton)Hub.EventsRouletteButton).OnPressed -= OnEventsRoulettePressed;
			((BaseButton)Hub.EventsRouletteButton).OnPressed += OnEventsRoulettePressed;
			((Control)Hub.EventsRouletteButton).OnMouseEntered -= OnEventsRouletteMouseEntered;
			((Control)Hub.EventsRouletteButton).OnMouseEntered += OnEventsRouletteMouseEntered;
			((Control)Hub.EventsRouletteButton).OnMouseExited -= OnEventsRouletteMouseExited;
			((Control)Hub.EventsRouletteButton).OnMouseExited += OnEventsRouletteMouseExited;
			((BaseButton)Hub.EventsPromoButton).OnPressed -= OnEventsPromoPressed;
			((BaseButton)Hub.EventsPromoButton).OnPressed += OnEventsPromoPressed;
			((Control)Hub.EventsPromoButton).OnMouseEntered -= OnEventsPromoMouseEntered;
			((Control)Hub.EventsPromoButton).OnMouseEntered += OnEventsPromoMouseEntered;
			((Control)Hub.EventsPromoButton).OnMouseExited -= OnEventsPromoMouseExited;
			((Control)Hub.EventsPromoButton).OnMouseExited += OnEventsPromoMouseExited;
			((BaseButton)Hub.EventsFeatureButton).OnPressed -= OnEventsFeaturePressed;
			((BaseButton)Hub.EventsFeatureButton).OnPressed += OnEventsFeaturePressed;
			((Control)Hub.EventsFeatureButton).OnMouseEntered -= OnEventsFeatureMouseEntered;
			((Control)Hub.EventsFeatureButton).OnMouseEntered += OnEventsFeatureMouseEntered;
			((Control)Hub.EventsFeatureButton).OnMouseExited -= OnEventsFeatureMouseExited;
			((Control)Hub.EventsFeatureButton).OnMouseExited += OnEventsFeatureMouseExited;
		}
	}

	private void UnsubscribeButtons()
	{
		if (Hub != null)
		{
			((BaseButton)Hub.PlayButton).OnPressed -= OnPlayPressed;
			((BaseButton)Hub.DiscordButton).OnPressed -= OnDiscordPressed;
			((BaseButton)Hub.SettingsButton).OnPressed -= OnSettingsPressed;
			((BaseButton)Hub.CalendarOpenButton).OnPressed -= OnCalendarOpenPressed;
			((Control)Hub.CalendarOpenButton).OnMouseEntered -= OnCalendarCardMouseEntered;
			((Control)Hub.CalendarOpenButton).OnMouseExited -= OnCalendarCardMouseExited;
			((BaseButton)Hub.InventoryButton).OnPressed -= OnInventoryPressed;
			((BaseButton)Hub.SponsorsButton).OnPressed -= OnSponsorsPressed;
			((BaseButton)Hub.EventsRouletteButton).OnPressed -= OnEventsRoulettePressed;
			((Control)Hub.EventsRouletteButton).OnMouseEntered -= OnEventsRouletteMouseEntered;
			((Control)Hub.EventsRouletteButton).OnMouseExited -= OnEventsRouletteMouseExited;
			((BaseButton)Hub.EventsPromoButton).OnPressed -= OnEventsPromoPressed;
			((Control)Hub.EventsPromoButton).OnMouseEntered -= OnEventsPromoMouseEntered;
			((Control)Hub.EventsPromoButton).OnMouseExited -= OnEventsPromoMouseExited;
			((BaseButton)Hub.EventsFeatureButton).OnPressed -= OnEventsFeaturePressed;
			((Control)Hub.EventsFeatureButton).OnMouseEntered -= OnEventsFeatureMouseEntered;
			((Control)Hub.EventsFeatureButton).OnMouseExited -= OnEventsFeatureMouseExited;
		}
	}

	private void OnPlayPressed(ButtonEventArgs args)
	{
		_stateManager.RequestStateChange<LobbyState>();
	}

	private void OnDiscordPressed(ButtonEventArgs args)
	{
		_uriOpener.OpenUri("https://discord.gg/xdQ4vSKRB8");
	}

	private void OnSettingsPressed(ButtonEventArgs args)
	{
		base.UIManager.GetUIController<OptionsUIController>().ToggleWindow();
	}

	private void OnCalendarOpenPressed(ButtonEventArgs args)
	{
		EnsureCalendarWindow();
		PubgPreLobbyCalendarWindow? calendarWindow = _calendarWindow;
		if (calendarWindow != null)
		{
			((BaseWindow)calendarWindow).OpenCentered();
		}
		if (_calendarState != null)
		{
			_calendarWindow?.UpdateState(_calendarState);
		}
		base.EntityManager.System<PubgCalendarSystem>().RequestCalendarState();
	}

	private void OnEventsRoulettePressed(ButtonEventArgs args)
	{
		OpenEventsWindow(ResolveEventKeyForCard("discord_link_once", 0));
	}

	private void OnEventsPromoPressed(ButtonEventArgs args)
	{
		OpenEventsWindow(ResolveEventKeyForCard("mars_opening", 1));
	}

	private void OnEventsFeaturePressed(ButtonEventArgs args)
	{
		OpenEventsWindow(ResolveEventKeyForCard("mapper_cookie", 2));
	}

	private void OnCalendarCardMouseEntered(GUIMouseHoverEventArgs args)
	{
		_calendarCardHovered = true;
		UpdateInteractiveCardStyles();
	}

	private void OnCalendarCardMouseExited(GUIMouseHoverEventArgs args)
	{
		_calendarCardHovered = false;
		UpdateInteractiveCardStyles();
	}

	private void OnEventsRouletteMouseEntered(GUIMouseHoverEventArgs args)
	{
		_eventsRouletteCardHovered = true;
		UpdateInteractiveCardStyles();
	}

	private void OnEventsRouletteMouseExited(GUIMouseHoverEventArgs args)
	{
		_eventsRouletteCardHovered = false;
		UpdateInteractiveCardStyles();
	}

	private void OnEventsPromoMouseEntered(GUIMouseHoverEventArgs args)
	{
		_eventsPromoCardHovered = true;
		UpdateInteractiveCardStyles();
	}

	private void OnEventsPromoMouseExited(GUIMouseHoverEventArgs args)
	{
		_eventsPromoCardHovered = false;
		UpdateInteractiveCardStyles();
	}

	private void OnEventsFeatureMouseEntered(GUIMouseHoverEventArgs args)
	{
		_eventsFeatureCardHovered = true;
		UpdateInteractiveCardStyles();
	}

	private void OnEventsFeatureMouseExited(GUIMouseHoverEventArgs args)
	{
		_eventsFeatureCardHovered = false;
		UpdateInteractiveCardStyles();
	}

	private void OpenEventsWindow(string? preferredEventKey)
	{
		EnsureEventsWindow();
		PubgPreLobbyEventsWindow? eventsWindow = _eventsWindow;
		if (eventsWindow != null)
		{
			((BaseWindow)eventsWindow).OpenCentered();
		}
		_eventsWindow?.SetError(string.Empty);
		_pendingPreferredEventKey = (string.IsNullOrWhiteSpace(preferredEventKey) ? _lastSelectedEventKey : preferredEventKey);
		PubgEventsSystem eventsSystem = base.EntityManager.System<PubgEventsSystem>();
		if (_eventsHubState != null)
		{
			_eventsWindow?.UpdateHub(_eventsHubState, _pendingPreferredEventKey ?? _eventsWindow?.SelectedEventKey);
		}
		if (_eventsHubState == null && _pendingPreferredEventKey != null)
		{
			RequestEventStateIfStale(eventsSystem, _pendingPreferredEventKey, force: true);
		}
		RequestHubIfStale(eventsSystem, _eventsHubState == null);
	}

	private void OnInventoryPressed(ButtonEventArgs args)
	{
		EnsureInventoryWindow();
		PubgPreLobbyInventoryWindow? inventoryWindow = _inventoryWindow;
		if (inventoryWindow != null)
		{
			((BaseWindow)inventoryWindow).OpenCentered();
		}
		PubgEventInventorySystem pubgEventInventorySystem = base.EntityManager.System<PubgEventInventorySystem>();
		if (pubgEventInventorySystem.LastInventoryState != null)
		{
			_inventoryWindow?.UpdateInventory(pubgEventInventorySystem.LastInventoryState);
		}
		pubgEventInventorySystem.RequestInventory();
	}

	private void OnSponsorsPressed(ButtonEventArgs args)
	{
		base.UIManager.GetUIController<MainMenuUIController>().OpenSponsorDisplayWindow();
	}

	private void OnCalendarClaimRequested()
	{
		base.EntityManager.System<PubgCalendarSystem>().ClaimNextDay();
	}

	private void OnEventsWindowSelected(string eventKey)
	{
		_lastSelectedEventKey = eventKey;
		PubgEventsSystem pubgEventsSystem = base.EntityManager.System<PubgEventsSystem>();
		if (pubgEventsSystem.LastEventStates.TryGetValue(eventKey, out PubgEventDetailInfo value))
		{
			_eventsWindow?.UpdateEventState(value);
		}
		else
		{
			_eventsWindow?.ShowLoadingState(eventKey);
		}
		RequestEventStateIfStale(pubgEventsSystem, eventKey);
	}

	private void OnEventsWindowClaimRequested(string eventKey, string claimType, string claimId)
	{
		if (!string.IsNullOrWhiteSpace(eventKey))
		{
			PubgEventsSystem pubgEventsSystem = base.EntityManager.System<PubgEventsSystem>();
			if (pubgEventsSystem.LastEventStates.TryGetValue(eventKey, out PubgEventDetailInfo value) && !IsEventActiveNow(value, DateTime.UtcNow))
			{
				_eventsWindow?.SetError(Loc.GetString("pubg-events-error-event-not-active"));
			}
			else if (pubgEventsSystem.TryClaim(eventKey, claimType, claimId))
			{
				_eventsWindow?.SetError(string.Empty);
				ApplyOptimisticClaim(eventKey, claimType, claimId, pubgEventsSystem);
			}
		}
	}

	private void OnEventsWindowRefreshRequested(string eventKey)
	{
		if (!string.IsNullOrWhiteSpace(eventKey))
		{
			PubgEventsSystem eventsSystem = base.EntityManager.System<PubgEventsSystem>();
			RequestHubIfStale(eventsSystem, force: true);
			RequestEventStateIfStale(eventsSystem, eventKey, force: true);
		}
	}

	private void OnInventoryWindowRefreshRequested()
	{
		base.EntityManager.System<PubgEventInventorySystem>().RequestInventory();
	}

	private void EnsureCalendarWindow()
	{
		if (_calendarWindow == null)
		{
			_calendarWindow = base.UIManager.CreateWindow<PubgPreLobbyCalendarWindow>();
			((BaseWindow)_calendarWindow).OnClose += OnCalendarWindowClosed;
			_calendarWindow.ClaimRequested += OnCalendarClaimRequested;
		}
	}

	private void EnsureEventsWindow()
	{
		if (_eventsWindow == null)
		{
			_eventsWindow = base.UIManager.CreateWindow<PubgPreLobbyEventsWindow>();
			((BaseWindow)_eventsWindow).OnClose += OnEventsWindowClosed;
			_eventsWindow.EventSelected += OnEventsWindowSelected;
			_eventsWindow.ClaimRequested += OnEventsWindowClaimRequested;
			_eventsWindow.RefreshRequested += OnEventsWindowRefreshRequested;
		}
	}

	private void EnsureInventoryWindow()
	{
		if (_inventoryWindow == null)
		{
			_inventoryWindow = base.UIManager.CreateWindow<PubgPreLobbyInventoryWindow>();
			((BaseWindow)_inventoryWindow).OnClose += OnInventoryWindowClosed;
			_inventoryWindow.RefreshRequested += OnInventoryWindowRefreshRequested;
		}
	}

	private void OnCalendarWindowClosed()
	{
		CloseCalendarWindow();
	}

	private void OnEventsWindowClosed()
	{
		CloseEventsWindow();
	}

	private void OnInventoryWindowClosed()
	{
		CloseInventoryWindow();
	}

	private void CloseCalendarWindow()
	{
		if (_calendarWindow != null)
		{
			((BaseWindow)_calendarWindow).OnClose -= OnCalendarWindowClosed;
			_calendarWindow.ClaimRequested -= OnCalendarClaimRequested;
			((BaseWindow)_calendarWindow).Close();
			_calendarWindow = null;
		}
	}

	private void CloseEventsWindow()
	{
		if (_eventsWindow != null)
		{
			((BaseWindow)_eventsWindow).OnClose -= OnEventsWindowClosed;
			_eventsWindow.EventSelected -= OnEventsWindowSelected;
			_eventsWindow.ClaimRequested -= OnEventsWindowClaimRequested;
			_eventsWindow.RefreshRequested -= OnEventsWindowRefreshRequested;
			((BaseWindow)_eventsWindow).Close();
			_eventsWindow = null;
		}
	}

	private void CloseInventoryWindow()
	{
		if (_inventoryWindow != null)
		{
			((BaseWindow)_inventoryWindow).OnClose -= OnInventoryWindowClosed;
			_inventoryWindow.RefreshRequested -= OnInventoryWindowRefreshRequested;
			((BaseWindow)_inventoryWindow).Close();
			_inventoryWindow = null;
		}
	}

	private void OnSkinState(SkinStateMessage msg)
	{
		_coins = msg.PlayerCoins;
		_scrap = msg.PlayerScrap;
		_premiumCoins = msg.PlayerPremiumCoins;
		_playerLevel = msg.PlayerLevel;
		_sponsorDisplayTier = msg.SponsorDisplayTier;
		_sponsorDisplayMode = msg.SponsorDisplayMode;
		UpdateBalanceLabels();
		UpdateProfileLabels();
	}

	private void OnBalanceUpdate(BalanceUpdateMessage msg)
	{
		_coins = msg.Coins;
		_scrap = msg.Scrap;
		_premiumCoins = msg.PremiumCoins;
		UpdateBalanceLabels();
	}

	private void OnBattlePassState(BattlePassStateMessage msg)
	{
		_battlePassState = msg;
		UpdateBattlePassLabels();
	}

	private void OnCalendarState(PubgCalendarStateMessage msg)
	{
		_calendarState = msg;
		_calendarWindow?.UpdateState(msg);
	}

	private void OnCalendarClaimResult(PubgCalendarClaimResultMessage msg)
	{
		if (_calendarWindow != null)
		{
			if (msg.Success)
			{
				_calendarWindow.SetError(string.Empty);
			}
			else
			{
				_calendarWindow.SetError(ResolveCalendarErrorText(msg.Error));
			}
		}
	}

	private void OnEventsHubState(PubgEventsHubStateMessage msg)
	{
		_eventsHubState = msg;
		_lastHubResponseAt = DateTime.UtcNow;
		if (string.IsNullOrWhiteSpace(_lastSelectedEventKey) && msg.Events.Count > 0)
		{
			_lastSelectedEventKey = msg.Events.OrderBy((PubgEventCardInfo card) => card.SortOrder).First().EventKey;
		}
		PubgEventsSystem eventsSystem = base.EntityManager.System<PubgEventsSystem>();
		PreloadEventStates(msg, eventsSystem);
		UpdateEventCardLabels();
		UpdateEventsRedDots();
		_eventsWindow?.UpdateHub(msg, _pendingPreferredEventKey ?? _eventsWindow?.SelectedEventKey);
		_pendingPreferredEventKey = null;
	}

	private void RequestHubIfStale(PubgEventsSystem eventsSystem, bool force = false)
	{
		DateTime utcNow = DateTime.UtcNow;
		bool flag = utcNow - _lastHubRequestAt < TimeSpan.FromSeconds(3L);
		bool flag2 = _eventsHubState != null && utcNow - _lastHubResponseAt < TimeSpan.FromSeconds(8L);
		if (force || !(flag || flag2))
		{
			_lastHubRequestAt = utcNow;
			eventsSystem.RequestHub(force);
		}
	}

	private void OnEventState(PubgEventStateMessage msg)
	{
		_lastEventStateResponseAt[msg.State.EventKey] = DateTime.UtcNow;
		UpdateHubCardFromEventState(msg.State);
		_eventsWindow?.UpdateEventState(msg.State);
		UpdateEventCardLabels();
		UpdateEventsRedDots();
	}

	private void OnEventClaimResult(PubgEventClaimResultMessage msg)
	{
		PubgEventsSystem eventsSystem = base.EntityManager.System<PubgEventsSystem>();
		if (msg.HasBalances)
		{
			_coins = msg.Coins;
			_scrap = msg.Scrap;
			_premiumCoins = msg.PremiumCoins;
			UpdateBalanceLabels();
		}
		if (msg.Success)
		{
			ApplyClaimResultToLocalState(msg, eventsSystem);
			_pendingEventClaim = null;
			_eventsWindow?.SetError(string.Empty);
			return;
		}
		string text = _pendingEventClaim?.EventKey ?? _lastSelectedEventKey ?? _eventsWindow?.SelectedEventKey;
		RollbackOptimisticClaim(eventsSystem);
		_pendingEventClaim = null;
		_eventsWindow?.SetError(ResolveEventErrorText(msg.Error));
		RequestHubIfStale(eventsSystem, force: true);
		if (!string.IsNullOrWhiteSpace(text))
		{
			RequestEventStateIfStale(eventsSystem, text, force: true);
		}
	}

	private void ApplyOptimisticClaim(string eventKey, string claimType, string claimId, PubgEventsSystem eventsSystem)
	{
		if (eventsSystem.LastEventStates.TryGetValue(eventKey, out PubgEventDetailInfo value))
		{
			int coins = _coins;
			int scrap = _scrap;
			int premiumCoins = _premiumCoins;
			PubgEventDetailInfo eventStateBefore = CloneEventState(value);
			if (TryApplyClaimToState(value, claimType, claimId))
			{
				_pendingEventClaim = new PendingEventClaim
				{
					EventKey = eventKey,
					CoinsBefore = coins,
					ScrapBefore = scrap,
					PremiumCoinsBefore = premiumCoins,
					EventStateBefore = eventStateBefore
				};
				UpdateHubCardFromEventState(value);
				_eventsWindow?.UpdateEventState(value);
				UpdateEventCardLabels();
				UpdateEventsRedDots();
				UpdateBalanceLabels();
			}
		}
	}

	private void ApplyClaimResultToLocalState(PubgEventClaimResultMessage msg, PubgEventsSystem eventsSystem)
	{
		string text = _pendingEventClaim?.EventKey ?? _lastSelectedEventKey ?? _eventsWindow?.SelectedEventKey;
		if (string.IsNullOrWhiteSpace(text) || !eventsSystem.LastEventStates.TryGetValue(text, out PubgEventDetailInfo value))
		{
			return;
		}
		if (msg.ClaimResult != null)
		{
			TryApplyClaimToState(value, msg.ClaimResult.ClaimType, msg.ClaimResult.ClaimId, !msg.HasBalances);
		}
		if (value.MarsState != null)
		{
			foreach (PubgEventWalletDeltaInfo walletsDeltum in msg.WalletsDelta)
			{
				if (string.Equals(walletsDeltum.WalletKey, value.MarsState.Wallet.WalletKey, StringComparison.Ordinal))
				{
					value.MarsState.Wallet.Balance = walletsDeltum.Balance;
					value.MarsState.Wallet.TotalEarned = walletsDeltum.TotalEarned;
				}
			}
		}
		RefreshClaimableFlags(value);
		UpdateHubCardFromEventState(value);
		_eventsWindow?.UpdateEventState(value);
		UpdateEventCardLabels();
		UpdateEventsRedDots();
	}

	private void RollbackOptimisticClaim(PubgEventsSystem eventsSystem)
	{
		if (_pendingEventClaim != null)
		{
			_coins = _pendingEventClaim.CoinsBefore;
			_scrap = _pendingEventClaim.ScrapBefore;
			_premiumCoins = _pendingEventClaim.PremiumCoinsBefore;
			UpdateBalanceLabels();
			eventsSystem.LastEventStates[_pendingEventClaim.EventKey] = CloneEventState(_pendingEventClaim.EventStateBefore);
			PubgEventDetailInfo state = eventsSystem.LastEventStates[_pendingEventClaim.EventKey];
			UpdateHubCardFromEventState(state);
			_eventsWindow?.UpdateEventState(state);
			UpdateEventCardLabels();
			UpdateEventsRedDots();
		}
	}

	private bool TryApplyClaimToState(PubgEventDetailInfo state, string claimType, string claimId, bool applyBalanceReward = true)
	{
		if (string.IsNullOrWhiteSpace(claimType) || string.IsNullOrWhiteSpace(claimId))
		{
			return false;
		}
		bool flag = false;
		if (string.Equals(claimType, "one_time", StringComparison.OrdinalIgnoreCase))
		{
			PubgDiscordEventStateInfo discordState = state.DiscordState;
			if (discordState == null || !string.Equals(discordState.ClaimKey, claimId, StringComparison.Ordinal))
			{
				return false;
			}
			if (discordState.AlreadyClaimed)
			{
				return false;
			}
			discordState.AlreadyClaimed = true;
			discordState.CanClaim = false;
			if (applyBalanceReward)
			{
				ApplyBalanceReward(discordState.RewardType, discordState.RewardValue);
			}
			flag = true;
		}
		else if (string.Equals(claimType, "task", StringComparison.OrdinalIgnoreCase))
		{
			PubgMarsEventStateInfo marsState = state.MarsState;
			if (marsState == null)
			{
				return false;
			}
			PubgMarsTaskInfo pubgMarsTaskInfo = marsState.LoginTasks.FirstOrDefault((PubgMarsTaskInfo item) => string.Equals(item.TaskKey, claimId, StringComparison.Ordinal));
			if (pubgMarsTaskInfo == null)
			{
				pubgMarsTaskInfo = marsState.ChallengeTasks.FirstOrDefault((PubgMarsTaskInfo item) => string.Equals(item.TaskKey, claimId, StringComparison.Ordinal));
			}
			if (pubgMarsTaskInfo == null || pubgMarsTaskInfo.IsClaimed || !pubgMarsTaskInfo.IsClaimable)
			{
				return false;
			}
			pubgMarsTaskInfo.IsClaimed = true;
			pubgMarsTaskInfo.IsClaimable = false;
			marsState.Points += pubgMarsTaskInfo.TokenReward;
			marsState.Wallet.Balance += pubgMarsTaskInfo.TokenReward;
			marsState.Wallet.TotalEarned += pubgMarsTaskInfo.TokenReward;
			flag = true;
		}
		else if (string.Equals(claimType, "milestone", StringComparison.OrdinalIgnoreCase))
		{
			PubgMarsEventStateInfo marsState2 = state.MarsState;
			if (marsState2 == null)
			{
				return false;
			}
			PubgMarsMilestoneInfo pubgMarsMilestoneInfo = marsState2.Milestones.FirstOrDefault((PubgMarsMilestoneInfo item) => string.Equals(item.MilestoneId, claimId, StringComparison.Ordinal));
			if (pubgMarsMilestoneInfo == null || pubgMarsMilestoneInfo.IsClaimed || !pubgMarsMilestoneInfo.IsClaimable)
			{
				return false;
			}
			pubgMarsMilestoneInfo.IsClaimed = true;
			pubgMarsMilestoneInfo.IsClaimable = false;
			if (applyBalanceReward)
			{
				ApplyBalanceReward(pubgMarsMilestoneInfo.RewardType, pubgMarsMilestoneInfo.RewardValue);
			}
			flag = true;
		}
		if (!flag)
		{
			return false;
		}
		RefreshClaimableFlags(state);
		return true;
	}

	private void RefreshClaimableFlags(PubgEventDetailInfo state)
	{
		state.RedDotOneTime = state.DiscordState?.CanClaim ?? false;
		if (state.MarsState == null)
		{
			state.RedDotTasks = false;
			state.RedDotMilestones = false;
		}
		else
		{
			state.RedDotTasks = state.MarsState.LoginTasks.Any((PubgMarsTaskInfo task) => task.IsClaimable && !task.IsClaimed) || state.MarsState.ChallengeTasks.Any((PubgMarsTaskInfo task) => task.IsClaimable && !task.IsClaimed);
			state.RedDotMilestones = state.MarsState.Milestones.Any((PubgMarsMilestoneInfo milestone) => milestone.IsClaimable && !milestone.IsClaimed);
		}
		state.HasClaimable = state.RedDotOneTime || state.RedDotTasks || state.RedDotMilestones;
	}

	private void ApplyBalanceReward(string rewardType, string rewardValue)
	{
		if (int.TryParse(rewardValue, out var result) && result > 0)
		{
			if (string.Equals(rewardType, "coins", StringComparison.OrdinalIgnoreCase))
			{
				_coins += result;
			}
			else if (string.Equals(rewardType, "scrap", StringComparison.OrdinalIgnoreCase))
			{
				_scrap += result;
			}
			else if (string.Equals(rewardType, "premiumCoins", StringComparison.OrdinalIgnoreCase))
			{
				_premiumCoins += result;
			}
		}
	}

	private void UpdateHubCardFromEventState(PubgEventDetailInfo state)
	{
		if (_eventsHubState == null)
		{
			return;
		}
		PubgEventCardInfo pubgEventCardInfo = _eventsHubState.Events.FirstOrDefault((PubgEventCardInfo item) => string.Equals(item.EventKey, state.EventKey, StringComparison.Ordinal));
		if (pubgEventCardInfo != null)
		{
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
			pubgEventCardInfo.HubSummary = BuildHubSummaryFromEventState(state);
			if (!string.IsNullOrWhiteSpace(state.SelectorIconPath))
			{
				pubgEventCardInfo.SelectorIconPath = state.SelectorIconPath;
			}
			if (!string.IsNullOrWhiteSpace(state.SelectorBannerPath))
			{
				pubgEventCardInfo.SelectorBannerPath = state.SelectorBannerPath;
			}
			if (!string.IsNullOrWhiteSpace(state.SelectorAccentHex))
			{
				pubgEventCardInfo.SelectorAccentHex = state.SelectorAccentHex;
			}
		}
	}

	private static PubgEventDetailInfo CloneEventState(PubgEventDetailInfo source)
	{
		return new PubgEventDetailInfo
		{
			EventKey = source.EventKey,
			Kind = source.Kind,
			TitleKey = source.TitleKey,
			DescriptionKey = source.DescriptionKey,
			SelectorIconPath = source.SelectorIconPath,
			SelectorBannerPath = source.SelectorBannerPath,
			SelectorAccentHex = source.SelectorAccentHex,
			StartAt = source.StartAt,
			EndAt = source.EndAt,
			SortOrder = source.SortOrder,
			IsActive = source.IsActive,
			HasClaimable = source.HasClaimable,
			RedDotOneTime = source.RedDotOneTime,
			RedDotTasks = source.RedDotTasks,
			RedDotMilestones = source.RedDotMilestones,
			DiscordState = ((source.DiscordState == null) ? null : new PubgDiscordEventStateInfo
			{
				ClaimKey = source.DiscordState.ClaimKey,
				RewardType = source.DiscordState.RewardType,
				RewardValue = source.DiscordState.RewardValue,
				Linked = source.DiscordState.Linked,
				AlreadyClaimed = source.DiscordState.AlreadyClaimed,
				ClaimedAt = source.DiscordState.ClaimedAt,
				CanClaim = source.DiscordState.CanClaim
			}),
			MarsState = ((source.MarsState == null) ? null : new PubgMarsEventStateInfo
			{
				Points = source.MarsState.Points,
				BalanceLabelKey = source.MarsState.BalanceLabelKey,
				TaskRewardLabelKey = source.MarsState.TaskRewardLabelKey,
				Wallet = new PubgEventWalletInfo
				{
					WalletKey = source.MarsState.Wallet.WalletKey,
					Balance = source.MarsState.Wallet.Balance,
					TotalEarned = source.MarsState.Wallet.TotalEarned
				},
				WeekKey = source.MarsState.WeekKey,
				WeekStartsAt = source.MarsState.WeekStartsAt,
				WeekEndsAt = source.MarsState.WeekEndsAt,
				Milestones = source.MarsState.Milestones.Select((PubgMarsMilestoneInfo milestone) => new PubgMarsMilestoneInfo
				{
					MilestoneId = milestone.MilestoneId,
					Threshold = milestone.Threshold,
					RewardType = milestone.RewardType,
					RewardValue = milestone.RewardValue,
					DurationDays = milestone.DurationDays,
					IsClaimed = milestone.IsClaimed,
					ClaimedAt = milestone.ClaimedAt,
					IsClaimable = milestone.IsClaimable
				}).ToList(),
				LoginTasks = source.MarsState.LoginTasks.Select((PubgMarsTaskInfo task) => new PubgMarsTaskInfo
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
				}).ToList(),
				ChallengeTasks = source.MarsState.ChallengeTasks.Select((PubgMarsTaskInfo task) => new PubgMarsTaskInfo
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
				}).ToList()
			})
		};
	}

	private static PubgEventCardHubSummaryInfo? BuildHubSummaryFromEventState(PubgEventDetailInfo state)
	{
		if (!string.Equals(state.EventKey, "mapper_cookie", StringComparison.OrdinalIgnoreCase))
		{
			return null;
		}
		if (state.MarsState == null)
		{
			return null;
		}
		int num = Math.Max(0, state.MarsState.Points);
		List<int> list = (from threshold in (from milestone in state.MarsState.Milestones
				select milestone.Threshold into threshold
				where threshold > 0
				select threshold).Distinct()
			orderby threshold
			select threshold).ToList();
		int progressTarget = Math.Max(num, list.LastOrDefault());
		int? nextRewardThreshold = null;
		foreach (int item in list)
		{
			if (item > num)
			{
				nextRewardThreshold = item;
				break;
			}
		}
		List<PubgMarsTaskInfo> list2 = state.MarsState.ChallengeTasks.Where((PubgMarsTaskInfo task) => string.Equals(task.PeriodType, "daily", StringComparison.OrdinalIgnoreCase)).ToList();
		return new PubgEventCardHubSummaryInfo
		{
			ProgressCurrent = num,
			ProgressTarget = progressTarget,
			NextRewardThreshold = nextRewardThreshold,
			NextRewardIn = (nextRewardThreshold.HasValue ? Math.Max(0, nextRewardThreshold.Value - num) : 0),
			DailyCompleted = list2.Count((PubgMarsTaskInfo task) => task.IsCompleted || task.IsClaimed),
			DailyTotal = list2.Count,
			MilestoneThresholds = list
		};
	}

	private void OnInventoryState(PubgEventInventoryStateMessage msg)
	{
		_inventoryWindow?.UpdateInventory(msg);
	}

	private string ResolveCalendarErrorText(string? error)
	{
		if (string.IsNullOrWhiteSpace(error))
		{
			return Loc.GetString("pubg-prelobby-calendar-error-generic");
		}
		string result = default(string);
		if (error.StartsWith("pubg-prelobby-calendar-error-") && _loc.TryGetString(error, ref result))
		{
			return result;
		}
		string text = "pubg-prelobby-calendar-error-" + error.Trim().Replace('_', '-');
		string result2 = default(string);
		if (_loc.TryGetString(text, ref result2))
		{
			return result2;
		}
		return Loc.GetString("pubg-prelobby-calendar-error-generic");
	}

	private string ResolveEventErrorText(string? error)
	{
		if (string.IsNullOrWhiteSpace(error))
		{
			return Loc.GetString("pubg-events-error-generic");
		}
		string result = default(string);
		if (error.StartsWith("pubg-events-error-") && _loc.TryGetString(error, ref result))
		{
			return result;
		}
		string text = "pubg-events-error-" + error.Trim().Replace('_', '-');
		string result2 = default(string);
		if (_loc.TryGetString(text, ref result2))
		{
			return result2;
		}
		return Loc.GetString("pubg-events-error-generic");
	}

	private void UpdateBalanceLabels()
	{
		if (Hub != null)
		{
			Hub.CoinsLabel.Text = _coins.ToString("N0");
			Hub.ScrapLabel.Text = _scrap.ToString("N0");
			Hub.PremiumCoinsLabel.Text = _premiumCoins.ToString("N0");
		}
	}

	private void UpdateProfileLabels()
	{
		if (Hub != null)
		{
			ICommonSession localSession = ((ISharedPlayerManager)_playerManager).LocalSession;
			string item = ((localSession != null) ? localSession.Name : null) ?? Loc.GetString("pubg-prelobby-hub-player-unknown");
			Hub.PlayerCkeyLabel.Text = Loc.GetString("pubg-prelobby-hub-profile-ckey", new(string, object)[1] { ("ckey", item) });
			Hub.PlayerLevelLabel.Text = Loc.GetString("pubg-prelobby-hub-profile-level", new(string, object)[1] { ("level", _playerLevel) });
			string item2 = ((_sponsorDisplayMode == SponsorDisplayMode.Hidden) ? Loc.GetString("mainmenu-sponsor-display-current-hidden") : (_sponsorDisplayTier?.TierName ?? Loc.GetString("mainmenu-sponsor-display-current-none")));
			Hub.PlayerSponsorLabel.Text = Loc.GetString("pubg-prelobby-hub-profile-sponsor", new(string, object)[1] { ("tier", item2) });
		}
	}

	private void UpdateBattlePassLabels()
	{
		if (Hub == null)
		{
			return;
		}
		if (_battlePassState == null)
		{
			Hub.BattlePassSeasonLabel.Text = Loc.GetString("pubg-prelobby-hub-battlepass-loading");
			Hub.BattlePassLevelLabel.Text = string.Empty;
			Hub.BattlePassProgressLabel.Text = string.Empty;
			Hub.BattlePassStatusLabel.Text = string.Empty;
			((Range)Hub.BattlePassProgressBar).MinValue = 0f;
			((Range)Hub.BattlePassProgressBar).MaxValue = 100f;
			((Range)Hub.BattlePassProgressBar).Value = 0f;
			RenderBattlePassTasks();
			return;
		}
		(int, int, int) tuple = ResolveBattlePassProgress(_battlePassState, _battlePassState.CurrentXp);
		int num = _battlePassState.Tasks.Count((BattlePassTaskInfo task) => task.IsCompleted && !task.XpClaimed);
		Hub.BattlePassSeasonLabel.Text = Loc.GetString("pubg-prelobby-hub-battlepass-season", new(string, object)[1] { ("name", _battlePassState.SeasonName) });
		Hub.BattlePassLevelLabel.Text = Loc.GetString("pubg-bp-level", new(string, object)[1] { ("level", tuple.Item1) });
		Hub.BattlePassProgressLabel.Text = Loc.GetString("pubg-bp-xp", new(string, object)[2]
		{
			("current", tuple.Item2),
			("required", tuple.Item3)
		});
		Hub.BattlePassStatusLabel.Text = Loc.GetString("pubg-prelobby-hub-battlepass-status", new(string, object)[1] { ("count", num) });
		((Range)Hub.BattlePassProgressBar).MinValue = 0f;
		((Range)Hub.BattlePassProgressBar).MaxValue = tuple.Item3;
		((Range)Hub.BattlePassProgressBar).Value = Math.Min(tuple.Item2, tuple.Item3);
		RenderBattlePassTasks();
	}

	private void RenderBattlePassTasks()
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		if (Hub == null)
		{
			return;
		}
		((Control)Hub.BattlePassTasksList).RemoveAllChildren();
		if (_battlePassState == null)
		{
			((Control)Hub.BattlePassTasksList).AddChild((Control)(object)CreateBattlePassTasksPlaceholder("pubg-prelobby-hub-battlepass-loading", Color.FromHex((ReadOnlySpan<char>)"#9FB2CE", (Color?)null)));
			return;
		}
		List<BattlePassTaskInfo> list = (from task in _battlePassState.Tasks
			where !task.IsSkipped
			orderby task.Slot
			select task).ToList();
		if (list.Count == 0)
		{
			((Control)Hub.BattlePassTasksList).AddChild((Control)(object)CreateBattlePassTasksPlaceholder("pubg-prelobby-hub-bp-tasks-empty", Color.FromHex((ReadOnlySpan<char>)"#9FB2CE", (Color?)null)));
			return;
		}
		foreach (BattlePassTaskInfo item in list)
		{
			((Control)Hub.BattlePassTasksList).AddChild(CreateBattlePassTaskRow(item));
		}
	}

	private static Label CreateBattlePassTasksPlaceholder(string locKey, Color textColor)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		return new Label
		{
			Text = Loc.GetString(locKey),
			HorizontalExpand = true,
			FontColorOverride = textColor
		};
	}

	private static Control CreateBattlePassTaskRow(BattlePassTaskInfo task)
	{
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Expected O, but got Unknown
		//IL_01fc: Expected O, but got Unknown
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Expected O, but got Unknown
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Expected O, but got Unknown
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Expected O, but got Unknown
		//IL_02ee: Expected O, but got Unknown
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0340: Unknown result type (might be due to invalid IL or missing references)
		//IL_0354: Expected O, but got Unknown
		//IL_036d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0372: Unknown result type (might be due to invalid IL or missing references)
		//IL_037a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		//IL_0388: Unknown result type (might be due to invalid IL or missing references)
		//IL_039d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ae: Expected O, but got Unknown
		//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0407: Unknown result type (might be due to invalid IL or missing references)
		//IL_0419: Expected O, but got Unknown
		//IL_041b: Expected O, but got Unknown
		//IL_041d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0422: Unknown result type (might be due to invalid IL or missing references)
		//IL_042f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0444: Unknown result type (might be due to invalid IL or missing references)
		//IL_0458: Expected O, but got Unknown
		//IL_0496: Unknown result type (might be due to invalid IL or missing references)
		//IL_049b: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ab: Expected O, but got Unknown
		//IL_04ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b4: Expected O, but got Unknown
		//IL_051d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0535: Unknown result type (might be due to invalid IL or missing references)
		//IL_053c: Expected O, but got Unknown
		//IL_0585: Unknown result type (might be due to invalid IL or missing references)
		bool xpClaimed = task.XpClaimed;
		bool flag = task.IsCompleted && !task.XpClaimed;
		Color backgroundColor = (flag ? Color.FromHex((ReadOnlySpan<char>)"#3A3A22", (Color?)null) : (xpClaimed ? Color.FromHex((ReadOnlySpan<char>)"#242E24", (Color?)null) : Color.FromHex((ReadOnlySpan<char>)"#22242B", (Color?)null)));
		Color borderColor = (flag ? Color.FromHex((ReadOnlySpan<char>)"#DDA02B", (Color?)null) : (xpClaimed ? Color.FromHex((ReadOnlySpan<char>)"#507650", (Color?)null) : Color.FromHex((ReadOnlySpan<char>)"#575C66", (Color?)null)));
		Color backgroundColor2 = (flag ? Color.FromHex((ReadOnlySpan<char>)"#5A4520", (Color?)null) : (xpClaimed ? Color.FromHex((ReadOnlySpan<char>)"#2A442A", (Color?)null) : Color.FromHex((ReadOnlySpan<char>)"#2A2C34", (Color?)null)));
		Color borderColor2 = (flag ? Color.FromHex((ReadOnlySpan<char>)"#F2A900", (Color?)null) : (xpClaimed ? Color.FromHex((ReadOnlySpan<char>)"#669766", (Color?)null) : Color.FromHex((ReadOnlySpan<char>)"#707785", (Color?)null)));
		string text = (xpClaimed ? "pubg-prelobby-hub-bp-task-claimed" : (flag ? "pubg-prelobby-hub-bp-task-ready" : "pubg-prelobby-hub-bp-task-in-progress"));
		PanelContainer val = new PanelContainer
		{
			HorizontalExpand = true,
			PanelOverride = (StyleBox)new StyleBoxFlat
			{
				BackgroundColor = backgroundColor,
				BorderColor = borderColor,
				BorderThickness = new Thickness(1f),
				ContentMarginLeftOverride = 8f,
				ContentMarginRightOverride = 8f,
				ContentMarginTopOverride = 6f,
				ContentMarginBottomOverride = 6f
			}
		};
		BoxContainer val2 = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			HorizontalExpand = true,
			SeparationOverride = 4
		};
		BoxContainer val3 = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			HorizontalExpand = true,
			SeparationOverride = 6
		};
		PanelContainer val4 = new PanelContainer
		{
			MinSize = new Vector2(40f, 22f),
			VerticalAlignment = (VAlignment)2,
			PanelOverride = (StyleBox)new StyleBoxFlat
			{
				BackgroundColor = Color.FromHex((ReadOnlySpan<char>)"#11192A", (Color?)null),
				BorderColor = Color.FromHex((ReadOnlySpan<char>)"#4D6E9F", (Color?)null),
				BorderThickness = new Thickness(1f),
				ContentMarginLeftOverride = 4f,
				ContentMarginRightOverride = 4f,
				ContentMarginTopOverride = 1f,
				ContentMarginBottomOverride = 1f
			}
		};
		((Control)val4).AddChild((Control)new Label
		{
			Text = $"#{task.Slot}",
			HorizontalAlignment = (HAlignment)2,
			FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#CFE1FF", (Color?)null)
		});
		string text2 = PubgBattlePassTaskFormatter.GetTaskDisplayText(task);
		if (string.IsNullOrWhiteSpace(text2))
		{
			text2 = task.NameKey;
		}
		Label val5 = new Label
		{
			Text = text2,
			HorizontalExpand = true,
			ClipText = true,
			FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#E9F2FF", (Color?)null)
		};
		PanelContainer val6 = new PanelContainer
		{
			VerticalAlignment = (VAlignment)2,
			PanelOverride = (StyleBox)new StyleBoxFlat
			{
				BackgroundColor = backgroundColor2,
				BorderColor = borderColor2,
				BorderThickness = new Thickness(1f),
				ContentMarginLeftOverride = 6f,
				ContentMarginRightOverride = 6f,
				ContentMarginTopOverride = 1f,
				ContentMarginBottomOverride = 1f
			}
		};
		((Control)val6).AddChild((Control)new Label
		{
			Text = Loc.GetString(text),
			FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#EAF4FF", (Color?)null)
		});
		((Control)val3).AddChild((Control)(object)val4);
		((Control)val3).AddChild((Control)(object)val5);
		((Control)val3).AddChild((Control)(object)val6);
		((Control)val2).AddChild((Control)(object)val3);
		int num = Math.Clamp(task.Progress, 0, Math.Max(1, task.TargetValue));
		BoxContainer val7 = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			HorizontalExpand = true
		};
		Label val8 = new Label();
		val8.Text = Loc.GetString("pubg-bp-task-progress", new(string, object)[2]
		{
			("current", num),
			("target", task.TargetValue)
		});
		((Control)val8).HorizontalExpand = true;
		val8.FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#9FB6D9", (Color?)null);
		((Control)val7).AddChild((Control)(object)val8);
		val8 = new Label();
		val8.Text = Loc.GetString("pubg-bp-task-xp", new(string, object)[1] { ("xp", task.XpReward) });
		val8.FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#FFD57E", (Color?)null);
		((Control)val7).AddChild((Control)(object)val8);
		((Control)val2).AddChild((Control)(object)val7);
		((Control)val).AddChild((Control)(object)val2);
		return (Control)(object)val;
	}

	private void UpdateEventsRedDots()
	{
		if (Hub != null)
		{
			((Control)Hub.EventsRouletteRedDot).Visible = ResolveEventCard("discord_link_once", 0)?.HasClaimable ?? false;
			((Control)Hub.EventsPromoRedDot).Visible = ResolveEventCard("mars_opening", 1)?.HasClaimable ?? false;
			((Control)Hub.EventsFeatureRedDot).Visible = ResolveEventCard("mapper_cookie", 2)?.HasClaimable ?? false;
		}
	}

	private void PreloadEventStates(PubgEventsHubStateMessage hubState, PubgEventsSystem eventsSystem)
	{
		foreach (PubgEventCardInfo item in hubState.Events.OrderBy((PubgEventCardInfo card) => card.SortOrder))
		{
			if (!string.IsNullOrWhiteSpace(item.EventKey) && _preloadedEventKeys.Add(item.EventKey))
			{
				RequestEventStateIfStale(eventsSystem, item.EventKey);
			}
		}
	}

	private void RequestEventStateIfStale(PubgEventsSystem eventsSystem, string eventKey, bool force = false)
	{
		if (!string.IsNullOrWhiteSpace(eventKey))
		{
			DateTime utcNow = DateTime.UtcNow;
			DateTime value;
			bool flag = _lastEventStateRequestAt.TryGetValue(eventKey, out value) && utcNow - value < TimeSpan.FromSeconds(3L);
			PubgEventDetailInfo value2;
			DateTime value3;
			bool flag2 = eventsSystem.LastEventStates.TryGetValue(eventKey, out value2) && _lastEventStateResponseAt.TryGetValue(eventKey, out value3) && utcNow - value3 < TimeSpan.FromSeconds(12L);
			if (force || !(flag || flag2))
			{
				_lastEventStateRequestAt[eventKey] = utcNow;
				eventsSystem.RequestEventState(eventKey, force);
			}
		}
	}

	private void UpdateEventCardLabels()
	{
		if (Hub != null)
		{
			PubgEventCardInfo pubgEventCardInfo = ResolveEventCard("discord_link_once", 0);
			PubgEventCardInfo pubgEventCardInfo2 = ResolveEventCard("mars_opening", 1);
			PubgEventCardInfo pubgEventCardInfo3 = ResolveEventCard("mapper_cookie", 2);
			DateTime nowUtc = ResolveEventsNowUtc();
			Hub.EventsRouletteTitleLabel.Text = ResolveLocalizedCardText(pubgEventCardInfo?.TitleKey, "pubg-prelobby-hub-events-roulette-title");
			Hub.EventsRouletteSubtitleLabel.Text = BuildEventCardSubtitle(pubgEventCardInfo, nowUtc, "pubg-prelobby-hub-events-roulette-subtitle");
			Hub.EventsPromoTitleLabel.Text = ResolveLocalizedCardText(pubgEventCardInfo2?.TitleKey, "pubg-prelobby-hub-events-promo-title");
			Hub.EventsPromoSubtitleLabel.Text = BuildEventCardSubtitle(pubgEventCardInfo2, nowUtc, "pubg-prelobby-hub-events-promo-subtitle");
			Hub.EventsFeatureTitleLabel.Text = ResolveLocalizedCardText(pubgEventCardInfo3?.TitleKey, "pubg-prelobby-hub-events-feature-title");
			Hub.EventsFeatureSubtitleLabel.Text = BuildEventCardSubtitle(pubgEventCardInfo3, nowUtc, "pubg-prelobby-hub-events-feature-caption");
			UpdateFeatureCardSummary(pubgEventCardInfo3);
		}
	}

	private void UpdateInteractiveCardStyles()
	{
		if (Hub != null)
		{
			Hub.CalendarCardPanel.PanelOverride = (StyleBox)(object)(_calendarCardHovered ? CalendarCardHoverStyle : CalendarCardStyle);
			Hub.EventsRouletteCardPanel.PanelOverride = (StyleBox)(object)(_eventsRouletteCardHovered ? EventsCardHoverStyle : EventsCardStyle);
			Hub.EventsPromoCardPanel.PanelOverride = (StyleBox)(object)(_eventsPromoCardHovered ? EventsCardHoverStyle : EventsCardStyle);
			Hub.EventsFeatureCardPanel.PanelOverride = (StyleBox)(object)(_eventsFeatureCardHovered ? EventsFeatureCardHoverStyle : EventsFeatureCardStyle);
		}
	}

	private PubgEventCardInfo? ResolveEventCard(string preferredKey, int fallbackIndex)
	{
		if (_eventsHubState == null || _eventsHubState.Events.Count == 0)
		{
			return null;
		}
		PubgEventCardInfo pubgEventCardInfo = _eventsHubState.Events.FirstOrDefault((PubgEventCardInfo card) => card.EventKey == preferredKey);
		if (pubgEventCardInfo != null)
		{
			return pubgEventCardInfo;
		}
		List<PubgEventCardInfo> list = _eventsHubState.Events.OrderBy((PubgEventCardInfo card) => card.SortOrder).ToList();
		if (fallbackIndex < list.Count)
		{
			return list[fallbackIndex];
		}
		return list[0];
	}

	private string ResolveEventKeyForCard(string preferredKey, int fallbackIndex)
	{
		return ResolveEventCard(preferredKey, fallbackIndex)?.EventKey ?? preferredKey;
	}

	private static string ResolveLocalizedCardText(string? key, string fallbackKey)
	{
		string result = default(string);
		if (!string.IsNullOrWhiteSpace(key) && IoCManager.Resolve<ILocalizationManager>().TryGetString(key, ref result))
		{
			return result;
		}
		return Loc.GetString(fallbackKey);
	}

	private string BuildEventCardSubtitle(PubgEventCardInfo? card, DateTime nowUtc, string fallbackKey)
	{
		if (card == null)
		{
			return Loc.GetString(fallbackKey);
		}
		List<string> list = new List<string> { ResolveLocalizedCardText(card.DescriptionKey, fallbackKey) };
		list.Add(BuildEventCardTimeText(card, nowUtc));
		return string.Join(" | ", list.Where((string text) => !string.IsNullOrWhiteSpace(text)));
	}

	private void UpdateFeatureCardSummary(PubgEventCardInfo? featureCard)
	{
		if (Hub == null)
		{
			return;
		}
		PubgEventCardHubSummaryInfo pubgEventCardHubSummaryInfo = featureCard?.HubSummary;
		if (pubgEventCardHubSummaryInfo == null)
		{
			((Control)Hub.EventsFeatureSummaryPanel).Visible = false;
			((Control)Hub.EventsFeatureMilestonesRow).RemoveAllChildren();
			_lastFeatureMilestoneSignature = null;
			return;
		}
		int num = Math.Max(0, pubgEventCardHubSummaryInfo.ProgressCurrent);
		int num2 = Math.Max(1, Math.Max(pubgEventCardHubSummaryInfo.ProgressTarget, num));
		int? nextRewardThreshold = pubgEventCardHubSummaryInfo.NextRewardThreshold;
		int num3 = Math.Max(0, pubgEventCardHubSummaryInfo.NextRewardIn);
		int num4 = Math.Max(0, pubgEventCardHubSummaryInfo.DailyCompleted);
		int num5 = Math.Max(0, pubgEventCardHubSummaryInfo.DailyTotal);
		((Control)Hub.EventsFeatureSummaryPanel).Visible = true;
		Hub.EventsFeatureProgressStatLabel.Text = Loc.GetString("pubg-prelobby-hub-events-feature-progress", new(string, object)[2]
		{
			("current", num.ToString("N0", CultureInfo.InvariantCulture)),
			("target", num2.ToString("N0", CultureInfo.InvariantCulture))
		});
		Label eventsFeatureNextRewardStatLabel = Hub.EventsFeatureNextRewardStatLabel;
		int? num6 = nextRewardThreshold;
		string text = ((!num6.HasValue) ? Loc.GetString("pubg-prelobby-hub-events-feature-next-complete") : ((num3 > 0) ? Loc.GetString("pubg-prelobby-hub-events-feature-next-threshold", new(string, object)[2]
		{
			("threshold", nextRewardThreshold.Value.ToString("N0", CultureInfo.InvariantCulture)),
			("amount", num3.ToString("N0", CultureInfo.InvariantCulture))
		}) : Loc.GetString("pubg-prelobby-hub-events-feature-next-ready")));
		eventsFeatureNextRewardStatLabel.Text = text;
		Hub.EventsFeatureDailyStatLabel.Text = ((num5 > 0) ? Loc.GetString("pubg-prelobby-hub-events-feature-daily", new(string, object)[2]
		{
			("current", num4),
			("target", num5)
		}) : Loc.GetString("pubg-prelobby-hub-events-feature-daily-empty"));
		((Range)Hub.EventsFeatureProgressBar).MinValue = 0f;
		((Range)Hub.EventsFeatureProgressBar).MaxValue = num2;
		((Range)Hub.EventsFeatureProgressBar).Value = Math.Clamp(num, 0, num2);
		string text2 = BuildFeatureMilestoneSignature(pubgEventCardHubSummaryInfo.MilestoneThresholds, num);
		if (!string.Equals(_lastFeatureMilestoneSignature, text2, StringComparison.Ordinal))
		{
			RenderFeatureMilestoneBadges(pubgEventCardHubSummaryInfo.MilestoneThresholds, num);
			_lastFeatureMilestoneSignature = text2;
		}
	}

	private void RenderFeatureMilestoneBadges(List<int> thresholds, int currentPoints)
	{
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Expected O, but got Unknown
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Expected O, but got Unknown
		//IL_01a9: Expected O, but got Unknown
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Expected O, but got Unknown
		if (Hub == null)
		{
			return;
		}
		((Control)Hub.EventsFeatureMilestonesRow).RemoveAllChildren();
		List<int> list = (from threshold in thresholds.Where((int threshold) => threshold > 0).Distinct()
			orderby threshold
			select threshold).ToList();
		if (list.Count == 0)
		{
			((Control)Hub.EventsFeatureMilestonesRow).AddChild((Control)new Label
			{
				Text = Loc.GetString("pubg-prelobby-hub-events-feature-milestones-empty"),
				FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#97AFCD", (Color?)null)
			});
			return;
		}
		foreach (int item in list)
		{
			bool flag = currentPoints >= item;
			PanelContainer val = new PanelContainer
			{
				PanelOverride = (StyleBox)new StyleBoxFlat
				{
					BackgroundColor = (flag ? Color.FromHex((ReadOnlySpan<char>)"#2A3D2F", (Color?)null) : Color.FromHex((ReadOnlySpan<char>)"#1A2840", (Color?)null)),
					BorderColor = (flag ? Color.FromHex((ReadOnlySpan<char>)"#7CB684", (Color?)null) : Color.FromHex((ReadOnlySpan<char>)"#6082B0", (Color?)null)),
					BorderThickness = new Thickness(1f),
					ContentMarginLeftOverride = 6f,
					ContentMarginRightOverride = 6f,
					ContentMarginTopOverride = 2f,
					ContentMarginBottomOverride = 2f
				}
			};
			((Control)val).AddChild((Control)new Label
			{
				Text = item.ToString("N0", CultureInfo.InvariantCulture),
				FontColorOverride = (flag ? Color.FromHex((ReadOnlySpan<char>)"#D7F2DF", (Color?)null) : Color.FromHex((ReadOnlySpan<char>)"#C8DCF8", (Color?)null))
			});
			((Control)Hub.EventsFeatureMilestonesRow).AddChild((Control)(object)val);
		}
	}

	private static string BuildFeatureMilestoneSignature(List<int> thresholds, int currentPoints)
	{
		int[] values = (from threshold in thresholds.Where((int threshold) => threshold > 0).Distinct()
			orderby threshold
			select threshold).ToArray();
		return $"{currentPoints}:{string.Join(",", values)}";
	}

	private static StyleBoxFlat CreateCardStyle(string backgroundColorHex, string borderColorHex)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Expected O, but got Unknown
		return new StyleBoxFlat
		{
			BackgroundColor = Color.FromHex((ReadOnlySpan<char>)backgroundColorHex, (Color?)null),
			BorderColor = Color.FromHex((ReadOnlySpan<char>)borderColorHex, (Color?)null),
			BorderThickness = new Thickness(1f)
		};
	}

	private static string BuildEventCardTimeText(PubgEventCardInfo card, DateTime nowUtc)
	{
		DateTime dateTime = EnsureUtc(card.StartAt);
		if (dateTime > nowUtc)
		{
			TimeSpan timeSpan = dateTime - nowUtc;
			if (timeSpan.TotalDays >= 1.0)
			{
				return Loc.GetString("pubg-prelobby-hub-event-card-starts-in-days", new(string, object)[2]
				{
					("days", (int)timeSpan.TotalDays),
					("hours", timeSpan.Hours)
				});
			}
			return Loc.GetString("pubg-prelobby-hub-event-card-starts-in-hours", new(string, object)[2]
			{
				("hours", Math.Max(1, (int)timeSpan.TotalHours)),
				("minutes", Math.Max(0, timeSpan.Minutes))
			});
		}
		if (!card.EndAt.HasValue)
		{
			return Loc.GetString("pubg-prelobby-hub-event-card-open");
		}
		TimeSpan timeSpan2 = EnsureUtc(card.EndAt.Value) - nowUtc;
		if (timeSpan2 <= TimeSpan.Zero)
		{
			return Loc.GetString("pubg-prelobby-hub-event-card-ended");
		}
		if (timeSpan2.TotalDays >= 1.0)
		{
			return Loc.GetString("pubg-prelobby-hub-event-card-time-left-days", new(string, object)[2]
			{
				("days", (int)timeSpan2.TotalDays),
				("hours", timeSpan2.Hours)
			});
		}
		return Loc.GetString("pubg-prelobby-hub-event-card-time-left-hours", new(string, object)[2]
		{
			("hours", Math.Max(1, (int)timeSpan2.TotalHours)),
			("minutes", Math.Max(0, timeSpan2.Minutes))
		});
	}

	private DateTime ResolveEventsNowUtc()
	{
		if (_eventsHubState == null)
		{
			return DateTime.UtcNow;
		}
		if (_lastHubResponseAt == DateTime.MinValue)
		{
			return EnsureUtc(_eventsHubState.ServerNowUtc);
		}
		TimeSpan timeSpan = DateTime.UtcNow - _lastHubResponseAt;
		return EnsureUtc(_eventsHubState.ServerNowUtc) + timeSpan;
	}

	private static DateTime EnsureUtc(DateTime value)
	{
		if (value.Kind == DateTimeKind.Utc)
		{
			return value;
		}
		return DateTime.SpecifyKind(value, DateTimeKind.Utc);
	}

	private static bool IsEventActiveNow(PubgEventDetailInfo state, DateTime nowUtc)
	{
		if (!state.IsActive)
		{
			return false;
		}
		if (EnsureUtc(state.StartAt) > nowUtc)
		{
			return false;
		}
		if (state.EndAt.HasValue && EnsureUtc(state.EndAt.Value) <= nowUtc)
		{
			return false;
		}
		return true;
	}

	private static (int level, int xpInLevel, int xpRequired) ResolveBattlePassProgress(BattlePassStateMessage state, int totalXp)
	{
		List<BattlePassLevelInfo> list = state.Levels.OrderBy((BattlePassLevelInfo level) => level.Level).ToList();
		if (list.Count == 0)
		{
			return (level: Math.Max(1, state.CurrentLevel), xpInLevel: 0, xpRequired: 100);
		}
		int num = Math.Max(0, totalXp);
		foreach (BattlePassLevelInfo item in list)
		{
			int num2 = Math.Max(1, item.XpRequired);
			if (num < num2)
			{
				return (level: item.Level, xpInLevel: num, xpRequired: num2);
			}
			num -= num2;
		}
		BattlePassLevelInfo battlePassLevelInfo = list[list.Count - 1];
		int num3 = Math.Max(1, battlePassLevelInfo.XpRequired);
		return (level: Math.Max(battlePassLevelInfo.Level, state.CurrentLevel), xpInLevel: num3, xpRequired: num3);
	}
}
