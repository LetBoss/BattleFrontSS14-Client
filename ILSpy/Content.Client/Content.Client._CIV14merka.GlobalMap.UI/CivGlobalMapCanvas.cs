using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Client._CIV14merka.Capture;
using Content.Client._CIV14merka.HeliSupply;
using Content.Client.Stylesheets;
using Content.Shared._CIV14merka.Capture;
using Content.Shared._CIV14merka.Commander;
using Content.Shared._CIV14merka.GlobalMap;
using Content.Shared._RMC14.Vehicle;
using Content.Shared.CCVar;
using Content.Shared.Maps;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.Graphics;
using Robust.Shared.Input;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Map.Enumerators;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Content.Client._CIV14merka.GlobalMap.UI;

public sealed class CivGlobalMapCanvas : Control
{
	private readonly record struct MapTileDrawData(Vector2 WorldPosition, Color Color, bool IsWall);

	[Dependency]
	private IResourceCache _resourceCache;

	[Dependency]
	private IEntityManager _entity;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private IClyde _clyde;

	[Dependency]
	private ISharedPlayerManager _player;

	[Dependency]
	private IConfigurationManager _cfg;

	private readonly CivGlobalMapSystem _system;

	private readonly Font _font;

	private readonly Font _gridFont;

	private readonly SharedMapSystem _map;

	private readonly SharedTransformSystem _transform;

	private readonly TurfSystem _turf;

	private readonly VehicleSystem _vehicles;

	private readonly List<CivGlobalMapMarkerState> _markers = new List<CivGlobalMapMarkerState>();

	private readonly List<CivGlobalMapPlayerState> _players = new List<CivGlobalMapPlayerState>();

	private readonly List<CivPointCapturePointState> _points = new List<CivPointCapturePointState>();

	private readonly List<CivFobMarkerState> _fobs = new List<CivFobMarkerState>();

	private readonly List<CivCommanderOrderState> _orders = new List<CivCommanderOrderState>();

	private readonly List<CivGlobalMapDeathState> _deaths = new List<CivGlobalMapDeathState>();

	private readonly List<MapTileDrawData> _tileCache = new List<MapTileDrawData>();

	private readonly List<HeliMapBlip> _heliBlips = new List<HeliMapBlip>();

	private CivHeliSupplySystem? _heli;

	private MapId _mapId = MapId.Nullspace;

	private MapId _tileCacheMapId = MapId.Nullspace;

	private OwnedTexture? _tileTexture;

	private Vector2 _tileTextureWorldMin;

	private Vector2 _tileTextureWorldMax;

	private int _viewerTeamId;

	private int _viewerSquadId;

	private bool _hasBounds;

	private Vector2 _boundsMin;

	private Vector2 _boundsMax;

	private float _zoom = 1f;

	private Vector2? _viewCenter;

	private int _hoveredMarkerId;

	private int _pendingRightClickMarkerId;

	private bool _isDraggingMarker;

	private int _dragMarkerId;

	private Vector2 _dragStartLocalPosition;

	private bool _dragMoved;

	private bool _isPanning;

	private Vector2 _lastPanLocalPosition;

	private readonly Dictionary<string, Vector2> _playerSmoothed = new Dictionary<string, Vector2>();

	private readonly HashSet<string> _playerSeen = new HashSet<string>();

	private readonly List<string> _playerStale = new List<string>();

	private TimeSpan _lastPlayerLerpTime;

	private const float PlayerLerpTau = 0.45f;

	private const float PlayerSnapDist = 25f;

	private const float MinZoom = 1f;

	private const float MaxZoom = 4f;

	private const float ZoomStep = 1.12f;

	private const float MarkerDragThresholdPixels = 6f;

	private static readonly Color WallTileColor = Color.FromHex((ReadOnlySpan<char>)"#8B4513", (Color?)null);

	private static readonly Color EmptyTileColor = new Color((byte)36, (byte)45, (byte)56, byte.MaxValue);

	public CivGlobalMapMarkerType? SelectedMarkerType { get; set; }

	public bool RemoveMode { get; set; }

	public int? CommanderSelectedSquadId { get; set; }

	public int? PendingCommanderOrderSquadId { get; set; }

	public CivCommanderOrderType? PendingCommanderOrderType { get; set; }

	public event Action? CommanderOrderPlaced;

	public CivGlobalMapCanvas(CivGlobalMapSystem system)
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		_system = system;
		IoCManager.InjectDependencies<CivGlobalMapCanvas>(this);
		_font = _resourceCache.NotoStack("Bold");
		_gridFont = _resourceCache.NotoStack("Bold", 14);
		_map = _entity.System<SharedMapSystem>();
		_transform = _entity.System<SharedTransformSystem>();
		_turf = _entity.System<TurfSystem>();
		_vehicles = _entity.System<VehicleSystem>();
		((Control)this).MouseFilter = (MouseFilterMode)0;
		((Control)this).MinSize = new Vector2(300f, 300f);
	}

	public void UpdateData(MapId mapId, bool hasBounds, Vector2 boundsMin, Vector2 boundsMax, int viewerTeamId, int viewerSquadId, IReadOnlyList<CivGlobalMapMarkerState> markers, IReadOnlyList<CivGlobalMapPlayerState> players, IReadOnlyList<CivPointCapturePointState> points, IReadOnlyList<CivCommanderOrderState> orders, IReadOnlyList<CivGlobalMapDeathState> deaths, IReadOnlyList<CivFobMarkerState> fobs, int? commanderSelectedSquadId)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		if (_mapId != mapId)
		{
			_zoom = 1f;
			_viewCenter = null;
			_hoveredMarkerId = 0;
			_pendingRightClickMarkerId = 0;
			_isDraggingMarker = false;
			_dragMarkerId = 0;
			_dragMoved = false;
			_isPanning = false;
		}
		_mapId = mapId;
		_hasBounds = hasBounds;
		_boundsMin = boundsMin;
		_boundsMax = boundsMax;
		_viewerTeamId = viewerTeamId;
		_viewerSquadId = viewerSquadId;
		CommanderSelectedSquadId = commanderSelectedSquadId;
		_markers.Clear();
		_markers.AddRange(markers);
		_players.Clear();
		_players.AddRange(players);
		_points.Clear();
		_points.AddRange(points);
		_orders.Clear();
		_orders.AddRange(orders);
		_deaths.Clear();
		_deaths.AddRange(deaths);
		_fobs.Clear();
		_fobs.AddRange(fobs);
		EnsureViewCenter();
		RefreshTileCache();
	}

	public void CenterOnSelf()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (_hasBounds && !(_mapId == MapId.Nullspace))
		{
			if (TryGetSelfPosition(out var position))
			{
				_viewCenter = position;
			}
			else
			{
				_viewCenter = (_boundsMin + _boundsMax) * 0.5f;
			}
		}
	}

	protected override void KeyBindDown(GUIBoundKeyEventArgs args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		((Control)this).KeyBindDown(args);
		UIBox2 mapRect;
		if (((BoundKeyEventArgs)args).Function == EngineKeyFunctions.UIClick)
		{
			if (!RemoveMode)
			{
				Vector2 vector = ((BoundKeyEventArgs)args).PointerLocation.Position - Vector2i.op_Implicit(((Control)this).GlobalPixelPosition);
				mapRect = GetMapRect();
				if (((UIBox2)(ref mapRect)).Contains(vector, true) && TryGetMarkerAt(vector, out var markerId, includeObjectives: false))
				{
					_isDraggingMarker = true;
					_dragMoved = false;
					_dragMarkerId = markerId;
					_dragStartLocalPosition = vector;
					((BoundKeyEventArgs)args).Handle();
				}
			}
		}
		else
		{
			if (((BoundKeyEventArgs)args).Function != EngineKeyFunctions.UIRightClick)
			{
				return;
			}
			Vector2 vector2 = ((BoundKeyEventArgs)args).PointerLocation.Position - Vector2i.op_Implicit(((Control)this).GlobalPixelPosition);
			mapRect = GetMapRect();
			if (((UIBox2)(ref mapRect)).Contains(vector2, true))
			{
				if (TryGetMarkerAt(vector2, out var markerId2, includeObjectives: false))
				{
					_pendingRightClickMarkerId = markerId2;
					((BoundKeyEventArgs)args).Handle();
					return;
				}
				_pendingRightClickMarkerId = 0;
				_isPanning = true;
				_lastPanLocalPosition = vector2;
				((BoundKeyEventArgs)args).Handle();
			}
		}
	}

	protected override void KeyBindUp(GUIBoundKeyEventArgs args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		((Control)this).KeyBindUp(args);
		if (((BoundKeyEventArgs)args).Function == EngineKeyFunctions.UIRightClick)
		{
			if (_isPanning)
			{
				_isPanning = false;
				((BoundKeyEventArgs)args).Handle();
				return;
			}
			if (_pendingRightClickMarkerId != 0)
			{
				_system.RequestRemoveMarker(_pendingRightClickMarkerId);
				_pendingRightClickMarkerId = 0;
			}
			((BoundKeyEventArgs)args).Handle();
		}
		else
		{
			if (((BoundKeyEventArgs)args).Function != EngineKeyFunctions.UIClick || _isPanning)
			{
				return;
			}
			Vector2 localPosition = ((BoundKeyEventArgs)args).PointerLocation.Position - Vector2i.op_Implicit(((Control)this).GlobalPixelPosition);
			if (_isDraggingMarker)
			{
				if (_dragMoved && _dragMarkerId != 0 && _mapId != MapId.Nullspace && TryLocalToMapPosition(localPosition, out var mapPosition))
				{
					_system.RequestMoveMarker(_dragMarkerId, _mapId, mapPosition);
				}
				_isDraggingMarker = false;
				_dragMarkerId = 0;
				_dragMoved = false;
				((BoundKeyEventArgs)args).Handle();
				return;
			}
			if (RemoveMode)
			{
				if (TryGetMarkerAt(localPosition, out var markerId, includeObjectives: false))
				{
					_system.RequestRemoveMarker(markerId);
				}
				((BoundKeyEventArgs)args).Handle();
				return;
			}
			int? pendingCommanderOrderSquadId = PendingCommanderOrderSquadId;
			if (pendingCommanderOrderSquadId.HasValue)
			{
				int valueOrDefault = pendingCommanderOrderSquadId.GetValueOrDefault();
				CivCommanderOrderType? pendingCommanderOrderType = PendingCommanderOrderType;
				if (pendingCommanderOrderType.HasValue)
				{
					CivCommanderOrderType valueOrDefault2 = pendingCommanderOrderType.GetValueOrDefault();
					if (_mapId != MapId.Nullspace)
					{
						if (TryLocalToMapPosition(localPosition, out var mapPosition2))
						{
							_system.RequestCommanderSetOrder(valueOrDefault, valueOrDefault2, _mapId, mapPosition2);
							PendingCommanderOrderSquadId = null;
							PendingCommanderOrderType = null;
							this.CommanderOrderPlaced?.Invoke();
							((BoundKeyEventArgs)args).Handle();
						}
						return;
					}
				}
			}
			if (SelectedMarkerType.HasValue && !(_mapId == MapId.Nullspace) && TryLocalToMapPosition(localPosition, out var mapPosition3))
			{
				_system.RequestPlaceMarker(SelectedMarkerType.Value, _mapId, mapPosition3);
				((BoundKeyEventArgs)args).Handle();
			}
		}
	}

	protected override void MouseMove(GUIMouseMoveEventArgs args)
	{
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		((Control)this).MouseMove(args);
		Vector2 relativePosition = ((GUIMouseEventArgs)args).RelativePosition;
		UpdateHoveredMarker(relativePosition);
		if (_isDraggingMarker)
		{
			float num = 36f * ((Control)this).UIScale * ((Control)this).UIScale;
			if ((relativePosition - _dragStartLocalPosition).LengthSquared() >= num)
			{
				_dragMoved = true;
			}
			((InputEventArgs)args).Handle();
		}
		else
		{
			if (!_isPanning || !TryGetVisibleBounds(out var visibleMin, out var visibleMax))
			{
				return;
			}
			Vector2 vector = relativePosition - _lastPanLocalPosition;
			_lastPanLocalPosition = relativePosition;
			UIBox2 mapRect = GetMapRect();
			if (!(((UIBox2)(ref mapRect)).Width <= 0f) && !(((UIBox2)(ref mapRect)).Height <= 0f))
			{
				Vector2 vector2 = visibleMax - visibleMin;
				Vector2 value = new Vector2((0f - vector.X) / ((UIBox2)(ref mapRect)).Width * vector2.X, vector.Y / ((UIBox2)(ref mapRect)).Height * vector2.Y);
				Vector2 valueOrDefault = _viewCenter.GetValueOrDefault();
				if (!_viewCenter.HasValue)
				{
					valueOrDefault = (visibleMin + visibleMax) * 0.5f;
					_viewCenter = valueOrDefault;
				}
				_viewCenter += value;
				((InputEventArgs)args).Handle();
			}
		}
	}

	protected override void MouseWheel(GUIMouseWheelEventArgs args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		((Control)this).MouseWheel(args);
		if (!_hasBounds || _mapId == MapId.Nullspace)
		{
			return;
		}
		Vector2 relativePosition = ((GUIMouseEventArgs)args).RelativePosition;
		Vector2 mapPosition;
		bool flag = TryLocalToMapPosition(relativePosition, out mapPosition);
		float zoom = _zoom;
		if (args.Delta.Y > 0f)
		{
			_zoom *= 1.12f;
		}
		else if (args.Delta.Y < 0f)
		{
			_zoom /= 1.12f;
		}
		_zoom = Math.Clamp(_zoom, 1f, 4f);
		if (Math.Abs(_zoom - zoom) <= 0.0001f)
		{
			return;
		}
		if (flag && TryLocalToMapPosition(relativePosition, out var mapPosition2))
		{
			Vector2 valueOrDefault = _viewCenter.GetValueOrDefault();
			if (!_viewCenter.HasValue)
			{
				valueOrDefault = (_boundsMin + _boundsMax) * 0.5f;
				_viewCenter = valueOrDefault;
			}
			_viewCenter += mapPosition - mapPosition2;
		}
		((InputEventArgs)args).Handle();
	}

	protected override void Draw(DrawingHandleScreen handle)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0360: Unknown result type (might be due to invalid IL or missing references)
		//IL_0390: Unknown result type (might be due to invalid IL or missing references)
		//IL_0395: Unknown result type (might be due to invalid IL or missing references)
		//IL_039d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0560: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_05eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f1: Unknown result type (might be due to invalid IL or missing references)
		((Control)this).Draw(handle);
		handle.DrawRect(UIBox2i.op_Implicit(((Control)this).PixelSizeBox), new Color((byte)13, (byte)18, (byte)24, byte.MaxValue), true);
		UIBox2 mapRect = GetMapRect();
		if (((UIBox2)(ref mapRect)).Width <= 1f || ((UIBox2)(ref mapRect)).Height <= 1f)
		{
			return;
		}
		handle.DrawRect(mapRect, new Color((byte)21, (byte)28, (byte)36, byte.MaxValue), true);
		handle.DrawRect(mapRect, new Color((byte)78, (byte)96, (byte)118, byte.MaxValue), false);
		if (!_hasBounds)
		{
			DrawGrid(handle, mapRect);
			DrawCenteredText(handle, mapRect, Loc.GetString("civ-gmap-canvas-no-map-data"));
			return;
		}
		DrawMapTiles(handle);
		DrawGrid(handle, mapRect);
		foreach (CivGlobalMapMarkerState marker2 in _markers)
		{
			if (!marker2.IsObjective && !(marker2.MapId != _mapId) && TryMapToLocalPosition(marker2.Position, out var localPosition))
			{
				DrawMarker(handle, marker2, localPosition);
			}
		}
		foreach (CivPointCapturePointState point in _points)
		{
			if (!(point.MapId != _mapId) && TryMapToLocalPosition(point.Position, out var localPosition2))
			{
				DrawPoint(handle, point, localPosition2);
			}
		}
		foreach (CivCommanderOrderState order in _orders)
		{
			if (!(order.MapId != _mapId) && TryMapToLocalPosition(order.Position, out var localPosition3))
			{
				DrawCommanderOrder(handle, order, localPosition3);
			}
		}
		foreach (CivFobMarkerState fob in _fobs)
		{
			if (!(fob.MapId != _mapId) && TryMapToLocalPosition(fob.Position, out var localPosition4))
			{
				DrawFob(handle, localPosition4);
			}
		}
		if (_hoveredMarkerId != 0 && TryGetMarkerById(_hoveredMarkerId, out CivGlobalMapMarkerState marker) && marker.MapId == _mapId && TryMapToLocalPosition(marker.Position, out var localPosition5))
		{
			DrawMarkerHoverInfo(handle, marker, localPosition5);
		}
		foreach (CivGlobalMapDeathState death in _deaths)
		{
			if (!(death.MapId != _mapId) && TryMapToLocalPosition(death.Position, out var localPosition6))
			{
				DrawDeath(handle, death, localPosition6);
			}
		}
		TimeSpan curTime = _timing.CurTime;
		float num = (float)(curTime - _lastPlayerLerpTime).TotalSeconds;
		_lastPlayerLerpTime = curTime;
		float amount = ((num <= 0f) ? 1f : (1f - MathF.Exp((0f - num) / 0.45f)));
		_playerSeen.Clear();
		foreach (CivGlobalMapPlayerState player in _players)
		{
			if (player.MapId != _mapId)
			{
				continue;
			}
			Vector2 vector;
			if (player.IsSelf)
			{
				EntityUid? localEntity = _player.LocalEntity;
				if (localEntity.HasValue)
				{
					EntityUid valueOrDefault = localEntity.GetValueOrDefault();
					if (_vehicles.TryGetDisplayMapCoordinates(valueOrDefault, out var coordinates) && coordinates.MapId == _mapId)
					{
						vector = coordinates.Position;
						goto IL_044b;
					}
				}
				vector = player.Position;
			}
			else
			{
				Vector2 position = player.Position;
				vector = ((!_playerSmoothed.TryGetValue(player.Name, out var value)) ? position : (((position - value).Length() > 25f) ? position : Vector2.Lerp(value, position, amount)));
				_playerSmoothed[player.Name] = vector;
				_playerSeen.Add(player.Name);
			}
			goto IL_044b;
			IL_044b:
			if (TryMapToLocalPosition(vector, out var localPosition7))
			{
				DrawPlayer(handle, player, localPosition7);
			}
		}
		if (_playerSmoothed.Count != _playerSeen.Count)
		{
			_playerStale.Clear();
			foreach (string key in _playerSmoothed.Keys)
			{
				if (!_playerSeen.Contains(key))
				{
					_playerStale.Add(key);
				}
			}
			foreach (string item in _playerStale)
			{
				_playerSmoothed.Remove(item);
			}
		}
		if (_heli == null)
		{
			_heli = _entity.System<CivHeliSupplySystem>();
		}
		_heliBlips.Clear();
		_heli.GetActiveHeliBlips(_mapId, _heliBlips);
		foreach (HeliMapBlip heliBlip in _heliBlips)
		{
			if (TryMapToLocalPosition(heliBlip.Position, out var localPosition8))
			{
				DrawHeli(handle, localPosition8, heliBlip.Heading, heliBlip.Side);
			}
		}
		foreach (CivGlobalMapMarkerState marker3 in _markers)
		{
			if (marker3.IsObjective && !(marker3.MapId != _mapId) && TryMapToLocalPosition(marker3.Position, out var localPosition9))
			{
				DrawMarker(handle, marker3, localPosition9);
			}
		}
	}

	private void DrawGrid(DrawingHandleScreen handle, UIBox2 mapRect)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		Color val = default(Color);
		((Color)(ref val))._002Ector((byte)66, (byte)79, (byte)97, (byte)130);
		if (!_hasBounds || !TryGetVisibleBounds(out var visibleMin, out var visibleMax))
		{
			for (int i = 1; i < 4; i++)
			{
				float num = (float)i / 4f;
				float x = MathHelper.Lerp(mapRect.Left, mapRect.Right, num);
				float y = MathHelper.Lerp(mapRect.Top, mapRect.Bottom, num);
				((DrawingHandleBase)handle).DrawLine(new Vector2(x, mapRect.Top), new Vector2(x, mapRect.Bottom), val);
				((DrawingHandleBase)handle).DrawLine(new Vector2(mapRect.Left, y), new Vector2(mapRect.Right, y), val);
			}
			return;
		}
		Vector2 visibleSize = visibleMax - visibleMin;
		if (visibleSize.X <= 0f || visibleSize.Y <= 0f)
		{
			return;
		}
		for (int j = 0; j <= 8; j++)
		{
			float num2 = CivMapGrid.LineX(_boundsMin, _boundsMax, j);
			if (!(num2 < visibleMin.X) && !(num2 > visibleMax.X))
			{
				float x2 = mapRect.Left + (num2 - visibleMin.X) / visibleSize.X * ((UIBox2)(ref mapRect)).Width;
				((DrawingHandleBase)handle).DrawLine(new Vector2(x2, mapRect.Top), new Vector2(x2, mapRect.Bottom), val);
			}
		}
		for (int k = 0; k <= 8; k++)
		{
			float num3 = CivMapGrid.LineY(_boundsMin, _boundsMax, k);
			if (!(num3 < visibleMin.Y) && !(num3 > visibleMax.Y))
			{
				float y2 = mapRect.Bottom - (num3 - visibleMin.Y) / visibleSize.Y * ((UIBox2)(ref mapRect)).Height;
				((DrawingHandleBase)handle).DrawLine(new Vector2(mapRect.Left, y2), new Vector2(mapRect.Right, y2), val);
			}
		}
		DrawGridLabels(handle, mapRect, visibleMin, visibleMax, visibleSize);
	}

	private void DrawGridLabels(DrawingHandleScreen handle, UIBox2 mapRect, Vector2 visibleMin, Vector2 visibleMax, Vector2 visibleSize)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		Color color = Color.FromHex((ReadOnlySpan<char>)"#FFE9A8", (Color?)null);
		for (int i = 0; i < 8; i++)
		{
			float num = CivMapGrid.ColumnCenterX(_boundsMin, _boundsMax, i);
			if (!(num < visibleMin.X) && !(num > visibleMax.X))
			{
				float num2 = mapRect.Left + (num - visibleMin.X) / visibleSize.X * ((UIBox2)(ref mapRect)).Width;
				string columnLabel = CivMapGrid.GetColumnLabel(i);
				DrawLabelOutlined(handle, new Vector2(num2 - handle.GetDimensions(_gridFont, (ReadOnlySpan<char>)columnLabel, ((Control)this).UIScale).X / 2f, mapRect.Top + 2f * ((Control)this).UIScale), columnLabel, color);
			}
		}
		for (int j = 0; j < 8; j++)
		{
			float num3 = CivMapGrid.RowCenterY(_boundsMin, _boundsMax, j);
			if (!(num3 < visibleMin.Y) && !(num3 > visibleMax.Y))
			{
				float num4 = mapRect.Bottom - (num3 - visibleMin.Y) / visibleSize.Y * ((UIBox2)(ref mapRect)).Height;
				string text = (j + 1).ToString();
				Vector2 dimensions = handle.GetDimensions(_gridFont, (ReadOnlySpan<char>)text, ((Control)this).UIScale);
				DrawLabelOutlined(handle, new Vector2(mapRect.Left + 2f * ((Control)this).UIScale, num4 - dimensions.Y / 2f), text, color);
			}
		}
	}

	private void DrawLabelOutlined(DrawingHandleScreen handle, Vector2 pos, string text, Color color)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		Color black = Color.Black;
		Color val = ((Color)(ref black)).WithAlpha(0.95f);
		handle.DrawString(_gridFont, pos + new Vector2(-1f, 0f), (ReadOnlySpan<char>)text, ((Control)this).UIScale, val);
		handle.DrawString(_gridFont, pos + new Vector2(1f, 0f), (ReadOnlySpan<char>)text, ((Control)this).UIScale, val);
		handle.DrawString(_gridFont, pos + new Vector2(0f, -1f), (ReadOnlySpan<char>)text, ((Control)this).UIScale, val);
		handle.DrawString(_gridFont, pos + new Vector2(0f, 1f), (ReadOnlySpan<char>)text, ((Control)this).UIScale, val);
		handle.DrawString(_gridFont, pos, (ReadOnlySpan<char>)text, ((Control)this).UIScale, color);
	}

	private void DrawDeath(DrawingHandleScreen handle, CivGlobalMapDeathState death, Vector2 deathPos)
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		float num = ((death.LifetimeSeconds > 0f) ? Math.Clamp(death.RemainingSeconds / death.LifetimeSeconds, 0f, 1f) : 1f);
		float num2 = 0.25f + 0.65f * num;
		Color playerColor = CivGlobalMapColorResolver.GetPlayerColor(_viewerTeamId, _viewerSquadId, death.TeamId, death.SquadId);
		Color val = ((Color)(ref playerColor)).WithAlpha(num2);
		((DrawingHandleBase)handle).DrawLine(deathPos + new Vector2(-5f, -5f), deathPos + new Vector2(5f, 5f), val);
		((DrawingHandleBase)handle).DrawLine(deathPos + new Vector2(-5f, 5f), deathPos + new Vector2(5f, -5f), val);
		((DrawingHandleBase)handle).DrawCircle(deathPos, 7f, ((Color)(ref val)).WithAlpha(num2 * 0.5f), false);
	}

	private void DrawMapTiles(DrawingHandleScreen handle)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		//IL_0329: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Unknown result type (might be due to invalid IL or missing references)
		if (_tileCache.Count == 0 || !TryGetVisibleBounds(out var visibleMin, out var visibleMax))
		{
			return;
		}
		UIBox2 mapRect = GetMapRect();
		Vector2 vector = visibleMax - visibleMin;
		if (vector.X <= 0f || vector.Y <= 0f)
		{
			return;
		}
		if (_tileTexture != null)
		{
			Vector2 vector2 = Vector2.Max(visibleMin, _tileTextureWorldMin);
			Vector2 vector3 = Vector2.Min(visibleMax, _tileTextureWorldMax);
			if (!(vector2.X >= vector3.X) && !(vector2.Y >= vector3.Y))
			{
				UIBox2 val = default(UIBox2);
				((UIBox2)(ref val))._002Ector(mapRect.Left + (vector2.X - visibleMin.X) / vector.X * ((UIBox2)(ref mapRect)).Width, mapRect.Bottom - (vector3.Y - visibleMin.Y) / vector.Y * ((UIBox2)(ref mapRect)).Height, mapRect.Left + (vector3.X - visibleMin.X) / vector.X * ((UIBox2)(ref mapRect)).Width, mapRect.Bottom - (vector2.Y - visibleMin.Y) / vector.Y * ((UIBox2)(ref mapRect)).Height);
				Vector2 vector4 = _tileTextureWorldMax - _tileTextureWorldMin;
				Vector2i size = ((Texture)_tileTexture).Size;
				UIBox2 value = default(UIBox2);
				((UIBox2)(ref value))._002Ector((vector2.X - _tileTextureWorldMin.X) / vector4.X * (float)size.X, (_tileTextureWorldMax.Y - vector3.Y) / vector4.Y * (float)size.Y, (vector3.X - _tileTextureWorldMin.X) / vector4.X * (float)size.X, (_tileTextureWorldMax.Y - vector2.Y) / vector4.Y * (float)size.Y);
				handle.DrawTextureRectRegion((Texture)(object)_tileTexture, val, (UIBox2?)value, (Color?)null);
			}
			return;
		}
		Vector2 vector5 = new Vector2(((UIBox2)(ref mapRect)).Width / MathF.Max(1f, vector.X) * 0.5f, ((UIBox2)(ref mapRect)).Height / MathF.Max(1f, vector.Y) * 0.5f);
		UIBox2 val2 = default(UIBox2);
		foreach (MapTileDrawData item in _tileCache)
		{
			if (!(item.WorldPosition.X < visibleMin.X - 1f) && !(item.WorldPosition.X > visibleMax.X + 1f) && !(item.WorldPosition.Y < visibleMin.Y - 1f) && !(item.WorldPosition.Y > visibleMax.Y + 1f) && TryMapToLocalPosition(item.WorldPosition, out var localPosition))
			{
				((UIBox2)(ref val2))._002Ector(localPosition - vector5, localPosition + vector5);
				Color val3 = (item.IsWall ? WallTileColor : item.Color);
				handle.DrawRect(val2, val3, true);
				if (item.IsWall)
				{
					UIBox2 val4 = val2;
					Color black = Color.Black;
					handle.DrawRect(val4, ((Color)(ref black)).WithAlpha(0.72f), false);
				}
			}
		}
	}

	private void RefreshTileCache()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		if (!_hasBounds || _mapId == MapId.Nullspace)
		{
			_tileCache.Clear();
			_tileCacheMapId = MapId.Nullspace;
			ClearTileTexture();
		}
		else
		{
			if (_tileCacheMapId == _mapId && _tileCache.Count > 0)
			{
				return;
			}
			_tileCache.Clear();
			_tileCacheMapId = _mapId;
			EntityQueryEnumerator<MapGridComponent, TransformComponent> val = _entity.EntityQueryEnumerator<MapGridComponent, TransformComponent>();
			EntityUid val2 = default(EntityUid);
			MapGridComponent val3 = default(MapGridComponent);
			TransformComponent val4 = default(TransformComponent);
			TileRef? val5 = default(TileRef?);
			Vector2i item2 = default(Vector2i);
			while (val.MoveNext(ref val2, ref val3, ref val4))
			{
				if (val4.MapID != _mapId)
				{
					continue;
				}
				Vector2 worldPosition = _transform.GetWorldPosition(val4);
				GridTileEnumerator allTilesEnumerator = _map.GetAllTilesEnumerator(val2, val3, true);
				HashSet<Vector2i> hashSet = new HashSet<Vector2i>();
				List<(Vector2i, Vector2, Color)> list = new List<(Vector2i, Vector2, Color)>();
				while (((GridTileEnumerator)(ref allTilesEnumerator)).MoveNext(ref val5))
				{
					Vector2i gridIndices = val5.Value.GridIndices;
					Vector2 item = worldPosition + Vector2i.op_Implicit(gridIndices);
					Color val6 = _turf.GetContentTileDefinition(val5.Value.Tile).MinimapColor;
					if (val6.A == 0f)
					{
						val6 = EmptyTileColor;
					}
					hashSet.Add(gridIndices);
					list.Add((gridIndices, item, val6));
				}
				foreach (var item3 in list)
				{
					bool flag = false;
					for (int i = -1; i <= 1; i++)
					{
						if (flag)
						{
							break;
						}
						for (int j = -1; j <= 1; j++)
						{
							if (i != 0 || j != 0)
							{
								((Vector2i)(ref item2))._002Ector(item3.Item1.X + i, item3.Item1.Y + j);
								if (!hashSet.Contains(item2))
								{
									flag = true;
									break;
								}
							}
						}
					}
					_tileCache.Add(new MapTileDrawData(item3.Item2, item3.Item3, flag));
				}
			}
			BakeTileTexture();
		}
	}

	private void BakeTileTexture()
	{
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		ClearTileTexture();
		if (_tileCache.Count == 0)
		{
			return;
		}
		Vector2 vector = _tileCache[0].WorldPosition;
		Vector2 vector2 = vector;
		foreach (MapTileDrawData item in _tileCache)
		{
			vector = Vector2.Min(vector, item.WorldPosition);
			vector2 = Vector2.Max(vector2, item.WorldPosition);
		}
		float num = MathF.Max(vector2.X - vector.X, vector2.Y - vector.Y) + 1f;
		if (num <= 0f || num > 4096f)
		{
			return;
		}
		int num2 = Math.Clamp((int)(4096f / num), 1, 4);
		int num3 = (int)MathF.Ceiling((vector2.X - vector.X + 1f) * (float)num2);
		int num4 = (int)MathF.Ceiling((vector2.Y - vector.Y + 1f) * (float)num2);
		if (num3 <= 0 || num4 <= 0)
		{
			return;
		}
		Rgba32 val = default(Rgba32);
		((Rgba32)(ref val))._002Ector(((Color)(ref WallTileColor)).RByte, ((Color)(ref WallTileColor)).GByte, ((Color)(ref WallTileColor)).BByte, ((Color)(ref WallTileColor)).AByte);
		Color val2 = Color.InterpolateBetween(WallTileColor, Color.Black, 0.72f);
		Rgba32 val3 = default(Rgba32);
		((Rgba32)(ref val3))._002Ector(((Color)(ref val2)).RByte, ((Color)(ref val2)).GByte, ((Color)(ref val2)).BByte, byte.MaxValue);
		Image<Rgba32> val4 = new Image<Rgba32>(num3, num4);
		try
		{
			foreach (MapTileDrawData item2 in _tileCache)
			{
				_003F val5;
				if (!item2.IsWall)
				{
					Color color = item2.Color;
					byte rByte = ((Color)(ref color)).RByte;
					color = item2.Color;
					byte gByte = ((Color)(ref color)).GByte;
					color = item2.Color;
					byte bByte = ((Color)(ref color)).BByte;
					color = item2.Color;
					val5 = new Rgba32(rByte, gByte, bByte, ((Color)(ref color)).AByte);
				}
				else
				{
					val5 = val;
				}
				Rgba32 val6 = (Rgba32)val5;
				int num5 = (int)MathF.Round((item2.WorldPosition.X - vector.X) * (float)num2);
				int num6 = (int)MathF.Round((vector2.Y - item2.WorldPosition.Y) * (float)num2);
				for (int i = 0; i < num2; i++)
				{
					for (int j = 0; j < num2; j++)
					{
						int num7 = num5 + i;
						int num8 = num6 + j;
						if (num7 >= 0 && num8 >= 0 && num7 < num3 && num8 < num4)
						{
							bool flag = item2.IsWall && num2 >= 3 && (i == 0 || j == 0 || i == num2 - 1 || j == num2 - 1);
							val4[num7, num8] = (flag ? val3 : val6);
						}
					}
				}
			}
			_tileTexture = _clyde.LoadTextureFromImage<Rgba32>(val4, "civ-gmap-tiles", (TextureLoadParameters?)null);
			_tileTextureWorldMin = vector - new Vector2(0.5f, 0.5f);
			_tileTextureWorldMax = vector2 + new Vector2(0.5f, 0.5f);
		}
		finally
		{
			((IDisposable)val4)?.Dispose();
		}
	}

	private void ClearTileTexture()
	{
		OwnedTexture? tileTexture = _tileTexture;
		if (tileTexture != null)
		{
			tileTexture.Dispose();
		}
		_tileTexture = null;
	}

	protected override void Dispose(bool disposing)
	{
		((Control)this).Dispose(disposing);
		if (disposing)
		{
			ClearTileTexture();
		}
	}

	private void DrawMarker(DrawingHandleScreen handle, CivGlobalMapMarkerState marker, Vector2 markerPos)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		Color color = CivGlobalMapColorResolver.GetColor(marker.Type);
		float num = (marker.Type.IsGlobal() ? 7f : 5f);
		if (marker.IsObjective)
		{
			DrawObjectiveMarker(handle, marker, markerPos, color);
			return;
		}
		if (marker.Type.IsGlobal())
		{
			Vector2 vector = markerPos + new Vector2(0f, (0f - num) * 1.5f);
			Vector2 vector2 = markerPos + new Vector2(0f - num, num);
			Vector2 vector3 = markerPos + new Vector2(num, num);
			((DrawingHandleBase)handle).DrawPrimitives((DrawPrimitiveTopology)1, (ReadOnlySpan<Vector2>)new Vector2[3] { vector, vector2, vector3 }, color);
			((DrawingHandleBase)handle).DrawPrimitives((DrawPrimitiveTopology)5, (ReadOnlySpan<Vector2>)new Vector2[4] { vector, vector2, vector3, vector }, Color.Black);
		}
		else
		{
			((DrawingHandleBase)handle).DrawCircle(markerPos, num, color, true);
			((DrawingHandleBase)handle).DrawCircle(markerPos, num + 1f, Color.Black, false);
		}
		if (RemoveMode && !marker.IsObjective)
		{
			float num2 = num + 4f;
			Color white = Color.White;
			((DrawingHandleBase)handle).DrawCircle(markerPos, num2, ((Color)(ref white)).WithAlpha(0.8f), false);
		}
		string shortText = CivGlobalMapColorResolver.GetShortText(marker.Type);
		Vector2 dimensions = handle.GetDimensions(_font, (ReadOnlySpan<char>)shortText, ((Control)this).UIScale);
		Vector2 vector4 = markerPos - dimensions / 2f;
		handle.DrawString(_font, vector4, (ReadOnlySpan<char>)shortText, ((Control)this).UIScale, Color.White);
	}

	private void DrawCommanderOrder(DrawingHandleScreen handle, CivCommanderOrderState order, Vector2 orderPos)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		Color val = (Color)(order.Order switch
		{
			CivCommanderOrderType.Attack => Color.FromHex((ReadOnlySpan<char>)"#ff5449", (Color?)null), 
			CivCommanderOrderType.Defense => Color.FromHex((ReadOnlySpan<char>)"#5ca8ff", (Color?)null), 
			CivCommanderOrderType.Artillery => Color.FromHex((ReadOnlySpan<char>)"#ffd85a", (Color?)null), 
			_ => Color.White, 
		});
		Color val2 = val;
		((DrawingHandleBase)handle).DrawCircle(orderPos, 11f, ((Color)(ref val2)).WithAlpha(0.18f), true);
		((DrawingHandleBase)handle).DrawCircle(orderPos, 11f, val2, false);
		val = Color.Black;
		((DrawingHandleBase)handle).DrawCircle(orderPos, 13f, ((Color)(ref val)).WithAlpha(0.8f), false);
		string text = order.Order switch
		{
			CivCommanderOrderType.Attack => "ATK", 
			CivCommanderOrderType.Defense => "DEF", 
			CivCommanderOrderType.Artillery => "ART", 
			_ => "ORD", 
		};
		Vector2 dimensions = handle.GetDimensions(_font, (ReadOnlySpan<char>)text, ((Control)this).UIScale);
		Vector2 vector = orderPos - dimensions / 2f;
		handle.DrawString(_font, vector, (ReadOnlySpan<char>)text, ((Control)this).UIScale, Color.White);
		string text2 = order.SquadLabel + " " + text;
		Vector2 vector2 = orderPos + new Vector2((0f - handle.GetDimensions(_font, (ReadOnlySpan<char>)text2, ((Control)this).UIScale).X) / 2f, 11f + 4f * ((Control)this).UIScale);
		Font font = _font;
		Vector2 vector3 = vector2 + new Vector2(1f, 1f);
		ReadOnlySpan<char> readOnlySpan = text2;
		float uIScale = ((Control)this).UIScale;
		val = Color.Black;
		handle.DrawString(font, vector3, readOnlySpan, uIScale, ((Color)(ref val)).WithAlpha(0.85f));
		handle.DrawString(_font, vector2, (ReadOnlySpan<char>)text2, ((Control)this).UIScale, val2);
	}

	private void DrawFob(DrawingHandleScreen handle, Vector2 fobPos)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		Color val = Color.FromHex((ReadOnlySpan<char>)"#43d96b", (Color?)null);
		UIBox2 val2 = default(UIBox2);
		((UIBox2)(ref val2))._002Ector(fobPos - new Vector2(9f, 9f), fobPos + new Vector2(9f, 9f));
		handle.DrawRect(val2, ((Color)(ref val)).WithAlpha(0.25f), true);
		handle.DrawRect(val2, val, false);
		string text = "FOB";
		Vector2 dimensions = handle.GetDimensions(_font, (ReadOnlySpan<char>)text, ((Control)this).UIScale);
		Vector2 vector = fobPos - dimensions / 2f;
		handle.DrawString(_font, vector, (ReadOnlySpan<char>)text, ((Control)this).UIScale, Color.White);
	}

	private void DrawPoint(DrawingHandleScreen handle, CivPointCapturePointState point, Vector2 pointPos)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		Color relationColor = CivPointCaptureColorResolver.GetRelationColor(_viewerTeamId, point.OwnerTeamId);
		bool flag = point.CapturingTeamId != 0 && point.CaptureProgress > 0f;
		Color val = (flag ? CivPointCaptureColorResolver.GetCapturePulseColor(_viewerTeamId, point.OwnerTeamId, point.CapturingTeamId, (float)_timing.CurTime.TotalSeconds) : relationColor);
		float num = (flag ? CivPointCaptureColorResolver.GetCapturePulseAmount((float)_timing.CurTime.TotalSeconds) : 0f);
		float num2 = 24f + num * 4f;
		((DrawingHandleBase)handle).DrawCircle(pointPos, num2, ((Color)(ref val)).WithAlpha(0.18f), true);
		((DrawingHandleBase)handle).DrawCircle(pointPos, num2, ((Color)(ref val)).WithAlpha(0.98f), false);
		((DrawingHandleBase)handle).DrawCircle(pointPos, 15f, ((Color)(ref val)).WithAlpha(0.8f), false);
		float num3 = num2 + 2f;
		Color black = Color.Black;
		((DrawingHandleBase)handle).DrawCircle(pointPos, num3, ((Color)(ref black)).WithAlpha(0.75f), false);
		((DrawingHandleBase)handle).DrawCircle(pointPos, 5f, relationColor, true);
		((DrawingHandleBase)handle).DrawCircle(pointPos, 6f, Color.Black, false);
		if (flag)
		{
			Color relationColor2 = CivPointCaptureColorResolver.GetRelationColor(_viewerTeamId, point.CapturingTeamId);
			DrawProgressArc(handle, pointPos, num2 + 5f, point.CaptureProgress, relationColor2);
		}
		string text = (string.IsNullOrWhiteSpace(point.Label) ? "P" : point.Label);
		Vector2 dimensions = handle.GetDimensions(_font, (ReadOnlySpan<char>)text, ((Control)this).UIScale);
		Vector2 vector = pointPos - new Vector2(dimensions.X / 2f, 24f + dimensions.Y + 4f * ((Control)this).UIScale);
		Font font = _font;
		Vector2 vector2 = vector + new Vector2(1f, 1f);
		ReadOnlySpan<char> readOnlySpan = text;
		float uIScale = ((Control)this).UIScale;
		black = Color.Black;
		handle.DrawString(font, vector2, readOnlySpan, uIScale, ((Color)(ref black)).WithAlpha(0.85f));
		handle.DrawString(_font, vector, (ReadOnlySpan<char>)text, ((Control)this).UIScale, flag ? val : relationColor);
	}

	private static void DrawProgressArc(DrawingHandleScreen handle, Vector2 center, float radius, float progress, Color color)
	{
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		progress = Math.Clamp(progress, 0f, 1f);
		if (!(progress <= 0f))
		{
			int num = Math.Max(6, (int)MathF.Ceiling(28f * progress));
			Vector2[] array = new Vector2[num + 1];
			for (int i = 0; i <= num; i++)
			{
				float num2 = progress * (float)i / (float)num;
				float x = -MathF.PI / 2f + MathF.PI * 2f * num2;
				array[i] = center + new Vector2(MathF.Cos(x), MathF.Sin(x)) * radius;
			}
			((DrawingHandleBase)handle).DrawPrimitives((DrawPrimitiveTopology)5, (ReadOnlySpan<Vector2>)array, color);
		}
	}

	private void DrawObjectiveMarker(DrawingHandleScreen handle, CivGlobalMapMarkerState marker, Vector2 markerPos, Color color)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		((DrawingHandleBase)handle).DrawCircle(markerPos, 28f, ((Color)(ref color)).WithAlpha(0.1f), true);
		((DrawingHandleBase)handle).DrawCircle(markerPos, 28f, ((Color)(ref color)).WithAlpha(0.95f), false);
		((DrawingHandleBase)handle).DrawCircle(markerPos, 18f, ((Color)(ref color)).WithAlpha(0.75f), false);
		Color black = Color.Black;
		((DrawingHandleBase)handle).DrawCircle(markerPos, 30f, ((Color)(ref black)).WithAlpha(0.75f), false);
		((DrawingHandleBase)handle).DrawCircle(markerPos, 6f, color, true);
		((DrawingHandleBase)handle).DrawCircle(markerPos, 7f, Color.Black, false);
		float num = 10f;
		((DrawingHandleBase)handle).DrawLine(markerPos + new Vector2(0f - num, 0f), markerPos + new Vector2(num, 0f), color);
		((DrawingHandleBase)handle).DrawLine(markerPos + new Vector2(0f, 0f - num), markerPos + new Vector2(0f, num), color);
		string text = ((marker.Type == CivGlobalMapMarkerType.Attack) ? "ATK" : "DEF");
		Vector2 dimensions = handle.GetDimensions(_font, (ReadOnlySpan<char>)text, ((Control)this).UIScale);
		Vector2 vector = markerPos - new Vector2(dimensions.X / 2f, 28f + dimensions.Y + 4f * ((Control)this).UIScale);
		Font font = _font;
		Vector2 vector2 = vector + new Vector2(1f, 1f);
		ReadOnlySpan<char> readOnlySpan = text;
		float uIScale = ((Control)this).UIScale;
		black = Color.Black;
		handle.DrawString(font, vector2, readOnlySpan, uIScale, ((Color)(ref black)).WithAlpha(0.85f));
		handle.DrawString(_font, vector, (ReadOnlySpan<char>)text, ((Control)this).UIScale, Color.White);
	}

	private void DrawMarkerHoverInfo(DrawingHandleScreen handle, CivGlobalMapMarkerState marker, Vector2 markerPos)
	{
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		string text = (marker.IsObjective ? marker.PlacedByName : ((marker.SquadId > 0) ? ((marker.PlacedBySquadLeader ? $"[LS{marker.SquadId}]" : $"[S{marker.SquadId}]") + " " + marker.PlacedByName) : marker.PlacedByName));
		Vector2 vector = markerPos + new Vector2(10f * ((Control)this).UIScale, -22f * ((Control)this).UIScale);
		Font font = _font;
		Vector2 vector2 = vector + new Vector2(1f, 1f);
		ReadOnlySpan<char> readOnlySpan = text;
		float uIScale = ((Control)this).UIScale;
		Color black = Color.Black;
		handle.DrawString(font, vector2, readOnlySpan, uIScale, ((Color)(ref black)).WithAlpha(0.85f));
		handle.DrawString(_font, vector, (ReadOnlySpan<char>)text, ((Control)this).UIScale, Color.White);
	}

	private void DrawPlayer(DrawingHandleScreen handle, CivGlobalMapPlayerState player, Vector2 playerPos)
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		int? commanderSelectedSquadId = CommanderSelectedSquadId;
		int num;
		if (commanderSelectedSquadId.HasValue)
		{
			int valueOrDefault = commanderSelectedSquadId.GetValueOrDefault();
			num = ((player.SquadId == valueOrDefault) ? 1 : 0);
		}
		else
		{
			num = 0;
		}
		bool flag = (byte)num != 0;
		Color val = (player.IsSelf ? Color.White : (flag ? CivGlobalMapColorResolver.SquadColor : CivGlobalMapColorResolver.GetPlayerColor(_viewerTeamId, _viewerSquadId, player.TeamId, player.SquadId)));
		float num2 = (player.IsSelf ? 7f : 6f);
		if (flag && !player.IsSelf)
		{
			((DrawingHandleBase)handle).DrawCircle(playerPos, num2 + 4f, ((Color)(ref val)).WithAlpha(0.18f), true);
		}
		if (player.IsSquadLeader && !player.IsSelf)
		{
			Vector2 vector = playerPos + new Vector2(0f, (0f - num2) * 1.35f);
			Vector2 vector2 = playerPos + new Vector2(0f - num2, num2);
			Vector2 vector3 = playerPos + new Vector2(num2, num2);
			((DrawingHandleBase)handle).DrawPrimitives((DrawPrimitiveTopology)1, (ReadOnlySpan<Vector2>)new Vector2[3] { vector, vector2, vector3 }, val);
			((DrawingHandleBase)handle).DrawPrimitives((DrawPrimitiveTopology)5, (ReadOnlySpan<Vector2>)new Vector2[4] { vector, vector2, vector3, vector }, Color.Black);
		}
		else
		{
			((DrawingHandleBase)handle).DrawCircle(playerPos, num2, val, true);
			((DrawingHandleBase)handle).DrawCircle(playerPos, num2 + 1f, Color.Black, false);
		}
		if (player.IsSelf || _cfg.GetCVar<bool>(CCVars.Civ14ShowForeignNames) || (_viewerSquadId != 0 && player.SquadId == _viewerSquadId))
		{
			string text = (player.IsSelf ? Loc.GetString("civ-gmap-canvas-player-self") : ((player.SquadId == 0) ? Loc.GetString("civ-gmap-canvas-player-reserve") : (player.IsSquadLeader ? $"[LS{player.SquadId}]" : $"[S{player.SquadId}]"))) + " " + player.Name;
			Vector2 vector4 = playerPos + new Vector2(9f * ((Control)this).UIScale, -14f * ((Control)this).UIScale);
			Font font = _font;
			Vector2 vector5 = vector4 + new Vector2(1f, 1f);
			ReadOnlySpan<char> readOnlySpan = text;
			float uIScale = ((Control)this).UIScale;
			Color black = Color.Black;
			handle.DrawString(font, vector5, readOnlySpan, uIScale, ((Color)(ref black)).WithAlpha(0.85f));
			handle.DrawString(_font, vector4, (ReadOnlySpan<char>)text, ((Control)this).UIScale, val);
		}
	}

	private void DrawHeli(DrawingHandleScreen handle, Vector2 pos, Vector2 heading, CivAirstrikeSide side)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		int playerTeamId = ((side != CivAirstrikeSide.Ru) ? 1 : 2);
		Color playerColor = CivGlobalMapColorResolver.GetPlayerColor(_viewerTeamId, _viewerSquadId, playerTeamId, 0);
		Vector2 vector = new Vector2(heading.X, 0f - heading.Y);
		float x = ((vector.LengthSquared() > 0.0001f) ? MathF.Atan2(vector.Y, vector.X) : 0f);
		Vector2 vector2 = new Vector2(MathF.Cos(x), MathF.Sin(x));
		Vector2 vector3 = new Vector2(0f - vector2.Y, vector2.X);
		Vector2 vector4 = pos + vector2 * 8f;
		Vector2 vector5 = pos - vector2 * 4.8f + vector3 * 6f;
		Vector2 vector6 = pos - vector2 * 4.8f - vector3 * 6f;
		((DrawingHandleBase)handle).DrawPrimitives((DrawPrimitiveTopology)1, (ReadOnlySpan<Vector2>)new Vector2[3] { vector4, vector5, vector6 }, playerColor);
		((DrawingHandleBase)handle).DrawPrimitives((DrawPrimitiveTopology)5, (ReadOnlySpan<Vector2>)new Vector2[4] { vector4, vector5, vector6, vector4 }, Color.Black);
		string text = Loc.GetString("civ-gmap-canvas-heli");
		Vector2 vector7 = pos + new Vector2(9f * ((Control)this).UIScale, -14f * ((Control)this).UIScale);
		Font font = _font;
		Vector2 vector8 = vector7 + new Vector2(1f, 1f);
		ReadOnlySpan<char> readOnlySpan = text;
		float uIScale = ((Control)this).UIScale;
		Color black = Color.Black;
		handle.DrawString(font, vector8, readOnlySpan, uIScale, ((Color)(ref black)).WithAlpha(0.85f));
		handle.DrawString(_font, vector7, (ReadOnlySpan<char>)text, ((Control)this).UIScale, playerColor);
	}

	private void DrawCenteredText(DrawingHandleScreen handle, UIBox2 mapRect, string text)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		Vector2 dimensions = handle.GetDimensions(_font, (ReadOnlySpan<char>)text, ((Control)this).UIScale);
		Vector2 vector = ((UIBox2)(ref mapRect)).Center - dimensions / 2f;
		handle.DrawString(_font, vector, (ReadOnlySpan<char>)text, ((Control)this).UIScale, Color.LightGray);
	}

	private UIBox2 GetMapRect()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		float num = 14f * ((Control)this).UIScale;
		Vector2 vector = Vector2i.op_Implicit(((Control)this).PixelSize) - new Vector2(num * 2f, num * 2f);
		if (vector.X <= 1f || vector.Y <= 1f)
		{
			return UIBox2.FromDimensions(Vector2.Zero, Vector2.Zero);
		}
		float num2 = MathF.Min(vector.X, vector.Y);
		float x = (vector.X - num2) * 0.5f + num;
		float y = (vector.Y - num2) * 0.5f + num;
		return UIBox2.FromDimensions(new Vector2(x, y), new Vector2(num2, num2));
	}

	private bool TryLocalToMapPosition(Vector2 localPosition, out Vector2 mapPosition)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		mapPosition = default(Vector2);
		if (!TryGetVisibleBounds(out var visibleMin, out var visibleMax))
		{
			return false;
		}
		UIBox2 mapRect = GetMapRect();
		if (!((UIBox2)(ref mapRect)).Contains(localPosition, true))
		{
			return false;
		}
		Vector2 vector = visibleMax - visibleMin;
		if (vector.X <= 0f || vector.Y <= 0f)
		{
			return false;
		}
		float num = (localPosition.X - mapRect.Left) / ((UIBox2)(ref mapRect)).Width;
		float num2 = (mapRect.Bottom - localPosition.Y) / ((UIBox2)(ref mapRect)).Height;
		mapPosition = new Vector2(visibleMin.X + num * vector.X, visibleMin.Y + num2 * vector.Y);
		return true;
	}

	private bool TryMapToLocalPosition(Vector2 mapPosition, out Vector2 localPosition)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		localPosition = default(Vector2);
		if (!TryGetVisibleBounds(out var visibleMin, out var visibleMax))
		{
			return false;
		}
		Vector2 vector = visibleMax - visibleMin;
		if (vector.X <= 0f || vector.Y <= 0f)
		{
			return false;
		}
		UIBox2 mapRect = GetMapRect();
		float num = (mapPosition.X - visibleMin.X) / vector.X;
		float num2 = (mapPosition.Y - visibleMin.Y) / vector.Y;
		if (num < 0f || num > 1f || num2 < 0f || num2 > 1f)
		{
			return false;
		}
		localPosition = new Vector2(mapRect.Left + num * ((UIBox2)(ref mapRect)).Width, mapRect.Bottom - num2 * ((UIBox2)(ref mapRect)).Height);
		return true;
	}

	private bool TryGetVisibleBounds(out Vector2 visibleMin, out Vector2 visibleMax)
	{
		visibleMin = default(Vector2);
		visibleMax = default(Vector2);
		if (!_hasBounds)
		{
			return false;
		}
		Vector2 vector = _boundsMax - _boundsMin;
		if (vector.X <= 0f || vector.Y <= 0f)
		{
			return false;
		}
		EnsureViewCenter();
		Vector2 value = _viewCenter ?? ((_boundsMin + _boundsMax) * 0.5f);
		float num = Math.Clamp(_zoom, 1f, 4f);
		Vector2 vector2 = vector / (2f * num);
		Vector2 min = _boundsMin + vector2;
		Vector2 max = _boundsMax - vector2;
		if (min.X > max.X)
		{
			max.X = (min.X = (_boundsMin.X + _boundsMax.X) * 0.5f);
		}
		if (min.Y > max.Y)
		{
			max.Y = (min.Y = (_boundsMin.Y + _boundsMax.Y) * 0.5f);
		}
		value = Vector2.Clamp(value, min, max);
		_viewCenter = value;
		visibleMin = value - vector2;
		visibleMax = value + vector2;
		return true;
	}

	private void EnsureViewCenter()
	{
		if (!_hasBounds)
		{
			_viewCenter = null;
		}
		else if (!_viewCenter.HasValue)
		{
			if (TryGetSelfPosition(out var position))
			{
				_viewCenter = position;
			}
			else
			{
				_viewCenter = (_boundsMin + _boundsMax) * 0.5f;
			}
		}
	}

	private bool TryGetSelfPosition(out Vector2 position)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		position = default(Vector2);
		foreach (CivGlobalMapPlayerState player in _players)
		{
			if (player.IsSelf && !(player.MapId != _mapId))
			{
				position = player.Position;
				return true;
			}
		}
		return false;
	}

	private void UpdateHoveredMarker(Vector2 localPosition)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		UIBox2 mapRect = GetMapRect();
		if (!((UIBox2)(ref mapRect)).Contains(localPosition, true))
		{
			_hoveredMarkerId = 0;
		}
		else
		{
			_hoveredMarkerId = (TryGetMarkerAt(localPosition, out var markerId, includeObjectives: true) ? markerId : 0);
		}
	}

	private bool TryGetMarkerById(int markerId, out CivGlobalMapMarkerState marker)
	{
		foreach (CivGlobalMapMarkerState marker2 in _markers)
		{
			if (marker2.Id == markerId)
			{
				marker = marker2;
				return true;
			}
		}
		marker = null;
		return false;
	}

	private bool TryGetMarkerAt(Vector2 localPosition, out int markerId, bool includeObjectives)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		markerId = 0;
		float num = float.MaxValue;
		foreach (CivGlobalMapMarkerState marker in _markers)
		{
			if (!(marker.MapId != _mapId) && (includeObjectives || !marker.IsObjective) && TryMapToLocalPosition(marker.Position, out var localPosition2))
			{
				float num2 = (marker.IsObjective ? 25f : 11f);
				float num3 = num2 * num2 * ((Control)this).UIScale * ((Control)this).UIScale;
				float num4 = (localPosition2 - localPosition).LengthSquared();
				if (!(num4 > num3) && !(num4 >= num))
				{
					num = num4;
					markerId = marker.Id;
				}
			}
		}
		return markerId != 0;
	}
}
