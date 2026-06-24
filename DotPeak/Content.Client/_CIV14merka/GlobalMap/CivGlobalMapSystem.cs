// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.GlobalMap.CivGlobalMapSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._CIV14merka.Commander;
using Content.Client._CIV14merka.GlobalMap.UI;
using Content.Client.UserInterface.Screens;
using Content.Shared._CIV14merka.Capture;
using Content.Shared._CIV14merka.Commander;
using Content.Shared._CIV14merka.GlobalMap;
using Content.Shared._CIV14merka.Input;
using Content.Shared._CIV14merka.PurchaseRequest;
using Content.Shared._CIV14merka.Teams;
using Content.Shared._RMC14.Vehicle;
using Content.Shared.GameTicking;
using Content.Shared.Ghost;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client._CIV14merka.GlobalMap;

public sealed class CivGlobalMapSystem : EntitySystem
{
  [Dependency]
  private IPlayerManager _player;
  [Dependency]
  private IOverlayManager _overlays;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private VehicleSystem _vehicles;
  [Dependency]
  private IPrototypeManager _prototypeManager;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private IResourceCache _resourceCache;
  [Dependency]
  private IConfigurationManager _cfg;
  [Dependency]
  private IUserInterfaceManager _ui;
  private static readonly TimeSpan CommanderStatePollInterval = TimeSpan.FromSeconds(1L);
  private readonly List<CivGlobalMapMarkerState> _visibleMarkers = new List<CivGlobalMapMarkerState>();
  private readonly List<CivGlobalMapPlayerState> _visiblePlayers = new List<CivGlobalMapPlayerState>();
  private readonly List<CivPointCapturePointState> _visiblePoints = new List<CivPointCapturePointState>();
  private readonly List<CivCommanderOrderState> _visibleOrders = new List<CivCommanderOrderState>();
  private readonly List<CivGlobalMapDeathState> _visibleDeaths = new List<CivGlobalMapDeathState>();
  private readonly List<CivFobMarkerState> _visibleFobs = new List<CivFobMarkerState>();
  private CivGlobalMapWindow? _window;
  private CivGlobalMapCanvas? _zoneCanvas;
  private CivCommanderWindow? _commanderWindow;
  private CivCommanderShopWindow? _shopWindow;
  private CivGlobalPathOverlay? _overlay;
  private MapId _mapId = MapId.Nullspace;
  private bool _hasBounds;
  private Vector2 _boundsMin;
  private Vector2 _boundsMax;
  private int _teamId;
  private int _squadId;
  private bool _isSquadLeader;
  private bool _isCommander;
  private bool _showAllTeamMatesOnMaps;
  private string _statusLabel = Loc.GetString("civ-gmap-status-round-end");
  private float _roundTimeLeftSeconds;
  private int _team1AliveCount;
  private int _team2AliveCount;
  private int _team1Score;
  private int _team2Score;
  private float _airstrikeCooldown;
  private float _artilleryCooldown;
  private float _smokeSupportCooldown;
  private TimeSpan _nextCommanderStatePoll;
  private int? _commanderSelectedSquadId;
  private CivCommanderState? _commanderState;

  public IReadOnlyList<CivGlobalMapMarkerState> VisibleMarkers
  {
    get => (IReadOnlyList<CivGlobalMapMarkerState>) this._visibleMarkers;
  }

  public IReadOnlyList<CivGlobalMapPlayerState> VisiblePlayers
  {
    get => (IReadOnlyList<CivGlobalMapPlayerState>) this._visiblePlayers;
  }

  public IReadOnlyList<CivPointCapturePointState> VisiblePoints
  {
    get => (IReadOnlyList<CivPointCapturePointState>) this._visiblePoints;
  }

  public IReadOnlyList<CivCommanderOrderState> VisibleOrders
  {
    get => (IReadOnlyList<CivCommanderOrderState>) this._visibleOrders;
  }

  public IReadOnlyList<CivGlobalMapDeathState> VisibleDeaths
  {
    get => (IReadOnlyList<CivGlobalMapDeathState>) this._visibleDeaths;
  }

  public IReadOnlyList<CivFobMarkerState> VisibleFobs
  {
    get => (IReadOnlyList<CivFobMarkerState>) this._visibleFobs;
  }

  public bool ShowAllTeamMatesOnMaps => this._showAllTeamMatesOnMaps;

  public int ViewerTeamId => this._teamId;

  public int ViewerSquadId => this._squadId;

  public bool ViewerIsSquadLeader => this._isSquadLeader;

  public bool HasBounds => this._hasBounds;

  public Vector2 BoundsMin => this._boundsMin;

  public Vector2 BoundsMax => this._boundsMax;

  public bool GlobalMapActive
  {
    get => MapId.op_Inequality(this._mapId, MapId.Nullspace) && this._hasBounds;
  }

  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeNetworkEvent<CivGlobalMapStateEvent>(new EntitySessionEventHandler<CivGlobalMapStateEvent>(this.OnState), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<RoundRestartCleanupEvent>(new EntityEventHandler<RoundRestartCleanupEvent>(this.OnRoundRestart), (Type[]) null, (Type[]) null);
    this.SubscribeLocalEvent<LocalPlayerAttachedEvent>(new EntityEventHandler<LocalPlayerAttachedEvent>(this.OnLocalPlayerAttached), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<CivTeamMemberComponent, OpenCivGlobalMapAlertEvent>(new EntityEventRefHandler<CivTeamMemberComponent, OpenCivGlobalMapAlertEvent>((object) this, __methodptr(OnOpenAlert)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    CommandBinds.Builder.Bind(CivKeyFunctions.CivGlobalMap, InputCmdHandler.FromDelegate(new StateInputCmdDelegate((object) this, __methodptr(\u003CInitialize\u003Eb__70_0)), (StateInputCmdDelegate) null, true, true)).Register<CivGlobalMapSystem>();
    this._overlay = new CivGlobalPathOverlay(this._player, this._transform, this, this._vehicles, this._resourceCache, this._cfg);
    this._overlays.AddOverlay((Overlay) this._overlay);
  }

  public virtual void Shutdown()
  {
    base.Shutdown();
    CommandBinds.Unregister<CivGlobalMapSystem>();
    if (this._zoneCanvas != null)
    {
      this._zoneCanvas.Orphan();
      this._zoneCanvas = (CivGlobalMapCanvas) null;
    }
    if (this._overlay != null)
    {
      this._overlays.RemoveOverlay((Overlay) this._overlay);
      this._overlay = (CivGlobalPathOverlay) null;
    }
    if (this._window != null)
    {
      ((Control) this._window).Dispose();
      this._window = (CivGlobalMapWindow) null;
    }
    if (this._commanderWindow != null)
    {
      ((Control) this._commanderWindow).Dispose();
      this._commanderWindow = (CivCommanderWindow) null;
    }
    if (this._shopWindow == null)
      return;
    ((Control) this._shopWindow).Dispose();
    this._shopWindow = (CivCommanderShopWindow) null;
  }

  public virtual void Update(float frameTime)
  {
    base.Update(frameTime);
    this.UpdateZoneCanvas();
    if (!this._isCommander)
      return;
    bool isActive = this.EntityManager.System<CivCommanderPurchasePlacementSystem>().IsActive;
    CivCommanderShopWindow shopWindow = this._shopWindow;
    if (((shopWindow != null ? (shopWindow.IsOpen ? 1 : 0) : 0) | (isActive ? 1 : 0)) == 0 || this._timing.CurTime < this._nextCommanderStatePoll)
      return;
    this._nextCommanderStatePoll = this._timing.CurTime + CivGlobalMapSystem.CommanderStatePollInterval;
    this.RequestState();
  }

  private void OnOpenAlert(Entity<CivTeamMemberComponent> ent, ref OpenCivGlobalMapAlertEvent args)
  {
    if (args.Handled)
      return;
    args.Handled = true;
    this.EnsureMapWindow();
    CivGlobalMapWindow window = this._window;
    if (window == null)
      return;
    if (!((BaseWindow) window).IsOpen)
      ((BaseWindow) window).OpenCentered();
    this.RefreshWindows();
    this.RequestState();
  }

  private void OnState(CivGlobalMapStateEvent msg, EntitySessionEventArgs args)
  {
    this._mapId = msg.MapId;
    this._hasBounds = msg.HasBounds;
    this._boundsMin = msg.BoundsMin;
    this._boundsMax = msg.BoundsMax;
    this._teamId = msg.TeamId;
    this._squadId = msg.SquadId;
    this._isSquadLeader = msg.IsSquadLeader;
    this._isCommander = msg.IsCommander;
    this._showAllTeamMatesOnMaps = msg.ShowAllTeamMatesOnMaps;
    this._statusLabel = msg.StatusLabel;
    this._roundTimeLeftSeconds = msg.RoundTimeLeftSeconds;
    this._team1AliveCount = msg.Team1AliveCount;
    this._team2AliveCount = msg.Team2AliveCount;
    this._team1Score = msg.Team1Score;
    this._team2Score = msg.Team2Score;
    this._airstrikeCooldown = msg.AirstrikeCooldown;
    this._artilleryCooldown = msg.ArtilleryCooldown;
    this._smokeSupportCooldown = msg.SmokeSupportCooldown;
    this._commanderState = msg.CommanderState;
    if (this._isCommander)
    {
      CivAirstrikeSystem civAirstrikeSystem = this.EntityManager.System<CivAirstrikeSystem>();
      civAirstrikeSystem.UpdateCooldown(this._airstrikeCooldown);
      civAirstrikeSystem.UpdateArtilleryCooldown(this._artilleryCooldown);
      civAirstrikeSystem.UpdateSmokeCooldown(this._smokeSupportCooldown);
    }
    if (this._commanderState == null || this._commanderState.Squads.Count == 0)
      this._commanderSelectedSquadId = new int?();
    else if (!this._commanderSelectedSquadId.HasValue || !this.HasCommanderSquad(this._commanderSelectedSquadId.Value))
      this._commanderSelectedSquadId = new int?(this._commanderState.Squads[0].SquadId);
    this._visibleMarkers.Clear();
    this._visibleMarkers.AddRange((IEnumerable<CivGlobalMapMarkerState>) msg.Markers);
    this._visiblePlayers.Clear();
    this._visiblePlayers.AddRange((IEnumerable<CivGlobalMapPlayerState>) msg.Players);
    this._visiblePoints.Clear();
    this._visiblePoints.AddRange((IEnumerable<CivPointCapturePointState>) msg.Points);
    this._visibleOrders.Clear();
    this._visibleOrders.AddRange((IEnumerable<CivCommanderOrderState>) msg.Orders);
    this._visibleDeaths.Clear();
    this._visibleDeaths.AddRange((IEnumerable<CivGlobalMapDeathState>) msg.Deaths);
    this._visibleFobs.Clear();
    this._visibleFobs.AddRange((IEnumerable<CivFobMarkerState>) msg.Fobs);
    if (!this._isCommander)
    {
      CivCommanderWindow commanderWindow = this._commanderWindow;
      if (commanderWindow != null && commanderWindow.IsOpen)
        this._commanderWindow.Close();
    }
    if (!this._isCommander)
    {
      CivCommanderShopWindow shopWindow = this._shopWindow;
      if (shopWindow != null && shopWindow.IsOpen)
        this._shopWindow.Close();
    }
    this.RefreshWindows();
  }

  private void OnRoundRestart(RoundRestartCleanupEvent ev) => this.ResetCivMapState();

  private void OnLocalPlayerAttached(LocalPlayerAttachedEvent ev)
  {
    if (this.HasComp<CivTeamMemberComponent>(ev.Entity) || this.HasComp<GhostComponent>(ev.Entity))
      return;
    this.ResetCivMapState();
  }

  private void ResetCivMapState()
  {
    this._mapId = MapId.Nullspace;
    this._hasBounds = false;
    this._boundsMin = Vector2.Zero;
    this._boundsMax = Vector2.Zero;
    this._visibleMarkers.Clear();
    this._visiblePlayers.Clear();
    this._visiblePoints.Clear();
    this._visibleOrders.Clear();
    this._visibleDeaths.Clear();
    this._visibleFobs.Clear();
    CivGlobalMapCanvas zoneCanvas = this._zoneCanvas;
    if (zoneCanvas == null || zoneCanvas.Disposed || zoneCanvas.Parent == null)
      return;
    if (this._ui.ActiveScreen is BattlefrontGameScreen activeScreen)
      activeScreen.ClearMapZoneContent((Control) zoneCanvas);
    else
      zoneCanvas.Orphan();
  }

  public void RequestState()
  {
    this.RaiseNetworkEvent((EntityEventArgs) new CivGlobalMapRequestStateEvent());
  }

  public void RequestPlaceMarker(CivGlobalMapMarkerType type, MapId mapId, Vector2 position)
  {
    if (MapId.op_Equality(mapId, MapId.Nullspace))
      return;
    this.RaiseNetworkEvent((EntityEventArgs) new CivGlobalMapPlaceMarkerRequestEvent(type, mapId, position));
  }

  public void RequestRemoveMarker(int markerId)
  {
    if (markerId <= 0)
      return;
    this.RaiseNetworkEvent((EntityEventArgs) new CivGlobalMapRemoveMarkerRequestEvent(markerId));
  }

  public void RequestMoveMarker(int markerId, MapId mapId, Vector2 position)
  {
    if (markerId <= 0 || MapId.op_Equality(mapId, MapId.Nullspace))
      return;
    this.RaiseNetworkEvent((EntityEventArgs) new CivGlobalMapMoveMarkerRequestEvent(markerId, mapId, position));
  }

  public void RequestCommanderMovePlayer(
    NetUserId userId,
    int destinationSquadId,
    bool createNewSquad = false)
  {
    this.RaiseNetworkEvent((EntityEventArgs) new CivCommanderMovePlayerRequestEvent(userId, destinationSquadId, createNewSquad));
  }

  public void RequestCommanderMoveToPosition(MapId mapId, Vector2 position)
  {
    if (MapId.op_Equality(mapId, MapId.Nullspace))
      return;
    this.RaiseNetworkEvent((EntityEventArgs) new CivCommanderMoveToPositionRequestEvent(mapId, position));
  }

  public void RequestCommanderSetOrder(
    int squadId,
    CivCommanderOrderType order,
    MapId mapId,
    Vector2 position)
  {
    this.RaiseNetworkEvent((EntityEventArgs) new CivCommanderSetSquadOrderRequestEvent(squadId, order, mapId, position));
  }

  public void RequestCommanderCallArtillery(MapId mapId, Vector2 position)
  {
    if (MapId.op_Equality(mapId, MapId.Nullspace))
      return;
    this.RaiseNetworkEvent((EntityEventArgs) new CivCommanderCallArtilleryRequestEvent(mapId, position));
  }

  public void RequestCommanderRecon(Vector2 position)
  {
    this.RaiseNetworkEvent((EntityEventArgs) new CivReconDroneRequestEvent(position));
  }

  public void RequestCommanderCallHeli()
  {
    this.RaiseNetworkEvent((EntityEventArgs) new CivCommanderCallHeliRequestEvent());
  }

  public void RequestCommanderShopPurchase(string entryId)
  {
    this.RaiseNetworkEvent((EntityEventArgs) new CivCommanderShopPurchaseRequestEvent(entryId));
  }

  public void RequestCommanderPlacePurchase(string entryId, MapId mapId, Vector2 position)
  {
    if (MapId.op_Equality(mapId, MapId.Nullspace) || string.IsNullOrWhiteSpace(entryId))
      return;
    this.RaiseNetworkEvent((EntityEventArgs) new CivCommanderPlacePurchaseRequestEvent(entryId, mapId, position));
  }

  public void ApprovePurchaseRequest(string requestId)
  {
    this.RaiseNetworkEvent((EntityEventArgs) new PurchaseRequestApproveEvent(Guid.Parse(requestId)));
  }

  public void DenyPurchaseRequest(string requestId)
  {
    this.RaiseNetworkEvent((EntityEventArgs) new PurchaseRequestDenyEvent(Guid.Parse(requestId)));
  }

  public void RequestCommanderMoveBot(NetEntity bot, MapId mapId, Vector2 position)
  {
    if (NetEntity.op_Equality(bot, NetEntity.Invalid) || MapId.op_Equality(mapId, MapId.Nullspace))
      return;
    this.RaiseNetworkEvent((EntityEventArgs) new CivCommanderBotMoveRequestEvent(bot, mapId, position));
  }

  public bool TryStartCommanderShopPlacement(string entryId)
  {
    CivCommanderShopEntryPrototype shopEntryPrototype;
    if (this._commanderState == null || string.IsNullOrWhiteSpace(entryId) || !this._prototypeManager.TryIndex<CivCommanderShopEntryPrototype>(entryId, ref shopEntryPrototype) || shopEntryPrototype.Kind != CivCommanderShopEntryKind.EntityPlacement || !shopEntryPrototype.EntityPrototype.HasValue)
      return false;
    CivCommanderShopEntryState commanderShopEntryState = this._commanderState.ShopEntries.Find((Predicate<CivCommanderShopEntryState>) (candidate => candidate.EntryId == entryId));
    if (commanderShopEntryState == null || commanderShopEntryState.PurchaseLimitPerTeam > 0 && commanderShopEntryState.PurchasedCount >= commanderShopEntryState.PurchaseLimitPerTeam)
      return false;
    this.EntityManager.System<CivCommanderPurchasePlacementSystem>().BeginPlacement(entryId, shopEntryPrototype.Name, shopEntryPrototype.EntityPrototype.Value, commanderShopEntryState.Price, shopEntryPrototype.KeepPlacing);
    this.CloseCommanderShopWindow();
    return true;
  }

  public int? GetCommanderSelectedSquadId() => this._commanderSelectedSquadId;

  public bool TryGetCommanderSelectedSquadId(out int squadId)
  {
    int? commanderSelectedSquadId = this._commanderSelectedSquadId;
    if (commanderSelectedSquadId.HasValue)
    {
      int valueOrDefault = commanderSelectedSquadId.GetValueOrDefault();
      if (this.HasCommanderSquad(valueOrDefault))
      {
        squadId = valueOrDefault;
        return true;
      }
    }
    squadId = 0;
    return false;
  }

  public void SetCommanderSelectedSquad(int squadId)
  {
    if (!this.HasCommanderSquad(squadId))
      return;
    int? commanderSelectedSquadId = this._commanderSelectedSquadId;
    int num = squadId;
    if (commanderSelectedSquadId.GetValueOrDefault() == num & commanderSelectedSquadId.HasValue)
      return;
    this._commanderSelectedSquadId = new int?(squadId);
    this.RefreshWindows();
  }

  public void OpenCommanderWindow(int? focusSquadId = null)
  {
    if (focusSquadId.HasValue)
      this.SetCommanderSelectedSquad(focusSquadId.GetValueOrDefault());
    if (!this._isCommander)
    {
      EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
      CivTeamMemberComponent teamMemberComponent;
      if (!localEntity.HasValue || !this.TryComp<CivTeamMemberComponent>(localEntity.GetValueOrDefault(), ref teamMemberComponent) || !teamMemberComponent.IsCommander)
        return;
    }
    CivCommanderWindow civCommanderWindow = this._commanderWindow ?? (this._commanderWindow = new CivCommanderWindow(this));
    civCommanderWindow.UpdateState(this._commanderState, this._commanderSelectedSquadId);
    if (!civCommanderWindow.IsOpen)
      civCommanderWindow.OpenCentered();
    this.RequestState();
  }

  public void OpenCommanderShopWindow()
  {
    if (!this._isCommander)
    {
      EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
      CivTeamMemberComponent teamMemberComponent;
      if (!localEntity.HasValue || !this.TryComp<CivTeamMemberComponent>(localEntity.GetValueOrDefault(), ref teamMemberComponent) || !teamMemberComponent.IsCommander)
        return;
    }
    CivCommanderShopWindow commanderShopWindow = this._shopWindow ?? (this._shopWindow = new CivCommanderShopWindow(this));
    commanderShopWindow.UpdateState(this._commanderState);
    if (!commanderShopWindow.IsOpen)
      commanderShopWindow.OpenCentered();
    this.RequestState();
  }

  public void CloseCommanderWindow() => this._commanderWindow?.Close();

  public void CloseCommanderShopWindow() => this._shopWindow?.Close();

  public CivCommanderState? GetCommanderState() => this._commanderState;

  public int GetCommanderCurrency()
  {
    CivCommanderState commanderState = this._commanderState;
    return commanderState == null ? 0 : commanderState.Currency;
  }

  public float GetAirstrikeCooldown() => this._airstrikeCooldown;

  public float GetArtilleryCooldown() => this._artilleryCooldown;

  public float GetSmokeSupportCooldown() => this._smokeSupportCooldown;

  public bool TryGetCommanderShopEntryPrice(string entryId, out int price)
  {
    price = 0;
    if (this._commanderState == null || string.IsNullOrWhiteSpace(entryId))
      return false;
    foreach (CivCommanderShopEntryState shopEntry in this._commanderState.ShopEntries)
    {
      if (!(shopEntry.EntryId != entryId))
      {
        price = shopEntry.Price;
        return true;
      }
    }
    return false;
  }

  private void OpenWindow()
  {
    this.EnsureMapWindow();
    CivGlobalMapWindow window = this._window;
    if (window == null)
      return;
    if (((BaseWindow) window).IsOpen)
    {
      ((BaseWindow) window).Close();
    }
    else
    {
      ((BaseWindow) window).OpenCentered();
      this.RefreshWindows();
    }
  }

  private void EnsureMapWindow()
  {
    if (this._window != null)
      return;
    this._window = new CivGlobalMapWindow(this);
  }

  private void UpdateZoneCanvas()
  {
    CivGlobalMapCanvas zoneCanvas1 = this._zoneCanvas;
    if (zoneCanvas1 != null && zoneCanvas1.Disposed)
      this._zoneCanvas = (CivGlobalMapCanvas) null;
    if (!(this._ui.ActiveScreen is BattlefrontGameScreen activeScreen1) || !this.GlobalMapActive)
    {
      CivGlobalMapCanvas zoneCanvas2 = this._zoneCanvas;
      if (zoneCanvas2 == null || zoneCanvas2.Parent == null || !(this._ui.ActiveScreen is BattlefrontGameScreen activeScreen))
        return;
      activeScreen.ClearMapZoneContent((Control) zoneCanvas2);
    }
    else
    {
      if (this._zoneCanvas == null)
      {
        CivGlobalMapCanvas civGlobalMapCanvas = new CivGlobalMapCanvas(this);
        civGlobalMapCanvas.MinSize = Vector2.Zero;
        this._zoneCanvas = civGlobalMapCanvas;
        this.FeedZoneCanvas();
      }
      if (this._zoneCanvas.Parent != null)
        return;
      activeScreen1.SetMapZoneContent((Control) this._zoneCanvas);
    }
  }

  private void FeedZoneCanvas()
  {
    this._zoneCanvas?.UpdateData(this._mapId, this._hasBounds, this._boundsMin, this._boundsMax, this._teamId, this._squadId, (IReadOnlyList<CivGlobalMapMarkerState>) this._visibleMarkers, (IReadOnlyList<CivGlobalMapPlayerState>) this._visiblePlayers, (IReadOnlyList<CivPointCapturePointState>) this._visiblePoints, (IReadOnlyList<CivCommanderOrderState>) this._visibleOrders, (IReadOnlyList<CivGlobalMapDeathState>) this._visibleDeaths, (IReadOnlyList<CivFobMarkerState>) this._visibleFobs, this._isCommander ? this._commanderSelectedSquadId : new int?());
  }

  private bool HasCommanderSquad(int squadId)
  {
    if (this._commanderState == null)
      return false;
    foreach (CivCommanderSquadState squad in this._commanderState.Squads)
    {
      if (squad.SquadId == squadId)
        return true;
    }
    return false;
  }

  private void RefreshWindows()
  {
    this._window?.UpdateState(this._mapId, this._hasBounds, this._boundsMin, this._boundsMax, this._teamId, this._squadId, this._isSquadLeader, this._isCommander, this._statusLabel, this._roundTimeLeftSeconds, this._team1AliveCount, this._team2AliveCount, this._team1Score, this._team2Score, (IReadOnlyList<CivGlobalMapMarkerState>) this._visibleMarkers, (IReadOnlyList<CivGlobalMapPlayerState>) this._visiblePlayers, (IReadOnlyList<CivPointCapturePointState>) this._visiblePoints, (IReadOnlyList<CivCommanderOrderState>) this._visibleOrders, (IReadOnlyList<CivGlobalMapDeathState>) this._visibleDeaths, (IReadOnlyList<CivFobMarkerState>) this._visibleFobs, this._commanderState);
    this._commanderWindow?.UpdateState(this._commanderState, this._commanderSelectedSquadId);
    this._shopWindow?.UpdateState(this._commanderState);
    this.UpdateZoneCanvas();
    this.FeedZoneCanvas();
  }
}
