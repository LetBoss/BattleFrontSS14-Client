// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.UserInterface.Systems.Minimap.PubgMinimapUIController
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._CIV14merka.GlobalMap;
using Content.Client._PUBG.Airdrop;
using Content.Client._PUBG.Party;
using Content.Client._PUBG.RespawnTowers;
using Content.Client.Gameplay;
using Content.Client.UserInterface.Controls;
using Content.Client.UserInterface.Screens;
using Content.Client.UserInterface.Systems.Gameplay;
using Content.Shared._CIV14merka.Capture;
using Content.Shared._CIV14merka.Commander;
using Content.Shared._CIV14merka.GlobalMap;
using Content.Shared._CIV14merka.Teams;
using Content.Shared._PUBG.Party;
using Content.Shared.CCVar;
using Content.Shared.Ghost;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client._PUBG.UserInterface.Systems.Minimap;

public sealed class PubgMinimapUIController : 
  UIController,
  IOnStateEntered<GameplayState>,
  IOnStateExited<GameplayState>
{
  private const float DefaultMinimapSize = 200f;
  private const int MinimapAnchorMargin = 15;
  private const float DefaultLayoutBottomMargin = -220f;
  private const float DefaultLayoutTopMargin = 200f;
  private const float DefaultLayoutRightMargin = -15f;
  private const int MinWidgetSize = 120;
  private const int MaxWidgetSize = 500;
  [Dependency]
  private IPlayerManager _playerManager;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private IConfigurationManager _cfg;
  private bool _minimapVisible;
  private PubgMinimapControl? _minimapControl;
  private PanelContainer? _minimapPanel;
  private PubgMinimapControl? _mapZoneControl;
  private bool _partyMarkersEnabled = true;
  private float _partyMarkersOpacity = 1f;
  private TimeSpan _nextUpdate;
  private const float UpdateInterval = 0.1f;

  public virtual void Initialize()
  {
    base.Initialize();
    GameplayStateLoadController uiController = this.UIManager.GetUIController<GameplayStateLoadController>();
    uiController.OnScreenLoad += (Action) (() =>
    {
      if (!this._minimapVisible)
        return;
      this.EnsureUI();
    });
    uiController.OnScreenUnload += new Action(this.OnScreenUnload);
    this._cfg.OnValueChanged<float>(CCVars.PubgMinimapOpacity, new Action<float>(this.OnOpacityChanged), false);
    this._cfg.OnValueChanged<int>(CCVars.PubgMinimapWidgetSize, new Action<int>(this.OnWidgetSizeChanged), false);
    this._cfg.OnValueChanged<int>(CCVars.PubgMinimapOffsetX, new Action<int>(this.OnPositionChanged), false);
    this._cfg.OnValueChanged<int>(CCVars.PubgMinimapOffsetY, new Action<int>(this.OnPositionChanged), false);
    this._cfg.OnValueChanged<string>(CCVars.UILayout, new Action<string>(this.OnScreenLayoutChanged), false);
    this._cfg.OnValueChanged<bool>(CCVars.PubgPartyMarkersEnabled, new Action<bool>(this.OnPartyMarkersEnabledChanged), true);
    this._cfg.OnValueChanged<float>(CCVars.PubgPartyMarkersOpacity, new Action<float>(this.OnPartyMarkersOpacityChanged), true);
  }

  public void OnStateEntered(GameplayState state)
  {
    if (!this._minimapVisible)
      return;
    this.EnsureUI();
  }

  public void OnStateExited(GameplayState state)
  {
    this.HideMinimap();
    this.ReleaseMapZoneMinimap();
  }

  private void OnOpacityChanged(float _) => this.UpdateMinimapOpacity();

  private void OnWidgetSizeChanged(int _) => this.UpdateMinimapSize();

  private void OnPositionChanged(int _) => this.UpdateMinimapLayout();

  private void OnScreenLayoutChanged(string _) => this.UpdateMinimapLayout();

  private void OnPartyMarkersEnabledChanged(bool enabled) => this._partyMarkersEnabled = enabled;

  private void OnPartyMarkersOpacityChanged(float opacity)
  {
    this._partyMarkersOpacity = Math.Clamp(opacity, 0.0f, 1f);
  }

  private void OnScreenUnload()
  {
    this.ReleaseMapZoneMinimap();
    if (this._minimapPanel == null)
      return;
    if (this._minimapControl != null)
      this._minimapControl.OnMapClick = (Action<MapCoordinates>) null;
    if (!((Control) this._minimapPanel).Disposed)
      ((Control) this._minimapPanel).Orphan();
    this._minimapPanel = (PanelContainer) null;
    this._minimapControl = (PubgMinimapControl) null;
  }

  private ScreenType GetScreenType()
  {
    ScreenType result;
    return !Enum.TryParse<ScreenType>(this._cfg.GetCVar<string>(CCVars.UILayout), out result) ? ScreenType.Default : result;
  }

  private void UpdateMinimapOpacity()
  {
    if (this._minimapPanel == null)
      return;
    float cvar = this._cfg.GetCVar<float>(CCVars.PubgMinimapOpacity);
    PanelContainer minimapPanel = this._minimapPanel;
    Color white = Color.White;
    Color color = ((Color) ref white).WithAlpha(cvar);
    ((Control) minimapPanel).Modulate = color;
  }

  private void UpdateMinimapSize()
  {
    if (this._minimapPanel == null || this._minimapControl == null)
      return;
    int num = Math.Clamp(this._cfg.GetCVar<int>(CCVars.PubgMinimapWidgetSize), 120, 500);
    Vector2 vector2 = new Vector2((float) num, (float) num);
    ((Control) this._minimapControl).SetSize = vector2;
    ((Control) this._minimapControl).MinSize = vector2;
    ((Control) this._minimapControl).MaxSize = vector2;
    ((Control) this._minimapPanel).SetSize = vector2;
    ((Control) this._minimapPanel).MinSize = vector2;
    ((Control) this._minimapPanel).MaxSize = vector2;
  }

  private void UpdateMinimapLayout()
  {
    if (this._minimapPanel == null)
      return;
    int screenType = (int) this.GetScreenType();
    int cvar1 = this._cfg.GetCVar<int>(CCVars.PubgMinimapOffsetX);
    int cvar2 = this._cfg.GetCVar<int>(CCVars.PubgMinimapOffsetY);
    if (screenType == 0)
    {
      LayoutContainer.SetAnchorAndMarginPreset((Control) this._minimapPanel, (LayoutContainer.LayoutPreset) 3, (LayoutContainer.LayoutPresetMode) 0, 15);
      LayoutContainer.SetMarginBottom((Control) this._minimapPanel, (float) cvar2 - 220f);
      LayoutContainer.SetMarginRight((Control) this._minimapPanel, (float) cvar1 - 15f);
    }
    else
    {
      LayoutContainer.SetAnchorAndMarginPreset((Control) this._minimapPanel, (LayoutContainer.LayoutPreset) 1, (LayoutContainer.LayoutPresetMode) 0, 15);
      LayoutContainer.SetMarginTop((Control) this._minimapPanel, 200f + (float) cvar2);
      LayoutContainer.SetMarginRight((Control) this._minimapPanel, (float) cvar1 - 15f);
    }
  }

  public virtual void FrameUpdate(FrameEventArgs args)
  {
    base.FrameUpdate(args);
    EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    if (!localEntity.HasValue)
    {
      if (this._minimapVisible)
        this.HideMinimap();
      this.ReleaseMapZoneMinimap();
    }
    else if (this.UIManager.ActiveScreen is BattlefrontGameScreen activeScreen)
    {
      if (this._minimapVisible)
        this.HideMinimap();
      if (this.EntityManager.System<CivGlobalMapSystem>().GlobalMapActive)
      {
        this.ReleaseMapZoneMinimap();
      }
      else
      {
        PubgMinimapControl control = this.EnsureMapZoneMinimap(activeScreen);
        control.UpdatePlayerPosition(localEntity.Value);
        if (this._timing.CurTime < this._nextUpdate)
          return;
        this._nextUpdate = this._timing.CurTime + TimeSpan.FromSeconds(0.10000000149011612);
        this.FeedMinimap(control, localEntity.Value);
      }
    }
    else
    {
      this.ReleaseMapZoneMinimap();
      if (!this._minimapVisible)
        this.ShowMinimap();
      if (!this._minimapVisible || this._minimapControl == null)
        return;
      this._minimapControl.UpdatePlayerPosition(localEntity.Value);
      if (this._timing.CurTime < this._nextUpdate)
        return;
      this._nextUpdate = this._timing.CurTime + TimeSpan.FromSeconds(0.10000000149011612);
      this.FeedMinimap(this._minimapControl, localEntity.Value);
    }
  }

  private void FeedMinimap(PubgMinimapControl control, EntityUid player)
  {
    control.UpdatePlayerPosition(player);
    PubgMinimapStateSystem minimapStateSystem = this.EntityManager.System<PubgMinimapStateSystem>();
    control.ZoneCurrentCenter = minimapStateSystem.ZoneCurrentCenter;
    control.ZoneCurrentRadius = minimapStateSystem.ZoneCurrentRadius;
    control.ZoneNextCenter = minimapStateSystem.ZoneNextCenter;
    control.ZoneNextRadius = minimapStateSystem.ZoneNextRadius;
    control.ZoneActive = minimapStateSystem.ZoneActive;
    control.ZoneVisible = minimapStateSystem.ZoneVisible;
    control.ZoneMapId = minimapStateSystem.ZoneMapId;
    control.RedZoneActive = minimapStateSystem.RedZoneActive;
    control.RedZoneCenter = minimapStateSystem.RedZoneCenter;
    control.RedZoneRadius = minimapStateSystem.RedZoneRadius;
    PubgPartyClientSystem partyClientSystem = this.EntityManager.System<PubgPartyClientSystem>();
    control.PartyMembers = this._partyMarkersEnabled ? partyClientSystem.Members : (IReadOnlyList<PubgPartyMemberState>) null;
    control.PartyMarkersOpacity = this._partyMarkersOpacity;
    PubgPartyPingClientSystem pingClientSystem = this.EntityManager.System<PubgPartyPingClientSystem>();
    control.ActivePings = pingClientSystem.ActivePings;
    PubgAirdropSystem pubgAirdropSystem = this.EntityManager.System<PubgAirdropSystem>();
    control.AirdropActive = pubgAirdropSystem.Active;
    if (!pubgAirdropSystem.Active)
    {
      control.AirdropCenter = new Vector2?();
      control.AirdropRemainingSeconds = 0;
    }
    else
    {
      control.AirdropCenter = new Vector2?(pubgAirdropSystem.Position);
      control.AirdropRemainingSeconds = pubgAirdropSystem.RemainingSeconds;
      control.AirdropMapId = pubgAirdropSystem.MapId;
    }
    PubgRespawnTowerSystem respawnTowerSystem = this.EntityManager.System<PubgRespawnTowerSystem>();
    control.RespawnTowerMapId = respawnTowerSystem.MapId;
    control.RespawnTowerPositions = respawnTowerSystem.TowerPositions;
    control.ActiveRespawnTowerPositions = respawnTowerSystem.ActiveTowerPositions;
    CivTeamMemberComponent teamMemberComponent;
    if (this.EntityManager.TryGetComponent<CivTeamMemberComponent>(player, ref teamMemberComponent))
    {
      CivGlobalMapSystem civGlobalMapSystem = this.EntityManager.System<CivGlobalMapSystem>();
      control.CivGlobalMapMarkers = civGlobalMapSystem.VisibleMarkers;
      control.CivGlobalMapPlayers = civGlobalMapSystem.VisiblePlayers;
      control.CivGlobalMapPoints = civGlobalMapSystem.VisiblePoints;
      control.CivGlobalMapOrders = civGlobalMapSystem.VisibleOrders;
      control.CivGlobalMapDeaths = civGlobalMapSystem.VisibleDeaths;
      control.CivViewerTeamId = teamMemberComponent.TeamId;
      control.CivViewerSquadId = teamMemberComponent.SquadId;
      control.CivHasBounds = civGlobalMapSystem.HasBounds;
      control.CivBoundsMin = civGlobalMapSystem.BoundsMin;
      control.CivBoundsMax = civGlobalMapSystem.BoundsMax;
    }
    else if (this.EntityManager.HasComponent<GhostComponent>(player))
    {
      CivGlobalMapSystem civGlobalMapSystem = this.EntityManager.System<CivGlobalMapSystem>();
      control.CivGlobalMapMarkers = civGlobalMapSystem.VisibleMarkers;
      control.CivGlobalMapPlayers = civGlobalMapSystem.VisiblePlayers;
      control.CivGlobalMapPoints = civGlobalMapSystem.VisiblePoints;
      control.CivGlobalMapOrders = civGlobalMapSystem.VisibleOrders;
      control.CivGlobalMapDeaths = civGlobalMapSystem.VisibleDeaths;
      control.CivViewerTeamId = civGlobalMapSystem.ViewerTeamId;
      control.CivViewerSquadId = civGlobalMapSystem.ViewerSquadId;
      control.CivHasBounds = civGlobalMapSystem.HasBounds;
      control.CivBoundsMin = civGlobalMapSystem.BoundsMin;
      control.CivBoundsMax = civGlobalMapSystem.BoundsMax;
    }
    else
    {
      control.CivGlobalMapMarkers = (IReadOnlyList<CivGlobalMapMarkerState>) null;
      control.CivGlobalMapPlayers = (IReadOnlyList<CivGlobalMapPlayerState>) null;
      control.CivGlobalMapPoints = (IReadOnlyList<CivPointCapturePointState>) null;
      control.CivGlobalMapOrders = (IReadOnlyList<CivCommanderOrderState>) null;
      control.CivGlobalMapDeaths = (IReadOnlyList<CivGlobalMapDeathState>) null;
      control.CivViewerTeamId = 0;
      control.CivViewerSquadId = 0;
      control.CivHasBounds = false;
    }
  }

  private PubgMinimapControl EnsureMapZoneMinimap(BattlefrontGameScreen screen)
  {
    PubgMinimapControl mapZoneControl = this._mapZoneControl;
    if (mapZoneControl != null && !((Control) mapZoneControl).Disposed)
    {
      if (((Control) this._mapZoneControl).Parent == null)
        screen.SetMapZoneContent((Control) this._mapZoneControl);
      return this._mapZoneControl;
    }
    PubgMinimapControl pubgMinimapControl = new PubgMinimapControl();
    ((Control) pubgMinimapControl).HorizontalExpand = true;
    ((Control) pubgMinimapControl).VerticalExpand = true;
    ((Control) pubgMinimapControl).MinSize = Vector2.Zero;
    ((Control) pubgMinimapControl).SetSize = new Vector2(float.NaN, float.NaN);
    this._mapZoneControl = pubgMinimapControl;
    this._mapZoneControl.OnMapClick = new Action<MapCoordinates>(this.HandleMinimapClick);
    screen.SetMapZoneContent((Control) this._mapZoneControl);
    return this._mapZoneControl;
  }

  private void ReleaseMapZoneMinimap()
  {
    if (this._mapZoneControl == null)
      return;
    if (!((Control) this._mapZoneControl).Disposed && ((Control) this._mapZoneControl).Parent != null)
    {
      if (this.UIManager.ActiveScreen is BattlefrontGameScreen activeScreen)
        activeScreen.ClearMapZoneContent((Control) this._mapZoneControl);
      else
        ((Control) this._mapZoneControl).Orphan();
    }
    this._mapZoneControl.OnMapClick = (Action<MapCoordinates>) null;
    this._mapZoneControl = (PubgMinimapControl) null;
  }

  private void EnsureUI()
  {
    UIScreen activeScreen = this.UIManager.ActiveScreen;
    if (activeScreen == null || activeScreen is BattlefrontGameScreen)
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
    if (this._minimapPanel == null)
    {
      PubgMinimapControl pubgMinimapControl = new PubgMinimapControl();
      ((Control) pubgMinimapControl).MinSize = new Vector2(200f, 200f);
      ((Control) pubgMinimapControl).MaxSize = new Vector2(200f, 200f);
      this._minimapControl = pubgMinimapControl;
      this._minimapControl.OnMapClick = new Action<MapCoordinates>(this.HandleMinimapClick);
      this._minimapPanel = new PanelContainer()
      {
        PanelOverride = (StyleBox) new StyleBoxFlat()
        {
          BackgroundColor = Color.FromHex((ReadOnlySpan<char>) "#000000DD", new Color?()),
          BorderColor = Color.FromHex((ReadOnlySpan<char>) "#ffa500", new Color?()),
          BorderThickness = new Thickness(2f)
        }
      };
      ((Control) this._minimapPanel).AddChild((Control) this._minimapControl);
      ((Control) control).AddChild((Control) this._minimapPanel);
    }
    else if (((Control) this._minimapPanel).Parent != control)
    {
      ((Control) this._minimapPanel).Orphan();
      ((Control) control).AddChild((Control) this._minimapPanel);
    }
    this.UpdateMinimapSize();
    this.UpdateMinimapLayout();
    this.UpdateMinimapOpacity();
  }

  private void HandleMinimapClick(MapCoordinates coords)
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    CivTeamMemberComponent teamMemberComponent;
    if (localEntity.HasValue && this.EntityManager.TryGetComponent<CivTeamMemberComponent>(localEntity.Value, ref teamMemberComponent) && teamMemberComponent.IsCommander)
      this.EntityManager.System<CivGlobalMapSystem>().RequestCommanderMoveToPosition(coords.MapId, coords.Position);
    else
      this.EntityManager.System<PubgPartyPingClientSystem>().QueueMapClick(coords);
  }

  public void ShowMinimap()
  {
    this._minimapVisible = true;
    this.EnsureUI();
  }

  public void HideMinimap()
  {
    this._minimapVisible = false;
    this._nextUpdate = TimeSpan.Zero;
    PanelContainer minimapPanel = this._minimapPanel;
    if (minimapPanel != null && !((Control) minimapPanel).Disposed)
      ((Control) minimapPanel).Orphan();
    this._minimapPanel = (PanelContainer) null;
    this._minimapControl = (PubgMinimapControl) null;
  }
}
