// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.UserInterface.Systems.Minimap.PubgMinimapControl
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

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
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client._PUBG.UserInterface.Systems.Minimap;

public sealed class PubgMinimapControl : BaseShuttleControl
{
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
  private readonly Dictionary<EntityUid, PubgMinimapControl.GridCacheData> _gridCache = new Dictionary<EntityUid, PubgMinimapControl.GridCacheData>();
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
  public IReadOnlyList<PubgActivePingState> ActivePings = (IReadOnlyList<PubgActivePingState>) Array.Empty<PubgActivePingState>();
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
  public IReadOnlyList<Vector2> RespawnTowerPositions = (IReadOnlyList<Vector2>) Array.Empty<Vector2>();
  public IReadOnlyList<Vector2> ActiveRespawnTowerPositions = (IReadOnlyList<Vector2>) Array.Empty<Vector2>();
  private readonly Color _wallColor = Color.FromHex((ReadOnlySpan<char>) "#8B4513", new Color?());
  private readonly Color _floorColor = Color.FromHex((ReadOnlySpan<char>) "#2a2a2a", new Color?());
  private readonly Color _spaceColor = Color.FromHex((ReadOnlySpan<char>) "#000000", new Color?());
  private readonly Color _playerMarkerColor = Color.FromHex((ReadOnlySpan<char>) "#00ff00", new Color?());
  private readonly Color _pingEnemyColor = Color.FromHex((ReadOnlySpan<char>) "#FF3B30", new Color?());
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
    IoCManager.InjectDependencies<PubgMinimapControl>(this);
    this._transform = this.EntManager.System<SharedTransformSystem>();
    this._turf = this.EntManager.System<TurfSystem>();
    this._sprite = this.EntManager.System<SpriteSystem>();
    this._vehicles = this.EntManager.System<VehicleSystem>();
    ((Control) this).SetSize = new Vector2(200f, 200f);
    ((Control) this).MouseFilter = (Control.MouseFilterMode) 0;
    this._cfg.OnValueChanged<float>(CCVars.PubgMinimapZoom, new Action<float>(this.OnZoomChanged), true);
    this._cfg.OnValueChanged<int>(CCVars.PubgMinimapMarkerScale, new Action<int>(this.OnMarkerScaleChanged), true);
  }

  private void OnZoomChanged(float _) => this.UpdateZoom();

  private void OnMarkerScaleChanged(int markerScalePercent)
  {
    this._markerScale = Math.Clamp((float) markerScalePercent / 100f, 0.25f, 4f);
  }

  private void UpdateZoom()
  {
    this.WorldRange = 32f * this._cfg.GetCVar<float>(CCVars.PubgMinimapZoom);
  }

  protected virtual void ExitedTree()
  {
    ((Control) this).ExitedTree();
    this._cfg.UnsubValueChanged<float>(CCVars.PubgMinimapZoom, new Action<float>(this.OnZoomChanged));
    this._cfg.UnsubValueChanged<int>(CCVars.PubgMinimapMarkerScale, new Action<int>(this.OnMarkerScaleChanged));
  }

  public void UpdatePlayerPosition(EntityUid player)
  {
    EntityUid displayEntity;
    if (!this._vehicles.TryGetDisplayEntity(player, out displayEntity))
      displayEntity = player;
    TransformComponent transformComponent;
    if (!this.EntManager.TryGetComponent<TransformComponent>(displayEntity, ref transformComponent))
      return;
    this._localNetEntity = this.EntManager.GetNetEntity(player, (MetaDataComponent) null);
    this._playerCoordinates = new EntityCoordinates?(transformComponent.Coordinates);
    this._playerRotation = new Angle?(transformComponent.LocalRotation);
    this._grids.Clear();
    EntityUid? gridUid = transformComponent.GridUid;
    EntityUid? lastGridUid = this._lastGridUid;
    EntityUid? nullable = gridUid;
    if ((lastGridUid.HasValue == nullable.HasValue ? (lastGridUid.HasValue ? (EntityUid.op_Inequality(lastGridUid.GetValueOrDefault(), nullable.GetValueOrDefault()) ? 1 : 0) : 0) : 1) != 0)
    {
      this._gridCache.Clear();
      this._lastGridUid = gridUid;
    }
    MapGridComponent mapGridComponent;
    if (!gridUid.HasValue || !this.EntManager.TryGetComponent<MapGridComponent>(gridUid.Value, ref mapGridComponent))
      return;
    this._grids.Add(Entity<MapGridComponent>.op_Implicit((gridUid.Value, mapGridComponent)));
  }

  protected override void Draw(DrawingHandleScreen handle)
  {
    base.Draw(handle);
    if (!this._playerCoordinates.HasValue || !this._playerRotation.HasValue)
      return;
    if (!this.EntManager.EntityExists(this._playerCoordinates.Value.EntityId))
    {
      this._playerCoordinates = new EntityCoordinates?();
    }
    else
    {
      handle.DrawRect(new UIBox2(Vector2.Zero, Vector2i.op_Implicit(((Control) this).PixelSize)), this._spaceColor, true);
      Vector2 center = Vector2i.op_Division(((Control) this).PixelSize, 2f);
      foreach (Entity<MapGridComponent> grid1 in this._grids)
      {
        EntityUid entityUid;
        MapGridComponent mapGridComponent;
        grid1.Deconstruct(ref entityUid, ref mapGridComponent);
        EntityUid gridUid = entityUid;
        MapGridComponent grid2 = mapGridComponent;
        TransformComponent transformComponent;
        if (this.EntManager.TryGetComponent<TransformComponent>(gridUid, ref transformComponent))
        {
          Matrix3x2 invWorldMatrix = this._transform.GetInvWorldMatrix(transformComponent);
          Vector2 gridPos = Vector2.Transform(this._transform.ToMapCoordinates(this._playerCoordinates.Value, true).Position, invWorldMatrix);
          this.DrawGrid(handle, center, gridUid, grid2, gridPos);
        }
      }
      this.DrawCivGrid(handle, center);
      if (this.ZoneActive && this.ZoneVisible && this.ZoneCurrentCenter.HasValue)
      {
        MapCoordinates mapCoordinates = this._transform.ToMapCoordinates(this._playerCoordinates.Value, true);
        if (MapId.op_Equality(mapCoordinates.MapId, this.ZoneMapId))
          this.DrawZones(handle, center, new Vector2(mapCoordinates.Position.X, mapCoordinates.Position.Y));
      }
      if (this.AirdropActive && this.AirdropCenter.HasValue)
      {
        MapCoordinates mapCoordinates = this._transform.ToMapCoordinates(this._playerCoordinates.Value, true);
        if (MapId.op_Equality(mapCoordinates.MapId, this.AirdropMapId))
          this.DrawAirdrop(handle, center, new Vector2(mapCoordinates.Position.X, mapCoordinates.Position.Y));
      }
      if (this.RespawnTowerPositions.Count > 0)
      {
        MapCoordinates mapCoordinates = this._transform.ToMapCoordinates(this._playerCoordinates.Value, true);
        if (MapId.op_Equality(mapCoordinates.MapId, this.RespawnTowerMapId))
          this.DrawRespawnTowers(handle, center, new Vector2(mapCoordinates.Position.X, mapCoordinates.Position.Y));
      }
      this.DrawPings(handle, center);
      this.DrawPartyMembers(handle, center);
      this.DrawCivGlobalMap(handle, center);
      this.DrawCivOrders(handle, center);
      this.DrawCivSquadLeaderArrow(handle, center);
      this.DrawCivDeaths(handle, center);
      this.DrawPlayerMarker(handle, center, this._playerRotation.Value);
    }
  }

  private void DrawGrid(
    DrawingHandleScreen handle,
    Vector2 center,
    EntityUid gridUid,
    MapGridComponent grid,
    Vector2 gridPos)
  {
    float worldRange = this.WorldRange;
    SharedMapSystem mapSystem = this.EntManager.System<SharedMapSystem>();
    float num1 = (float) ((Control) this).PixelSize.X / (worldRange * 2f);
    PubgMinimapControl.GridCacheData gridCacheData;
    if (!this._gridCache.TryGetValue(gridUid, out gridCacheData))
    {
      gridCacheData = this.BuildGridCache(mapSystem, gridUid, grid);
      if (gridCacheData == null)
        return;
      this._gridCache[gridUid] = gridCacheData;
    }
    if (gridCacheData.TileSet.Count == 0)
      return;
    Vector2 vector2_1 = new Vector2((float) gridCacheData.MinX, (float) gridCacheData.MinY) - gridPos;
    Vector2 vector2_2 = new Vector2((float) (gridCacheData.MaxX + 1), (float) (gridCacheData.MaxY + 1)) - gridPos;
    Vector2 vector2_3 = center + new Vector2(vector2_1.X * num1, -vector2_2.Y * num1);
    Vector2 vector2_4 = center + new Vector2(vector2_2.X * num1, -vector2_1.Y * num1);
    handle.DrawRect(new UIBox2(vector2_3, vector2_4), this._floorColor, true);
    int num2 = (int) MathF.Floor(gridPos.X - worldRange) - 1;
    int num3 = (int) MathF.Ceiling(gridPos.X + worldRange) + 1;
    int num4 = (int) MathF.Floor(gridPos.Y - worldRange) - 1;
    int num5 = (int) MathF.Ceiling(gridPos.Y + worldRange) + 1;
    foreach ((Vector2i key, Color color) in gridCacheData.TileColors)
    {
      if (key.X >= num2 && key.X <= num3 && key.Y >= num4 && key.Y <= num5)
      {
        Vector2 vector2_5 = Vector2i.op_Implicit(key) - gridPos;
        Vector2 vector2_6 = center + new Vector2(vector2_5.X * num1, -vector2_5.Y * num1);
        float num6 = num1 / 2f;
        handle.DrawRect(new UIBox2(vector2_6 - new Vector2(num6), vector2_6 + new Vector2(num6)), color, true);
      }
    }
    foreach (Vector2i wallTile in gridCacheData.WallTiles)
    {
      if (wallTile.X >= num2 && wallTile.X <= num3 && wallTile.Y >= num4 && wallTile.Y <= num5)
      {
        Vector2 vector2_7 = Vector2i.op_Implicit(wallTile) - gridPos;
        Vector2 vector2_8 = center + new Vector2(vector2_7.X * num1, -vector2_7.Y * num1);
        float num7 = num1 / 2f;
        handle.DrawRect(new UIBox2(vector2_8 - new Vector2(num7), vector2_8 + new Vector2(num7)), this._wallColor, true);
      }
    }
  }

  private PubgMinimapControl.GridCacheData? BuildGridCache(
    SharedMapSystem mapSystem,
    EntityUid gridUid,
    MapGridComponent grid)
  {
    GridTileEnumerator allTilesEnumerator = mapSystem.GetAllTilesEnumerator(gridUid, grid, true);
    PubgMinimapControl.GridCacheData gridCacheData = new PubgMinimapControl.GridCacheData()
    {
      MinX = int.MaxValue,
      MinY = int.MaxValue,
      MaxX = int.MinValue,
      MaxY = int.MinValue
    };
    TileRef? nullable;
    while (((GridTileEnumerator) ref allTilesEnumerator).MoveNext(ref nullable))
    {
      Vector2i gridIndices = nullable.Value.GridIndices;
      gridCacheData.TileSet.Add(gridIndices);
      gridCacheData.MinX = Math.Min(gridCacheData.MinX, gridIndices.X);
      gridCacheData.MinY = Math.Min(gridCacheData.MinY, gridIndices.Y);
      gridCacheData.MaxX = Math.Max(gridCacheData.MaxX, gridIndices.X);
      gridCacheData.MaxY = Math.Max(gridCacheData.MaxY, gridIndices.Y);
      ContentTileDefinition contentTileDefinition = this._turf.GetContentTileDefinition(nullable.Value.Tile);
      if (Color.op_Inequality(contentTileDefinition.MinimapColor, new Color()))
        gridCacheData.TileColors[gridIndices] = contentTileDefinition.MinimapColor;
    }
    if (gridCacheData.TileSet.Count == 0)
      return (PubgMinimapControl.GridCacheData) null;
    foreach (Vector2i tile in gridCacheData.TileSet)
    {
      bool flag = false;
      for (int index1 = -1; index1 <= 1; ++index1)
      {
        for (int index2 = -1; index2 <= 1; ++index2)
        {
          if (index1 != 0 || index2 != 0)
          {
            Vector2i vector2i;
            // ISSUE: explicit constructor call
            ((Vector2i) ref vector2i).\u002Ector(tile.X + index1, tile.Y + index2);
            if (!gridCacheData.TileSet.Contains(vector2i))
            {
              flag = true;
              break;
            }
          }
        }
        if (flag)
          break;
      }
      if (flag)
        gridCacheData.WallTiles.Add(tile);
    }
    return gridCacheData;
  }

  private void DrawZones(DrawingHandleScreen handle, Vector2 center, Vector2 playerWorldPos)
  {
    float scale1 = (float) ((Control) this).PixelSize.X / (this.WorldRange * 2f);
    if (this.ZoneCurrentCenter.HasValue)
      this.DrawZoneCircle(handle, center, playerWorldPos, this.ZoneCurrentCenter.Value, this.ZoneCurrentRadius, scale1, Color.CornflowerBlue);
    if (this.ZoneNextCenter.HasValue)
    {
      DrawingHandleScreen handle1 = handle;
      Vector2 center1 = center;
      Vector2 playerWorldPos1 = playerWorldPos;
      Vector2 zoneCenter = this.ZoneNextCenter.Value;
      double zoneNextRadius = (double) this.ZoneNextRadius;
      double scale2 = (double) scale1;
      Color white = Color.White;
      Color color = ((Color) ref white).WithAlpha(0.5f);
      this.DrawZoneCircle(handle1, center1, playerWorldPos1, zoneCenter, (float) zoneNextRadius, (float) scale2, color);
      if ((double) Vector2.Distance(playerWorldPos, this.ZoneNextCenter.Value) > (double) this.ZoneNextRadius)
        this.DrawZoneDirectionLine(handle, center, playerWorldPos, this.ZoneNextCenter.Value);
    }
    if (!this.RedZoneActive || !this.RedZoneCenter.HasValue || (double) this.RedZoneRadius <= 0.0)
      return;
    DrawingHandleScreen handle2 = handle;
    Vector2 center2 = center;
    Vector2 playerWorldPos2 = playerWorldPos;
    Vector2 zoneCenter1 = this.RedZoneCenter.Value;
    double redZoneRadius = (double) this.RedZoneRadius;
    double scale3 = (double) scale1;
    Color red = Color.Red;
    Color color1 = ((Color) ref red).WithAlpha(0.3f);
    this.DrawFilledCircle(handle2, center2, playerWorldPos2, zoneCenter1, (float) redZoneRadius, (float) scale3, color1);
  }

  private void DrawZoneCircle(
    DrawingHandleScreen handle,
    Vector2 center,
    Vector2 playerWorldPos,
    Vector2 zoneCenter,
    float zoneRadius,
    float scale,
    Color color)
  {
    Vector2 vector2_1 = zoneCenter - playerWorldPos;
    Vector2 vector2_2 = center + new Vector2(vector2_1.X * scale, -vector2_1.Y * scale);
    float num = zoneRadius * scale;
    for (int index = 0; index < 32 /*0x20*/; ++index)
    {
      float x1 = (float) ((double) index / 32.0 * 3.1415927410125732 * 2.0);
      float x2 = (float) ((double) (index + 1) / 32.0 * 3.1415927410125732 * 2.0);
      Vector2 vector2_3 = vector2_2 + new Vector2(MathF.Cos(x1) * num, MathF.Sin(x1) * num);
      Vector2 vector2_4 = vector2_2 + new Vector2(MathF.Cos(x2) * num, MathF.Sin(x2) * num);
      ((DrawingHandleBase) handle).DrawLine(vector2_3, vector2_4, color);
    }
  }

  private void DrawFilledCircle(
    DrawingHandleScreen handle,
    Vector2 center,
    Vector2 playerWorldPos,
    Vector2 zoneCenter,
    float zoneRadius,
    float scale,
    Color color)
  {
    Vector2 vector2_1 = zoneCenter - playerWorldPos;
    Vector2 vector2_2 = center + new Vector2(vector2_1.X * scale, -vector2_1.Y * scale);
    float num = zoneRadius * scale;
    List<Vector2> vector2List = new List<Vector2>();
    for (int index = 0; index < 32 /*0x20*/; ++index)
    {
      float x1 = (float) ((double) index / 32.0 * 3.1415927410125732 * 2.0);
      float x2 = (float) ((double) (index + 1) / 32.0 * 3.1415927410125732 * 2.0);
      Vector2 vector2_3 = vector2_2 + new Vector2(MathF.Cos(x1) * num, MathF.Sin(x1) * num);
      Vector2 vector2_4 = vector2_2 + new Vector2(MathF.Cos(x2) * num, MathF.Sin(x2) * num);
      vector2List.Add(vector2_2);
      vector2List.Add(vector2_3);
      vector2List.Add(vector2_4);
    }
    ((DrawingHandleBase) handle).DrawPrimitives((DrawPrimitiveTopology) 1, (ReadOnlySpan<Vector2>) vector2List.ToArray(), color);
  }

  private void DrawZoneDirectionLine(
    DrawingHandleScreen handle,
    Vector2 center,
    Vector2 playerWorldPos,
    Vector2 zoneCenter)
  {
    Vector2 vector2_1 = zoneCenter - playerWorldPos;
    Vector2 vector2_2 = Vector2Helpers.Normalized(new Vector2(vector2_1.X, -vector2_1.Y));
    float num1 = (float) ((double) Math.Min(((Control) this).PixelSize.X, ((Control) this).PixelSize.Y) / 2.0 - 10.0);
    Vector2 vector2_3 = center + vector2_2 * num1;
    Color white = Color.White;
    Color color = ((Color) ref white).WithAlpha(0.7f);
    ((DrawingHandleBase) handle).DrawLine(center, vector2_3, Color.Black);
    ((DrawingHandleBase) handle).DrawLine(center + vector2_2 * 2f, vector2_3 - vector2_2 * 2f, color);
    float num2 = 8f;
    Vector2 vector2_4 = vector2_3;
    Vector2 vector2_5 = new Vector2(vector2_2.Y, -vector2_2.X);
    Vector2 vector2_6 = vector2_3 - vector2_2 * num2 + vector2_5 * num2 * 0.5f;
    Vector2 vector2_7 = vector2_3 - vector2_2 * num2 - vector2_5 * num2 * 0.5f;
    ((DrawingHandleBase) handle).DrawPrimitives((DrawPrimitiveTopology) 1, (ReadOnlySpan<Vector2>) new Vector2[3]
    {
      vector2_4,
      vector2_6,
      vector2_7
    }, color);
    ((DrawingHandleBase) handle).DrawPrimitives((DrawPrimitiveTopology) 5, (ReadOnlySpan<Vector2>) new Vector2[4]
    {
      vector2_4,
      vector2_6,
      vector2_7,
      vector2_4
    }, Color.Black);
  }

  private void DrawAirdrop(DrawingHandleScreen handle, Vector2 center, Vector2 playerWorldPos)
  {
    if (!this.AirdropCenter.HasValue)
      return;
    float num1 = (float) ((Control) this).PixelSize.X / (this.WorldRange * 2f);
    Vector2 vector2_1 = this.AirdropCenter.Value - playerWorldPos;
    Vector2 vector2_2 = center + new Vector2(vector2_1.X * num1, -vector2_1.Y * num1);
    float num2 = MathF.Max(this.GetAirdropMarkerSize(), 4.5f * this._markerScale);
    float num3 = num2 + 2f * this._markerScale;
    float num4 = num2 + 1f * this._markerScale;
    Color color1 = Color.FromHex((ReadOnlySpan<char>) "#FF3B30", new Color?());
    DrawingHandleScreen drawingHandleScreen1 = handle;
    Vector2 vector2_3 = vector2_2;
    double num5 = (double) num3;
    Color color2 = Color.Black;
    Color color3 = ((Color) ref color2).WithAlpha(0.95f);
    ((DrawingHandleBase) drawingHandleScreen1).DrawCircle(vector2_3, (float) num5, color3, true);
    DrawingHandleScreen drawingHandleScreen2 = handle;
    Vector2 vector2_4 = vector2_2;
    double num6 = (double) num4;
    color2 = Color.White;
    Color color4 = ((Color) ref color2).WithAlpha(0.95f);
    ((DrawingHandleBase) drawingHandleScreen2).DrawCircle(vector2_4, (float) num6, color4, true);
    ((DrawingHandleBase) handle).DrawCircle(vector2_2, num2, color1, true);
    ((DrawingHandleBase) handle).DrawCircle(vector2_2, num3, Color.Black, false);
    float num7 = num2 * 0.65f;
    Vector2 vector2_5 = vector2_2 + new Vector2(-num7, 0.0f);
    Vector2 vector2_6 = vector2_2 + new Vector2(num7, 0.0f);
    Vector2 vector2_7 = vector2_2 + new Vector2(0.0f, -num7);
    Vector2 vector2_8 = vector2_2 + new Vector2(0.0f, num7);
    Vector2 vector2_9 = new Vector2(1f, 1f);
    DrawingHandleScreen drawingHandleScreen3 = handle;
    Vector2 vector2_10 = vector2_5 + vector2_9;
    Vector2 vector2_11 = vector2_6 + vector2_9;
    Color black1 = Color.Black;
    Color color5 = ((Color) ref black1).WithAlpha(0.95f);
    ((DrawingHandleBase) drawingHandleScreen3).DrawLine(vector2_10, vector2_11, color5);
    DrawingHandleScreen drawingHandleScreen4 = handle;
    Vector2 vector2_12 = vector2_7 + vector2_9;
    Vector2 vector2_13 = vector2_8 + vector2_9;
    Color black2 = Color.Black;
    Color color6 = ((Color) ref black2).WithAlpha(0.95f);
    ((DrawingHandleBase) drawingHandleScreen4).DrawLine(vector2_12, vector2_13, color6);
    ((DrawingHandleBase) handle).DrawLine(vector2_5, vector2_6, Color.White);
    ((DrawingHandleBase) handle).DrawLine(vector2_7, vector2_8, Color.White);
    string str = Loc.GetString("pubg-airdrop-countdown", new (string, object)[1]
    {
      ("time", (object) PubgMinimapControl.FormatTime(this.AirdropRemainingSeconds))
    });
    Vector2 vector2_14 = new Vector2(num3 + 2f * this._markerScale, (float) (-(double) num3 - 2.0 * (double) this._markerScale));
    Vector2 vector2_15 = vector2_2 + vector2_14;
    Font scaledFont = this.GetScaledFont(10);
    Color black3 = Color.Black;
    Color color7 = ((Color) ref black3).WithAlpha(0.95f);
    Color color8 = Color.FromHex((ReadOnlySpan<char>) "#FFF2A8", new Color?());
    float num8 = 1f;
    handle.DrawString(scaledFont, vector2_15 + new Vector2(-num8, -num8), str, color7);
    handle.DrawString(scaledFont, vector2_15 + new Vector2(num8, -num8), str, color7);
    handle.DrawString(scaledFont, vector2_15 + new Vector2(-num8, num8), str, color7);
    handle.DrawString(scaledFont, vector2_15 + new Vector2(num8, num8), str, color7);
    handle.DrawString(scaledFont, vector2_15, str, color8);
    handle.DrawString(scaledFont, vector2_15 + new Vector2(0.75f, 0.0f), str, color8);
  }

  private float GetAirdropMarkerSize()
  {
    float airdropMarkerSize = 4f * this._markerScale;
    float num1 = 0.0f;
    foreach (Entity<MapGridComponent> grid in this._grids)
    {
      EntityUid entityUid;
      MapGridComponent mapGridComponent;
      grid.Deconstruct(ref entityUid, ref mapGridComponent);
      Box2 localAabb = mapGridComponent.LocalAABB;
      float num2 = MathF.Max(((Box2) ref localAabb).Width, ((Box2) ref localAabb).Height);
      if ((double) num2 > (double) num1)
        num1 = num2;
    }
    if ((double) num1 <= 220.0)
      return airdropMarkerSize;
    float num3 = Math.Clamp((float) (((double) num1 - 220.0) / 380.0), 0.0f, 1f);
    return airdropMarkerSize * (float) (1.0 + (double) num3 * 0.800000011920929);
  }

  private void DrawRespawnTowers(
    DrawingHandleScreen handle,
    Vector2 center,
    Vector2 playerWorldPos)
  {
    float num1 = (float) ((Control) this).PixelSize.X / (this.WorldRange * 2f);
    foreach (Vector2 respawnTowerPosition in (IEnumerable<Vector2>) this.RespawnTowerPositions)
    {
      Vector2 vector2_1 = respawnTowerPosition - playerWorldPos;
      Vector2 vector2_2 = center + new Vector2(vector2_1.X * num1, -vector2_1.Y * num1);
      ((DrawingHandleBase) handle).DrawCircle(vector2_2, 3f * this._markerScale, Color.FromHex((ReadOnlySpan<char>) "#FFD54F", new Color?()), true);
      ((DrawingHandleBase) handle).DrawCircle(vector2_2, 3f * this._markerScale, Color.Black, false);
    }
    foreach (Vector2 respawnTowerPosition in (IEnumerable<Vector2>) this.ActiveRespawnTowerPositions)
    {
      Vector2 vector2_3 = respawnTowerPosition - playerWorldPos;
      Vector2 vector2_4 = center + new Vector2(vector2_3.X * num1, -vector2_3.Y * num1);
      DrawingHandleScreen drawingHandleScreen = handle;
      Vector2 vector2_5 = vector2_4;
      double num2 = 5.0 * (double) this._markerScale;
      Color color1 = Color.FromHex((ReadOnlySpan<char>) "#FF3B30", new Color?());
      Color color2 = ((Color) ref color1).WithAlpha(0.35f);
      ((DrawingHandleBase) drawingHandleScreen).DrawCircle(vector2_5, (float) num2, color2, true);
      ((DrawingHandleBase) handle).DrawCircle(vector2_4, 4f * this._markerScale, Color.FromHex((ReadOnlySpan<char>) "#FF3B30", new Color?()), false);
    }
  }

  private void DrawPartyMembers(DrawingHandleScreen handle, Vector2 center)
  {
    if (this.PartyMembers == null || !this._playerCoordinates.HasValue)
      return;
    MapCoordinates mapCoordinates = this._transform.ToMapCoordinates(this._playerCoordinates.Value, true);
    float num1 = (float) ((Control) this).PixelSize.X / (this.WorldRange * 2f);
    foreach (PubgPartyMemberState partyMember in (IEnumerable<PubgPartyMemberState>) this.PartyMembers)
    {
      if (!NetEntity.op_Equality(partyMember.Entity, this._localNetEntity) && !MapId.op_Inequality(partyMember.MapId, mapCoordinates.MapId))
      {
        Vector2 vector2_1 = partyMember.Position - mapCoordinates.Position;
        Vector2 vector2_2 = center + new Vector2(vector2_1.X * num1, -vector2_1.Y * num1);
        Color color1 = PubgMinimapControl.GetPartyColor(partyMember.SlotIndex);
        Color color2 = ((Color) ref color1).WithAlpha(this.PartyMarkersOpacity);
        float num2 = 3f * this._markerScale;
        ((DrawingHandleBase) handle).DrawCircle(vector2_2, num2, color2, true);
        DrawingHandleScreen drawingHandleScreen1 = handle;
        Vector2 vector2_3 = vector2_2;
        double num3 = (double) num2;
        color1 = Color.Black;
        Color color3 = ((Color) ref color1).WithAlpha(this.PartyMarkersOpacity);
        ((DrawingHandleBase) drawingHandleScreen1).DrawCircle(vector2_3, (float) num3, color3, false);
        Vector2 vector2_4 = vector2_2 + new Vector2(5f, -6f) * this._markerScale;
        DrawingHandleScreen drawingHandleScreen2 = handle;
        Font scaledFont = this.GetScaledFont();
        Vector2 vector2_5 = vector2_4;
        string username = partyMember.Username;
        color1 = Color.White;
        Color color4 = ((Color) ref color1).WithAlpha(this.PartyMarkersOpacity);
        drawingHandleScreen2.DrawString(scaledFont, vector2_5, username, color4);
      }
    }
  }

  private void DrawPings(DrawingHandleScreen handle, Vector2 center)
  {
    if (!this._playerCoordinates.HasValue || this.ActivePings.Count == 0)
      return;
    MapCoordinates mapCoordinates = this._transform.ToMapCoordinates(this._playerCoordinates.Value, true);
    if (MapId.op_Equality(mapCoordinates.MapId, MapId.Nullspace))
      return;
    float num1 = (float) ((Control) this).PixelSize.X / (this.WorldRange * 2f);
    foreach (PubgActivePingState activePing in (IEnumerable<PubgActivePingState>) this.ActivePings)
    {
      if (!MapId.op_Inequality(activePing.MapId, mapCoordinates.MapId))
      {
        Vector2 vector2 = activePing.Position - mapCoordinates.Position;
        Vector2 screenPos = center + new Vector2(vector2.X * num1, -vector2.Y * num1);
        Color color = PubgPartyPingColorResolver.GetColor(activePing.Source);
        switch (activePing.Kind)
        {
          case PubgPartyPingKind.Enemy:
            float num2 = (float) (4.0 * (double) this._markerScale * 1.2000000476837158);
            ((DrawingHandleBase) handle).DrawCircle(screenPos, num2, color, true);
            ((DrawingHandleBase) handle).DrawCircle(screenPos, num2 + 1f, this._pingEnemyColor, false);
            ((DrawingHandleBase) handle).DrawCircle(screenPos, num2 + 2f, Color.Black, false);
            continue;
          case PubgPartyPingKind.Item:
            this.DrawItemPing(handle, screenPos, activePing.ItemPrototypeId, color);
            continue;
          default:
            float num3 = 4f * this._markerScale;
            ((DrawingHandleBase) handle).DrawCircle(screenPos, num3, color, true);
            ((DrawingHandleBase) handle).DrawCircle(screenPos, num3, Color.Black, false);
            continue;
        }
      }
    }
  }

  private void DrawCivGlobalMap(DrawingHandleScreen handle, Vector2 center)
  {
    if (!this._playerCoordinates.HasValue || (this.CivGlobalMapMarkers == null || this.CivGlobalMapMarkers.Count == 0) && (this.CivGlobalMapPoints == null || this.CivGlobalMapPoints.Count == 0) && (this.CivGlobalMapPlayers == null || this.CivGlobalMapPlayers.Count == 0))
      return;
    MapCoordinates mapCoordinates = this._transform.ToMapCoordinates(this._playerCoordinates.Value, true);
    if (MapId.op_Equality(mapCoordinates.MapId, MapId.Nullspace))
      return;
    float num = (float) ((Control) this).PixelSize.X / (this.WorldRange * 2f);
    if (this.CivGlobalMapMarkers != null)
    {
      foreach (CivGlobalMapMarkerState civGlobalMapMarker in (IEnumerable<CivGlobalMapMarkerState>) this.CivGlobalMapMarkers)
      {
        if (!MapId.op_Inequality(civGlobalMapMarker.MapId, mapCoordinates.MapId))
        {
          Vector2 vector2 = civGlobalMapMarker.Position - mapCoordinates.Position;
          Vector2 screenPos = center + new Vector2(vector2.X * num, -vector2.Y * num);
          this.DrawCivGlobalMarker(handle, screenPos, civGlobalMapMarker.Type);
        }
      }
    }
    if (this.CivGlobalMapPoints != null)
    {
      foreach (CivPointCapturePointState civGlobalMapPoint in (IEnumerable<CivPointCapturePointState>) this.CivGlobalMapPoints)
      {
        if (!MapId.op_Inequality(civGlobalMapPoint.MapId, mapCoordinates.MapId))
        {
          Vector2 vector2 = civGlobalMapPoint.Position - mapCoordinates.Position;
          Vector2 screenPos = center + new Vector2(vector2.X * num, -vector2.Y * num);
          this.DrawCivGlobalPoint(handle, screenPos, civGlobalMapPoint);
        }
      }
    }
    if (this.CivGlobalMapPlayers == null)
      return;
    TimeSpan curTime = this._timing.CurTime;
    float totalSeconds = (float) (curTime - this._lastPlayerLerpTime).TotalSeconds;
    this._lastPlayerLerpTime = curTime;
    float amount = (double) totalSeconds <= 0.0 ? 1f : 1f - MathF.Exp((float) (-(double) totalSeconds / 0.079999998211860657));
    this._civPlayerSeen.Clear();
    foreach (CivGlobalMapPlayerState civGlobalMapPlayer in (IEnumerable<CivGlobalMapPlayerState>) this.CivGlobalMapPlayers)
    {
      if (!civGlobalMapPlayer.IsSelf && !MapId.op_Inequality(civGlobalMapPlayer.MapId, mapCoordinates.MapId))
      {
        Vector2 position = civGlobalMapPlayer.Position;
        Vector2 vector2_1;
        Vector2 vector2_2 = !this._civPlayerSmoothed.TryGetValue(civGlobalMapPlayer.Name, out vector2_1) ? position : ((double) (position - vector2_1).Length() > 25.0 ? position : Vector2.Lerp(vector2_1, position, amount));
        this._civPlayerSmoothed[civGlobalMapPlayer.Name] = vector2_2;
        this._civPlayerSeen.Add(civGlobalMapPlayer.Name);
        Vector2 vector2_3 = vector2_2 - mapCoordinates.Position;
        Vector2 screenPos = center + new Vector2(vector2_3.X * num, -vector2_3.Y * num);
        this.DrawCivGlobalPlayer(handle, screenPos, civGlobalMapPlayer);
      }
    }
    if (this._civPlayerSmoothed.Count == this._civPlayerSeen.Count)
      return;
    this._civPlayerStale.Clear();
    foreach (string key in this._civPlayerSmoothed.Keys)
    {
      if (!this._civPlayerSeen.Contains(key))
        this._civPlayerStale.Add(key);
    }
    foreach (string key in this._civPlayerStale)
      this._civPlayerSmoothed.Remove(key);
  }

  private void DrawCivGlobalMarker(
    DrawingHandleScreen handle,
    Vector2 screenPos,
    CivGlobalMapMarkerType type)
  {
    Color color1;
    switch (type)
    {
      case CivGlobalMapMarkerType.Attack:
        color1 = Color.FromHex((ReadOnlySpan<char>) "#ff5449", new Color?());
        break;
      case CivGlobalMapMarkerType.Defense:
        color1 = Color.FromHex((ReadOnlySpan<char>) "#5ca8ff", new Color?());
        break;
      case CivGlobalMapMarkerType.Enemy:
        color1 = Color.FromHex((ReadOnlySpan<char>) "#ff6d3f", new Color?());
        break;
      case CivGlobalMapMarkerType.Help:
        color1 = Color.FromHex((ReadOnlySpan<char>) "#ffd85a", new Color?());
        break;
      case CivGlobalMapMarkerType.Allies:
        color1 = Color.FromHex((ReadOnlySpan<char>) "#6de685", new Color?());
        break;
      default:
        color1 = Color.White;
        break;
    }
    Color color2 = color1;
    float num1 = (uint) type <= 1U ? 5f * this._markerScale : 4f * this._markerScale;
    if ((uint) type <= 1U)
    {
      Vector2 vector2_1 = screenPos + new Vector2(0.0f, (float) (-(double) num1 * 1.3500000238418579));
      Vector2 vector2_2 = screenPos + new Vector2(-num1, num1);
      Vector2 vector2_3 = screenPos + new Vector2(num1, num1);
      ((DrawingHandleBase) handle).DrawPrimitives((DrawPrimitiveTopology) 1, (ReadOnlySpan<Vector2>) new Vector2[3]
      {
        vector2_1,
        vector2_2,
        vector2_3
      }, ((Color) ref color2).WithAlpha(0.95f));
      ((DrawingHandleBase) handle).DrawPrimitives((DrawPrimitiveTopology) 5, (ReadOnlySpan<Vector2>) new Vector2[4]
      {
        vector2_1,
        vector2_2,
        vector2_3,
        vector2_1
      }, Color.Black);
    }
    else
    {
      ((DrawingHandleBase) handle).DrawCircle(screenPos, num1, ((Color) ref color2).WithAlpha(0.95f), true);
      DrawingHandleScreen drawingHandleScreen = handle;
      Vector2 vector2 = screenPos;
      double num2 = (double) num1 + 1.0;
      Color black = Color.Black;
      Color color3 = ((Color) ref black).WithAlpha(0.9f);
      ((DrawingHandleBase) drawingHandleScreen).DrawCircle(vector2, (float) num2, color3, false);
    }
  }

  private void DrawCivGlobalPlayer(
    DrawingHandleScreen handle,
    Vector2 screenPos,
    CivGlobalMapPlayerState player)
  {
    Color playerColor = CivGlobalMapColorResolver.GetPlayerColor(this.CivViewerTeamId, this.CivViewerSquadId, player.TeamId, player.SquadId);
    float num1 = player.IsSquadLeader ? 4.5f * this._markerScale : 4f * this._markerScale;
    if (player.IsSquadLeader)
    {
      Vector2 vector2_1 = screenPos + new Vector2(0.0f, (float) (-(double) num1 * 1.3500000238418579));
      Vector2 vector2_2 = screenPos + new Vector2(-num1, num1);
      Vector2 vector2_3 = screenPos + new Vector2(num1, num1);
      ((DrawingHandleBase) handle).DrawPrimitives((DrawPrimitiveTopology) 1, (ReadOnlySpan<Vector2>) new Vector2[3]
      {
        vector2_1,
        vector2_2,
        vector2_3
      }, ((Color) ref playerColor).WithAlpha(0.9f));
      ((DrawingHandleBase) handle).DrawPrimitives((DrawPrimitiveTopology) 5, (ReadOnlySpan<Vector2>) new Vector2[4]
      {
        vector2_1,
        vector2_2,
        vector2_3,
        vector2_1
      }, Color.Black);
    }
    else
    {
      ((DrawingHandleBase) handle).DrawCircle(screenPos, num1, ((Color) ref playerColor).WithAlpha(0.9f), true);
      DrawingHandleScreen drawingHandleScreen = handle;
      Vector2 vector2 = screenPos;
      double num2 = (double) num1 + 1.0;
      Color black = Color.Black;
      Color color = ((Color) ref black).WithAlpha(0.8f);
      ((DrawingHandleBase) drawingHandleScreen).DrawCircle(vector2, (float) num2, color, false);
    }
  }

  private void DrawCivGlobalPoint(
    DrawingHandleScreen handle,
    Vector2 screenPos,
    CivPointCapturePointState point)
  {
    Color relationColor1 = CivPointCaptureColorResolver.GetRelationColor(this.CivViewerTeamId, point.OwnerTeamId);
    int num1 = point.CapturingTeamId == 0 ? 0 : ((double) point.CaptureProgress > 0.0 ? 1 : 0);
    TimeSpan curTime;
    Color color1;
    if (num1 == 0)
    {
      color1 = relationColor1;
    }
    else
    {
      int civViewerTeamId = this.CivViewerTeamId;
      int ownerTeamId = point.OwnerTeamId;
      int capturingTeamId = point.CapturingTeamId;
      curTime = this._timing.CurTime;
      double totalSeconds = curTime.TotalSeconds;
      color1 = CivPointCaptureColorResolver.GetCapturePulseColor(civViewerTeamId, ownerTeamId, capturingTeamId, (float) totalSeconds);
    }
    Color color2 = color1;
    float num2 = 6f * this._markerScale;
    float num3 = 3.5f * this._markerScale;
    float num4 = 1.75f * this._markerScale;
    float num5 = num2;
    if (num1 != 0)
    {
      double num6 = (double) num5;
      curTime = this._timing.CurTime;
      double num7 = (double) CivPointCaptureColorResolver.GetCapturePulseAmount((float) curTime.TotalSeconds) * 2.0 * (double) this._markerScale;
      num5 = (float) (num6 + num7);
    }
    ((DrawingHandleBase) handle).DrawCircle(screenPos, num5, ((Color) ref color2).WithAlpha(0.14f), true);
    ((DrawingHandleBase) handle).DrawCircle(screenPos, num5, ((Color) ref color2).WithAlpha(0.95f), false);
    ((DrawingHandleBase) handle).DrawCircle(screenPos, num3, ((Color) ref color2).WithAlpha(0.75f), false);
    ((DrawingHandleBase) handle).DrawCircle(screenPos, num4, ((Color) ref relationColor1).WithAlpha(0.95f), true);
    DrawingHandleScreen drawingHandleScreen1 = handle;
    Vector2 vector2_1 = screenPos;
    double num8 = (double) num5 + 1.0;
    Color black1 = Color.Black;
    Color color3 = ((Color) ref black1).WithAlpha(0.8f);
    ((DrawingHandleBase) drawingHandleScreen1).DrawCircle(vector2_1, (float) num8, color3, false);
    if (num1 != 0)
    {
      Color relationColor2 = CivPointCaptureColorResolver.GetRelationColor(this.CivViewerTeamId, point.CapturingTeamId);
      PubgMinimapControl.DrawProgressArc(handle, screenPos, num5 + 2f, point.CaptureProgress, relationColor2);
    }
    if (string.IsNullOrWhiteSpace(point.Label))
      return;
    Vector2 vector2_2 = screenPos + new Vector2(num2 + 2f * this._markerScale, (float) (-(double) num2 - 1.0 * (double) this._markerScale));
    Font scaledFont = this.GetScaledFont();
    DrawingHandleScreen drawingHandleScreen2 = handle;
    Font font = scaledFont;
    Vector2 vector2_3 = vector2_2 + Vector2.One;
    string label = point.Label;
    Color black2 = Color.Black;
    Color color4 = ((Color) ref black2).WithAlpha(0.85f);
    drawingHandleScreen2.DrawString(font, vector2_3, label, color4);
    handle.DrawString(scaledFont, vector2_2, point.Label, color2);
  }

  private void DrawCivGrid(DrawingHandleScreen handle, Vector2 center)
  {
    if (!this.CivHasBounds || !this._playerCoordinates.HasValue)
      return;
    MapCoordinates mapCoordinates = this._transform.ToMapCoordinates(this._playerCoordinates.Value, true);
    if (MapId.op_Equality(mapCoordinates.MapId, MapId.Nullspace))
      return;
    float num1 = (float) ((Control) this).PixelSize.X / (this.WorldRange * 2f);
    Vector2 position = mapCoordinates.Position;
    Color color1;
    // ISSUE: explicit constructor call
    ((Color) ref color1).\u002Ector((byte) 130, (byte) 150, (byte) 175, (byte) 120);
    Color color2 = Color.FromHex((ReadOnlySpan<char>) "#FFE9A8", new Color?());
    for (int i = 0; i <= 8; ++i)
    {
      float x = center.X + (CivMapGrid.LineX(this.CivBoundsMin, this.CivBoundsMax, i) - position.X) * num1;
      if ((double) x >= 0.0 && (double) x <= (double) ((Control) this).PixelSize.X)
        ((DrawingHandleBase) handle).DrawLine(new Vector2(x, 0.0f), new Vector2(x, (float) ((Control) this).PixelSize.Y), color1);
    }
    for (int i = 0; i <= 8; ++i)
    {
      float y = center.Y - (CivMapGrid.LineY(this.CivBoundsMin, this.CivBoundsMax, i) - position.Y) * num1;
      if ((double) y >= 0.0 && (double) y <= (double) ((Control) this).PixelSize.Y)
        ((DrawingHandleBase) handle).DrawLine(new Vector2(0.0f, y), new Vector2((float) ((Control) this).PixelSize.X, y), color1);
    }
    Font scaledFont = this.GetScaledFont(11);
    for (int col = 0; col < 8; ++col)
    {
      float num2 = center.X + (CivMapGrid.ColumnCenterX(this.CivBoundsMin, this.CivBoundsMax, col) - position.X) * num1;
      if ((double) num2 >= 4.0 && (double) num2 <= (double) ((Control) this).PixelSize.X - 4.0)
        this.DrawLabelOutlined(handle, scaledFont, new Vector2(num2 - 3f, 1f), CivMapGrid.GetColumnLabel(col), color2);
    }
    for (int row = 0; row < 8; ++row)
    {
      float num3 = center.Y - (CivMapGrid.RowCenterY(this.CivBoundsMin, this.CivBoundsMax, row) - position.Y) * num1;
      if ((double) num3 >= 4.0 && (double) num3 <= (double) ((Control) this).PixelSize.Y - 4.0)
        this.DrawLabelOutlined(handle, scaledFont, new Vector2(1f, num3 - 6f), (row + 1).ToString(), color2);
    }
  }

  private void DrawLabelOutlined(
    DrawingHandleScreen handle,
    Font font,
    Vector2 pos,
    string text,
    Color color)
  {
    Color black = Color.Black;
    Color color1 = ((Color) ref black).WithAlpha(0.95f);
    handle.DrawString(font, pos + new Vector2(-1f, 0.0f), text, color1);
    handle.DrawString(font, pos + new Vector2(1f, 0.0f), text, color1);
    handle.DrawString(font, pos + new Vector2(0.0f, -1f), text, color1);
    handle.DrawString(font, pos + new Vector2(0.0f, 1f), text, color1);
    handle.DrawString(font, pos, text, color);
  }

  private void DrawCivOrders(DrawingHandleScreen handle, Vector2 center)
  {
    if (this.CivGlobalMapOrders == null || this.CivGlobalMapOrders.Count == 0 || !this._playerCoordinates.HasValue)
      return;
    MapCoordinates mapCoordinates = this._transform.ToMapCoordinates(this._playerCoordinates.Value, true);
    if (MapId.op_Equality(mapCoordinates.MapId, MapId.Nullspace))
      return;
    float num = (float) ((Control) this).PixelSize.X / (this.WorldRange * 2f);
    foreach (CivCommanderOrderState civGlobalMapOrder in (IEnumerable<CivCommanderOrderState>) this.CivGlobalMapOrders)
    {
      if (civGlobalMapOrder.Order != CivCommanderOrderType.None && !MapId.op_Inequality(civGlobalMapOrder.MapId, mapCoordinates.MapId))
      {
        Vector2 vector2_1 = civGlobalMapOrder.Position - mapCoordinates.Position;
        Vector2 vector2_2 = center + new Vector2(vector2_1.X * num, -vector2_1.Y * num);
        Color orderColor = PubgMinimapControl.GetOrderColor(civGlobalMapOrder.Order);
        if (((double) vector2_2.X < 0.0 || (double) vector2_2.X > (double) ((Control) this).PixelSize.X || (double) vector2_2.Y < 0.0 ? 0 : ((double) vector2_2.Y <= (double) ((Control) this).PixelSize.Y ? 1 : 0)) != 0)
          this.DrawOrderMarker(handle, vector2_2, civGlobalMapOrder.Order, orderColor);
        else if (this.CivViewerSquadId != 0 && civGlobalMapOrder.SquadId == this.CivViewerSquadId)
          this.DrawEdgeArrow(handle, center, vector2_2, orderColor);
      }
    }
  }

  private void DrawOrderMarker(
    DrawingHandleScreen handle,
    Vector2 screenPos,
    CivCommanderOrderType order,
    Color color)
  {
    float num1 = 5f * this._markerScale;
    ((DrawingHandleBase) handle).DrawCircle(screenPos, num1, ((Color) ref color).WithAlpha(0.2f), true);
    ((DrawingHandleBase) handle).DrawCircle(screenPos, num1, color, false);
    DrawingHandleScreen drawingHandleScreen1 = handle;
    Vector2 vector2_1 = screenPos;
    double num2 = (double) num1 + 1.0;
    Color black1 = Color.Black;
    Color color1 = ((Color) ref black1).WithAlpha(0.8f);
    ((DrawingHandleBase) drawingHandleScreen1).DrawCircle(vector2_1, (float) num2, color1, false);
    string orderShort = PubgMinimapControl.GetOrderShort(order);
    Font scaledFont = this.GetScaledFont();
    Vector2 vector2_2 = screenPos + new Vector2(num1 + 1f, (float) (-(double) num1 - 1.0));
    DrawingHandleScreen drawingHandleScreen2 = handle;
    Font font = scaledFont;
    Vector2 vector2_3 = vector2_2 + Vector2.One;
    string str = orderShort;
    Color black2 = Color.Black;
    Color color2 = ((Color) ref black2).WithAlpha(0.85f);
    drawingHandleScreen2.DrawString(font, vector2_3, str, color2);
    handle.DrawString(scaledFont, vector2_2, orderShort, color);
  }

  private void DrawCivSquadLeaderArrow(DrawingHandleScreen handle, Vector2 center)
  {
    if (this.CivGlobalMapPlayers == null || this.CivViewerSquadId == 0 || !this._playerCoordinates.HasValue)
      return;
    MapCoordinates mapCoordinates = this._transform.ToMapCoordinates(this._playerCoordinates.Value, true);
    if (MapId.op_Equality(mapCoordinates.MapId, MapId.Nullspace))
      return;
    float num = (float) ((Control) this).PixelSize.X / (this.WorldRange * 2f);
    foreach (CivGlobalMapPlayerState civGlobalMapPlayer in (IEnumerable<CivGlobalMapPlayerState>) this.CivGlobalMapPlayers)
    {
      if (!civGlobalMapPlayer.IsSelf && civGlobalMapPlayer.IsSquadLeader && civGlobalMapPlayer.SquadId == this.CivViewerSquadId && !MapId.op_Inequality(civGlobalMapPlayer.MapId, mapCoordinates.MapId))
      {
        Vector2 vector2 = civGlobalMapPlayer.Position - mapCoordinates.Position;
        Vector2 targetScreen = center + new Vector2(vector2.X * num, -vector2.Y * num);
        if (((double) targetScreen.X < 0.0 || (double) targetScreen.X > (double) ((Control) this).PixelSize.X || (double) targetScreen.Y < 0.0 ? 0 : ((double) targetScreen.Y <= (double) ((Control) this).PixelSize.Y ? 1 : 0)) != 0)
          break;
        this.DrawEdgeArrow(handle, center, targetScreen, CivGlobalMapColorResolver.SquadColor);
        break;
      }
    }
  }

  private void DrawCivDeaths(DrawingHandleScreen handle, Vector2 center)
  {
    if (this.CivGlobalMapDeaths == null || this.CivGlobalMapDeaths.Count == 0 || !this._playerCoordinates.HasValue)
      return;
    MapCoordinates mapCoordinates = this._transform.ToMapCoordinates(this._playerCoordinates.Value, true);
    if (MapId.op_Equality(mapCoordinates.MapId, MapId.Nullspace))
      return;
    float num1 = (float) ((Control) this).PixelSize.X / (this.WorldRange * 2f);
    foreach (CivGlobalMapDeathState civGlobalMapDeath in (IEnumerable<CivGlobalMapDeathState>) this.CivGlobalMapDeaths)
    {
      if (!MapId.op_Inequality(civGlobalMapDeath.MapId, mapCoordinates.MapId))
      {
        Vector2 vector2_1 = civGlobalMapDeath.Position - mapCoordinates.Position;
        Vector2 vector2_2 = center + new Vector2(vector2_1.X * num1, -vector2_1.Y * num1);
        float num2 = (float) (0.25 + 0.64999997615814209 * ((double) civGlobalMapDeath.LifetimeSeconds > 0.0 ? (double) Math.Clamp(civGlobalMapDeath.RemainingSeconds / civGlobalMapDeath.LifetimeSeconds, 0.0f, 1f) : 1.0));
        Color playerColor = CivGlobalMapColorResolver.GetPlayerColor(this.CivViewerTeamId, this.CivViewerSquadId, civGlobalMapDeath.TeamId, civGlobalMapDeath.SquadId);
        Color color = ((Color) ref playerColor).WithAlpha(num2);
        float num3 = 3.5f * this._markerScale;
        ((DrawingHandleBase) handle).DrawLine(vector2_2 + new Vector2(-num3, -num3), vector2_2 + new Vector2(num3, num3), color);
        ((DrawingHandleBase) handle).DrawLine(vector2_2 + new Vector2(-num3, num3), vector2_2 + new Vector2(num3, -num3), color);
      }
    }
  }

  private void DrawEdgeArrow(
    DrawingHandleScreen handle,
    Vector2 center,
    Vector2 targetScreen,
    Color color)
  {
    Vector2 vector2_1 = targetScreen - center;
    if ((double) vector2_1.LengthSquared() < 1.0 / 1000.0)
      return;
    Vector2 vector2_2 = Vector2Helpers.Normalized(vector2_1);
    float num1 = (float) ((double) Math.Min(((Control) this).PixelSize.X, ((Control) this).PixelSize.Y) / 2.0 - 8.0);
    Vector2 vector2_3 = center + vector2_2 * num1;
    float num2 = 7f * this._markerScale;
    Vector2 vector2_4 = new Vector2(vector2_2.Y, -vector2_2.X);
    Vector2 vector2_5 = vector2_3;
    Vector2 vector2_6 = vector2_3 - vector2_2 * num2 + vector2_4 * num2 * 0.5f;
    Vector2 vector2_7 = vector2_3 - vector2_2 * num2 - vector2_4 * num2 * 0.5f;
    DrawingHandleScreen drawingHandleScreen = handle;
    Vector2 vector2_8 = center;
    Vector2 vector2_9 = vector2_3;
    Color black = Color.Black;
    Color color1 = ((Color) ref black).WithAlpha(0.4f);
    ((DrawingHandleBase) drawingHandleScreen).DrawLine(vector2_8, vector2_9, color1);
    ((DrawingHandleBase) handle).DrawPrimitives((DrawPrimitiveTopology) 1, (ReadOnlySpan<Vector2>) new Vector2[3]
    {
      vector2_5,
      vector2_6,
      vector2_7
    }, color);
    ((DrawingHandleBase) handle).DrawPrimitives((DrawPrimitiveTopology) 5, (ReadOnlySpan<Vector2>) new Vector2[4]
    {
      vector2_5,
      vector2_6,
      vector2_7,
      vector2_5
    }, Color.Black);
  }

  private static void DrawProgressArc(
    DrawingHandleScreen handle,
    Vector2 arcCenter,
    float radius,
    float progress,
    Color color)
  {
    progress = Math.Clamp(progress, 0.0f, 1f);
    if ((double) progress <= 0.0)
      return;
    int num = Math.Max(6, (int) MathF.Ceiling(28f * progress));
    Vector2[] vector2Array = new Vector2[num + 1];
    for (int index = 0; index <= num; ++index)
    {
      float x = (float) (6.2831854820251465 * (double) (progress * (float) index / (float) num) - 1.5707963705062866);
      vector2Array[index] = arcCenter + new Vector2(MathF.Cos(x), MathF.Sin(x)) * radius;
    }
    ((DrawingHandleBase) handle).DrawPrimitives((DrawPrimitiveTopology) 5, (ReadOnlySpan<Vector2>) vector2Array, color);
  }

  private static Color GetOrderColor(CivCommanderOrderType order)
  {
    Color orderColor;
    switch (order)
    {
      case CivCommanderOrderType.Attack:
        orderColor = Color.FromHex((ReadOnlySpan<char>) "#ff5449", new Color?());
        break;
      case CivCommanderOrderType.Defense:
        orderColor = Color.FromHex((ReadOnlySpan<char>) "#5ca8ff", new Color?());
        break;
      case CivCommanderOrderType.Artillery:
        orderColor = Color.FromHex((ReadOnlySpan<char>) "#ffd85a", new Color?());
        break;
      default:
        orderColor = Color.White;
        break;
    }
    return orderColor;
  }

  private static string GetOrderShort(CivCommanderOrderType order)
  {
    string orderShort;
    switch (order)
    {
      case CivCommanderOrderType.Attack:
        orderShort = "ATK";
        break;
      case CivCommanderOrderType.Defense:
        orderShort = "DEF";
        break;
      case CivCommanderOrderType.Artillery:
        orderShort = "ART";
        break;
      default:
        orderShort = "ORD";
        break;
    }
    return orderShort;
  }

  private void DrawItemPing(
    DrawingHandleScreen handle,
    Vector2 screenPos,
    string? prototypeId,
    Color color)
  {
    Texture itemPingTexture = this.TryGetItemPingTexture(prototypeId);
    if (itemPingTexture == null)
    {
      this.DrawFallbackItemPing(handle, screenPos, color);
    }
    else
    {
      Vector2 vector2 = new Vector2(5f, 5f) * this._markerScale;
      UIBox2 uiBox2_1;
      // ISSUE: explicit constructor call
      ((UIBox2) ref uiBox2_1).\u002Ector(screenPos - vector2 - Vector2.One, screenPos + vector2 + Vector2.One);
      UIBox2 uiBox2_2;
      // ISSUE: explicit constructor call
      ((UIBox2) ref uiBox2_2).\u002Ector(screenPos - vector2, screenPos + vector2);
      DrawingHandleScreen drawingHandleScreen = handle;
      UIBox2 uiBox2_3 = uiBox2_1;
      Color black = Color.Black;
      Color color1 = ((Color) ref black).WithAlpha(0.65f);
      drawingHandleScreen.DrawRect(uiBox2_3, color1, true);
      handle.DrawTextureRect(itemPingTexture, uiBox2_2, new Color?());
      ((DrawingHandleBase) handle).DrawCircle(screenPos, vector2.X + 1f, color, false);
    }
  }

  private void DrawFallbackItemPing(DrawingHandleScreen handle, Vector2 screenPos, Color color)
  {
    float num = (float) (4.0 * (double) this._markerScale * 1.2000000476837158);
    Vector2 vector2_1 = screenPos + new Vector2(0.0f, -num);
    Vector2 vector2_2 = screenPos + new Vector2(num, 0.0f);
    Vector2 vector2_3 = screenPos + new Vector2(0.0f, num);
    Vector2 vector2_4 = screenPos + new Vector2(-num, 0.0f);
    ((DrawingHandleBase) handle).DrawPrimitives((DrawPrimitiveTopology) 1, (ReadOnlySpan<Vector2>) new Vector2[6]
    {
      vector2_1,
      vector2_2,
      vector2_3,
      vector2_1,
      vector2_3,
      vector2_4
    }, color);
    ((DrawingHandleBase) handle).DrawPrimitives((DrawPrimitiveTopology) 5, (ReadOnlySpan<Vector2>) new Vector2[5]
    {
      vector2_1,
      vector2_2,
      vector2_3,
      vector2_4,
      vector2_1
    }, Color.Black);
  }

  private Texture? TryGetItemPingTexture(string? prototypeId)
  {
    if (string.IsNullOrWhiteSpace(prototypeId))
      return (Texture) null;
    Texture itemPingTexture1;
    if (this._itemPingIconCache.TryGetValue(prototypeId, out itemPingTexture1))
      return itemPingTexture1;
    EntityPrototype entityPrototype;
    if (!this._prototype.TryIndex<EntityPrototype>(prototypeId, ref entityPrototype))
    {
      this._itemPingIconCache[prototypeId] = (Texture) null;
      return (Texture) null;
    }
    Texture itemPingTexture2 = ((IDirectionalTextureProvider) this._sprite.GetPrototypeIcon(entityPrototype)).Default;
    this._itemPingIconCache[prototypeId] = itemPingTexture2;
    return itemPingTexture2;
  }

  private Font GetScaledFont(int baseFontSize = 6)
  {
    int key = Math.Clamp((int) MathF.Round((float) baseFontSize * this._markerScale), 4, 36);
    Font scaledFont1;
    if (this._fontCache.TryGetValue(key, out scaledFont1))
      return scaledFont1;
    VectorFont scaledFont2 = new VectorFont(this._cache.GetResource<FontResource>("/Fonts/NotoSans/NotoSans-Regular.ttf", true), key);
    this._fontCache[key] = (Font) scaledFont2;
    return (Font) scaledFont2;
  }

  private static Color GetPartyColor(int slotIndex)
  {
    Color partyColor;
    switch (slotIndex)
    {
      case 1:
        partyColor = Color.FromHex((ReadOnlySpan<char>) "#00bcd4", new Color?());
        break;
      case 2:
        partyColor = Color.FromHex((ReadOnlySpan<char>) "#ffeb3b", new Color?());
        break;
      case 3:
        partyColor = Color.FromHex((ReadOnlySpan<char>) "#ff9800", new Color?());
        break;
      default:
        partyColor = Color.FromHex((ReadOnlySpan<char>) "#4caf50", new Color?());
        break;
    }
    return partyColor;
  }

  private static string FormatTime(int seconds)
  {
    if (seconds < 0)
      seconds = 0;
    return $"{seconds / 60:D2}:{seconds % 60:D2}";
  }

  protected override void KeyBindDown(GUIBoundKeyEventArgs args)
  {
    base.KeyBindDown(args);
    MapCoordinates mapCoordinates;
    if (((BoundKeyEventArgs) args).Handled || BoundKeyFunction.op_Inequality(((BoundKeyEventArgs) args).Function, EngineKeyFunctions.UIClick) || this.OnMapClick == null || !this.TryGetMapCoordinates(((BoundKeyEventArgs) args).PointerLocation.Position - Vector2i.op_Implicit(((Control) this).GlobalPixelPosition), out mapCoordinates))
      return;
    this.OnMapClick(mapCoordinates);
    ((BoundKeyEventArgs) args).Handle();
  }

  private bool TryGetMapCoordinates(Vector2 localPosition, out MapCoordinates mapCoordinates)
  {
    mapCoordinates = new MapCoordinates();
    if (!this._playerCoordinates.HasValue || (double) localPosition.X < 0.0 || (double) localPosition.Y < 0.0 || (double) localPosition.X > (double) ((Control) this).PixelSize.X || (double) localPosition.Y > (double) ((Control) this).PixelSize.Y)
      return false;
    MapCoordinates mapCoordinates1 = this._transform.ToMapCoordinates(this._playerCoordinates.Value, true);
    if (MapId.op_Equality(mapCoordinates1.MapId, MapId.Nullspace))
      return false;
    float num = (float) ((Control) this).PixelSize.X / (this.WorldRange * 2f);
    if ((double) num <= 0.0)
      return false;
    Vector2 vector2_1 = Vector2i.op_Division(((Control) this).PixelSize, 2f);
    Vector2 vector2_2 = localPosition - vector2_1;
    Vector2 vector2_3 = mapCoordinates1.Position + new Vector2(vector2_2.X / num, -vector2_2.Y / num);
    mapCoordinates = new MapCoordinates(vector2_3, mapCoordinates1.MapId);
    return true;
  }

  private void DrawPlayerMarker(DrawingHandleScreen handle, Vector2 center, Angle playerRotation)
  {
    float num = 8f * this._markerScale;
    Vector2 vector2_1 = new Vector2(0.0f, -num);
    Vector2 vector2_2 = new Vector2((float) (-(double) num * 0.699999988079071), num * 0.5f);
    Vector2 vector2_3 = new Vector2(num * 0.7f, num * 0.5f);
    Angle angle = Angle.op_Addition(Angle.op_UnaryNegation(playerRotation), Angle.FromDegrees(180.0));
    Vector2 vector2_4 = ((Angle) ref angle).RotateVec(ref vector2_1) + center;
    Vector2 vector2_5 = ((Angle) ref angle).RotateVec(ref vector2_2) + center;
    Vector2 vector2_6 = ((Angle) ref angle).RotateVec(ref vector2_3) + center;
    ((DrawingHandleBase) handle).DrawPrimitives((DrawPrimitiveTopology) 1, (ReadOnlySpan<Vector2>) new Vector2[3]
    {
      vector2_4,
      vector2_5,
      vector2_6
    }, this._playerMarkerColor);
    ((DrawingHandleBase) handle).DrawPrimitives((DrawPrimitiveTopology) 5, (ReadOnlySpan<Vector2>) new Vector2[4]
    {
      vector2_4,
      vector2_5,
      vector2_6,
      vector2_4
    }, Color.Black);
  }

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
}
