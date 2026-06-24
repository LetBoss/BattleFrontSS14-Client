using System;
using System.Collections.Generic;
using System.Numerics;
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

	public IReadOnlyList<CivGlobalMapMarkerState> VisibleMarkers => _visibleMarkers;

	public IReadOnlyList<CivGlobalMapPlayerState> VisiblePlayers => _visiblePlayers;

	public IReadOnlyList<CivPointCapturePointState> VisiblePoints => _visiblePoints;

	public IReadOnlyList<CivCommanderOrderState> VisibleOrders => _visibleOrders;

	public IReadOnlyList<CivGlobalMapDeathState> VisibleDeaths => _visibleDeaths;

	public IReadOnlyList<CivFobMarkerState> VisibleFobs => _visibleFobs;

	public bool ShowAllTeamMatesOnMaps => _showAllTeamMatesOnMaps;

	public int ViewerTeamId => _teamId;

	public int ViewerSquadId => _squadId;

	public bool ViewerIsSquadLeader => _isSquadLeader;

	public bool HasBounds => _hasBounds;

	public Vector2 BoundsMin => _boundsMin;

	public Vector2 BoundsMax => _boundsMax;

	public bool GlobalMapActive
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			if (_mapId != MapId.Nullspace)
			{
				return _hasBounds;
			}
			return false;
		}
	}

	public override void Initialize()
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Expected O, but got Unknown
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeNetworkEvent<CivGlobalMapStateEvent>((EntitySessionEventHandler<CivGlobalMapStateEvent>)OnState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<RoundRestartCleanupEvent>((EntityEventHandler<RoundRestartCleanupEvent>)OnRoundRestart, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<LocalPlayerAttachedEvent>((EntityEventHandler<LocalPlayerAttachedEvent>)OnLocalPlayerAttached, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CivTeamMemberComponent, OpenCivGlobalMapAlertEvent>((EntityEventRefHandler<CivTeamMemberComponent, OpenCivGlobalMapAlertEvent>)OnOpenAlert, (Type[])null, (Type[])null);
		CommandBinds.Builder.Bind(CivKeyFunctions.CivGlobalMap, InputCmdHandler.FromDelegate((StateInputCmdDelegate)delegate
		{
			OpenWindow();
		}, (StateInputCmdDelegate)null, true, true)).Register<CivGlobalMapSystem>();
		_overlay = new CivGlobalPathOverlay(_player, _transform, this, _vehicles, _resourceCache, _cfg);
		_overlays.AddOverlay((Overlay)(object)_overlay);
	}

	public override void Shutdown()
	{
		((EntitySystem)this).Shutdown();
		CommandBinds.Unregister<CivGlobalMapSystem>();
		if (_zoneCanvas != null)
		{
			((Control)_zoneCanvas).Orphan();
			_zoneCanvas = null;
		}
		if (_overlay != null)
		{
			_overlays.RemoveOverlay((Overlay)(object)_overlay);
			_overlay = null;
		}
		if (_window != null)
		{
			((Control)_window).Dispose();
			_window = null;
		}
		if (_commanderWindow != null)
		{
			((Control)_commanderWindow).Dispose();
			_commanderWindow = null;
		}
		if (_shopWindow != null)
		{
			((Control)_shopWindow).Dispose();
			_shopWindow = null;
		}
	}

	public override void Update(float frameTime)
	{
		((EntitySystem)this).Update(frameTime);
		UpdateZoneCanvas();
		if (_isCommander)
		{
			bool isActive = base.EntityManager.System<CivCommanderPurchasePlacementSystem>().IsActive;
			CivCommanderShopWindow? shopWindow = _shopWindow;
			if (((shopWindow != null && ((BaseWindow)shopWindow).IsOpen) || isActive) && !(_timing.CurTime < _nextCommanderStatePoll))
			{
				_nextCommanderStatePoll = _timing.CurTime + CommanderStatePollInterval;
				RequestState();
			}
		}
	}

	private void OnOpenAlert(Entity<CivTeamMemberComponent> ent, ref OpenCivGlobalMapAlertEvent args)
	{
		if (((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		EnsureMapWindow();
		CivGlobalMapWindow window = _window;
		if (window != null)
		{
			if (!((BaseWindow)window).IsOpen)
			{
				((BaseWindow)window).OpenCentered();
			}
			RefreshWindows();
			RequestState();
		}
	}

	private void OnState(CivGlobalMapStateEvent msg, EntitySessionEventArgs args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_mapId = msg.MapId;
		_hasBounds = msg.HasBounds;
		_boundsMin = msg.BoundsMin;
		_boundsMax = msg.BoundsMax;
		_teamId = msg.TeamId;
		_squadId = msg.SquadId;
		_isSquadLeader = msg.IsSquadLeader;
		_isCommander = msg.IsCommander;
		_showAllTeamMatesOnMaps = msg.ShowAllTeamMatesOnMaps;
		_statusLabel = msg.StatusLabel;
		_roundTimeLeftSeconds = msg.RoundTimeLeftSeconds;
		_team1AliveCount = msg.Team1AliveCount;
		_team2AliveCount = msg.Team2AliveCount;
		_team1Score = msg.Team1Score;
		_team2Score = msg.Team2Score;
		_airstrikeCooldown = msg.AirstrikeCooldown;
		_artilleryCooldown = msg.ArtilleryCooldown;
		_smokeSupportCooldown = msg.SmokeSupportCooldown;
		_commanderState = msg.CommanderState;
		if (_isCommander)
		{
			CivAirstrikeSystem civAirstrikeSystem = base.EntityManager.System<CivAirstrikeSystem>();
			civAirstrikeSystem.UpdateCooldown(_airstrikeCooldown);
			civAirstrikeSystem.UpdateArtilleryCooldown(_artilleryCooldown);
			civAirstrikeSystem.UpdateSmokeCooldown(_smokeSupportCooldown);
		}
		if (_commanderState == null || _commanderState.Squads.Count == 0)
		{
			_commanderSelectedSquadId = null;
		}
		else if (!_commanderSelectedSquadId.HasValue || !HasCommanderSquad(_commanderSelectedSquadId.Value))
		{
			_commanderSelectedSquadId = _commanderState.Squads[0].SquadId;
		}
		_visibleMarkers.Clear();
		_visibleMarkers.AddRange(msg.Markers);
		_visiblePlayers.Clear();
		_visiblePlayers.AddRange(msg.Players);
		_visiblePoints.Clear();
		_visiblePoints.AddRange(msg.Points);
		_visibleOrders.Clear();
		_visibleOrders.AddRange(msg.Orders);
		_visibleDeaths.Clear();
		_visibleDeaths.AddRange(msg.Deaths);
		_visibleFobs.Clear();
		_visibleFobs.AddRange(msg.Fobs);
		if (!_isCommander)
		{
			CivCommanderWindow commanderWindow = _commanderWindow;
			if (commanderWindow != null && ((BaseWindow)commanderWindow).IsOpen)
			{
				((BaseWindow)_commanderWindow).Close();
			}
		}
		if (!_isCommander)
		{
			CivCommanderShopWindow shopWindow = _shopWindow;
			if (shopWindow != null && ((BaseWindow)shopWindow).IsOpen)
			{
				((BaseWindow)_shopWindow).Close();
			}
		}
		RefreshWindows();
	}

	private void OnRoundRestart(RoundRestartCleanupEvent ev)
	{
		ResetCivMapState();
	}

	private void OnLocalPlayerAttached(LocalPlayerAttachedEvent ev)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).HasComp<CivTeamMemberComponent>(ev.Entity) && !((EntitySystem)this).HasComp<GhostComponent>(ev.Entity))
		{
			ResetCivMapState();
		}
	}

	private void ResetCivMapState()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		_mapId = MapId.Nullspace;
		_hasBounds = false;
		_boundsMin = Vector2.Zero;
		_boundsMax = Vector2.Zero;
		_visibleMarkers.Clear();
		_visiblePlayers.Clear();
		_visiblePoints.Clear();
		_visibleOrders.Clear();
		_visibleDeaths.Clear();
		_visibleFobs.Clear();
		CivGlobalMapCanvas zoneCanvas = _zoneCanvas;
		if (zoneCanvas != null && !((Control)zoneCanvas).Disposed && ((Control)zoneCanvas).Parent != null)
		{
			if (_ui.ActiveScreen is BattlefrontGameScreen battlefrontGameScreen)
			{
				battlefrontGameScreen.ClearMapZoneContent((Control)(object)zoneCanvas);
			}
			else
			{
				((Control)zoneCanvas).Orphan();
			}
		}
	}

	public void RequestState()
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new CivGlobalMapRequestStateEvent());
	}

	public void RequestPlaceMarker(CivGlobalMapMarkerType type, MapId mapId, Vector2 position)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (!(mapId == MapId.Nullspace))
		{
			((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new CivGlobalMapPlaceMarkerRequestEvent(type, mapId, position));
		}
	}

	public void RequestRemoveMarker(int markerId)
	{
		if (markerId > 0)
		{
			((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new CivGlobalMapRemoveMarkerRequestEvent(markerId));
		}
	}

	public void RequestMoveMarker(int markerId, MapId mapId, Vector2 position)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if (markerId > 0 && !(mapId == MapId.Nullspace))
		{
			((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new CivGlobalMapMoveMarkerRequestEvent(markerId, mapId, position));
		}
	}

	public void RequestCommanderMovePlayer(NetUserId userId, int destinationSquadId, bool createNewSquad = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new CivCommanderMovePlayerRequestEvent(userId, destinationSquadId, createNewSquad));
	}

	public void RequestCommanderMoveToPosition(MapId mapId, Vector2 position)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (!(mapId == MapId.Nullspace))
		{
			((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new CivCommanderMoveToPositionRequestEvent(mapId, position));
		}
	}

	public void RequestCommanderSetOrder(int squadId, CivCommanderOrderType order, MapId mapId, Vector2 position)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new CivCommanderSetSquadOrderRequestEvent(squadId, order, mapId, position));
	}

	public void RequestCommanderCallArtillery(MapId mapId, Vector2 position)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (!(mapId == MapId.Nullspace))
		{
			((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new CivCommanderCallArtilleryRequestEvent(mapId, position));
		}
	}

	public void RequestCommanderRecon(Vector2 position)
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new CivReconDroneRequestEvent(position));
	}

	public void RequestCommanderCallHeli()
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new CivCommanderCallHeliRequestEvent());
	}

	public void RequestCommanderShopPurchase(string entryId)
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new CivCommanderShopPurchaseRequestEvent(entryId));
	}

	public void RequestCommanderPlacePurchase(string entryId, MapId mapId, Vector2 position)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		if (!(mapId == MapId.Nullspace) && !string.IsNullOrWhiteSpace(entryId))
		{
			((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new CivCommanderPlacePurchaseRequestEvent(entryId, mapId, position));
		}
	}

	public void ApprovePurchaseRequest(string requestId)
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new PurchaseRequestApproveEvent(Guid.Parse(requestId)));
	}

	public void DenyPurchaseRequest(string requestId)
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new PurchaseRequestDenyEvent(Guid.Parse(requestId)));
	}

	public void RequestCommanderMoveBot(NetEntity bot, MapId mapId, Vector2 position)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		if (!(bot == NetEntity.Invalid) && !(mapId == MapId.Nullspace))
		{
			((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new CivCommanderBotMoveRequestEvent(bot, mapId, position));
		}
	}

	public bool TryStartCommanderShopPlacement(string entryId)
	{
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		CivCommanderShopEntryPrototype civCommanderShopEntryPrototype = default(CivCommanderShopEntryPrototype);
		if (_commanderState == null || string.IsNullOrWhiteSpace(entryId) || !_prototypeManager.TryIndex<CivCommanderShopEntryPrototype>(entryId, ref civCommanderShopEntryPrototype) || civCommanderShopEntryPrototype.Kind != CivCommanderShopEntryKind.EntityPlacement || !civCommanderShopEntryPrototype.EntityPrototype.HasValue)
		{
			return false;
		}
		CivCommanderShopEntryState civCommanderShopEntryState = _commanderState.ShopEntries.Find((CivCommanderShopEntryState candidate) => candidate.EntryId == entryId);
		if (civCommanderShopEntryState == null)
		{
			return false;
		}
		if (civCommanderShopEntryState.PurchaseLimitPerTeam > 0 && civCommanderShopEntryState.PurchasedCount >= civCommanderShopEntryState.PurchaseLimitPerTeam)
		{
			return false;
		}
		base.EntityManager.System<CivCommanderPurchasePlacementSystem>().BeginPlacement(entryId, civCommanderShopEntryPrototype.Name, civCommanderShopEntryPrototype.EntityPrototype.Value, civCommanderShopEntryState.Price, civCommanderShopEntryPrototype.KeepPlacing);
		CloseCommanderShopWindow();
		return true;
	}

	public int? GetCommanderSelectedSquadId()
	{
		return _commanderSelectedSquadId;
	}

	public bool TryGetCommanderSelectedSquadId(out int squadId)
	{
		int? commanderSelectedSquadId = _commanderSelectedSquadId;
		if (commanderSelectedSquadId.HasValue)
		{
			int valueOrDefault = commanderSelectedSquadId.GetValueOrDefault();
			if (HasCommanderSquad(valueOrDefault))
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
		if (HasCommanderSquad(squadId) && _commanderSelectedSquadId != squadId)
		{
			_commanderSelectedSquadId = squadId;
			RefreshWindows();
		}
	}

	public void OpenCommanderWindow(int? focusSquadId = null)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if (focusSquadId.HasValue)
		{
			int valueOrDefault = focusSquadId.GetValueOrDefault();
			SetCommanderSelectedSquad(valueOrDefault);
		}
		if (!_isCommander)
		{
			EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
			if (!localEntity.HasValue)
			{
				return;
			}
			EntityUid valueOrDefault2 = localEntity.GetValueOrDefault();
			CivTeamMemberComponent civTeamMemberComponent = default(CivTeamMemberComponent);
			if (!((EntitySystem)this).TryComp<CivTeamMemberComponent>(valueOrDefault2, ref civTeamMemberComponent) || !civTeamMemberComponent.IsCommander)
			{
				return;
			}
		}
		CivCommanderWindow civCommanderWindow = _commanderWindow ?? (_commanderWindow = new CivCommanderWindow(this));
		civCommanderWindow.UpdateState(_commanderState, _commanderSelectedSquadId);
		if (!((BaseWindow)civCommanderWindow).IsOpen)
		{
			((BaseWindow)civCommanderWindow).OpenCentered();
		}
		RequestState();
	}

	public void OpenCommanderShopWindow()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		if (!_isCommander)
		{
			EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
			if (!localEntity.HasValue)
			{
				return;
			}
			EntityUid valueOrDefault = localEntity.GetValueOrDefault();
			CivTeamMemberComponent civTeamMemberComponent = default(CivTeamMemberComponent);
			if (!((EntitySystem)this).TryComp<CivTeamMemberComponent>(valueOrDefault, ref civTeamMemberComponent) || !civTeamMemberComponent.IsCommander)
			{
				return;
			}
		}
		CivCommanderShopWindow civCommanderShopWindow = _shopWindow ?? (_shopWindow = new CivCommanderShopWindow(this));
		civCommanderShopWindow.UpdateState(_commanderState);
		if (!((BaseWindow)civCommanderShopWindow).IsOpen)
		{
			((BaseWindow)civCommanderShopWindow).OpenCentered();
		}
		RequestState();
	}

	public void CloseCommanderWindow()
	{
		CivCommanderWindow? commanderWindow = _commanderWindow;
		if (commanderWindow != null)
		{
			((BaseWindow)commanderWindow).Close();
		}
	}

	public void CloseCommanderShopWindow()
	{
		CivCommanderShopWindow? shopWindow = _shopWindow;
		if (shopWindow != null)
		{
			((BaseWindow)shopWindow).Close();
		}
	}

	public CivCommanderState? GetCommanderState()
	{
		return _commanderState;
	}

	public int GetCommanderCurrency()
	{
		return _commanderState?.Currency ?? 0;
	}

	public float GetAirstrikeCooldown()
	{
		return _airstrikeCooldown;
	}

	public float GetArtilleryCooldown()
	{
		return _artilleryCooldown;
	}

	public float GetSmokeSupportCooldown()
	{
		return _smokeSupportCooldown;
	}

	public bool TryGetCommanderShopEntryPrice(string entryId, out int price)
	{
		price = 0;
		if (_commanderState == null || string.IsNullOrWhiteSpace(entryId))
		{
			return false;
		}
		foreach (CivCommanderShopEntryState shopEntry in _commanderState.ShopEntries)
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
		EnsureMapWindow();
		CivGlobalMapWindow window = _window;
		if (window != null)
		{
			if (((BaseWindow)window).IsOpen)
			{
				((BaseWindow)window).Close();
				return;
			}
			((BaseWindow)window).OpenCentered();
			RefreshWindows();
		}
	}

	private void EnsureMapWindow()
	{
		if (_window == null)
		{
			_window = new CivGlobalMapWindow(this);
		}
	}

	private void UpdateZoneCanvas()
	{
		CivGlobalMapCanvas zoneCanvas = _zoneCanvas;
		if (zoneCanvas != null && ((Control)zoneCanvas).Disposed)
		{
			_zoneCanvas = null;
		}
		if (!(_ui.ActiveScreen is BattlefrontGameScreen battlefrontGameScreen) || !GlobalMapActive)
		{
			CivGlobalMapCanvas zoneCanvas2 = _zoneCanvas;
			if (zoneCanvas2 != null && ((Control)zoneCanvas2).Parent != null && _ui.ActiveScreen is BattlefrontGameScreen battlefrontGameScreen2)
			{
				battlefrontGameScreen2.ClearMapZoneContent((Control)(object)zoneCanvas2);
			}
			return;
		}
		if (_zoneCanvas == null)
		{
			CivGlobalMapCanvas civGlobalMapCanvas = new CivGlobalMapCanvas(this);
			((Control)civGlobalMapCanvas).MinSize = Vector2.Zero;
			_zoneCanvas = civGlobalMapCanvas;
			FeedZoneCanvas();
		}
		if (((Control)_zoneCanvas).Parent == null)
		{
			battlefrontGameScreen.SetMapZoneContent((Control)(object)_zoneCanvas);
		}
	}

	private void FeedZoneCanvas()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		_zoneCanvas?.UpdateData(_mapId, _hasBounds, _boundsMin, _boundsMax, _teamId, _squadId, _visibleMarkers, _visiblePlayers, _visiblePoints, _visibleOrders, _visibleDeaths, _visibleFobs, _isCommander ? _commanderSelectedSquadId : ((int?)null));
	}

	private bool HasCommanderSquad(int squadId)
	{
		if (_commanderState == null)
		{
			return false;
		}
		foreach (CivCommanderSquadState squad in _commanderState.Squads)
		{
			if (squad.SquadId == squadId)
			{
				return true;
			}
		}
		return false;
	}

	private void RefreshWindows()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		_window?.UpdateState(_mapId, _hasBounds, _boundsMin, _boundsMax, _teamId, _squadId, _isSquadLeader, _isCommander, _statusLabel, _roundTimeLeftSeconds, _team1AliveCount, _team2AliveCount, _team1Score, _team2Score, _visibleMarkers, _visiblePlayers, _visiblePoints, _visibleOrders, _visibleDeaths, _visibleFobs, _commanderState);
		_commanderWindow?.UpdateState(_commanderState, _commanderSelectedSquadId);
		_shopWindow?.UpdateState(_commanderState);
		UpdateZoneCanvas();
		FeedZoneCanvas();
	}
}
