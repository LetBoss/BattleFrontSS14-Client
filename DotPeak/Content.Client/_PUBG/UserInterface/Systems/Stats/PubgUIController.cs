// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.UserInterface.Systems.Stats.PubgUIController
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

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
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client._PUBG.UserInterface.Systems.Stats;

public sealed class PubgUIController : 
  UIController,
  IOnStateEntered<GameplayState>,
  IOnStateExited<GameplayState>
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

  public virtual void Initialize()
  {
    base.Initialize();
    PubgLoadoutUIController loadout = this._loadout;
    GameplayStateLoadController uiController = this.UIManager.GetUIController<GameplayStateLoadController>();
    uiController.OnScreenLoad += (Action) (() =>
    {
      if (!this._pubgEnabled)
        return;
      this.EnsureLabels();
    });
    uiController.OnScreenUnload += new Action(this.OnScreenUnload);
  }

  public void OnStateEntered(GameplayState state)
  {
    this.EnsureSystemSubscribed();
    if (!this._pubgEnabled)
      return;
    this.EnsureLabels();
  }

  public void OnStateExited(GameplayState state)
  {
    this.UnsubscribeFromSystem();
    this.HideStatsScreen();
  }

  public virtual void FrameUpdate(FrameEventArgs args)
  {
    base.FrameUpdate(args);
    if (this.IsCiv14TdmActive())
      this.HideKillsAndAlivePanels();
    if (this._infoViewport == null || this._infoStack == null)
      return;
    this.PositionInfoStack(this._infoViewport, (Control) this._infoStack, this.GetScreenType());
  }

  private bool IsCiv14TdmActive()
  {
    CivHudEventsSystem civHud = this._civHud;
    return civHud != null && civHud.LastStatus?.Enabled.GetValueOrDefault();
  }

  private void HideKillsAndAlivePanels()
  {
    PanelContainer killsPanel = this._killsPanel;
    if (killsPanel != null && !((Control) killsPanel).Disposed)
      ((Control) killsPanel).Orphan();
    PanelContainer alivePanel = this._alivePanel;
    if (alivePanel != null && !((Control) alivePanel).Disposed)
      ((Control) alivePanel).Orphan();
    this._killsPanel = (PanelContainer) null;
    this._alivePanel = (PanelContainer) null;
    this._killsLabel = (Label) null;
    this._aliveLabel = (Label) null;
  }

  private void EnsureSystemSubscribed()
  {
    if (this._systemSubscribed)
      return;
    this._civHud = this.EntityManager.SystemOrNull<CivHudEventsSystem>();
    PubgHudEventsSystem system = this.EntityManager.System<PubgHudEventsSystem>();
    system.OnPubgModeStatusReceived += new Action<PubgModeStatusEvent>(this.OnPubgStatus);
    system.OnKillsChangedReceived += new Action<PubgKillsChangedEvent>(this.OnKillsChanged);
    system.OnWarmupStatusReceived += new Action<PubgWarmupStatusEvent>(this.OnWarmupStatus);
    system.OnLobbyStatusReceived += new Action<LobbyStatusEvent>(this.OnLobbyStatus);
    system.OnZoneStateReceived += new Action<PubgZoneStateEvent>(this.OnZoneStateUpdate);
    system.OnGameEndReceived += new Action<PubgGameEndEvent>(this.OnGameEnd);
    BattlePassSystem battlePassSystem = this.EntityManager.System<BattlePassSystem>();
    battlePassSystem.OnStateReceived += new Action<BattlePassStateMessage>(this.OnBattlePassState);
    if (this._cachedBattlePassState == null)
      this._cachedBattlePassState = battlePassSystem.LastState;
    this._systemSubscribed = true;
    this.ApplyCachedState(system);
  }

  private void UnsubscribeFromSystem()
  {
    if (!this._systemSubscribed)
      return;
    PubgHudEventsSystem pubgHudEventsSystem = this.EntityManager.SystemOrNull<PubgHudEventsSystem>();
    if (pubgHudEventsSystem != null)
    {
      pubgHudEventsSystem.OnPubgModeStatusReceived -= new Action<PubgModeStatusEvent>(this.OnPubgStatus);
      pubgHudEventsSystem.OnKillsChangedReceived -= new Action<PubgKillsChangedEvent>(this.OnKillsChanged);
      pubgHudEventsSystem.OnWarmupStatusReceived -= new Action<PubgWarmupStatusEvent>(this.OnWarmupStatus);
      pubgHudEventsSystem.OnLobbyStatusReceived -= new Action<LobbyStatusEvent>(this.OnLobbyStatus);
      pubgHudEventsSystem.OnZoneStateReceived -= new Action<PubgZoneStateEvent>(this.OnZoneStateUpdate);
      pubgHudEventsSystem.OnGameEndReceived -= new Action<PubgGameEndEvent>(this.OnGameEnd);
    }
    BattlePassSystem battlePassSystem = this.EntityManager.SystemOrNull<BattlePassSystem>();
    if (battlePassSystem != null)
      battlePassSystem.OnStateReceived -= new Action<BattlePassStateMessage>(this.OnBattlePassState);
    this._civHud = (CivHudEventsSystem) null;
    this._systemSubscribed = false;
  }

  private void ApplyCachedState(PubgHudEventsSystem system)
  {
    if (system.LastPubgModeStatus != null)
      this.OnPubgStatus(system.LastPubgModeStatus);
    if (system.LastLobbyStatus != null)
      this.OnLobbyStatus(system.LastLobbyStatus);
    if (system.LastKillsChanged != null)
      this.OnKillsChanged(system.LastKillsChanged);
    if (system.LastWarmupStatus != null)
      this.OnWarmupStatus(system.LastWarmupStatus);
    if (system.LastZoneState == null)
      return;
    this.OnZoneStateUpdate(system.LastZoneState);
  }

  private void EnsureLabels()
  {
    if (this._inLobby)
      return;
    UIScreen activeScreen = this.UIManager.ActiveScreen;
    if (activeScreen == null)
      return;
    if (activeScreen.GetWidget<MainViewport>() == null)
      return;
    LayoutContainer control;
    try
    {
      control = ((Control) activeScreen).FindControl<LayoutContainer>("ViewportContainer");
    }
    catch (ArgumentException ex)
    {
      return;
    }
    ScreenType screenType = this.GetScreenType();
    Control infoContainer = this.GetInfoContainer(control, screenType);
    if (!this.IsCiv14TdmActive())
    {
      if (this._killsPanel == null)
      {
        (this._killsPanel, this._killsLabel) = this.CreateStyledPanel(Color.FromHex((ReadOnlySpan<char>) "#dc3545", new Color?()), "✖", $"KILLS: {this._kills}");
        infoContainer.AddChild((Control) this._killsPanel);
        if (screenType != ScreenType.Default)
          this.ApplyPanelLayout(this._killsPanel, 60f, -475f, 15);
      }
      if (this._alivePanel == null)
      {
        (this._alivePanel, this._aliveLabel) = this.CreateStyledPanel(Color.FromHex((ReadOnlySpan<char>) "#28a745", new Color?()), "♦", PubgUIController.GetAliveLabelText(0, new int?(), new int?()));
        infoContainer.AddChild((Control) this._alivePanel);
        if (screenType != ScreenType.Default)
          this.ApplyPanelLayout(this._alivePanel, 105f, -430f, 15);
      }
    }
    if (this._zonePanel == null)
    {
      (this._zonePanel, this._zoneLabel) = this.CreateStyledPanel(Color.FromHex((ReadOnlySpan<char>) "#6c757d", new Color?()), "⊙", "ZONE: 120");
      infoContainer.AddChild((Control) this._zonePanel);
      if (screenType != ScreenType.Default)
        this.ApplyPanelLayout(this._zonePanel, 150f, -520f, 25);
      ((Control) this._zonePanel).Visible = false;
    }
    this.PositionInfoStack(control, infoContainer, screenType);
  }

  private (PanelContainer panel, Label label) CreateStyledPanel(
    Color accentColor,
    string icon,
    string text)
  {
    PanelContainer panelContainer = new PanelContainer();
    StyleBoxFlat styleBoxFlat = new StyleBoxFlat()
    {
      BackgroundColor = Color.FromHex((ReadOnlySpan<char>) "#1a1a1aDD", new Color?()),
      BorderColor = accentColor,
      BorderThickness = new Thickness(0.0f, 0.0f, 3f, 0.0f)
    };
    ((StyleBox) styleBoxFlat).SetContentMarginOverride((StyleBox.Margin) 15, 8f);
    panelContainer.PanelOverride = (StyleBox) styleBoxFlat;
    ((Control) panelContainer).MinWidth = 150f;
    BoxContainer boxContainer = new BoxContainer()
    {
      Orientation = (BoxContainer.LayoutOrientation) 0,
      SeparationOverride = new int?(8)
    };
    Label label1 = new Label();
    label1.Text = icon;
    label1.FontColorOverride = new Color?(accentColor);
    ((Control) label1).MinWidth = 20f;
    Label label2 = label1;
    Label label3 = new Label()
    {
      Text = text,
      FontColorOverride = new Color?(Color.White)
    };
    ((Control) boxContainer).AddChild((Control) label2);
    ((Control) boxContainer).AddChild((Control) label3);
    ((Control) panelContainer).AddChild((Control) boxContainer);
    return (panelContainer, label3);
  }

  private void OnPubgStatus(PubgModeStatusEvent msg)
  {
    this._pubgEnabled = msg.Enabled;
    if (this._pubgEnabled)
    {
      this.EnsureLabels();
    }
    else
    {
      this.ClearPanels();
      this._kills = 0;
    }
  }

  private void OnKillsChanged(PubgKillsChangedEvent msg)
  {
    this._kills = msg.Kills;
    if (this._killsLabel == null)
      return;
    this._killsLabel.Text = $"KILLS: {this._kills}";
  }

  private void OnWarmupStatus(PubgWarmupStatusEvent msg)
  {
    this.EnsureLabels();
    if (this._aliveLabel == null)
      return;
    this._aliveLabel.Text = PubgUIController.GetAliveLabelText(msg.AlivePlayers, msg.TeamAAlive, msg.TeamBAlive);
  }

  private static string GetAliveLabelText(int alivePlayers, int? teamAAlive, int? teamBAlive)
  {
    return teamAAlive.HasValue && teamBAlive.HasValue ? Loc.GetString("pubg-hud-alive-vs", new (string, object)[2]
    {
      ("teamA", (object) teamAAlive.Value),
      ("teamB", (object) teamBAlive.Value)
    }) : Loc.GetString("pubg-hud-alive", new (string, object)[1]
    {
      ("alive", (object) alivePlayers)
    });
  }

  private void OnLobbyStatus(LobbyStatusEvent msg)
  {
    this._inLobby = msg.InLobby;
    if (this._inLobby)
    {
      this.ClearPanels();
      this._kills = 0;
    }
    else
    {
      if (!this._pubgEnabled)
        return;
      this.EnsureLabels();
    }
  }

  private void OnZoneStateUpdate(PubgZoneStateEvent msg)
  {
    this.EnsureLabels();
    if (this._zoneLabel == null || this._zonePanel == null)
      return;
    if (msg.Active)
    {
      int num1 = (int) msg.TimeRemaining / 60;
      int num2 = (int) msg.TimeRemaining % 60;
      if (msg.State == ZoneState.Waiting)
      {
        this._zoneLabel.Text = $"NEXT ZONE: {num1:D2}:{num2:D2}";
        this.UpdatePanelColor(this._zonePanel, Color.FromHex((ReadOnlySpan<char>) "#6c9bd1", new Color?()));
      }
      else
      {
        this._zoneLabel.Text = $"ZONE: {num1:D2}:{num2:D2}";
        this.UpdatePanelColor(this._zonePanel, Color.FromHex((ReadOnlySpan<char>) "#dc3545", new Color?()));
      }
      ((Control) this._zonePanel).Visible = true;
    }
    else
      ((Control) this._zonePanel).Visible = false;
  }

  private void UpdatePanelColor(PanelContainer panel, Color color)
  {
    if (!(panel.PanelOverride is StyleBoxFlat panelOverride))
      return;
    panelOverride.BorderColor = color;
  }

  private void OnGameEnd(PubgGameEndEvent msg)
  {
    this._pendingBattlePassXpGain = Math.Max(0, msg.XpGained);
    BattlePassSystem battlePassSystem = this.EntityManager.System<BattlePassSystem>();
    if (this._cachedBattlePassState == null)
      this._cachedBattlePassState = battlePassSystem.LastState;
    int? previousPlayerLevel = new int?();
    int? previousPlayerXp = new int?();
    int? previousPlayerMaxXp = new int?();
    if (this._hasLastPlayerProgress)
    {
      previousPlayerLevel = new int?(this._lastPlayerLevel);
      previousPlayerXp = new int?(this._lastPlayerXp);
      previousPlayerMaxXp = new int?(this._lastPlayerMaxXp);
    }
    this.ShowStatsScreen(msg.Placement, msg.Kills, msg.Deaths, msg.DamageDealt, msg.DamageTaken, msg.SurvivalTime, msg.WeaponDamage, msg.CoinsEarned, msg.RatingChange, msg.CurrentRating, msg.NewRating, msg.NewCoins, msg.KillerUsername, msg.KillerRank, msg.CompletedTasks, msg.XpGained, msg.PlayerLevel, msg.PlayerXp, msg.PlayerMaxXp, msg.IsPartyMode, msg.PartyStats, this._cachedBattlePassState, previousPlayerLevel, previousPlayerXp, previousPlayerMaxXp);
    battlePassSystem.RequestBattlePass();
    this._lastPlayerLevel = Math.Max(1, msg.PlayerLevel);
    this._lastPlayerMaxXp = Math.Max(1, msg.PlayerMaxXp);
    this._lastPlayerXp = Math.Clamp(msg.PlayerXp, 0, this._lastPlayerMaxXp);
    this._hasLastPlayerProgress = true;
  }

  private void ShowStatsScreen(
    int placement,
    int kills,
    int deaths,
    int damageDealt,
    int damageTaken,
    int survivalTime,
    Dictionary<string, int> weaponDamage,
    int coinsEarned,
    int ratingChange,
    int currentRating,
    int newRating,
    int newCoins,
    string? killerUsername = null,
    string killerRank = "N",
    List<BattlePassTaskInfo>? completedTasks = null,
    int xpGained = 0,
    int playerLevel = 1,
    int playerXp = 0,
    int playerMaxXp = 100,
    bool isPartyMode = false,
    List<PubgPartyStatsEntry>? partyStats = null,
    BattlePassStateMessage? battlePassState = null,
    int? previousPlayerLevel = null,
    int? previousPlayerXp = null,
    int? previousPlayerMaxXp = null)
  {
    if (this._statsScreen != null)
    {
      this._statsScreen.SetStats(placement, kills, deaths, damageDealt, damageTaken, survivalTime, weaponDamage, coinsEarned, ratingChange, currentRating, newRating, newCoins, killerUsername, killerRank, completedTasks, xpGained, playerLevel, playerXp, playerMaxXp, isPartyMode, partyStats, battlePassState, previousPlayerLevel, previousPlayerXp, previousPlayerMaxXp);
    }
    else
    {
      UIScreen activeScreen = this.UIManager.ActiveScreen;
      if (activeScreen == null)
        return;
      this._statsScreen = new PubgStatsScreen();
      this._statsScreen.SetStats(placement, kills, deaths, damageDealt, damageTaken, survivalTime, weaponDamage, coinsEarned, ratingChange, currentRating, newRating, newCoins, killerUsername, killerRank, completedTasks, xpGained, playerLevel, playerXp, playerMaxXp, isPartyMode, partyStats, battlePassState, previousPlayerLevel, previousPlayerXp, previousPlayerMaxXp);
      this._statsScreen.OnContinue += new Action(this.HideStatsScreen);
      ((Control) activeScreen).AddChild((Control) this._statsScreen);
      LayoutContainer.SetAnchorAndMarginPreset((Control) this._statsScreen, (LayoutContainer.LayoutPreset) 15, (LayoutContainer.LayoutPresetMode) 0, 0);
    }
  }

  private void OnBattlePassState(BattlePassStateMessage msg)
  {
    this._cachedBattlePassState = msg;
    if (this._statsScreen == null)
      return;
    this._statsScreen.UpdateBattlePassState(msg, this._pendingBattlePassXpGain);
    this._pendingBattlePassXpGain = 0;
  }

  private void HideStatsScreen()
  {
    if (this._statsScreen != null)
    {
      if (!this._statsScreen.Disposed)
        this._statsScreen.Orphan();
      this._statsScreen = (PubgStatsScreen) null;
    }
    this._pendingBattlePassXpGain = 0;
  }

  private void OnScreenUnload()
  {
    this.ClearPanels();
    this.UnsubscribeInfoViewport();
    BoxContainer infoStack = this._infoStack;
    if (infoStack != null && !((Control) infoStack).Disposed)
      ((Control) infoStack).Orphan();
    this._infoStack = (BoxContainer) null;
  }

  private void ClearPanels()
  {
    PanelContainer killsPanel = this._killsPanel;
    if (killsPanel != null && !((Control) killsPanel).Disposed)
      ((Control) killsPanel).Orphan();
    PanelContainer alivePanel = this._alivePanel;
    if (alivePanel != null && !((Control) alivePanel).Disposed)
      ((Control) alivePanel).Orphan();
    PanelContainer zonePanel = this._zonePanel;
    if (zonePanel != null && !((Control) zonePanel).Disposed)
      ((Control) zonePanel).Orphan();
    this._killsPanel = (PanelContainer) null;
    this._alivePanel = (PanelContainer) null;
    this._zonePanel = (PanelContainer) null;
    this._killsLabel = (Label) null;
    this._aliveLabel = (Label) null;
    this._zoneLabel = (Label) null;
  }

  private ScreenType GetScreenType()
  {
    ScreenType result;
    return !Enum.TryParse<ScreenType>(this._cfg.GetCVar<string>(CCVars.UILayout), out result) ? ScreenType.Default : result;
  }

  private Control GetInfoContainer(LayoutContainer viewportContainer, ScreenType screenType)
  {
    if (screenType != ScreenType.Default)
      return (Control) viewportContainer;
    bool flag = true;
    BoxContainer infoContainer;
    try
    {
      infoContainer = ((Control) viewportContainer).FindControl<BoxContainer>("PubgHudInfoStack");
    }
    catch (ArgumentException ex)
    {
      flag = false;
      BoxContainer boxContainer = new BoxContainer();
      ((Control) boxContainer).Name = "PubgHudInfoStack";
      boxContainer.Orientation = (BoxContainer.LayoutOrientation) 1;
      boxContainer.SeparationOverride = new int?(8);
      ((Control) boxContainer).MinSize = new Vector2(200f, 0.0f);
      ((Control) boxContainer).HorizontalAlignment = (Control.HAlignment) 3;
      infoContainer = boxContainer;
      ((Control) viewportContainer).AddChild((Control) infoContainer);
    }
    if (flag)
    {
      if (!((Control) infoContainer).Disposed)
      {
        ((Control) infoContainer).Orphan();
        ((Control) viewportContainer).AddChild((Control) infoContainer);
      }
      else
      {
        BoxContainer boxContainer = new BoxContainer();
        ((Control) boxContainer).Name = "PubgHudInfoStack";
        boxContainer.Orientation = (BoxContainer.LayoutOrientation) 1;
        boxContainer.SeparationOverride = new int?(8);
        ((Control) boxContainer).MinSize = new Vector2(200f, 0.0f);
        ((Control) boxContainer).HorizontalAlignment = (Control.HAlignment) 3;
        infoContainer = boxContainer;
        ((Control) viewportContainer).AddChild((Control) infoContainer);
      }
    }
    this._infoStack = infoContainer;
    this.EnsureInfoViewport(viewportContainer);
    return (Control) infoContainer;
  }

  private void PositionInfoStack(
    LayoutContainer viewportContainer,
    Control infoContainer,
    ScreenType screenType)
  {
    if (screenType != ScreenType.Default || !(infoContainer is BoxContainer boxContainer))
      return;
    ((Control) boxContainer).Measure(Vector2Helpers.Infinity);
    Vector2 desiredSize = ((Control) boxContainer).DesiredSize;
    Vector2 viewportSize = this.GetViewportSize(viewportContainer);
    if ((double) viewportSize.X <= 0.0 || (double) viewportSize.Y <= 0.0)
      return;
    float x = (float) ((double) viewportSize.X - (double) desiredSize.X - 15.0);
    float y = (float) ((double) viewportSize.Y - 220.0 - 200.0 - 10.0 - (double) desiredSize.Y + 200.0);
    if ((double) x < 15.0)
      x = 15f;
    if ((double) y < 15.0)
      y = 15f;
    LayoutContainer.SetAnchorAndMarginPreset((Control) boxContainer, (LayoutContainer.LayoutPreset) 0, (LayoutContainer.LayoutPresetMode) 0, 0);
    LayoutContainer.SetPosition((Control) boxContainer, new Vector2(x, y));
  }

  private Vector2 GetViewportSize(LayoutContainer viewportContainer)
  {
    Vector2 viewportSize = ((Control) this.UIManager.RootControl).Size;
    if ((double) viewportSize.X <= 0.0 || (double) viewportSize.Y <= 0.0)
      viewportSize = ((Control) this.UIManager.StateRoot).Size;
    if ((double) viewportSize.X <= 0.0 || (double) viewportSize.Y <= 0.0)
      viewportSize = ((Control) viewportContainer).Size;
    if ((double) viewportSize.X <= 0.0 || (double) viewportSize.Y <= 0.0)
    {
      UIScreen activeScreen = this.UIManager.ActiveScreen;
      if (activeScreen != null)
      {
        MainViewport widget = activeScreen.GetWidget<MainViewport>();
        viewportSize = widget != null ? ((Control) widget).Size : Vector2.Zero;
      }
    }
    if ((double) viewportSize.X <= 0.0 || (double) viewportSize.Y <= 0.0)
    {
      Control parent = ((Control) viewportContainer).Parent;
      viewportSize = parent != null ? parent.Size : Vector2.Zero;
    }
    if ((double) viewportSize.X <= 0.0 || (double) viewportSize.Y <= 0.0)
    {
      UIScreen activeScreen = this.UIManager.ActiveScreen;
      viewportSize = activeScreen != null ? ((Control) activeScreen).Size : Vector2.Zero;
    }
    return viewportSize;
  }

  private void EnsureInfoViewport(LayoutContainer viewportContainer)
  {
    if (this._infoViewport == viewportContainer && this._infoViewportSubscribed)
      return;
    this.UnsubscribeInfoViewport();
    this._infoViewport = viewportContainer;
    ((Control) this._infoViewport).OnResized += new Action(this.OnInfoViewportResized);
    this._infoViewportSubscribed = true;
  }

  private void UnsubscribeInfoViewport()
  {
    if (this._infoViewport != null && this._infoViewportSubscribed)
      ((Control) this._infoViewport).OnResized -= new Action(this.OnInfoViewportResized);
    this._infoViewport = (LayoutContainer) null;
    this._infoViewportSubscribed = false;
  }

  private void OnInfoViewportResized()
  {
    if (this._infoViewport == null || this._infoStack == null)
      return;
    this.PositionInfoStack(this._infoViewport, (Control) this._infoStack, this.GetScreenType());
  }

  private void ApplyPanelLayout(
    PanelContainer panel,
    float topOffset,
    float defaultBottomOffset,
    int margin)
  {
    if (this.GetScreenType() == ScreenType.Default)
    {
      LayoutContainer.SetAnchorAndMarginPreset((Control) panel, (LayoutContainer.LayoutPreset) 3, (LayoutContainer.LayoutPresetMode) 0, margin);
      LayoutContainer.SetMarginBottom((Control) panel, defaultBottomOffset);
    }
    else
    {
      LayoutContainer.SetAnchorAndMarginPreset((Control) panel, (LayoutContainer.LayoutPreset) 1, (LayoutContainer.LayoutPresetMode) 0, margin);
      LayoutContainer.SetMarginTop((Control) panel, topOffset);
    }
  }
}
