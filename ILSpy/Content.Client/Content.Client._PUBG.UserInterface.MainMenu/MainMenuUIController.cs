using System.Collections.Generic;
using Content.Client._PUBG.BattlePass;
using Content.Client._PUBG.Sponsor.UI;
using Content.Client.Gameplay;
using Content.Shared._PUBG.BattlePass;
using Content.Shared._PUBG.Cases;
using Content.Shared._PUBG.Skin;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.CustomControls;

namespace Content.Client._PUBG.UserInterface.MainMenu;

public sealed class MainMenuUIController : UIController, IOnStateEntered<GameplayState>, IOnStateExited<GameplayState>
{
	private MainMenuWindow? _window;

	private PubgSponsorDisplayWindow? _sponsorDisplayWindow;

	private bool _systemSubscribed;

	private SponsorTierInfo? _cachedSponsorDisplayTier;

	private List<SponsorActiveTierInfo> _cachedSponsorActiveTiers = new List<SponsorActiveTierInfo>();

	private SponsorDisplayMode _cachedSponsorDisplayMode;

	private string? _cachedSponsorPreferredTierKey;

	private Dictionary<string, int> _cachedSponsorPermissions = new Dictionary<string, int>();

	private List<SponsorPermissionDetailInfo> _cachedSponsorPermissionDetails = new List<SponsorPermissionDetailInfo>();

	private bool _hasSponsorState;

	private bool _sponsorDisplayUpdating;

	public override void Initialize()
	{
		((UIController)this).Initialize();
	}

	private void EnsureSystemSubscribed()
	{
		if (!_systemSubscribed)
		{
			MainMenuSystem mainMenuSystem = base.EntityManager.System<MainMenuSystem>();
			mainMenuSystem.OnSkinOpenReceived += OnSkinOpen;
			mainMenuSystem.OnSkinStateReceived += OnSkinState;
			mainMenuSystem.OnBalanceUpdateReceived += OnBalanceUpdate;
			mainMenuSystem.OnSkinProfileStateReceived += OnSkinProfileState;
			mainMenuSystem.OnCaseOpenResultReceived += OnCaseOpenResult;
			mainMenuSystem.OnCaseOpenErrorReceived += OnCaseOpenError;
			base.EntityManager.System<BattlePassSystem>().OnStateReceived += OnBattlePassState;
			_systemSubscribed = true;
		}
	}

	public void OnStateEntered(GameplayState state)
	{
		EnsureSystemSubscribed();
	}

	public void OnStateExited(GameplayState state)
	{
		CloseWindow();
		CloseSponsorDisplayWindow();
		UnsubscribeFromSystems();
	}

	private void UnsubscribeFromSystems()
	{
		if (_systemSubscribed)
		{
			MainMenuSystem mainMenuSystem = base.EntityManager.SystemOrNull<MainMenuSystem>();
			if (mainMenuSystem != null)
			{
				mainMenuSystem.OnSkinOpenReceived -= OnSkinOpen;
				mainMenuSystem.OnSkinStateReceived -= OnSkinState;
				mainMenuSystem.OnBalanceUpdateReceived -= OnBalanceUpdate;
				mainMenuSystem.OnSkinProfileStateReceived -= OnSkinProfileState;
				mainMenuSystem.OnCaseOpenResultReceived -= OnCaseOpenResult;
				mainMenuSystem.OnCaseOpenErrorReceived -= OnCaseOpenError;
			}
			BattlePassSystem battlePassSystem = base.EntityManager.SystemOrNull<BattlePassSystem>();
			if (battlePassSystem != null)
			{
				battlePassSystem.OnStateReceived -= OnBattlePassState;
			}
			_systemSubscribed = false;
		}
	}

	private void OnSkinOpen(SkinOpenMessage msg)
	{
		ToggleWindow();
	}

	private void OnSkinState(SkinStateMessage msg)
	{
		_cachedSponsorDisplayTier = msg.SponsorDisplayTier;
		_cachedSponsorActiveTiers = new List<SponsorActiveTierInfo>(msg.SponsorActiveTiers);
		_cachedSponsorDisplayMode = msg.SponsorDisplayMode;
		_cachedSponsorPreferredTierKey = msg.SponsorPreferredTierKey;
		_cachedSponsorPermissions = new Dictionary<string, int>(msg.SponsorPermissions);
		_cachedSponsorPermissionDetails = new List<SponsorPermissionDetailInfo>(msg.SponsorPermissionDetails);
		_hasSponsorState = true;
		_sponsorDisplayUpdating = false;
		UpdateSponsorDisplayWindowState();
		EnsureWindow();
		_window?.UpdateSkinData(msg.AllItems, msg.UnlockedItems, msg.ItemExpiresAt, msg.RecipePrices, msg.ShopItems, msg.CurrentOutfit, msg.PlayerCoins, msg.PlayerScrap, msg.PlayerPremiumCoins, msg.AllEmotes, msg.UnlockedEmotes, msg.EquippedEmotes, msg.MaxEmoteSlots, msg.TotalCaseDropSkins, msg.UnlockedCaseDropSkins, msg.TotalUniqueSkins, msg.TotalEmotes, msg.AvailableEmotes, msg.SponsorPermissions, msg.SponsorPermissionDetails, msg.SponsorDisplayTier, msg.SponsorActiveTiers, msg.SponsorDisplayMode, msg.SponsorPreferredTierKey, msg.TotalGames, msg.Wins, msg.TotalKills, msg.TotalDamage, msg.AvgSurvivalTime, msg.TotalSurvivalTime, msg.Leaderboard, msg.PlayerRank, msg.PlayerRating, msg.Reputation, msg.MatchHistory, msg.TotalDeaths);
	}

	private void OnBattlePassState(BattlePassStateMessage msg)
	{
		_window?.UpdateBattlePassData(msg);
	}

	private void OnBalanceUpdate(BalanceUpdateMessage msg)
	{
		_window?.UpdateBalance(msg.Coins, msg.Scrap, msg.PremiumCoins);
	}

	private void OnSkinProfileState(SkinProfileStateMessage msg)
	{
		_window?.UpdateProfileData(msg.Leaderboard, msg.PlayerRank, msg.PlayerRating, msg.MatchHistory, msg.TotalDeaths);
	}

	private void OnCaseOpenResult(CaseOpenResultMessage msg)
	{
		_window?.HandleCaseOpenResult(msg);
	}

	private void OnCaseOpenError(CaseOpenErrorMessage msg)
	{
		_window?.HandleCaseOpenError(msg);
	}

	private void EnsureWindow()
	{
		if (_window == null)
		{
			_window = base.UIManager.CreateWindow<MainMenuWindow>();
			((BaseWindow)_window).OnClose += OnWindowClosed;
			_window.OnApplyOutfit += OnApplyOutfit;
			_window.OnOpenSponsorDisplaySettingsRequested += OpenSponsorDisplayWindow;
		}
	}

	private void ToggleWindow()
	{
		if (_window == null)
		{
			EnsureWindow();
			MainMenuWindow? window = _window;
			if (window != null)
			{
				((BaseWindow)window).OpenCentered();
			}
			SkinOpenMessage skinOpenMessage = new SkinOpenMessage();
			base.EntityManager.RaisePredictiveEvent<SkinOpenMessage>(skinOpenMessage);
		}
		else
		{
			CloseWindow();
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
			((BaseWindow)_window).OnClose -= OnWindowClosed;
			_window.OnApplyOutfit -= OnApplyOutfit;
			_window.OnOpenSponsorDisplaySettingsRequested -= OpenSponsorDisplayWindow;
			((BaseWindow)_window).Close();
			_window = null;
		}
	}

	private void OnApplyOutfit(Dictionary<string, string> outfit)
	{
		SkinApplyMessage skinApplyMessage = new SkinApplyMessage(outfit);
		base.EntityManager.RaisePredictiveEvent<SkinApplyMessage>(skinApplyMessage);
	}

	public void OpenSponsorDisplayWindow()
	{
		EnsureSystemSubscribed();
		EnsureSponsorDisplayWindow();
		UpdateSponsorDisplayWindowState();
		PubgSponsorDisplayWindow? sponsorDisplayWindow = _sponsorDisplayWindow;
		if (sponsorDisplayWindow != null)
		{
			((BaseWindow)sponsorDisplayWindow).OpenCentered();
		}
		base.EntityManager.System<MainMenuSystem>().RequestSkinState(!_hasSponsorState);
	}

	public bool TryGetCachedSponsorData(out SponsorTierInfo? displayTier, out List<SponsorActiveTierInfo> activeTiers, out SponsorDisplayMode displayMode, out string? preferredTierKey, out Dictionary<string, int> permissions, out List<SponsorPermissionDetailInfo> permissionDetails)
	{
		displayTier = _cachedSponsorDisplayTier;
		activeTiers = _cachedSponsorActiveTiers;
		displayMode = _cachedSponsorDisplayMode;
		preferredTierKey = _cachedSponsorPreferredTierKey;
		permissions = _cachedSponsorPermissions;
		permissionDetails = _cachedSponsorPermissionDetails;
		return _hasSponsorState;
	}

	private void EnsureSponsorDisplayWindow()
	{
		if (_sponsorDisplayWindow == null)
		{
			_sponsorDisplayWindow = base.UIManager.CreateWindow<PubgSponsorDisplayWindow>();
			((BaseWindow)_sponsorDisplayWindow).OnClose += OnSponsorDisplayWindowClosed;
			_sponsorDisplayWindow.SelectionRequested += OnSponsorDisplaySelectionRequested;
		}
	}

	private void OnSponsorDisplayWindowClosed()
	{
		CloseSponsorDisplayWindow();
	}

	private void CloseSponsorDisplayWindow()
	{
		if (_sponsorDisplayWindow != null)
		{
			((BaseWindow)_sponsorDisplayWindow).OnClose -= OnSponsorDisplayWindowClosed;
			_sponsorDisplayWindow.SelectionRequested -= OnSponsorDisplaySelectionRequested;
			((BaseWindow)_sponsorDisplayWindow).Close();
			_sponsorDisplayWindow = null;
		}
	}

	private void OnSponsorDisplaySelectionRequested(SponsorDisplayMode mode, string? tierKey)
	{
		_sponsorDisplayUpdating = true;
		UpdateSponsorDisplayWindowState();
		SponsorDisplayTierSelectMessage sponsorDisplayTierSelectMessage = new SponsorDisplayTierSelectMessage(mode, tierKey);
		base.EntityManager.RaisePredictiveEvent<SponsorDisplayTierSelectMessage>(sponsorDisplayTierSelectMessage);
	}

	private void UpdateSponsorDisplayWindowState()
	{
		_sponsorDisplayWindow?.UpdateState(_cachedSponsorDisplayTier, _cachedSponsorActiveTiers, _cachedSponsorDisplayMode, _cachedSponsorPreferredTierKey, _hasSponsorState, _sponsorDisplayUpdating);
	}

	public Dictionary<string, string> GetCachedCurrentOutfit()
	{
		return base.EntityManager.System<MainMenuSystem>().GetCachedCurrentOutfit();
	}

	public void OpenBattlePassMenu()
	{
		if (_window == null)
		{
			EnsureWindow();
			MainMenuWindow? window = _window;
			if (window != null)
			{
				((BaseWindow)window).OpenCentered();
			}
			SkinOpenMessage skinOpenMessage = new SkinOpenMessage();
			base.EntityManager.RaisePredictiveEvent<SkinOpenMessage>(skinOpenMessage);
		}
		_window?.SwitchToBattlePassTab();
	}
}
