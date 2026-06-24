using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Client._CIV14merka.UserInterface.Systems.Hud;
using Content.Client._PUBG.BattlePass;
using Content.Client._PUBG.UserInterface.Systems.Loadout;
using Content.Client.Gameplay;
using Content.Client.UserInterface.Controls;
using Content.Client.UserInterface.Screens;
using Content.Client.UserInterface.Systems.Gameplay;
using Content.Shared._PUBG;
using Content.Shared._PUBG.BattlePass;
using Content.Shared._PUBG.Lobby;
using Content.Shared.CCVar;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Timing;

namespace Content.Client._PUBG.UserInterface.Systems.Stats;

public sealed class PubgUIController : UIController, IOnStateEntered<GameplayState>, IOnStateExited<GameplayState>
{
	private const string PubgHudInfoStackName = "PubgHudInfoStack";

	private const float MinimapSize = 200f;

	private const float MinimapBottomOffset = 220f;

	private const float StackSpacing = 10f;

	private const float StackRightMargin = 15f;

	private const float StackMinTop = 15f;

	private const float StackDownOffset = 200f;

	[Dependency]
	private IConfigurationManager _cfg;

	[Dependency]
	private PubgLoadoutUIController _loadout;

	private bool _pubgEnabled;

	private bool _inLobby;

	private LayoutContainer? _infoViewport;

	private BoxContainer? _infoStack;

	private bool _infoViewportSubscribed;

	private PanelContainer? _killsPanel;

	private PanelContainer? _alivePanel;

	private PanelContainer? _zonePanel;

	private Label? _killsLabel;

	private Label? _aliveLabel;

	private Label? _zoneLabel;

	private int _kills;

	private PubgStatsScreen? _statsScreen;

	private CivHudEventsSystem? _civHud;

	private bool _systemSubscribed;

	private BattlePassStateMessage? _cachedBattlePassState;

	private int _pendingBattlePassXpGain;

	private bool _hasLastPlayerProgress;

	private int _lastPlayerLevel = 1;

	private int _lastPlayerXp;

	private int _lastPlayerMaxXp = 100;

	public override void Initialize()
	{
		((UIController)this).Initialize();
		_ = _loadout;
		GameplayStateLoadController uIController = base.UIManager.GetUIController<GameplayStateLoadController>();
		uIController.OnScreenLoad = (Action)Delegate.Combine(uIController.OnScreenLoad, (Action)delegate
		{
			if (_pubgEnabled)
			{
				EnsureLabels();
			}
		});
		uIController.OnScreenUnload = (Action)Delegate.Combine(uIController.OnScreenUnload, new Action(OnScreenUnload));
	}

	public void OnStateEntered(GameplayState state)
	{
		EnsureSystemSubscribed();
		if (_pubgEnabled)
		{
			EnsureLabels();
		}
	}

	public void OnStateExited(GameplayState state)
	{
		UnsubscribeFromSystem();
		HideStatsScreen();
	}

	public override void FrameUpdate(FrameEventArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((UIController)this).FrameUpdate(args);
		if (IsCiv14TdmActive())
		{
			HideKillsAndAlivePanels();
		}
		if (_infoViewport != null && _infoStack != null)
		{
			PositionInfoStack(_infoViewport, (Control)(object)_infoStack, GetScreenType());
		}
	}

	private bool IsCiv14TdmActive()
	{
		CivHudEventsSystem? civHud = _civHud;
		if (civHud == null)
		{
			return false;
		}
		return civHud.LastStatus?.Enabled == true;
	}

	private void HideKillsAndAlivePanels()
	{
		PanelContainer killsPanel = _killsPanel;
		if (killsPanel != null && !((Control)killsPanel).Disposed)
		{
			((Control)killsPanel).Orphan();
		}
		PanelContainer alivePanel = _alivePanel;
		if (alivePanel != null && !((Control)alivePanel).Disposed)
		{
			((Control)alivePanel).Orphan();
		}
		_killsPanel = null;
		_alivePanel = null;
		_killsLabel = null;
		_aliveLabel = null;
	}

	private void EnsureSystemSubscribed()
	{
		if (!_systemSubscribed)
		{
			_civHud = base.EntityManager.SystemOrNull<CivHudEventsSystem>();
			PubgHudEventsSystem pubgHudEventsSystem = base.EntityManager.System<PubgHudEventsSystem>();
			pubgHudEventsSystem.OnPubgModeStatusReceived += OnPubgStatus;
			pubgHudEventsSystem.OnKillsChangedReceived += OnKillsChanged;
			pubgHudEventsSystem.OnWarmupStatusReceived += OnWarmupStatus;
			pubgHudEventsSystem.OnLobbyStatusReceived += OnLobbyStatus;
			pubgHudEventsSystem.OnZoneStateReceived += OnZoneStateUpdate;
			pubgHudEventsSystem.OnGameEndReceived += OnGameEnd;
			BattlePassSystem battlePassSystem = base.EntityManager.System<BattlePassSystem>();
			battlePassSystem.OnStateReceived += OnBattlePassState;
			if (_cachedBattlePassState == null)
			{
				_cachedBattlePassState = battlePassSystem.LastState;
			}
			_systemSubscribed = true;
			ApplyCachedState(pubgHudEventsSystem);
		}
	}

	private void UnsubscribeFromSystem()
	{
		if (_systemSubscribed)
		{
			PubgHudEventsSystem pubgHudEventsSystem = base.EntityManager.SystemOrNull<PubgHudEventsSystem>();
			if (pubgHudEventsSystem != null)
			{
				pubgHudEventsSystem.OnPubgModeStatusReceived -= OnPubgStatus;
				pubgHudEventsSystem.OnKillsChangedReceived -= OnKillsChanged;
				pubgHudEventsSystem.OnWarmupStatusReceived -= OnWarmupStatus;
				pubgHudEventsSystem.OnLobbyStatusReceived -= OnLobbyStatus;
				pubgHudEventsSystem.OnZoneStateReceived -= OnZoneStateUpdate;
				pubgHudEventsSystem.OnGameEndReceived -= OnGameEnd;
			}
			BattlePassSystem battlePassSystem = base.EntityManager.SystemOrNull<BattlePassSystem>();
			if (battlePassSystem != null)
			{
				battlePassSystem.OnStateReceived -= OnBattlePassState;
			}
			_civHud = null;
			_systemSubscribed = false;
		}
	}

	private void ApplyCachedState(PubgHudEventsSystem system)
	{
		if (system.LastPubgModeStatus != null)
		{
			OnPubgStatus(system.LastPubgModeStatus);
		}
		if (system.LastLobbyStatus != null)
		{
			OnLobbyStatus(system.LastLobbyStatus);
		}
		if (system.LastKillsChanged != null)
		{
			OnKillsChanged(system.LastKillsChanged);
		}
		if (system.LastWarmupStatus != null)
		{
			OnWarmupStatus(system.LastWarmupStatus);
		}
		if (system.LastZoneState != null)
		{
			OnZoneStateUpdate(system.LastZoneState);
		}
	}

	private void EnsureLabels()
	{
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		if (_inLobby)
		{
			return;
		}
		UIScreen activeScreen = base.UIManager.ActiveScreen;
		if (activeScreen == null || activeScreen.GetWidget<MainViewport>() == null)
		{
			return;
		}
		LayoutContainer viewportContainer;
		try
		{
			viewportContainer = ((Control)activeScreen).FindControl<LayoutContainer>("ViewportContainer");
		}
		catch (ArgumentException)
		{
			return;
		}
		ScreenType screenType = GetScreenType();
		Control infoContainer = GetInfoContainer(viewportContainer, screenType);
		if (!IsCiv14TdmActive())
		{
			if (_killsPanel == null)
			{
				(_killsPanel, _killsLabel) = CreateStyledPanel(Color.FromHex((ReadOnlySpan<char>)"#dc3545", (Color?)null), "✖", $"KILLS: {_kills}");
				infoContainer.AddChild((Control)(object)_killsPanel);
				if (screenType != ScreenType.Default)
				{
					ApplyPanelLayout(_killsPanel, 60f, -475f, 15);
				}
			}
			if (_alivePanel == null)
			{
				(_alivePanel, _aliveLabel) = CreateStyledPanel(Color.FromHex((ReadOnlySpan<char>)"#28a745", (Color?)null), "♦", GetAliveLabelText(0, null, null));
				infoContainer.AddChild((Control)(object)_alivePanel);
				if (screenType != ScreenType.Default)
				{
					ApplyPanelLayout(_alivePanel, 105f, -430f, 15);
				}
			}
		}
		if (_zonePanel == null)
		{
			(_zonePanel, _zoneLabel) = CreateStyledPanel(Color.FromHex((ReadOnlySpan<char>)"#6c757d", (Color?)null), "⊙", "ZONE: 120");
			infoContainer.AddChild((Control)(object)_zonePanel);
			if (screenType != ScreenType.Default)
			{
				ApplyPanelLayout(_zonePanel, 150f, -520f, 25);
			}
			((Control)_zonePanel).Visible = false;
		}
		PositionInfoStack(viewportContainer, infoContainer, screenType);
	}

	private (PanelContainer panel, Label label) CreateStyledPanel(Color accentColor, string icon, string text)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Expected O, but got Unknown
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Expected O, but got Unknown
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Expected O, but got Unknown
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Expected O, but got Unknown
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Expected O, but got Unknown
		PanelContainer val = new PanelContainer();
		StyleBoxFlat val2 = new StyleBoxFlat
		{
			BackgroundColor = Color.FromHex((ReadOnlySpan<char>)"#1a1a1aDD", (Color?)null),
			BorderColor = accentColor,
			BorderThickness = new Thickness(0f, 0f, 3f, 0f)
		};
		((StyleBox)val2).SetContentMarginOverride((Margin)15, 8f);
		val.PanelOverride = (StyleBox)(object)val2;
		((Control)val).MinWidth = 150f;
		BoxContainer val3 = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			SeparationOverride = 8
		};
		Label val4 = new Label
		{
			Text = icon,
			FontColorOverride = accentColor,
			MinWidth = 20f
		};
		Label val5 = new Label
		{
			Text = text,
			FontColorOverride = Color.White
		};
		((Control)val3).AddChild((Control)(object)val4);
		((Control)val3).AddChild((Control)(object)val5);
		((Control)val).AddChild((Control)(object)val3);
		return (panel: val, label: val5);
	}

	private void OnPubgStatus(PubgModeStatusEvent msg)
	{
		_pubgEnabled = msg.Enabled;
		if (_pubgEnabled)
		{
			EnsureLabels();
			return;
		}
		ClearPanels();
		_kills = 0;
	}

	private void OnKillsChanged(PubgKillsChangedEvent msg)
	{
		_kills = msg.Kills;
		if (_killsLabel != null)
		{
			_killsLabel.Text = $"KILLS: {_kills}";
		}
	}

	private void OnWarmupStatus(PubgWarmupStatusEvent msg)
	{
		EnsureLabels();
		if (_aliveLabel != null)
		{
			_aliveLabel.Text = GetAliveLabelText(msg.AlivePlayers, msg.TeamAAlive, msg.TeamBAlive);
		}
	}

	private static string GetAliveLabelText(int alivePlayers, int? teamAAlive, int? teamBAlive)
	{
		if (teamAAlive.HasValue && teamBAlive.HasValue)
		{
			return Loc.GetString("pubg-hud-alive-vs", new(string, object)[2]
			{
				("teamA", teamAAlive.Value),
				("teamB", teamBAlive.Value)
			});
		}
		return Loc.GetString("pubg-hud-alive", new(string, object)[1] { ("alive", alivePlayers) });
	}

	private void OnLobbyStatus(LobbyStatusEvent msg)
	{
		_inLobby = msg.InLobby;
		if (_inLobby)
		{
			ClearPanels();
			_kills = 0;
		}
		else if (_pubgEnabled)
		{
			EnsureLabels();
		}
	}

	private void OnZoneStateUpdate(PubgZoneStateEvent msg)
	{
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		EnsureLabels();
		if (_zoneLabel == null || _zonePanel == null)
		{
			return;
		}
		if (msg.Active)
		{
			int value = (int)msg.TimeRemaining / 60;
			int value2 = (int)msg.TimeRemaining % 60;
			if (msg.State == ZoneState.Waiting)
			{
				_zoneLabel.Text = $"NEXT ZONE: {value:D2}:{value2:D2}";
				UpdatePanelColor(_zonePanel, Color.FromHex((ReadOnlySpan<char>)"#6c9bd1", (Color?)null));
			}
			else
			{
				_zoneLabel.Text = $"ZONE: {value:D2}:{value2:D2}";
				UpdatePanelColor(_zonePanel, Color.FromHex((ReadOnlySpan<char>)"#dc3545", (Color?)null));
			}
			((Control)_zonePanel).Visible = true;
		}
		else
		{
			((Control)_zonePanel).Visible = false;
		}
	}

	private void UpdatePanelColor(PanelContainer panel, Color color)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		StyleBox panelOverride = panel.PanelOverride;
		StyleBoxFlat val = (StyleBoxFlat)(object)((panelOverride is StyleBoxFlat) ? panelOverride : null);
		if (val != null)
		{
			val.BorderColor = color;
		}
	}

	private void OnGameEnd(PubgGameEndEvent msg)
	{
		_pendingBattlePassXpGain = Math.Max(0, msg.XpGained);
		BattlePassSystem battlePassSystem = base.EntityManager.System<BattlePassSystem>();
		if (_cachedBattlePassState == null)
		{
			_cachedBattlePassState = battlePassSystem.LastState;
		}
		int? previousPlayerLevel = null;
		int? previousPlayerXp = null;
		int? previousPlayerMaxXp = null;
		if (_hasLastPlayerProgress)
		{
			previousPlayerLevel = _lastPlayerLevel;
			previousPlayerXp = _lastPlayerXp;
			previousPlayerMaxXp = _lastPlayerMaxXp;
		}
		ShowStatsScreen(msg.Placement, msg.Kills, msg.Deaths, msg.DamageDealt, msg.DamageTaken, msg.SurvivalTime, msg.WeaponDamage, msg.CoinsEarned, msg.RatingChange, msg.CurrentRating, msg.NewRating, msg.NewCoins, msg.KillerUsername, msg.KillerRank, msg.CompletedTasks, msg.XpGained, msg.PlayerLevel, msg.PlayerXp, msg.PlayerMaxXp, msg.IsPartyMode, msg.PartyStats, _cachedBattlePassState, previousPlayerLevel, previousPlayerXp, previousPlayerMaxXp);
		battlePassSystem.RequestBattlePass();
		_lastPlayerLevel = Math.Max(1, msg.PlayerLevel);
		_lastPlayerMaxXp = Math.Max(1, msg.PlayerMaxXp);
		_lastPlayerXp = Math.Clamp(msg.PlayerXp, 0, _lastPlayerMaxXp);
		_hasLastPlayerProgress = true;
	}

	private void ShowStatsScreen(int placement, int kills, int deaths, int damageDealt, int damageTaken, int survivalTime, Dictionary<string, int> weaponDamage, int coinsEarned, int ratingChange, int currentRating, int newRating, int newCoins, string? killerUsername = null, string killerRank = "N", List<BattlePassTaskInfo>? completedTasks = null, int xpGained = 0, int playerLevel = 1, int playerXp = 0, int playerMaxXp = 100, bool isPartyMode = false, List<PubgPartyStatsEntry>? partyStats = null, BattlePassStateMessage? battlePassState = null, int? previousPlayerLevel = null, int? previousPlayerXp = null, int? previousPlayerMaxXp = null)
	{
		if (_statsScreen != null)
		{
			_statsScreen.SetStats(placement, kills, deaths, damageDealt, damageTaken, survivalTime, weaponDamage, coinsEarned, ratingChange, currentRating, newRating, newCoins, killerUsername, killerRank, completedTasks, xpGained, playerLevel, playerXp, playerMaxXp, isPartyMode, partyStats, battlePassState, previousPlayerLevel, previousPlayerXp, previousPlayerMaxXp);
			return;
		}
		UIScreen activeScreen = base.UIManager.ActiveScreen;
		if (activeScreen != null)
		{
			_statsScreen = new PubgStatsScreen();
			_statsScreen.SetStats(placement, kills, deaths, damageDealt, damageTaken, survivalTime, weaponDamage, coinsEarned, ratingChange, currentRating, newRating, newCoins, killerUsername, killerRank, completedTasks, xpGained, playerLevel, playerXp, playerMaxXp, isPartyMode, partyStats, battlePassState, previousPlayerLevel, previousPlayerXp, previousPlayerMaxXp);
			_statsScreen.OnContinue += HideStatsScreen;
			((Control)activeScreen).AddChild((Control)(object)_statsScreen);
			LayoutContainer.SetAnchorAndMarginPreset((Control)(object)_statsScreen, (LayoutPreset)15, (LayoutPresetMode)0, 0);
		}
	}

	private void OnBattlePassState(BattlePassStateMessage msg)
	{
		_cachedBattlePassState = msg;
		if (_statsScreen != null)
		{
			_statsScreen.UpdateBattlePassState(msg, _pendingBattlePassXpGain);
			_pendingBattlePassXpGain = 0;
		}
	}

	private void HideStatsScreen()
	{
		if (_statsScreen != null)
		{
			if (!((Control)_statsScreen).Disposed)
			{
				((Control)_statsScreen).Orphan();
			}
			_statsScreen = null;
		}
		_pendingBattlePassXpGain = 0;
	}

	private void OnScreenUnload()
	{
		ClearPanels();
		UnsubscribeInfoViewport();
		BoxContainer infoStack = _infoStack;
		if (infoStack != null && !((Control)infoStack).Disposed)
		{
			((Control)infoStack).Orphan();
		}
		_infoStack = null;
	}

	private void ClearPanels()
	{
		PanelContainer killsPanel = _killsPanel;
		if (killsPanel != null && !((Control)killsPanel).Disposed)
		{
			((Control)killsPanel).Orphan();
		}
		PanelContainer alivePanel = _alivePanel;
		if (alivePanel != null && !((Control)alivePanel).Disposed)
		{
			((Control)alivePanel).Orphan();
		}
		PanelContainer zonePanel = _zonePanel;
		if (zonePanel != null && !((Control)zonePanel).Disposed)
		{
			((Control)zonePanel).Orphan();
		}
		_killsPanel = null;
		_alivePanel = null;
		_zonePanel = null;
		_killsLabel = null;
		_aliveLabel = null;
		_zoneLabel = null;
	}

	private ScreenType GetScreenType()
	{
		if (!Enum.TryParse<ScreenType>(_cfg.GetCVar<string>(CCVars.UILayout), out var result))
		{
			return ScreenType.Default;
		}
		return result;
	}

	private Control GetInfoContainer(LayoutContainer viewportContainer, ScreenType screenType)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Expected O, but got Unknown
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Expected O, but got Unknown
		if (screenType != ScreenType.Default)
		{
			return (Control)(object)viewportContainer;
		}
		bool flag = true;
		BoxContainer val;
		try
		{
			val = ((Control)viewportContainer).FindControl<BoxContainer>("PubgHudInfoStack");
		}
		catch (ArgumentException)
		{
			flag = false;
			val = new BoxContainer
			{
				Name = "PubgHudInfoStack",
				Orientation = (LayoutOrientation)1,
				SeparationOverride = 8,
				MinSize = new Vector2(200f, 0f),
				HorizontalAlignment = (HAlignment)3
			};
			((Control)viewportContainer).AddChild((Control)(object)val);
		}
		if (flag)
		{
			if (!((Control)val).Disposed)
			{
				((Control)val).Orphan();
				((Control)viewportContainer).AddChild((Control)(object)val);
			}
			else
			{
				val = new BoxContainer
				{
					Name = "PubgHudInfoStack",
					Orientation = (LayoutOrientation)1,
					SeparationOverride = 8,
					MinSize = new Vector2(200f, 0f),
					HorizontalAlignment = (HAlignment)3
				};
				((Control)viewportContainer).AddChild((Control)(object)val);
			}
		}
		_infoStack = val;
		EnsureInfoViewport(viewportContainer);
		return (Control)(object)val;
	}

	private void PositionInfoStack(LayoutContainer viewportContainer, Control infoContainer, ScreenType screenType)
	{
		if (screenType != ScreenType.Default)
		{
			return;
		}
		BoxContainer val = (BoxContainer)(object)((infoContainer is BoxContainer) ? infoContainer : null);
		if (val == null)
		{
			return;
		}
		((Control)val).Measure(Vector2Helpers.Infinity);
		Vector2 desiredSize = ((Control)val).DesiredSize;
		Vector2 viewportSize = GetViewportSize(viewportContainer);
		if (!(viewportSize.X <= 0f) && !(viewportSize.Y <= 0f))
		{
			float num = viewportSize.X - desiredSize.X - 15f;
			float num2 = viewportSize.Y - 220f - 200f - 10f - desiredSize.Y + 200f;
			if (num < 15f)
			{
				num = 15f;
			}
			if (num2 < 15f)
			{
				num2 = 15f;
			}
			LayoutContainer.SetAnchorAndMarginPreset((Control)(object)val, (LayoutPreset)0, (LayoutPresetMode)0, 0);
			LayoutContainer.SetPosition((Control)(object)val, new Vector2(num, num2));
		}
	}

	private Vector2 GetViewportSize(LayoutContainer viewportContainer)
	{
		Vector2 result = ((Control)base.UIManager.RootControl).Size;
		if (result.X <= 0f || result.Y <= 0f)
		{
			result = ((Control)base.UIManager.StateRoot).Size;
		}
		if (result.X <= 0f || result.Y <= 0f)
		{
			result = ((Control)viewportContainer).Size;
		}
		if (result.X <= 0f || result.Y <= 0f)
		{
			UIScreen activeScreen = base.UIManager.ActiveScreen;
			if (activeScreen != null)
			{
				MainViewport widget = activeScreen.GetWidget<MainViewport>();
				result = ((widget != null) ? ((Control)widget).Size : Vector2.Zero);
			}
		}
		if (result.X <= 0f || result.Y <= 0f)
		{
			Control parent = ((Control)viewportContainer).Parent;
			result = ((parent != null) ? parent.Size : Vector2.Zero);
		}
		if (result.X <= 0f || result.Y <= 0f)
		{
			UIScreen activeScreen2 = base.UIManager.ActiveScreen;
			result = ((activeScreen2 != null) ? ((Control)activeScreen2).Size : Vector2.Zero);
		}
		return result;
	}

	private void EnsureInfoViewport(LayoutContainer viewportContainer)
	{
		if (_infoViewport != viewportContainer || !_infoViewportSubscribed)
		{
			UnsubscribeInfoViewport();
			_infoViewport = viewportContainer;
			((Control)_infoViewport).OnResized += OnInfoViewportResized;
			_infoViewportSubscribed = true;
		}
	}

	private void UnsubscribeInfoViewport()
	{
		if (_infoViewport != null && _infoViewportSubscribed)
		{
			((Control)_infoViewport).OnResized -= OnInfoViewportResized;
		}
		_infoViewport = null;
		_infoViewportSubscribed = false;
	}

	private void OnInfoViewportResized()
	{
		if (_infoViewport != null && _infoStack != null)
		{
			PositionInfoStack(_infoViewport, (Control)(object)_infoStack, GetScreenType());
		}
	}

	private void ApplyPanelLayout(PanelContainer panel, float topOffset, float defaultBottomOffset, int margin)
	{
		if (GetScreenType() == ScreenType.Default)
		{
			LayoutContainer.SetAnchorAndMarginPreset((Control)(object)panel, (LayoutPreset)3, (LayoutPresetMode)0, margin);
			LayoutContainer.SetMarginBottom((Control)(object)panel, defaultBottomOffset);
		}
		else
		{
			LayoutContainer.SetAnchorAndMarginPreset((Control)(object)panel, (LayoutPreset)1, (LayoutPresetMode)0, margin);
			LayoutContainer.SetMarginTop((Control)(object)panel, topOffset);
		}
	}
}
