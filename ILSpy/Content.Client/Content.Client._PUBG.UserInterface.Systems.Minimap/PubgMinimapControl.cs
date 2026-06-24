using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Client._CIV14merka.Capture;
using Content.Client._CIV14merka.GlobalMap;
using Content.Client.Shuttles.UI;
using Content.Shared._CIV14merka.Capture;
using Content.Shared._CIV14merka.Commander;
using Content.Shared._CIV14merka.GlobalMap;
using Content.Shared._PUBG.Party;
using Content.Shared._RMC14.Vehicle;
using Content.Shared.CCVar;
using Content.Shared.Maps;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Map.Enumerators;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Client._PUBG.UserInterface.Systems.Minimap;

public sealed class PubgMinimapControl : BaseShuttleControl
{
	private sealed class GridCacheData
	{
		public int MinX;

		public int MinY;

		public int MaxX;

		public int MaxY;

		public HashSet<Vector2i> TileSet { get; } = new HashSet<Vector2i>();

		public HashSet<Vector2i> WallTiles { get; } = new HashSet<Vector2i>();

		public Dictionary<Vector2i, Color> TileColors { get; } = new Dictionary<Vector2i, Color>();
	}

	[Dependency]
	private IConfigurationManager _cfg;

	[Dependency]
	private IResourceCache _cache;

	[Dependency]
	private IPrototypeManager _prototype;

	[Dependency]
	private IGameTiming _timing;

	private readonly SharedTransformSystem _transform;

	private readonly TurfSystem _turf;

	private readonly SpriteSystem _sprite;

	private readonly VehicleSystem _vehicles;

	private EntityCoordinates? _playerCoordinates;

	private Angle? _playerRotation;

	private readonly List<Entity<MapGridComponent>> _grids = new List<Entity<MapGridComponent>>();

	private NetEntity _localNetEntity;

	private readonly Dictionary<EntityUid, GridCacheData> _gridCache = new Dictionary<EntityUid, GridCacheData>();

	private EntityUid? _lastGridUid;

	private readonly Dictionary<int, Font> _fontCache = new Dictionary<int, Font>();

	private readonly Dictionary<string, Texture?> _itemPingIconCache = new Dictionary<string, Texture>();

	private float _markerScale = 1f;

	private readonly Dictionary<string, Vector2> _civPlayerSmoothed = new Dictionary<string, Vector2>();

	private readonly HashSet<string> _civPlayerSeen = new HashSet<string>();

	private readonly List<string> _civPlayerStale = new List<string>();

	private TimeSpan _lastPlayerLerpTime;

	private const float CivPlayerLerpTau = 0.08f;

	private const float CivPlayerSnapDist = 25f;

	public IReadOnlyList<PubgPartyMemberState>? PartyMembers;

	public float PartyMarkersOpacity = 1f;

	public IReadOnlyList<PubgActivePingState> ActivePings = Array.Empty<PubgActivePingState>();

	public IReadOnlyList<CivGlobalMapMarkerState>? CivGlobalMapMarkers;

	public IReadOnlyList<CivGlobalMapPlayerState>? CivGlobalMapPlayers;

	public IReadOnlyList<CivPointCapturePointState>? CivGlobalMapPoints;

	public IReadOnlyList<CivCommanderOrderState>? CivGlobalMapOrders;

	public IReadOnlyList<CivGlobalMapDeathState>? CivGlobalMapDeaths;

	public int CivViewerTeamId;

	public int CivViewerSquadId;

	public bool CivHasBounds;

	public Vector2 CivBoundsMin;

	public Vector2 CivBoundsMax;

	public Action<MapCoordinates>? OnMapClick;

	public Vector2? ZoneCurrentCenter;

	public float ZoneCurrentRadius;

	public Vector2? ZoneNextCenter;

	public float ZoneNextRadius;

	public bool ZoneActive;

	public bool ZoneVisible;

	public MapId ZoneMapId;

	public bool RedZoneActive;

	public Vector2? RedZoneCenter;

	public float RedZoneRadius;

	public bool AirdropActive;

	public Vector2? AirdropCenter;

	public int AirdropRemainingSeconds;

	public MapId AirdropMapId;

	public MapId RespawnTowerMapId;

	public IReadOnlyList<Vector2> RespawnTowerPositions = Array.Empty<Vector2>();

	public IReadOnlyList<Vector2> ActiveRespawnTowerPositions = Array.Empty<Vector2>();

	private readonly Color _wallColor = Color.FromHex((ReadOnlySpan<char>)"#8B4513", (Color?)null);

	private readonly Color _floorColor = Color.FromHex((ReadOnlySpan<char>)"#2a2a2a", (Color?)null);

	private readonly Color _spaceColor = Color.FromHex((ReadOnlySpan<char>)"#000000", (Color?)null);

	private readonly Color _playerMarkerColor = Color.FromHex((ReadOnlySpan<char>)"#00ff00", (Color?)null);

	private readonly Color _pingEnemyColor = Color.FromHex((ReadOnlySpan<char>)"#FF3B30", (Color?)null);

	private const float PlayerMarkerSize = 8f;

	private const float AirdropMarkerSize = 4f;

	private const float AirdropLargeMapStartTiles = 220f;

	private const float AirdropLargeMapRangeTiles = 380f;

	private const float AirdropLargeMapExtraScale = 0.8f;

	private const float PartyMarkerRadius = 3f;

	private const float PingMarkerRadius = 4f;

	private const float RespawnTowerRadius = 3f;

	private const float ActiveRespawnTowerOuterRadius = 5f;

	private const float ActiveRespawnTowerInnerRadius = 4f;

	private const string TextFontPath = "/Fonts/NotoSans/NotoSans-Regular.ttf";

	private const int TextFontSize = 6;

	private const int AirdropTextFontSize = 10;

	private const int GridLabelFontSize = 11;

	public PubgMinimapControl()
		: base(32f, 256f, 128f)
	{
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		IoCManager.InjectDependencies<PubgMinimapControl>(this);
		_transform = EntManager.System<SharedTransformSystem>();
		_turf = EntManager.System<TurfSystem>();
		_sprite = EntManager.System<SpriteSystem>();
		_vehicles = EntManager.System<VehicleSystem>();
		((Control)this).SetSize = new Vector2(200f, 200f);
		((Control)this).MouseFilter = (MouseFilterMode)0;
		_cfg.OnValueChanged<float>(CCVars.PubgMinimapZoom, (Action<float>)OnZoomChanged, true);
		_cfg.OnValueChanged<int>(CCVars.PubgMinimapMarkerScale, (Action<int>)OnMarkerScaleChanged, true);
	}

	private void OnZoomChanged(float _)
	{
		UpdateZoom();
	}

	private void OnMarkerScaleChanged(int markerScalePercent)
	{
		_markerScale = Math.Clamp((float)markerScalePercent / 100f, 0.25f, 4f);
	}

	private void UpdateZoom()
	{
		float cVar = _cfg.GetCVar<float>(CCVars.PubgMinimapZoom);
		WorldRange = 32f * cVar;
	}

	protected override void ExitedTree()
	{
		((Control)this).ExitedTree();
		_cfg.UnsubValueChanged<float>(CCVars.PubgMinimapZoom, (Action<float>)OnZoomChanged);
		_cfg.UnsubValueChanged<int>(CCVars.PubgMinimapMarkerScale, (Action<int>)OnMarkerScaleChanged);
	}

	public void UpdatePlayerPosition(EntityUid player)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		if (!_vehicles.TryGetDisplayEntity(player, out var displayEntity))
		{
			displayEntity = player;
		}
		TransformComponent val = default(TransformComponent);
		if (EntManager.TryGetComponent<TransformComponent>(displayEntity, ref val))
		{
			_localNetEntity = EntManager.GetNetEntity(player, (MetaDataComponent)null);
			_playerCoordinates = val.Coordinates;
			_playerRotation = val.LocalRotation;
			_grids.Clear();
			EntityUid? gridUid = val.GridUid;
			EntityUid? lastGridUid = _lastGridUid;
			EntityUid? val2 = gridUid;
			if (lastGridUid.HasValue != val2.HasValue || (lastGridUid.HasValue && lastGridUid.GetValueOrDefault() != val2.GetValueOrDefault()))
			{
				_gridCache.Clear();
				_lastGridUid = gridUid;
			}
			MapGridComponent item = default(MapGridComponent);
			if (gridUid.HasValue && EntManager.TryGetComponent<MapGridComponent>(gridUid.Value, ref item))
			{
				_grids.Add(Entity<MapGridComponent>.op_Implicit((gridUid.Value, item)));
			}
		}
	}

	protected override void Draw(DrawingHandleScreen handle)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		base.Draw(handle);
		if (!_playerCoordinates.HasValue || !_playerRotation.HasValue)
		{
			return;
		}
		if (!EntManager.EntityExists(_playerCoordinates.Value.EntityId))
		{
			_playerCoordinates = null;
			return;
		}
		handle.DrawRect(new UIBox2(Vector2.Zero, Vector2i.op_Implicit(((Control)this).PixelSize)), _spaceColor, true);
		Vector2 center = ((Control)this).PixelSize / 2f;
		EntityUid val = default(EntityUid);
		MapGridComponent val2 = default(MapGridComponent);
		TransformComponent val4 = default(TransformComponent);
		foreach (Entity<MapGridComponent> grid2 in _grids)
		{
			grid2.Deconstruct(ref val, ref val2);
			EntityUid val3 = val;
			MapGridComponent grid = val2;
			if (EntManager.TryGetComponent<TransformComponent>(val3, ref val4))
			{
				Matrix3x2 invWorldMatrix = _transform.GetInvWorldMatrix(val4);
				Vector2 gridPos = Vector2.Transform(_transform.ToMapCoordinates(_playerCoordinates.Value, true).Position, invWorldMatrix);
				DrawGrid(handle, center, val3, grid, gridPos);
			}
		}
		DrawCivGrid(handle, center);
		if (ZoneActive && ZoneVisible && ZoneCurrentCenter.HasValue)
		{
			MapCoordinates val5 = _transform.ToMapCoordinates(_playerCoordinates.Value, true);
			if (val5.MapId == ZoneMapId)
			{
				DrawZones(handle, center, new Vector2(val5.Position.X, val5.Position.Y));
			}
		}
		if (AirdropActive && AirdropCenter.HasValue)
		{
			MapCoordinates val6 = _transform.ToMapCoordinates(_playerCoordinates.Value, true);
			if (val6.MapId == AirdropMapId)
			{
				DrawAirdrop(handle, center, new Vector2(val6.Position.X, val6.Position.Y));
			}
		}
		if (RespawnTowerPositions.Count > 0)
		{
			MapCoordinates val7 = _transform.ToMapCoordinates(_playerCoordinates.Value, true);
			if (val7.MapId == RespawnTowerMapId)
			{
				DrawRespawnTowers(handle, center, new Vector2(val7.Position.X, val7.Position.Y));
			}
		}
		DrawPings(handle, center);
		DrawPartyMembers(handle, center);
		DrawCivGlobalMap(handle, center);
		DrawCivOrders(handle, center);
		DrawCivSquadLeaderArrow(handle, center);
		DrawCivDeaths(handle, center);
		DrawPlayerMarker(handle, center, _playerRotation.Value);
	}

	private void DrawGrid(DrawingHandleScreen handle, Vector2 center, EntityUid gridUid, MapGridComponent grid, Vector2 gridPos)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		float worldRange = WorldRange;
		SharedMapSystem mapSystem = EntManager.System<SharedMapSystem>();
		float num = (float)((Control)this).PixelSize.X / (worldRange * 2f);
		if (!_gridCache.TryGetValue(gridUid, out GridCacheData value))
		{
			value = BuildGridCache(mapSystem, gridUid, grid);
			if (value == null)
			{
				return;
			}
			_gridCache[gridUid] = value;
		}
		if (value.TileSet.Count == 0)
		{
			return;
		}
		Vector2 vector = new Vector2(value.MinX, value.MinY) - gridPos;
		Vector2 vector2 = new Vector2(value.MaxX + 1, value.MaxY + 1) - gridPos;
		Vector2 vector3 = center + new Vector2(vector.X * num, (0f - vector2.Y) * num);
		Vector2 vector4 = center + new Vector2(vector2.X * num, (0f - vector.Y) * num);
		handle.DrawRect(new UIBox2(vector3, vector4), _floorColor, true);
		int num2 = (int)MathF.Floor(gridPos.X - worldRange) - 1;
		int num3 = (int)MathF.Ceiling(gridPos.X + worldRange) + 1;
		int num4 = (int)MathF.Floor(gridPos.Y - worldRange) - 1;
		int num5 = (int)MathF.Ceiling(gridPos.Y + worldRange) + 1;
		foreach (var (val3, val4) in value.TileColors)
		{
			if (val3.X >= num2 && val3.X <= num3 && val3.Y >= num4 && val3.Y <= num5)
			{
				Vector2 vector5 = Vector2i.op_Implicit(val3) - gridPos;
				Vector2 vector6 = center + new Vector2(vector5.X * num, (0f - vector5.Y) * num);
				float value2 = num / 2f;
				handle.DrawRect(new UIBox2(vector6 - new Vector2(value2), vector6 + new Vector2(value2)), val4, true);
			}
		}
		foreach (Vector2i wallTile in value.WallTiles)
		{
			if (wallTile.X >= num2 && wallTile.X <= num3 && wallTile.Y >= num4 && wallTile.Y <= num5)
			{
				Vector2 vector7 = Vector2i.op_Implicit(wallTile) - gridPos;
				Vector2 vector8 = center + new Vector2(vector7.X * num, (0f - vector7.Y) * num);
				float value3 = num / 2f;
				handle.DrawRect(new UIBox2(vector8 - new Vector2(value3), vector8 + new Vector2(value3)), _wallColor, true);
			}
		}
	}

	private GridCacheData? BuildGridCache(SharedMapSystem mapSystem, EntityUid gridUid, MapGridComponent grid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		GridTileEnumerator allTilesEnumerator = mapSystem.GetAllTilesEnumerator(gridUid, grid, true);
		GridCacheData gridCacheData = new GridCacheData
		{
			MinX = int.MaxValue,
			MinY = int.MaxValue,
			MaxX = int.MinValue,
			MaxY = int.MinValue
		};
		TileRef? val = default(TileRef?);
		while (((GridTileEnumerator)(ref allTilesEnumerator)).MoveNext(ref val))
		{
			Vector2i gridIndices = val.Value.GridIndices;
			gridCacheData.TileSet.Add(gridIndices);
			gridCacheData.MinX = Math.Min(gridCacheData.MinX, gridIndices.X);
			gridCacheData.MinY = Math.Min(gridCacheData.MinY, gridIndices.Y);
			gridCacheData.MaxX = Math.Max(gridCacheData.MaxX, gridIndices.X);
			gridCacheData.MaxY = Math.Max(gridCacheData.MaxY, gridIndices.Y);
			ContentTileDefinition contentTileDefinition = _turf.GetContentTileDefinition(val.Value.Tile);
			if (contentTileDefinition.MinimapColor != default(Color))
			{
				gridCacheData.TileColors[gridIndices] = contentTileDefinition.MinimapColor;
			}
		}
		if (gridCacheData.TileSet.Count == 0)
		{
			return null;
		}
		Vector2i item = default(Vector2i);
		foreach (Vector2i item2 in gridCacheData.TileSet)
		{
			bool flag = false;
			for (int i = -1; i <= 1; i++)
			{
				for (int j = -1; j <= 1; j++)
				{
					if (i != 0 || j != 0)
					{
						((Vector2i)(ref item))._002Ector(item2.X + i, item2.Y + j);
						if (!gridCacheData.TileSet.Contains(item))
						{
							flag = true;
							break;
						}
					}
				}
				if (flag)
				{
					break;
				}
			}
			if (flag)
			{
				gridCacheData.WallTiles.Add(item2);
			}
		}
		return gridCacheData;
	}

	private void DrawZones(DrawingHandleScreen handle, Vector2 center, Vector2 playerWorldPos)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		float worldRange = WorldRange;
		float scale = (float)((Control)this).PixelSize.X / (worldRange * 2f);
		if (ZoneCurrentCenter.HasValue)
		{
			DrawZoneCircle(handle, center, playerWorldPos, ZoneCurrentCenter.Value, ZoneCurrentRadius, scale, Color.CornflowerBlue);
		}
		Color val;
		if (ZoneNextCenter.HasValue)
		{
			Vector2 value = ZoneNextCenter.Value;
			float zoneNextRadius = ZoneNextRadius;
			val = Color.White;
			DrawZoneCircle(handle, center, playerWorldPos, value, zoneNextRadius, scale, ((Color)(ref val)).WithAlpha(0.5f));
			if (Vector2.Distance(playerWorldPos, ZoneNextCenter.Value) > ZoneNextRadius)
			{
				DrawZoneDirectionLine(handle, center, playerWorldPos, ZoneNextCenter.Value);
			}
		}
		if (RedZoneActive && RedZoneCenter.HasValue && RedZoneRadius > 0f)
		{
			Vector2 value2 = RedZoneCenter.Value;
			float redZoneRadius = RedZoneRadius;
			val = Color.Red;
			DrawFilledCircle(handle, center, playerWorldPos, value2, redZoneRadius, scale, ((Color)(ref val)).WithAlpha(0.3f));
		}
	}

	private void DrawZoneCircle(DrawingHandleScreen handle, Vector2 center, Vector2 playerWorldPos, Vector2 zoneCenter, float zoneRadius, float scale, Color color)
	{
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		Vector2 vector = zoneCenter - playerWorldPos;
		Vector2 vector2 = center + new Vector2(vector.X * scale, (0f - vector.Y) * scale);
		float num = zoneRadius * scale;
		for (int i = 0; i < 32; i++)
		{
			float x = (float)i / 32f * MathF.PI * 2f;
			float x2 = (float)(i + 1) / 32f * MathF.PI * 2f;
			Vector2 vector3 = vector2 + new Vector2(MathF.Cos(x) * num, MathF.Sin(x) * num);
			Vector2 vector4 = vector2 + new Vector2(MathF.Cos(x2) * num, MathF.Sin(x2) * num);
			((DrawingHandleBase)handle).DrawLine(vector3, vector4, color);
		}
	}

	private void DrawFilledCircle(DrawingHandleScreen handle, Vector2 center, Vector2 playerWorldPos, Vector2 zoneCenter, float zoneRadius, float scale, Color color)
	{
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		Vector2 vector = zoneCenter - playerWorldPos;
		Vector2 vector2 = center + new Vector2(vector.X * scale, (0f - vector.Y) * scale);
		float num = zoneRadius * scale;
		List<Vector2> list = new List<Vector2>();
		for (int i = 0; i < 32; i++)
		{
			float x = (float)i / 32f * MathF.PI * 2f;
			float x2 = (float)(i + 1) / 32f * MathF.PI * 2f;
			Vector2 item = vector2 + new Vector2(MathF.Cos(x) * num, MathF.Sin(x) * num);
			Vector2 item2 = vector2 + new Vector2(MathF.Cos(x2) * num, MathF.Sin(x2) * num);
			list.Add(vector2);
			list.Add(item);
			list.Add(item2);
		}
		((DrawingHandleBase)handle).DrawPrimitives((DrawPrimitiveTopology)1, (ReadOnlySpan<Vector2>)list.ToArray(), color);
	}

	private void DrawZoneDirectionLine(DrawingHandleScreen handle, Vector2 center, Vector2 playerWorldPos, Vector2 zoneCenter)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		Vector2 vector = zoneCenter - playerWorldPos;
		Vector2 vector2 = Vector2Helpers.Normalized(new Vector2(vector.X, 0f - vector.Y));
		float num = (float)Math.Min(((Control)this).PixelSize.X, ((Control)this).PixelSize.Y) / 2f - 10f;
		Vector2 vector3 = center + vector2 * num;
		Color white = Color.White;
		Color val = ((Color)(ref white)).WithAlpha(0.7f);
		((DrawingHandleBase)handle).DrawLine(center, vector3, Color.Black);
		((DrawingHandleBase)handle).DrawLine(center + vector2 * 2f, vector3 - vector2 * 2f, val);
		float num2 = 8f;
		Vector2 vector4 = vector3;
		Vector2 vector5 = new Vector2(vector2.Y, 0f - vector2.X);
		Vector2 vector6 = vector3 - vector2 * num2 + vector5 * num2 * 0.5f;
		Vector2 vector7 = vector3 - vector2 * num2 - vector5 * num2 * 0.5f;
		((DrawingHandleBase)handle).DrawPrimitives((DrawPrimitiveTopology)1, (ReadOnlySpan<Vector2>)new Vector2[3] { vector4, vector6, vector7 }, val);
		((DrawingHandleBase)handle).DrawPrimitives((DrawPrimitiveTopology)5, (ReadOnlySpan<Vector2>)new Vector2[4] { vector4, vector6, vector7, vector4 }, Color.Black);
	}

	private void DrawAirdrop(DrawingHandleScreen handle, Vector2 center, Vector2 playerWorldPos)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_0323: Unknown result type (might be due to invalid IL or missing references)
		if (AirdropCenter.HasValue)
		{
			float num = (float)((Control)this).PixelSize.X / (WorldRange * 2f);
			Vector2 vector = AirdropCenter.Value - playerWorldPos;
			Vector2 vector2 = center + new Vector2(vector.X * num, (0f - vector.Y) * num);
			float num2 = MathF.Max(GetAirdropMarkerSize(), 4.5f * _markerScale);
			float num3 = num2 + 2f * _markerScale;
			float num4 = num2 + 1f * _markerScale;
			Color val = Color.FromHex((ReadOnlySpan<char>)"#FF3B30", (Color?)null);
			Color val2 = Color.Black;
			((DrawingHandleBase)handle).DrawCircle(vector2, num3, ((Color)(ref val2)).WithAlpha(0.95f), true);
			val2 = Color.White;
			((DrawingHandleBase)handle).DrawCircle(vector2, num4, ((Color)(ref val2)).WithAlpha(0.95f), true);
			((DrawingHandleBase)handle).DrawCircle(vector2, num2, val, true);
			((DrawingHandleBase)handle).DrawCircle(vector2, num3, Color.Black, false);
			float num5 = num2 * 0.65f;
			Vector2 vector3 = vector2 + new Vector2(0f - num5, 0f);
			Vector2 vector4 = vector2 + new Vector2(num5, 0f);
			Vector2 vector5 = vector2 + new Vector2(0f, 0f - num5);
			Vector2 vector6 = vector2 + new Vector2(0f, num5);
			Vector2 vector7 = new Vector2(1f, 1f);
			Vector2 vector8 = vector3 + vector7;
			Vector2 vector9 = vector4 + vector7;
			val2 = Color.Black;
			((DrawingHandleBase)handle).DrawLine(vector8, vector9, ((Color)(ref val2)).WithAlpha(0.95f));
			Vector2 vector10 = vector5 + vector7;
			Vector2 vector11 = vector6 + vector7;
			val2 = Color.Black;
			((DrawingHandleBase)handle).DrawLine(vector10, vector11, ((Color)(ref val2)).WithAlpha(0.95f));
			((DrawingHandleBase)handle).DrawLine(vector3, vector4, Color.White);
			((DrawingHandleBase)handle).DrawLine(vector5, vector6, Color.White);
			string item = FormatTime(AirdropRemainingSeconds);
			string text = Loc.GetString("pubg-airdrop-countdown", new(string, object)[1] { ("time", item) });
			Vector2 vector12 = new Vector2(num3 + 2f * _markerScale, 0f - num3 - 2f * _markerScale);
			Vector2 vector13 = vector2 + vector12;
			Font scaledFont = GetScaledFont(10);
			val2 = Color.Black;
			Color val3 = ((Color)(ref val2)).WithAlpha(0.95f);
			Color val4 = Color.FromHex((ReadOnlySpan<char>)"#FFF2A8", (Color?)null);
			float num6 = 1f;
			handle.DrawString(scaledFont, vector13 + new Vector2(0f - num6, 0f - num6), text, val3);
			handle.DrawString(scaledFont, vector13 + new Vector2(num6, 0f - num6), text, val3);
			handle.DrawString(scaledFont, vector13 + new Vector2(0f - num6, num6), text, val3);
			handle.DrawString(scaledFont, vector13 + new Vector2(num6, num6), text, val3);
			handle.DrawString(scaledFont, vector13, text, val4);
			handle.DrawString(scaledFont, vector13 + new Vector2(0.75f, 0f), text, val4);
		}
	}

	private float GetAirdropMarkerSize()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		float num = 4f * _markerScale;
		float num2 = 0f;
		EntityUid val = default(EntityUid);
		MapGridComponent val2 = default(MapGridComponent);
		foreach (Entity<MapGridComponent> grid in _grids)
		{
			grid.Deconstruct(ref val, ref val2);
			Box2 localAABB = val2.LocalAABB;
			float num3 = MathF.Max(((Box2)(ref localAABB)).Width, ((Box2)(ref localAABB)).Height);
			if (num3 > num2)
			{
				num2 = num3;
			}
		}
		if (num2 <= 220f)
		{
			return num;
		}
		float num4 = Math.Clamp((num2 - 220f) / 380f, 0f, 1f);
		return num * (1f + num4 * 0.8f);
	}

	private void DrawRespawnTowers(DrawingHandleScreen handle, Vector2 center, Vector2 playerWorldPos)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		float num = (float)((Control)this).PixelSize.X / (WorldRange * 2f);
		foreach (Vector2 respawnTowerPosition in RespawnTowerPositions)
		{
			Vector2 vector = respawnTowerPosition - playerWorldPos;
			Vector2 vector2 = center + new Vector2(vector.X * num, (0f - vector.Y) * num);
			((DrawingHandleBase)handle).DrawCircle(vector2, 3f * _markerScale, Color.FromHex((ReadOnlySpan<char>)"#FFD54F", (Color?)null), true);
			((DrawingHandleBase)handle).DrawCircle(vector2, 3f * _markerScale, Color.Black, false);
		}
		foreach (Vector2 activeRespawnTowerPosition in ActiveRespawnTowerPositions)
		{
			Vector2 vector3 = activeRespawnTowerPosition - playerWorldPos;
			Vector2 vector4 = center + new Vector2(vector3.X * num, (0f - vector3.Y) * num);
			float num2 = 5f * _markerScale;
			Color val = Color.FromHex((ReadOnlySpan<char>)"#FF3B30", (Color?)null);
			((DrawingHandleBase)handle).DrawCircle(vector4, num2, ((Color)(ref val)).WithAlpha(0.35f), true);
			((DrawingHandleBase)handle).DrawCircle(vector4, 4f * _markerScale, Color.FromHex((ReadOnlySpan<char>)"#FF3B30", (Color?)null), false);
		}
	}

	private void DrawPartyMembers(DrawingHandleScreen handle, Vector2 center)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		if (PartyMembers == null || !_playerCoordinates.HasValue)
		{
			return;
		}
		MapCoordinates val = _transform.ToMapCoordinates(_playerCoordinates.Value, true);
		float num = (float)((Control)this).PixelSize.X / (WorldRange * 2f);
		foreach (PubgPartyMemberState partyMember in PartyMembers)
		{
			if (!(partyMember.Entity == _localNetEntity) && !(partyMember.MapId != val.MapId))
			{
				Vector2 vector = partyMember.Position - val.Position;
				Vector2 vector2 = center + new Vector2(vector.X * num, (0f - vector.Y) * num);
				Color val2 = GetPartyColor(partyMember.SlotIndex);
				Color val3 = ((Color)(ref val2)).WithAlpha(PartyMarkersOpacity);
				float num2 = 3f * _markerScale;
				((DrawingHandleBase)handle).DrawCircle(vector2, num2, val3, true);
				val2 = Color.Black;
				((DrawingHandleBase)handle).DrawCircle(vector2, num2, ((Color)(ref val2)).WithAlpha(PartyMarkersOpacity), false);
				Vector2 vector3 = vector2 + new Vector2(5f, -6f) * _markerScale;
				Font scaledFont = GetScaledFont();
				string username = partyMember.Username;
				val2 = Color.White;
				handle.DrawString(scaledFont, vector3, username, ((Color)(ref val2)).WithAlpha(PartyMarkersOpacity));
			}
		}
	}

	private void DrawPings(DrawingHandleScreen handle, Vector2 center)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		if (!_playerCoordinates.HasValue || ActivePings.Count == 0)
		{
			return;
		}
		MapCoordinates val = _transform.ToMapCoordinates(_playerCoordinates.Value, true);
		if (val.MapId == MapId.Nullspace)
		{
			return;
		}
		float num = (float)((Control)this).PixelSize.X / (WorldRange * 2f);
		foreach (PubgActivePingState activePing in ActivePings)
		{
			if (!(activePing.MapId != val.MapId))
			{
				Vector2 vector = activePing.Position - val.Position;
				Vector2 vector2 = center + new Vector2(vector.X * num, (0f - vector.Y) * num);
				Color color = PubgPartyPingColorResolver.GetColor(activePing.Source);
				switch (activePing.Kind)
				{
				case PubgPartyPingKind.Enemy:
				{
					float num3 = 4f * _markerScale * 1.2f;
					((DrawingHandleBase)handle).DrawCircle(vector2, num3, color, true);
					((DrawingHandleBase)handle).DrawCircle(vector2, num3 + 1f, _pingEnemyColor, false);
					((DrawingHandleBase)handle).DrawCircle(vector2, num3 + 2f, Color.Black, false);
					break;
				}
				case PubgPartyPingKind.Item:
					DrawItemPing(handle, vector2, activePing.ItemPrototypeId, color);
					break;
				default:
				{
					float num2 = 4f * _markerScale;
					((DrawingHandleBase)handle).DrawCircle(vector2, num2, color, true);
					((DrawingHandleBase)handle).DrawCircle(vector2, num2, Color.Black, false);
					break;
				}
				}
			}
		}
	}

	private void DrawCivGlobalMap(DrawingHandleScreen handle, Vector2 center)
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		if (!_playerCoordinates.HasValue || ((CivGlobalMapMarkers == null || CivGlobalMapMarkers.Count == 0) && (CivGlobalMapPoints == null || CivGlobalMapPoints.Count == 0) && (CivGlobalMapPlayers == null || CivGlobalMapPlayers.Count == 0)))
		{
			return;
		}
		MapCoordinates val = _transform.ToMapCoordinates(_playerCoordinates.Value, true);
		if (val.MapId == MapId.Nullspace)
		{
			return;
		}
		float num = (float)((Control)this).PixelSize.X / (WorldRange * 2f);
		if (CivGlobalMapMarkers != null)
		{
			foreach (CivGlobalMapMarkerState civGlobalMapMarker in CivGlobalMapMarkers)
			{
				if (!(civGlobalMapMarker.MapId != val.MapId))
				{
					Vector2 vector = civGlobalMapMarker.Position - val.Position;
					Vector2 screenPos = center + new Vector2(vector.X * num, (0f - vector.Y) * num);
					DrawCivGlobalMarker(handle, screenPos, civGlobalMapMarker.Type);
				}
			}
		}
		if (CivGlobalMapPoints != null)
		{
			foreach (CivPointCapturePointState civGlobalMapPoint in CivGlobalMapPoints)
			{
				if (!(civGlobalMapPoint.MapId != val.MapId))
				{
					Vector2 vector2 = civGlobalMapPoint.Position - val.Position;
					Vector2 screenPos2 = center + new Vector2(vector2.X * num, (0f - vector2.Y) * num);
					DrawCivGlobalPoint(handle, screenPos2, civGlobalMapPoint);
				}
			}
		}
		if (CivGlobalMapPlayers == null)
		{
			return;
		}
		TimeSpan curTime = _timing.CurTime;
		float num2 = (float)(curTime - _lastPlayerLerpTime).TotalSeconds;
		_lastPlayerLerpTime = curTime;
		float amount = ((num2 <= 0f) ? 1f : (1f - MathF.Exp((0f - num2) / 0.08f)));
		_civPlayerSeen.Clear();
		foreach (CivGlobalMapPlayerState civGlobalMapPlayer in CivGlobalMapPlayers)
		{
			if (!civGlobalMapPlayer.IsSelf && !(civGlobalMapPlayer.MapId != val.MapId))
			{
				Vector2 position = civGlobalMapPlayer.Position;
				Vector2 value;
				Vector2 vector3 = ((!_civPlayerSmoothed.TryGetValue(civGlobalMapPlayer.Name, out value)) ? position : (((position - value).Length() > 25f) ? position : Vector2.Lerp(value, position, amount)));
				_civPlayerSmoothed[civGlobalMapPlayer.Name] = vector3;
				_civPlayerSeen.Add(civGlobalMapPlayer.Name);
				Vector2 vector4 = vector3 - val.Position;
				Vector2 screenPos3 = center + new Vector2(vector4.X * num, (0f - vector4.Y) * num);
				DrawCivGlobalPlayer(handle, screenPos3, civGlobalMapPlayer);
			}
		}
		if (_civPlayerSmoothed.Count == _civPlayerSeen.Count)
		{
			return;
		}
		_civPlayerStale.Clear();
		foreach (string key in _civPlayerSmoothed.Keys)
		{
			if (!_civPlayerSeen.Contains(key))
			{
				_civPlayerStale.Add(key);
			}
		}
		foreach (string item in _civPlayerStale)
		{
			_civPlayerSmoothed.Remove(item);
		}
	}

	private void DrawCivGlobalMarker(DrawingHandleScreen handle, Vector2 screenPos, CivGlobalMapMarkerType type)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		Color val = (Color)(type switch
		{
			CivGlobalMapMarkerType.Attack => Color.FromHex((ReadOnlySpan<char>)"#ff5449", (Color?)null), 
			CivGlobalMapMarkerType.Defense => Color.FromHex((ReadOnlySpan<char>)"#5ca8ff", (Color?)null), 
			CivGlobalMapMarkerType.Enemy => Color.FromHex((ReadOnlySpan<char>)"#ff6d3f", (Color?)null), 
			CivGlobalMapMarkerType.Help => Color.FromHex((ReadOnlySpan<char>)"#ffd85a", (Color?)null), 
			CivGlobalMapMarkerType.Allies => Color.FromHex((ReadOnlySpan<char>)"#6de685", (Color?)null), 
			_ => Color.White, 
		});
		Color val2 = val;
		bool flag = type <= CivGlobalMapMarkerType.Defense;
		float num = (flag ? (5f * _markerScale) : (4f * _markerScale));
		if (type <= CivGlobalMapMarkerType.Defense)
		{
			Vector2 vector = screenPos + new Vector2(0f, (0f - num) * 1.35f);
			Vector2 vector2 = screenPos + new Vector2(0f - num, num);
			Vector2 vector3 = screenPos + new Vector2(num, num);
			((DrawingHandleBase)handle).DrawPrimitives((DrawPrimitiveTopology)1, (ReadOnlySpan<Vector2>)new Vector2[3] { vector, vector2, vector3 }, ((Color)(ref val2)).WithAlpha(0.95f));
			((DrawingHandleBase)handle).DrawPrimitives((DrawPrimitiveTopology)5, (ReadOnlySpan<Vector2>)new Vector2[4] { vector, vector2, vector3, vector }, Color.Black);
		}
		else
		{
			((DrawingHandleBase)handle).DrawCircle(screenPos, num, ((Color)(ref val2)).WithAlpha(0.95f), true);
			float num2 = num + 1f;
			val = Color.Black;
			((DrawingHandleBase)handle).DrawCircle(screenPos, num2, ((Color)(ref val)).WithAlpha(0.9f), false);
		}
	}

	private void DrawCivGlobalPlayer(DrawingHandleScreen handle, Vector2 screenPos, CivGlobalMapPlayerState player)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		Color playerColor = CivGlobalMapColorResolver.GetPlayerColor(CivViewerTeamId, CivViewerSquadId, player.TeamId, player.SquadId);
		float num = (player.IsSquadLeader ? (4.5f * _markerScale) : (4f * _markerScale));
		if (player.IsSquadLeader)
		{
			Vector2 vector = screenPos + new Vector2(0f, (0f - num) * 1.35f);
			Vector2 vector2 = screenPos + new Vector2(0f - num, num);
			Vector2 vector3 = screenPos + new Vector2(num, num);
			((DrawingHandleBase)handle).DrawPrimitives((DrawPrimitiveTopology)1, (ReadOnlySpan<Vector2>)new Vector2[3] { vector, vector2, vector3 }, ((Color)(ref playerColor)).WithAlpha(0.9f));
			((DrawingHandleBase)handle).DrawPrimitives((DrawPrimitiveTopology)5, (ReadOnlySpan<Vector2>)new Vector2[4] { vector, vector2, vector3, vector }, Color.Black);
		}
		else
		{
			((DrawingHandleBase)handle).DrawCircle(screenPos, num, ((Color)(ref playerColor)).WithAlpha(0.9f), true);
			float num2 = num + 1f;
			Color black = Color.Black;
			((DrawingHandleBase)handle).DrawCircle(screenPos, num2, ((Color)(ref black)).WithAlpha(0.8f), false);
		}
	}

	private void DrawCivGlobalPoint(DrawingHandleScreen handle, Vector2 screenPos, CivPointCapturePointState point)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		Color relationColor = CivPointCaptureColorResolver.GetRelationColor(CivViewerTeamId, point.OwnerTeamId);
		int num;
		Color val;
		if (point.CapturingTeamId != 0)
		{
			num = ((point.CaptureProgress > 0f) ? 1 : 0);
			if (num != 0)
			{
				val = CivPointCaptureColorResolver.GetCapturePulseColor(CivViewerTeamId, point.OwnerTeamId, point.CapturingTeamId, (float)_timing.CurTime.TotalSeconds);
				goto IL_005c;
			}
		}
		else
		{
			num = 0;
		}
		val = relationColor;
		goto IL_005c;
		IL_005c:
		Color val2 = val;
		float num2 = 6f * _markerScale;
		float num3 = 3.5f * _markerScale;
		float num4 = 1.75f * _markerScale;
		float num5 = num2;
		if (num != 0)
		{
			num5 += CivPointCaptureColorResolver.GetCapturePulseAmount((float)_timing.CurTime.TotalSeconds) * 2f * _markerScale;
		}
		((DrawingHandleBase)handle).DrawCircle(screenPos, num5, ((Color)(ref val2)).WithAlpha(0.14f), true);
		((DrawingHandleBase)handle).DrawCircle(screenPos, num5, ((Color)(ref val2)).WithAlpha(0.95f), false);
		((DrawingHandleBase)handle).DrawCircle(screenPos, num3, ((Color)(ref val2)).WithAlpha(0.75f), false);
		((DrawingHandleBase)handle).DrawCircle(screenPos, num4, ((Color)(ref relationColor)).WithAlpha(0.95f), true);
		float num6 = num5 + 1f;
		Color black = Color.Black;
		((DrawingHandleBase)handle).DrawCircle(screenPos, num6, ((Color)(ref black)).WithAlpha(0.8f), false);
		if (num != 0)
		{
			Color relationColor2 = CivPointCaptureColorResolver.GetRelationColor(CivViewerTeamId, point.CapturingTeamId);
			DrawProgressArc(handle, screenPos, num5 + 2f, point.CaptureProgress, relationColor2);
		}
		if (!string.IsNullOrWhiteSpace(point.Label))
		{
			Vector2 vector = screenPos + new Vector2(num2 + 2f * _markerScale, 0f - num2 - 1f * _markerScale);
			Font scaledFont = GetScaledFont();
			Vector2 vector2 = vector + Vector2.One;
			string label = point.Label;
			black = Color.Black;
			handle.DrawString(scaledFont, vector2, label, ((Color)(ref black)).WithAlpha(0.85f));
			handle.DrawString(scaledFont, vector, point.Label, val2);
		}
	}

	private void DrawCivGrid(DrawingHandleScreen handle, Vector2 center)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		if (!CivHasBounds || !_playerCoordinates.HasValue)
		{
			return;
		}
		MapCoordinates val = _transform.ToMapCoordinates(_playerCoordinates.Value, true);
		if (val.MapId == MapId.Nullspace)
		{
			return;
		}
		float num = (float)((Control)this).PixelSize.X / (WorldRange * 2f);
		Vector2 position = val.Position;
		Color val2 = default(Color);
		((Color)(ref val2))._002Ector((byte)130, (byte)150, (byte)175, (byte)120);
		Color color = Color.FromHex((ReadOnlySpan<char>)"#FFE9A8", (Color?)null);
		for (int i = 0; i <= 8; i++)
		{
			float num2 = center.X + (CivMapGrid.LineX(CivBoundsMin, CivBoundsMax, i) - position.X) * num;
			if (!(num2 < 0f) && !(num2 > (float)((Control)this).PixelSize.X))
			{
				((DrawingHandleBase)handle).DrawLine(new Vector2(num2, 0f), new Vector2(num2, ((Control)this).PixelSize.Y), val2);
			}
		}
		for (int j = 0; j <= 8; j++)
		{
			float num3 = center.Y - (CivMapGrid.LineY(CivBoundsMin, CivBoundsMax, j) - position.Y) * num;
			if (!(num3 < 0f) && !(num3 > (float)((Control)this).PixelSize.Y))
			{
				((DrawingHandleBase)handle).DrawLine(new Vector2(0f, num3), new Vector2(((Control)this).PixelSize.X, num3), val2);
			}
		}
		Font scaledFont = GetScaledFont(11);
		for (int k = 0; k < 8; k++)
		{
			float num4 = center.X + (CivMapGrid.ColumnCenterX(CivBoundsMin, CivBoundsMax, k) - position.X) * num;
			if (!(num4 < 4f) && !(num4 > (float)((Control)this).PixelSize.X - 4f))
			{
				DrawLabelOutlined(handle, scaledFont, new Vector2(num4 - 3f, 1f), CivMapGrid.GetColumnLabel(k), color);
			}
		}
		for (int l = 0; l < 8; l++)
		{
			float num5 = center.Y - (CivMapGrid.RowCenterY(CivBoundsMin, CivBoundsMax, l) - position.Y) * num;
			if (!(num5 < 4f) && !(num5 > (float)((Control)this).PixelSize.Y - 4f))
			{
				DrawLabelOutlined(handle, scaledFont, new Vector2(1f, num5 - 6f), (l + 1).ToString(), color);
			}
		}
	}

	private void DrawLabelOutlined(DrawingHandleScreen handle, Font font, Vector2 pos, string text, Color color)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		Color black = Color.Black;
		Color val = ((Color)(ref black)).WithAlpha(0.95f);
		handle.DrawString(font, pos + new Vector2(-1f, 0f), text, val);
		handle.DrawString(font, pos + new Vector2(1f, 0f), text, val);
		handle.DrawString(font, pos + new Vector2(0f, -1f), text, val);
		handle.DrawString(font, pos + new Vector2(0f, 1f), text, val);
		handle.DrawString(font, pos, text, color);
	}

	private void DrawCivOrders(DrawingHandleScreen handle, Vector2 center)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		if (CivGlobalMapOrders == null || CivGlobalMapOrders.Count == 0 || !_playerCoordinates.HasValue)
		{
			return;
		}
		MapCoordinates val = _transform.ToMapCoordinates(_playerCoordinates.Value, true);
		if (val.MapId == MapId.Nullspace)
		{
			return;
		}
		float num = (float)((Control)this).PixelSize.X / (WorldRange * 2f);
		foreach (CivCommanderOrderState civGlobalMapOrder in CivGlobalMapOrders)
		{
			if (civGlobalMapOrder.Order != CivCommanderOrderType.None && !(civGlobalMapOrder.MapId != val.MapId))
			{
				Vector2 vector = civGlobalMapOrder.Position - val.Position;
				Vector2 vector2 = center + new Vector2(vector.X * num, (0f - vector.Y) * num);
				Color orderColor = GetOrderColor(civGlobalMapOrder.Order);
				if (vector2.X >= 0f && vector2.X <= (float)((Control)this).PixelSize.X && vector2.Y >= 0f && vector2.Y <= (float)((Control)this).PixelSize.Y)
				{
					DrawOrderMarker(handle, vector2, civGlobalMapOrder.Order, orderColor);
				}
				else if (CivViewerSquadId != 0 && civGlobalMapOrder.SquadId == CivViewerSquadId)
				{
					DrawEdgeArrow(handle, center, vector2, orderColor);
				}
			}
		}
	}

	private void DrawOrderMarker(DrawingHandleScreen handle, Vector2 screenPos, CivCommanderOrderType order, Color color)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		float num = 5f * _markerScale;
		((DrawingHandleBase)handle).DrawCircle(screenPos, num, ((Color)(ref color)).WithAlpha(0.2f), true);
		((DrawingHandleBase)handle).DrawCircle(screenPos, num, color, false);
		float num2 = num + 1f;
		Color black = Color.Black;
		((DrawingHandleBase)handle).DrawCircle(screenPos, num2, ((Color)(ref black)).WithAlpha(0.8f), false);
		string orderShort = GetOrderShort(order);
		Font scaledFont = GetScaledFont();
		Vector2 vector = screenPos + new Vector2(num + 1f, 0f - num - 1f);
		Vector2 vector2 = vector + Vector2.One;
		black = Color.Black;
		handle.DrawString(scaledFont, vector2, orderShort, ((Color)(ref black)).WithAlpha(0.85f));
		handle.DrawString(scaledFont, vector, orderShort, color);
	}

	private void DrawCivSquadLeaderArrow(DrawingHandleScreen handle, Vector2 center)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		if (CivGlobalMapPlayers == null || CivViewerSquadId == 0 || !_playerCoordinates.HasValue)
		{
			return;
		}
		MapCoordinates val = _transform.ToMapCoordinates(_playerCoordinates.Value, true);
		if (val.MapId == MapId.Nullspace)
		{
			return;
		}
		float num = (float)((Control)this).PixelSize.X / (WorldRange * 2f);
		foreach (CivGlobalMapPlayerState civGlobalMapPlayer in CivGlobalMapPlayers)
		{
			if (!civGlobalMapPlayer.IsSelf && civGlobalMapPlayer.IsSquadLeader && civGlobalMapPlayer.SquadId == CivViewerSquadId && !(civGlobalMapPlayer.MapId != val.MapId))
			{
				Vector2 vector = civGlobalMapPlayer.Position - val.Position;
				Vector2 targetScreen = center + new Vector2(vector.X * num, (0f - vector.Y) * num);
				if (!(targetScreen.X >= 0f) || !(targetScreen.X <= (float)((Control)this).PixelSize.X) || !(targetScreen.Y >= 0f) || !(targetScreen.Y <= (float)((Control)this).PixelSize.Y))
				{
					DrawEdgeArrow(handle, center, targetScreen, CivGlobalMapColorResolver.SquadColor);
				}
				break;
			}
		}
	}

	private void DrawCivDeaths(DrawingHandleScreen handle, Vector2 center)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		if (CivGlobalMapDeaths == null || CivGlobalMapDeaths.Count == 0 || !_playerCoordinates.HasValue)
		{
			return;
		}
		MapCoordinates val = _transform.ToMapCoordinates(_playerCoordinates.Value, true);
		if (val.MapId == MapId.Nullspace)
		{
			return;
		}
		float num = (float)((Control)this).PixelSize.X / (WorldRange * 2f);
		foreach (CivGlobalMapDeathState civGlobalMapDeath in CivGlobalMapDeaths)
		{
			if (!(civGlobalMapDeath.MapId != val.MapId))
			{
				Vector2 vector = civGlobalMapDeath.Position - val.Position;
				Vector2 vector2 = center + new Vector2(vector.X * num, (0f - vector.Y) * num);
				float num2 = ((civGlobalMapDeath.LifetimeSeconds > 0f) ? Math.Clamp(civGlobalMapDeath.RemainingSeconds / civGlobalMapDeath.LifetimeSeconds, 0f, 1f) : 1f);
				float num3 = 0.25f + 0.65f * num2;
				Color playerColor = CivGlobalMapColorResolver.GetPlayerColor(CivViewerTeamId, CivViewerSquadId, civGlobalMapDeath.TeamId, civGlobalMapDeath.SquadId);
				Color val2 = ((Color)(ref playerColor)).WithAlpha(num3);
				float num4 = 3.5f * _markerScale;
				((DrawingHandleBase)handle).DrawLine(vector2 + new Vector2(0f - num4, 0f - num4), vector2 + new Vector2(num4, num4), val2);
				((DrawingHandleBase)handle).DrawLine(vector2 + new Vector2(0f - num4, num4), vector2 + new Vector2(num4, 0f - num4), val2);
			}
		}
	}

	private void DrawEdgeArrow(DrawingHandleScreen handle, Vector2 center, Vector2 targetScreen, Color color)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		Vector2 vector = targetScreen - center;
		if (!(vector.LengthSquared() < 0.001f))
		{
			Vector2 vector2 = Vector2Helpers.Normalized(vector);
			float num = (float)Math.Min(((Control)this).PixelSize.X, ((Control)this).PixelSize.Y) / 2f - 8f;
			Vector2 vector3 = center + vector2 * num;
			float num2 = 7f * _markerScale;
			Vector2 vector4 = new Vector2(vector2.Y, 0f - vector2.X);
			Vector2 vector5 = vector3;
			Vector2 vector6 = vector3 - vector2 * num2 + vector4 * num2 * 0.5f;
			Vector2 vector7 = vector3 - vector2 * num2 - vector4 * num2 * 0.5f;
			Color black = Color.Black;
			((DrawingHandleBase)handle).DrawLine(center, vector3, ((Color)(ref black)).WithAlpha(0.4f));
			((DrawingHandleBase)handle).DrawPrimitives((DrawPrimitiveTopology)1, (ReadOnlySpan<Vector2>)new Vector2[3] { vector5, vector6, vector7 }, color);
			((DrawingHandleBase)handle).DrawPrimitives((DrawPrimitiveTopology)5, (ReadOnlySpan<Vector2>)new Vector2[4] { vector5, vector6, vector7, vector5 }, Color.Black);
		}
	}

	private static void DrawProgressArc(DrawingHandleScreen handle, Vector2 arcCenter, float radius, float progress, Color color)
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
				array[i] = arcCenter + new Vector2(MathF.Cos(x), MathF.Sin(x)) * radius;
			}
			((DrawingHandleBase)handle).DrawPrimitives((DrawPrimitiveTopology)5, (ReadOnlySpan<Vector2>)array, color);
		}
	}

	private static Color GetOrderColor(CivCommanderOrderType order)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		return (Color)(order switch
		{
			CivCommanderOrderType.Attack => Color.FromHex((ReadOnlySpan<char>)"#ff5449", (Color?)null), 
			CivCommanderOrderType.Defense => Color.FromHex((ReadOnlySpan<char>)"#5ca8ff", (Color?)null), 
			CivCommanderOrderType.Artillery => Color.FromHex((ReadOnlySpan<char>)"#ffd85a", (Color?)null), 
			_ => Color.White, 
		});
	}

	private static string GetOrderShort(CivCommanderOrderType order)
	{
		return order switch
		{
			CivCommanderOrderType.Attack => "ATK", 
			CivCommanderOrderType.Defense => "DEF", 
			CivCommanderOrderType.Artillery => "ART", 
			_ => "ORD", 
		};
	}

	private void DrawItemPing(DrawingHandleScreen handle, Vector2 screenPos, string? prototypeId, Color color)
	{
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		Texture val = TryGetItemPingTexture(prototypeId);
		if (val == null)
		{
			DrawFallbackItemPing(handle, screenPos, color);
			return;
		}
		Vector2 vector = new Vector2(5f, 5f) * _markerScale;
		UIBox2 val2 = default(UIBox2);
		((UIBox2)(ref val2))._002Ector(screenPos - vector - Vector2.One, screenPos + vector + Vector2.One);
		UIBox2 val3 = default(UIBox2);
		((UIBox2)(ref val3))._002Ector(screenPos - vector, screenPos + vector);
		UIBox2 val4 = val2;
		Color black = Color.Black;
		handle.DrawRect(val4, ((Color)(ref black)).WithAlpha(0.65f), true);
		handle.DrawTextureRect(val, val3, (Color?)null);
		((DrawingHandleBase)handle).DrawCircle(screenPos, vector.X + 1f, color, false);
	}

	private void DrawFallbackItemPing(DrawingHandleScreen handle, Vector2 screenPos, Color color)
	{
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		float num = 4f * _markerScale * 1.2f;
		Vector2 vector = screenPos + new Vector2(0f, 0f - num);
		Vector2 vector2 = screenPos + new Vector2(num, 0f);
		Vector2 vector3 = screenPos + new Vector2(0f, num);
		Vector2 vector4 = screenPos + new Vector2(0f - num, 0f);
		((DrawingHandleBase)handle).DrawPrimitives((DrawPrimitiveTopology)1, (ReadOnlySpan<Vector2>)new Vector2[6] { vector, vector2, vector3, vector, vector3, vector4 }, color);
		((DrawingHandleBase)handle).DrawPrimitives((DrawPrimitiveTopology)5, (ReadOnlySpan<Vector2>)new Vector2[5] { vector, vector2, vector3, vector4, vector }, Color.Black);
	}

	private Texture? TryGetItemPingTexture(string? prototypeId)
	{
		if (string.IsNullOrWhiteSpace(prototypeId))
		{
			return null;
		}
		if (_itemPingIconCache.TryGetValue(prototypeId, out Texture value))
		{
			return value;
		}
		EntityPrototype val = default(EntityPrototype);
		if (!_prototype.TryIndex<EntityPrototype>(prototypeId, ref val))
		{
			_itemPingIconCache[prototypeId] = null;
			return null;
		}
		Texture val2 = ((IDirectionalTextureProvider)_sprite.GetPrototypeIcon(val)).Default;
		_itemPingIconCache[prototypeId] = val2;
		return val2;
	}

	private Font GetScaledFont(int baseFontSize = 6)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Expected O, but got Unknown
		int num = Math.Clamp((int)MathF.Round((float)baseFontSize * _markerScale), 4, 36);
		if (_fontCache.TryGetValue(num, out Font value))
		{
			return value;
		}
		VectorFont val = new VectorFont(_cache.GetResource<FontResource>("/Fonts/NotoSans/NotoSans-Regular.ttf", true), num);
		_fontCache[num] = (Font)(object)val;
		return (Font)(object)val;
	}

	private static Color GetPartyColor(int slotIndex)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		return (Color)(slotIndex switch
		{
			1 => Color.FromHex((ReadOnlySpan<char>)"#00bcd4", (Color?)null), 
			2 => Color.FromHex((ReadOnlySpan<char>)"#ffeb3b", (Color?)null), 
			3 => Color.FromHex((ReadOnlySpan<char>)"#ff9800", (Color?)null), 
			_ => Color.FromHex((ReadOnlySpan<char>)"#4caf50", (Color?)null), 
		});
	}

	private static string FormatTime(int seconds)
	{
		if (seconds < 0)
		{
			seconds = 0;
		}
		int value = seconds / 60;
		int value2 = seconds % 60;
		return $"{value:D2}:{value2:D2}";
	}

	protected override void KeyBindDown(GUIBoundKeyEventArgs args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		base.KeyBindDown(args);
		if (!((BoundKeyEventArgs)args).Handled && !(((BoundKeyEventArgs)args).Function != EngineKeyFunctions.UIClick) && OnMapClick != null)
		{
			Vector2 localPosition = ((BoundKeyEventArgs)args).PointerLocation.Position - Vector2i.op_Implicit(((Control)this).GlobalPixelPosition);
			if (TryGetMapCoordinates(localPosition, out var mapCoordinates))
			{
				OnMapClick(mapCoordinates);
				((BoundKeyEventArgs)args).Handle();
			}
		}
	}

	private bool TryGetMapCoordinates(Vector2 localPosition, out MapCoordinates mapCoordinates)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		mapCoordinates = default(MapCoordinates);
		if (!_playerCoordinates.HasValue)
		{
			return false;
		}
		if (localPosition.X < 0f || localPosition.Y < 0f || localPosition.X > (float)((Control)this).PixelSize.X || localPosition.Y > (float)((Control)this).PixelSize.Y)
		{
			return false;
		}
		MapCoordinates val = _transform.ToMapCoordinates(_playerCoordinates.Value, true);
		if (val.MapId == MapId.Nullspace)
		{
			return false;
		}
		float num = (float)((Control)this).PixelSize.X / (WorldRange * 2f);
		if (num <= 0f)
		{
			return false;
		}
		Vector2 vector = ((Control)this).PixelSize / 2f;
		Vector2 vector2 = localPosition - vector;
		Vector2 vector3 = val.Position + new Vector2(vector2.X / num, (0f - vector2.Y) / num);
		mapCoordinates = new MapCoordinates(vector3, val.MapId);
		return true;
	}

	private void DrawPlayerMarker(DrawingHandleScreen handle, Vector2 center, Angle playerRotation)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		float num = 8f * _markerScale;
		Vector2 vector = new Vector2(0f, 0f - num);
		Vector2 vector2 = new Vector2((0f - num) * 0.7f, num * 0.5f);
		Vector2 vector3 = new Vector2(num * 0.7f, num * 0.5f);
		Angle val = -playerRotation + Angle.FromDegrees(180.0);
		vector = ((Angle)(ref val)).RotateVec(ref vector) + center;
		vector2 = ((Angle)(ref val)).RotateVec(ref vector2) + center;
		vector3 = ((Angle)(ref val)).RotateVec(ref vector3) + center;
		((DrawingHandleBase)handle).DrawPrimitives((DrawPrimitiveTopology)1, (ReadOnlySpan<Vector2>)new Vector2[3] { vector, vector2, vector3 }, _playerMarkerColor);
		((DrawingHandleBase)handle).DrawPrimitives((DrawPrimitiveTopology)5, (ReadOnlySpan<Vector2>)new Vector2[4] { vector, vector2, vector3, vector }, Color.Black);
	}
}
